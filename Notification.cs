using System;
using System.Collections.Generic;
using System.Text;

namespace EventNotifier
{
    public class Notification
    {
        public object Data { get; set; }
    }
    internal class NotificationWrapper
    {
        protected internal static NotificationWrapper Create<T>(Notification notification)
        {
            return new NotificationWrapper
            {
                Notification = notification,
                Type = typeof(T)
            };
        }
        public Type Type { get; set; }
        public Notification Notification { get; set; }
    }
}
