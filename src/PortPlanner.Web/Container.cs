using Microsoft.Extensions.DependencyInjection;
using PortPlanner.Services.Shared;
using PortPlanner.Web.SignalR;

namespace PortPlanner.Web
{
    // I servizi di ogni funzione si registrano nel suo file Container.<Funzione>.cs.
    public partial class Container
    {
        public static void RegisterTypes(IServiceCollection container)
        {
            // Servizi usati da tutte le funzioni.
            container.AddScoped<SharedService>();
            container.AddScoped<IPublishDomainEvents, SignalrPublishDomainEvents>();

            RegistraAnagrafica(container);
            RegistraPianificazione(container);
            RegistraRichieste(container);
            RegistraAssenze(container);
            RegistraAvvisi(container);
            RegistraOre(container);
        }
    }
}
