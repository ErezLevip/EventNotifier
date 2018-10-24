A simple event notifier to pub sub between Domain driven layers

use it with few simple steps:
1. Simply register the event notifier on the container in the setup file using services.AddEventNotifier()
2. Create a notification that is inherited from EventNotifier.Notification or just use Notification it self.
3. Create a subscriber on that Notification:

       notifier.Subscribe((SendHelloWorld notification) =>
                 {
                          Console.WriteLine(notification.Data);
                  });
4. Inject IEventNotifier and publish your message to the world!

       private readonly IEventNotifier _eventNotifier;
       public MicrosvcService(IEventNotifier eventNotifier)
       {
            _eventNotifier = eventNotifier;
        }

       public void SayHelloWorld()
       {
            _eventNotifier.Publish(new SendHelloWorld {
            Data = "hello my dear world"
            });
        }
