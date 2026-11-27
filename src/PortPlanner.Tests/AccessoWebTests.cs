using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using PortPlanner.Infrastructure;
using PortPlanner.Web;
using PortPlanner.Web.Areas.Capogruppo;
using PortPlanner.Web.Areas.Supervisore;

namespace PortPlanner.Tests
{
    // Avvia l'app in memoria e controlla chi può aprire cosa, passando dal login vero.
    // Serve `npm install` in PortPlanner.Web, come per avviare l'app.
    public class AccessoWebTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string Supervisore = "mario.rossi@portplanner.it";
        private const string Capogruppo = "anna.conti@portplanner.it";
        private const string Dipendente = "sara.fontana@portplanner.it";

        private static readonly string[] PagineSupervisore =
        {
            "/Supervisore/Home", "/Supervisore/Pianificazione", "/Supervisore/Team", "/Supervisore/Richieste",
            "/Supervisore/Utenti", "/Supervisore/Ore", "/Supervisore/Assenze",
        };

        private static readonly string[] PagineCapogruppo =
        {
            "/Capogruppo/Home", "/Capogruppo/Richieste", "/Capogruppo/Avvisi",
        };

        private readonly WebApplicationFactory<Program> _app;

        public AccessoWebTests(WebApplicationFactory<Program> app) => _app = app;

        private HttpClient Anonimo() => _app.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        private static Task<HttpResponseMessage> InviaLogin(HttpClient client, string email, string returnUrl = null)
        {
            var campi = new Dictionary<string, string>
            {
                ["Email"] = email,
                ["Password"] = DataGenerator.PasswordDiProva,
                ["RememberMe"] = "false",
            };
            if (returnUrl != null)
                campi["ReturnUrl"] = returnUrl;

            return client.PostAsync("/Login/Login", new FormUrlEncodedContent(campi));
        }

        private async Task<HttpClient> Collegato(string email)
        {
            var client = Anonimo();
            var risposta = await InviaLogin(client, email);
            Assert.Equal(HttpStatusCode.Redirect, risposta.StatusCode);
            return client;
        }

        private static string Destinazione(HttpResponseMessage risposta)
        {
            Assert.Equal(HttpStatusCode.Redirect, risposta.StatusCode);
            var url = risposta.Headers.Location;
            return url.IsAbsoluteUri ? url.AbsolutePath : url.OriginalString;
        }

        // Gli indirizzi delle voci della barra laterale, nell'ordine in cui compaiono.
        private static string[] VociDelMenu(string html)
        {
            var nav = Regex.Match(html, "<nav class=\"nav nav-pills.*?</nav>", RegexOptions.Singleline).Value;
            return Regex.Matches(nav, "href=\"([^\"]*)\"").Select(m => m.Groups[1].Value).ToArray();
        }

        // La Home di un'area ha l'indirizzo accorciato: /Supervisore invece di /Supervisore/Home.
        private static string[] IndirizziAttesi(string[] pagine) =>
            pagine.Select(p => p.EndsWith("/Home") ? p[..^"/Home".Length] : p).ToArray();

        public static TheoryData<string, string> Consentite()
        {
            var dati = new TheoryData<string, string>();
            foreach (var pagina in PagineSupervisore) dati.Add(Supervisore, pagina);
            foreach (var pagina in PagineCapogruppo) dati.Add(Capogruppo, pagina);
            return dati;
        }

        public static TheoryData<string, string> Vietate()
        {
            var dati = new TheoryData<string, string>();
            foreach (var pagina in PagineSupervisore)
            {
                dati.Add(Capogruppo, pagina);
                dati.Add(Dipendente, pagina);
            }
            foreach (var pagina in PagineCapogruppo)
            {
                dati.Add(Supervisore, pagina);
                dati.Add(Dipendente, pagina);
            }
            return dati;
        }

        [Theory]
        [MemberData(nameof(Consentite))]
        public async Task Il_profilo_giusto_apre_la_pagina(string email, string pagina)
        {
            var client = await Collegato(email);

            var risposta = await client.GetAsync(pagina);

            Assert.Equal(HttpStatusCode.OK, risposta.StatusCode);
        }

        [Theory]
        [MemberData(nameof(Vietate))]
        public async Task Un_altro_profilo_viene_mandato_ad_accesso_negato(string email, string pagina)
        {
            var client = await Collegato(email);

            var risposta = await client.GetAsync(pagina);

            Assert.Equal("/AccessoNegato/Index", Destinazione(risposta));
        }

