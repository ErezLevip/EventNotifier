using EventNotifier.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventNotifier
{
    public class EventNotifier : IEventNotifier, IDisposable
    {
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static ConcurrentQueue<NotificationWrapper> _notificationsQueue = new ConcurrentQueue<NotificationWrapper>();
        private static ConcurrentDictionary<Type, ConcurrentBag<Action<Notification>>> _subscribers = new ConcurrentDictionary<Type, ConcurrentBag<Action<Notification>>>();
        private static readonly object _lockObj = new object();
        private static int _pollingIntervalMiliseconeds = 200;

        public EventNotifier(int pollingIntervalMiliseconds = 200)
        {
            _pollingIntervalMiliseconeds = pollingIntervalMiliseconds;
        }

        static EventNotifier()
        {
            StartNotificationProcessing();
        }

        public void Publish<T>(T message) where T : Notification
        {
            _notificationsQueue.Enqueue(NotificationWrapper.Create<T>(message));
        }

        public void Subscribe<T>(Action<T> onNewMessage) where T : Notification
        {
            var messageType = typeof(T);
            if (_subscribers.TryGetValue(messageType, out var subscribersBag))
            {
                subscribersBag.Add((Action<Notification>)onNewMessage);
            }
            else
            {
                lock (_lockObj)
                {
                    if (_subscribers.TryGetValue(messageType, out subscribersBag))
                    {
                        subscribersBag.Add((Action<Notification>)onNewMessage);
                    }
                    else
                    {
                        subscribersBag = new ConcurrentBag<Action<Notification>>
                        {
                            ConvertAction(onNewMessage)
                        };
                        _subscribers.TryAdd(typeof(T), subscribersBag);
                    }
                }
            }
        }

        private static void StartNotificationProcessing()
        {
            var processorThread = new Thread(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (!_notificationsQueue.IsEmpty)
                    {
                        _notificationsQueue.TryDequeue(out var message);
                        if (_subscribers.TryGetValue(message.Type, out var onNewMessageBag))
                        {
                            var onNewMessagedTasks = onNewMessageBag.Select(f => Task.Run(() => f(message.Notification)));
                            await Task.WhenAll(onNewMessagedTasks);
                        }
                    }

                    await Task.Delay(_pollingIntervalMiliseconeds);
                }
            })
            {
                IsBackground = true
            };
            processorThread.Start();
        }


        private Action<Notification> ConvertAction<T>(Action<T> action) where T : Notification
        {
            if (action == null)
                return null;
            return new Action<Notification>(a => action((T)a));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
