using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using PortPlanner.Web.SignalR.Hubs;
using PortPlanner.Web.SignalR.Hubs.Events;

namespace PortPlanner.Web.SignalR
{
    public class SignalrPublishDomainEvents : IPublishDomainEvents
    {
        IHubContext<PortPlannerHub, IPortPlannerClientEvent> _templateHub;

        public SignalrPublishDomainEvents(IHubContext<PortPlannerHub, IPortPlannerClientEvent> templateHub)
        {
            _templateHub = templateHub;
        }

        private IPortPlannerClientEvent GetPortPlannerGroup(Guid id)
        {
            return _templateHub.Clients.Group(id.ToString());
        }

        public Task Publish(object evnt)
        {
            try
            {
                return ((dynamic)this).When((dynamic)evnt);
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                return Task.CompletedTask;
            }
        }

        public Task When(NewMessageEvent e)
        {
            return GetPortPlannerGroup(e.IdGroup).NewMessage(e.IdUser, e.IdMessage);
        }
    }
}
