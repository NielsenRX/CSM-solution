using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentTypeEditing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.ContentTypeEditing;
using Umbraco.Cms.Core.Strings;

namespace MyCMSSolution.Web.ContentTypes;

/// <summary>
/// Opretter de otte bekræftede sidetyper (Produktside, Kampagne/tilbud, Nyhed/presse, Driftsinfo,
/// FAQ, Jobopslag, Om/kontakt/vilkår, Partnerside) én gang, når Umbraco først er kørende med en
/// installeret database. Idempotent: springer alias'er, der allerede findes, over — så den er
/// sikker at lade køre ved hver opstart, også i produktion efter første succesfulde kørsel.
///
/// Kun struktur og felter oprettes her. Selve blok-biblioteket til Block Grid-feltet bygges endnu
/// ikke (afventer marketings svar på blok-prioritering, jf. kickoff-planens "Åbne spørgsmål").
/// </summary>
public class ConfirmedPageTypesSeeder : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private readonly IRuntimeState _runtimeState;
    private readonly IContentTypeService _contentTypeService;
    private readonly IContentTypeEditingService _contentTypeEditingService;
    private readonly IDataTypeService _dataTypeService;
    private readonly ITemplateService _templateService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
    private readonly ILogger<ConfirmedPageTypesSeeder> _logger;

    public ConfirmedPageTypesSeeder(
        IRuntimeState runtimeState,
        IContentTypeService contentTypeService,
        IContentTypeEditingService contentTypeEditingService,
        IDataTypeService dataTypeService,
        ITemplateService templateService,
        IShortStringHelper shortStringHelper,
        PropertyEditorCollection propertyEditors,
        IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
        ILogger<ConfirmedPageTypesSeeder> logger)
    {
        _runtimeState = runtimeState;
        _contentTypeService = contentTypeService;
        _contentTypeEditingService = contentTypeEditingService;
        _dataTypeService = dataTypeService;
        _templateService = templateService;
        _shortStringHelper = shortStringHelper;
        _propertyEditors = propertyEditors;
        _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        _logger = logger;
    }

    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        if (_runtimeState.Level != RuntimeLevel.Run)
        {
            // Databasen er ikke (færdig)installeret endnu - intet at oprette imod.
            return;
        }

        try
        {
            await SeedAsync();
        }
        catch (Exception ex)
        {
            // Denne opsætning må aldrig forhindre selve applikationen i at starte og betjene
            // trafik - en fejl her logges, men stopper ikke opstarten.
            _logger.LogError(ex, "Kunne ikke oprette/opdatere sidetyper og datatyper ved opstart.");
        }
    }

    private async Task SeedAsync()
    {
        Guid blockGridKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - Indholdsområde (Block Grid)",
            Constants.PropertyEditors.Aliases.BlockGrid,
            new BlockGridConfiguration { Blocks = Array.Empty<BlockGridConfiguration.BlockGridBlockConfiguration>() });

        Guid dateKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - Dato",
            Constants.PropertyEditors.Aliases.DateOnly,
            configurationObject: null);

        Guid shortTextKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - Kort tekst",
            Constants.PropertyEditors.Aliases.TextBox,
            configurationObject: null);

        Guid emailKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - Email",
            Constants.PropertyEditors.Aliases.EmailAddress,
            configurationObject: null);

        Guid linkKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - Link",
            Constants.PropertyEditors.Aliases.MultiUrlPicker,
            configurationObject: null);

        Guid produktsegmentTagsKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - Produktsegment (Tags)",
            Constants.PropertyEditors.Aliases.Tags,
            new TagConfiguration { Group = "produktsegment" });

        Guid faqKategoriTagsKey = await GetOrCreateDataTypeAsync(
            "MyCMSSolution - FAQ-kategori (Tags)",
            Constants.PropertyEditors.Aliases.Tags,
            new TagConfiguration { Group = "faqKategori" });

        var existingAliases = new HashSet<string>(_contentTypeService.GetAllContentTypeAliases(Array.Empty<Guid>()));

        // Produktside: én fleksibel dokumenttype for alle produktsegmenter (Bredbånd, TV & Streaming,
        // Sommerhus, Foreninger, Erhverv) - variation sker via blokke, ikke separate dokumenttyper.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.Produktside,
            "Produktside",
            "icon-shopping-basket",
            PageTypeAliases.ProduktsideTemplateAlias,
            blockGridKey,
            new[] { ("produktsegment", "Produktsegment", produktsegmentTagsKey) });

        // Kampagne/tilbud: start-/slutdato til tidsstyring af kampagnen.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.KampagneTilbud,
            "Kampagne/tilbud",
            "icon-tags",
            PageTypeAliases.KampagneTilbudTemplateAlias,
            blockGridKey,
            new[]
            {
                ("startdato", "Startdato", dateKey),
                ("slutdato", "Slutdato", dateKey),
            });

        // Nyhed/presse: standard blog-lignende struktur med udgivelsesdato.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.NyhedPresse,
            "Nyhed/presse",
            "icon-newspaper-alt",
            PageTypeAliases.NyhedPresseTemplateAlias,
            blockGridKey,
            new[] { ("udgivelsesdato", "Udgivelsesdato", dateKey) });

        // Driftsinfo: IKKE en indtastningsskabelon. "ApiKilde" identificerer blot hvilket eksternt
        // API/feed siden skal vise - selve driftsstatussen hentes og renderes live fra det API på
        // visningstidspunktet (Razor-visning), ikke fra et Umbraco-indholdsfelt. Får stadig det
        // fælles Block Grid-felt for evt. statisk rammetekst omkring den live-hentede status.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.Driftsinfo,
            "Driftsinfo",
            "icon-server-alt",
            PageTypeAliases.DriftsinfoTemplateAlias,
            blockGridKey,
            new[] { ("apiKilde", "API-kilde (identifikator/endpoint)", shortTextKey) });

        // FAQ: kategori til gruppering. Selve spørgsmål/svar-strukturen kommer fra den kommende
        // FAQ-accordion-blok i Block Grid-feltet (endnu ikke bygget).
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.Faq,
            "FAQ",
            "icon-help-alt",
            PageTypeAliases.FaqTemplateAlias,
            blockGridKey,
            new[] { ("kategori", "Kategori", faqKategoriTagsKey) });

        // Jobopslag: simpel struktur - titel er sidenavnet, beskrivelse er Block Grid-feltet,
        // ansøgningslink/-mail er to selvstændige, valgfrie felter.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.Jobopslag,
            "Jobopslag",
            "icon-briefcase-alt",
            PageTypeAliases.JobopslagTemplateAlias,
            blockGridKey,
            new[]
            {
                ("ansoegningslink", "Ansøgningslink", linkKey),
                ("ansoegningsemail", "Ansøgningsemail", emailKey),
            });

        // Om/kontakt/vilkår: statisk informationsside - kun det fælles Block Grid-felt.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.OmKontaktVilkaar,
            "Om/kontakt/vilkår",
            "icon-info",
            PageTypeAliases.OmKontaktVilkaarTemplateAlias,
            blockGridKey,
            Array.Empty<(string, string, Guid)>());

        // Partnerside: kun redigerbar internt - ingen ekstern partneradgang. Ingen særlig
        // Umbraco-adgangsbegrænsning ud over almindelig backoffice-login er nødvendig eller sat op.
        await CreateContentTypeIfMissingAsync(
            existingAliases,
            PageTypeAliases.Partnerside,
            "Partnerside",
            "icon-handshake",
            PageTypeAliases.PartnersideTemplateAlias,
            blockGridKey,
            new[] { ("partnernavn", "Partnernavn", shortTextKey) });

        // Produktkort (TASK-11): Block Grid-elementtype til brug på Produktside. Skal registreres
        // som en tilladt blok i det delte Block Grid-felt, ellers kan marketing ikke placere den.
        await EnsureProduktkortElementTypeAsync(existingAliases, shortTextKey);
        await EnsureProduktkortAllowedInBlockGridAsync();
    }

    private async Task EnsureProduktkortElementTypeAsync(HashSet<string> existingAliases, Guid shortTextDataTypeKey)
    {
        if (existingAliases.Contains(ProduktkortAliases.ElementTypeAlias))
        {
            _logger.LogInformation("Element-type '{Alias}' findes allerede - springer over.", ProduktkortAliases.ElementTypeAlias);
            return;
        }

        var containerKey = Guid.NewGuid();
        var model = new ContentTypeCreateModel
        {
            Key = ProduktkortAliases.ElementTypeKey,
            Alias = ProduktkortAliases.ElementTypeAlias,
            Name = "Produktkort",
            Icon = "icon-shopping-basket",
            AllowedAsRoot = false,
            IsElement = true,
            Containers = new[]
            {
                new ContentTypePropertyContainerModel
                {
                    Key = containerKey,
                    Name = "Indhold",
                    Type = PropertyGroupType.Group.ToString(),
                    SortOrder = 0,
                },
            },
            Properties = new[]
            {
                new ContentTypePropertyTypeModel
                {
                    Key = Guid.NewGuid(),
                    ContainerKey = containerKey,
                    Alias = ProduktkortAliases.ProduktReferencePropertyAlias,
                    Name = "Produktreference",
                    DataTypeKey = shortTextDataTypeKey,
                    SortOrder = 0,
                },
            },
        };

        var result = await _contentTypeEditingService.CreateAsync(model, Constants.Security.SuperUserKey);

        if (result.Success)
        {
            _logger.LogInformation("Oprettede element-type '{Alias}' (Produktkort).", ProduktkortAliases.ElementTypeAlias);
        }
        else
        {
            _logger.LogWarning("Kunne ikke oprette element-type '{Alias}': {Status}", ProduktkortAliases.ElementTypeAlias, result.Status);
        }
    }

    private async Task EnsureProduktkortAllowedInBlockGridAsync()
    {
        IDataType? blockGridDataType = await _dataTypeService.GetAsync("MyCMSSolution - Indholdsområde (Block Grid)");
        if (blockGridDataType is null)
        {
            _logger.LogWarning("Kunne ikke finde Block Grid-datatypen - Produktkort blev ikke tilføjet som tilladt blok.");
            return;
        }

        IDataEditor editor = _propertyEditors[Constants.PropertyEditors.Aliases.BlockGrid];
        IConfigurationEditor configurationEditor = editor.GetConfigurationEditor();

        var currentConfig = (BlockGridConfiguration)configurationEditor.ToConfigurationObject(
            blockGridDataType.ConfigurationData, _configurationEditorJsonSerializer)!;

        IEnumerable<BlockGridConfiguration.BlockGridBlockConfiguration> existingBlocks =
            currentConfig.Blocks ?? Array.Empty<BlockGridConfiguration.BlockGridBlockConfiguration>();

        if (existingBlocks.Any(b => b.ContentElementTypeKey == ProduktkortAliases.ElementTypeKey))
        {
            _logger.LogInformation("Produktkort er allerede tilladt i Block Grid-feltet - springer over.");
            return;
        }

        currentConfig.Blocks = existingBlocks
            .Append(new BlockGridConfiguration.BlockGridBlockConfiguration
            {
                ContentElementTypeKey = ProduktkortAliases.ElementTypeKey,
                AllowAtRoot = true,
                AllowInAreas = true,
            })
            .ToArray();

        blockGridDataType.ConfigurationData = configurationEditor.FromConfigurationObject(currentConfig, _configurationEditorJsonSerializer);
        _dataTypeService.Save(blockGridDataType, Constants.Security.SuperUserId);

        _logger.LogInformation("Tilføjede Produktkort som tilladt blok i Block Grid-feltet.");
    }

    private async Task<Guid> GetOrCreateDataTypeAsync(string name, string editorAlias, object? configurationObject)
    {
        IDataType? existing = await _dataTypeService.GetAsync(name);
        if (existing is not null)
        {
            return existing.Key;
        }

        IDataEditor editor = _propertyEditors[editorAlias];
        var dataType = new DataType(editor, _configurationEditorJsonSerializer, parentId: -1)
        {
            Name = name,
        };

        if (configurationObject is not null)
        {
            dataType.ConfigurationData = editor.GetConfigurationEditor()
                .FromConfigurationObject(configurationObject, _configurationEditorJsonSerializer);
        }

        _dataTypeService.Save(dataType, Constants.Security.SuperUserId);
        _logger.LogInformation("Oprettede datatype '{Name}' ({EditorAlias}).", name, editorAlias);
        return dataType.Key;
    }

    private async Task CreateContentTypeIfMissingAsync(
        HashSet<string> existingAliases,
        string alias,
        string name,
        string icon,
        string templateAlias,
        Guid blockGridDataTypeKey,
        IReadOnlyList<(string Alias, string Name, Guid DataTypeKey)> extraProperties)
    {
        if (existingAliases.Contains(alias))
        {
            _logger.LogInformation("Sidetype '{Alias}' findes allerede - springer over.", alias);
            await EnsureTemplateAssignedAsync(alias, name, templateAlias);
            return;
        }

        var containerKey = Guid.NewGuid();
        var properties = new List<ContentTypePropertyTypeModel>();
        var sortOrder = 0;

        foreach ((string propAlias, string propName, Guid dataTypeKey) in extraProperties)
        {
            properties.Add(new ContentTypePropertyTypeModel
            {
                Key = Guid.NewGuid(),
                ContainerKey = containerKey,
                Alias = propAlias,
                Name = propName,
                DataTypeKey = dataTypeKey,
                SortOrder = sortOrder++,
            });
        }

        properties.Add(new ContentTypePropertyTypeModel
        {
            Key = Guid.NewGuid(),
            ContainerKey = containerKey,
            Alias = PageTypeAliases.Indholdsomraade,
            Name = "Indholdsområde",
            DataTypeKey = blockGridDataTypeKey,
            SortOrder = sortOrder,
        });

        var model = new ContentTypeCreateModel
        {
            Alias = alias,
            Name = name,
            Icon = icon,
            AllowedAsRoot = true,
            IsElement = false,
            Containers = new[]
            {
                new ContentTypePropertyContainerModel
                {
                    Key = containerKey,
                    Name = "Indhold",
                    Type = PropertyGroupType.Group.ToString(),
                    SortOrder = 0,
                },
            },
            Properties = properties,
        };

        var result = await _contentTypeEditingService.CreateAsync(model, Constants.Security.SuperUserKey);

        if (result.Success)
        {
            _logger.LogInformation("Oprettede sidetype '{Alias}' ({Name}).", alias, name);
        }
        else
        {
            _logger.LogWarning("Kunne ikke oprette sidetype '{Alias}': {Status}", alias, result.Status);
        }

        await EnsureTemplateAssignedAsync(alias, name, templateAlias);
    }

    /// <summary>
    /// Opretter (hvis nødvendigt) en Template, der peger på den fysiske Razor-visning
    /// <c>Views/{templateAlias}.cshtml</c>, og tildeler den som standardtemplate for sidetypen (TASK-14).
    /// Idempotent: springer over, hvis sidetypen allerede har en standardtemplate.
    /// </summary>
    private async Task EnsureTemplateAssignedAsync(string contentTypeAlias, string contentTypeName, string templateAlias)
    {
        IContentType? contentType = _contentTypeService.Get(contentTypeAlias);
        if (contentType is null)
        {
            _logger.LogWarning("Sidetype '{Alias}' blev ikke fundet - kan ikke tildele template.", contentTypeAlias);
            return;
        }

        if (contentType.DefaultTemplate is not null)
        {
            _logger.LogInformation("Sidetype '{Alias}' har allerede en standardtemplate - springer over.", contentTypeAlias);
            return;
        }

        ITemplate? template = await _templateService.GetAsync(templateAlias);
        if (template is null)
        {
            var newTemplate = new Template(_shortStringHelper, contentTypeName, templateAlias);
            var createResult = await _templateService.CreateAsync(newTemplate, Constants.Security.SuperUserKey);

            if (!createResult.Success)
            {
                _logger.LogWarning("Kunne ikke oprette template '{TemplateAlias}': {Status}", templateAlias, createResult.Status);
                return;
            }

            template = createResult.Result;
        }

        contentType.SetDefaultTemplate(template);
        _contentTypeService.Save(contentType, Constants.Security.SuperUserId);
        _logger.LogInformation("Tildelte template '{TemplateAlias}' som standard for sidetype '{Alias}'.", templateAlias, contentTypeAlias);
    }
}
