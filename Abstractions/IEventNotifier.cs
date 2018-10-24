using System;
using System.Collections.Generic;
using System.Text;

namespace EventNotifier.Abstractions
{
    public interface IEventNotifier
    {
        void Publish<T>(T message) where T : Notification;

        void Subscribe<T>(Action<T> onNewMmessage) where T : Notification;
    }
}