        [Theory]
        [InlineData("/Supervisore/Utenti")]
        [InlineData("/Capogruppo/Avvisi")]
        [InlineData("/AccessoNegato/Index")]
        public async Task Senza_accesso_si_va_al_login(string pagina)
        {
            var risposta = await Anonimo().GetAsync(pagina);

            Assert.Equal("/Login/Login", Destinazione(risposta));
        }

        [Theory]
        [InlineData(Capogruppo, "Capogruppo")]
        [InlineData(Dipendente, "Dipendente")]
        public async Task La_pagina_di_accesso_negato_risponde_403_con_un_messaggio_chiaro(string email, string profilo)
        {
            var client = await Collegato(email);

            var risposta = await client.GetAsync("/AccessoNegato/Index");
            var html = WebUtility.HtmlDecode(await risposta.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, risposta.StatusCode);
            Assert.Contains($"Questa pagina non è disponibile per il profilo {profilo}.", html);
            Assert.Contains("Torna alla home", html);
        }

        [Fact]
        public async Task Il_supervisore_ha_il_suo_menu_con_la_voce_attiva()
        {
            var client = await Collegato(Supervisore);

            var html = await client.GetStringAsync("/Supervisore/Team");

            Assert.Equal(IndirizziAttesi(PagineSupervisore), VociDelMenu(html));
            Assert.Matches("aria-current=\"page\"\\s+href=\"/Supervisore/Team\"", html);
            Assert.Single(Regex.Matches(html, "aria-current=\"page\""));
            Assert.Contains("Mario Rossi", html);
        }

        [Fact]
        public async Task Il_capogruppo_ha_il_suo_menu()
        {
            var client = await Collegato(Capogruppo);

            var html = await client.GetStringAsync("/Capogruppo/Home");

            Assert.Equal(IndirizziAttesi(PagineCapogruppo), VociDelMenu(html));
            Assert.Contains("Anna Conti", html);
        }

        [Fact]
        public async Task Il_dipendente_non_ha_la_barra_laterale()
        {
            var client = await Collegato(Dipendente);

            var html = await client.GetStringAsync("/Dashboard/Index");

            Assert.DoesNotContain("barraLaterale", html);
            Assert.Contains("Sara Fontana", html);
        }

        [Theory]
        [InlineData(Supervisore, "/Supervisore")]
        [InlineData(Capogruppo, "/Capogruppo")]
        public async Task La_dashboard_manda_alla_home_del_profilo(string email, string home)
        {
            var client = await Collegato(email);

            var risposta = await client.GetAsync("/Dashboard/Index");

            Assert.Equal(home, Destinazione(risposta));
        }

        [Theory]
        [InlineData("https://sito-esterno.example/")]
        [InlineData("//sito-esterno.example/")]
        public async Task Dopo_il_login_non_si_va_su_un_sito_esterno(string returnUrl)
        {
            var risposta = await InviaLogin(Anonimo(), Supervisore, returnUrl);

            Assert.Equal("/Dashboard/Index", Destinazione(risposta));
        }

        [Fact]
        public async Task Chi_e_gia_collegato_non_viene_mandato_su_un_sito_esterno()
        {
            var client = await Collegato(Supervisore);

            var risposta = await client.GetAsync("/Login/Login?returnUrl=https://sito-esterno.example/");

            Assert.Equal("/Dashboard/Index", Destinazione(risposta));
        }

        [Fact]
        public async Task Dopo_il_login_si_torna_alla_pagina_richiesta()
        {
            var risposta = await InviaLogin(Anonimo(), Supervisore, "/Supervisore/Team");

            Assert.Equal("/Supervisore/Team", Destinazione(risposta));
        }

        // Chi aggiunge una pagina in un'area deve derivare dal controller base, altrimenti resta aperta a tutti.
        [Theory]
        [InlineData("Supervisore", typeof(SupervisoreBaseController))]
        [InlineData("Capogruppo", typeof(CapogruppoBaseController))]
        public void Ogni_controller_di_un_area_e_protetto(string area, Type controllerBase)
        {
            var controller = typeof(Startup).Assembly.GetTypes()
                .Where(t => t.Namespace == $"PortPlanner.Web.Areas.{area}" && typeof(Controller).IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract)
                .Where(t => t.Name.EndsWith("Controller"))
                .ToList();

            Assert.NotEmpty(controller);
            Assert.All(controller, t => Assert.True(controllerBase.IsAssignableFrom(t), $"{t.Name} non deriva da {controllerBase.Name}"));
        }
    }
}
