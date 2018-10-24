using EventNotifier;
using EventNotifier.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventNotifierDI
    {
        public static IServiceCollection AddEventNotifier(this IServiceCollection services)
        {
            return services.AddSingleton<IEventNotifier, EventNotifier>();
        }
    }
}

