using Azure.Messaging.EventHubs;
using System.Threading.Tasks;

namespace MarineTask.EventReceiver.Handler
{
    internal interface IEventDataHandler
    {
        Task ConsumeEvent(EventData eventData);
    }
}
