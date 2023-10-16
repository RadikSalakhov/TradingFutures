namespace TradingFutures.Server.WorkerHandlers
{
    public interface IWorkerHandler
    {
        DateTime WorkerDT { get; }

        void RegisterAction<T>(long intervalMS, Func<Task> action);

        void UnregisteredActions<T>();

        Task ProcessStepAsync<T>();
    }
}