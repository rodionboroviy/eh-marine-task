using System;
using System.Threading.Tasks;

namespace MarineTask.EventSender.Producer
{
    internal interface IEventProducer : IAsyncDisposable
    {
        Task Publish(PublishDetails details);
    }
}
