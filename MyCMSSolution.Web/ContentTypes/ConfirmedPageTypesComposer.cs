using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;

namespace MyCMSSolution.Web.ContentTypes;

/// <summary>
/// Registrerer opsætningen af de otte bekræftede sidetyper (kickoff-planens Spor 2, TASK-07)
/// til at køre, når Umbraco er booted færdigt og databasen er klar.
/// </summary>
public class ConfirmedPageTypesComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, ConfirmedPageTypesSeeder>();
    }
}
