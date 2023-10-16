using System.Collections.Generic;

namespace TradingFutures.Server.WorkerHandlers
{
    public class WorkerHandler : IWorkerHandler
    {
        private readonly Dictionary<Type, List<WorkerHandlerAction>> _registeredActionsDict = new();

        private readonly ILogger<WorkerHandler> _logger;

        public DateTime WorkerDT { get; private set; } = DateTime.MinValue;

        public WorkerHandler(ILogger<WorkerHandler> logger)
        {
            _logger = logger;
        }

        public void RegisterAction<T>(long intervalMS, Func<Task> action)
        {
            lock (_registeredActionsDict)
            {
                if (!_registeredActionsDict.TryGetValue(typeof(T), out List<WorkerHandlerAction>? registeredActions))
                {
                    registeredActions = new List<WorkerHandlerAction>();
                    _registeredActionsDict.Add(typeof(T), registeredActions);
                }

                registeredActions.Add(
                        new WorkerHandlerAction
                        {
                            IntervalMS = intervalMS,
                            Action = action,
                            LastDT = DateTime.MinValue
                        });
            }
        }

        public void UnregisteredActions<T>()
        {
            lock (_registeredActionsDict)
            {
                if (_registeredActionsDict.TryGetValue(typeof(T), out List<WorkerHandlerAction>? registeredActions))
                    registeredActions.Clear();
            }
        }

        public async Task ProcessStepAsync<T>()
        {
            WorkerDT = DateTime.UtcNow;

            List<WorkerHandlerAction>? registeredActions;

            lock (_registeredActionsDict)
            {
                if (!_registeredActionsDict.TryGetValue(typeof(T), out registeredActions))
                    registeredActions = null;
            }

            if (registeredActions == null)
            {
                await Task.Delay(1);
                return;
            }

            var executeActions = new List<WorkerHandlerAction>();
            foreach (var registeredAction in registeredActions)
            {
                if ((DateTime.UtcNow - registeredAction.LastDT).TotalMilliseconds >= registeredAction.IntervalMS)
                    executeActions.Add(registeredAction);
            }

            if (!executeActions.Any())
            {
                await Task.Delay(1);
                return;
            }

            foreach (var executeAction in executeActions)
            {
                try
                {
                    await executeAction.Action();
                }
                catch (Exception exp)
                {
                    _logger.LogError(exp, string.Empty);
                }
                finally
                {
                    executeAction.LastDT = DateTime.UtcNow;
                }
            }
        }
    }
}