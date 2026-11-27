using System;
using System.Collections.Generic;
using PortPlanner.Domain;

namespace PortPlanner.Web.Infrastructure
{
    // Una voce della barra laterale: porta al controller indicato, nell'area del profilo.
    public record VoceMenu(string Testo, string Controller, string Icona);

    // Il menu di ogni profilo, nell'ordine delle specifiche.
    public static class VociMenu
    {
        private static readonly VoceMenu[] Supervisore =
        {
            new("Home", "Home", "fa-house"),
            new("Pianificazione", "Pianificazione", "fa-calendar-days"),
            new("Team", "Team", "fa-users"),
            new("Richieste", "Richieste", "fa-envelope"),
            new("Utenti", "Utenti", "fa-user"),
            new("Ore", "Ore", "fa-clock"),
            new("Assenze", "Assenze", "fa-calendar-xmark"),
        };

        private static readonly VoceMenu[] Capogruppo =
        {
            new("Home", "Home", "fa-house"),
            new("Richieste", "Richieste", "fa-envelope"),
            new("Avvisi", "Avvisi", "fa-bell"),
        };

        // Il Dipendente non ha la barra laterale: il suo menu è quello dell'app mobile.
        public static IReadOnlyList<VoceMenu> Per(Profilo profilo) => profilo switch
        {
            Profilo.Supervisore => Supervisore,
            Profilo.Capogruppo => Capogruppo,
            _ => Array.Empty<VoceMenu>(),
        };
    }
}
