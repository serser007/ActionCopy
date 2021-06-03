using System;
using System.Collections.Concurrent;

namespace VoidNetworking
{
    public abstract class ActionQueue
    { 
        private readonly ConcurrentQueue<Action> queuedActions = new ConcurrentQueue<Action>();

        public void RunQueuedActions()
        {
            while (queuedActions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        public void EnqueueAction(Action action) => queuedActions.Enqueue(action);
    }
}
