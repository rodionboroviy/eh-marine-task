using System.Threading.Tasks;

namespace MarineTask.EventReceiver.Consumer
{
    internal interface IEventConsumer
    {
        Task Start();

        Task Stop();
    }
}
