using System;
using System.Collections.Generic;
using PortPlanner.Domain;

namespace PortPlanner.Web.Infrastructure
{
    // Una voce della barra laterale: porta al controller indicato, nell'area del profilo.
    // Le voci con lo stesso Gruppo stanno di seguito, sotto un'intestazione; senza gruppo non c'è intestazione.
    public record VoceMenu(string Testo, string Controller, string Icona, string Gruppo = null);

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

        // Il capogruppo è anche un lavoratore: le pagine personali stanno nel gruppo «Per me».
        private const string PerMe = "Per me";

        private static readonly VoceMenu[] Capogruppo =
        {
            new("Home", "Home", "fa-house"),
            new("Pianificazione", "Pianificazione", "fa-calendar-days"),
            new("Team", "Team", "fa-users"),
            new("Richieste", "Richieste", "fa-envelope"),
            new("Utenti", "Utenti", "fa-user"),
            new("Avvisi", "Avvisi", "fa-bell"),
            new("I miei turni", "Turni", "fa-calendar-check", PerMe),
            new("Le mie ore", "Ore", "fa-clock", PerMe),
            new("Le mie assenze", "Assenze", "fa-calendar-xmark", PerMe),
            new("Profilo", "Profilo", "fa-id-card", PerMe),
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
