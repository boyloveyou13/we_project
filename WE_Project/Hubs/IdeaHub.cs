using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WE_Project.Hubs
{
    [HubName("ideaHub")]
    public class IdeaHub : Hub
    {
       public static void BroadcastData()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<IdeaHub>();
            context.Clients.All.refreshIdeaData();
        }
    }
}