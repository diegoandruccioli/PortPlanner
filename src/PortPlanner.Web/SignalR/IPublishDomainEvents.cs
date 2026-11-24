using System.Threading.Tasks;

namespace PortPlanner.Web.SignalR
{
    public interface IPublishDomainEvents
    {
        Task Publish(object evnt);
    }
}
