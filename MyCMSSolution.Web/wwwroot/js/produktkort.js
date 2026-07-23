// Produktkort (TASK-11): "vælg"-knappens visuelle tilstand (valgt/ikke valgt).
// Ren UI-tilstand, ingen data sendes til serveren - aria-pressed bruges til at
// kommunikere tilstanden tilgængeligt til skærmlæsere.
(function () {
  document.addEventListener("click", function (event) {
    var button = event.target.closest("[data-produktkort-vaelg]");
    if (!button) {
      return;
    }

    var isPressed = button.getAttribute("aria-pressed") === "true";
    button.setAttribute("aria-pressed", isPressed ? "false" : "true");
  });
})();
