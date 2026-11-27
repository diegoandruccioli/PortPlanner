using PortPlanner.Services.Shared;
using PortPlanner.Services.Validazione;
using Microsoft.Extensions.DependencyInjection;
using PortPlanner.Web.SignalR;

namespace PortPlanner.Web
{
    public class Container
    {
        public static void RegisterTypes(IServiceCollection container)
        {
            // Registration of all the database services you have
            container.AddScoped<SharedService>();
            container.AddScoped<ValidazioneService>();

            // Registration of SignalR events
            container.AddScoped<IPublishDomainEvents, SignalrPublishDomainEvents>();
        }
    }
}
