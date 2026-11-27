using Microsoft.Extensions.DependencyInjection;
using PortPlanner.Services.Validazione;

namespace PortPlanner.Web
{
    public partial class Container
    {
        private static void RegistraPianificazione(IServiceCollection container)
        {
            container.AddScoped<ValidazioneService>();
        }
    }
}
