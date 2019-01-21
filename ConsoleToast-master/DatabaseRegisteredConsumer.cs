using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using message_types.events;
using MassTransit;

// https://docs.microsoft.com/en-us/uwp/api/windows.ui.notifications.toastnotificationmanager.createtoastnotifier

namespace notifications
{
    public partial class DatabaseRegisteredConsumer : IConsumer<DatabaseRegistered>
    {

        private IBusControl bus;

        public DatabaseRegisteredConsumer()
        {

            ShortCutCreator.TryCreateShortcut("ConsoleToast.App", "ConsoleToast");

            bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            bus.Start();
        }

        [MTAThread]
        public Task Consume(ConsumeContext<DatabaseRegistered> context)
        {
            var appId = "ConsoleToast.App";
            var title = $"Database Registry";
            var message = "A new database has been registered";
            var imagePath = Path.GetFullPath("search.png");

            ShowToast(appId, title, message, imagePath);

            //0ShowToast(appId, title, message,null);

            return Task.CompletedTask;
        }

        static void ShowToast(string appId, string title, string message, string image)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            if (!string.IsNullOrWhiteSpace(image))
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

                // Specify the absolute path to an image
                String imagePath = "file:///" + image;
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
            }

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            ToastEvents events = new ToastEvents();

            toast.Activated += events.ToastActivated;
            toast.Dismissed += events.ToastDismissed;
            toast.Failed += events.ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId
            // on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }


//        static void ShowTextToast(string appId, string title, string message)
//        {
//
//            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
//
//            // Fill in the text elements
//            Windows.Data.Xml.Dom.XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
//            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
//            stringElements[1].AppendChild(toastXml.CreateTextNode(message));
//
//            // Create the toast and attach event listeners
//            ToastNotification toast = new ToastNotification(toastXml);
//
//            ToastEvents events = new ToastEvents();
//
//            toast.Activated += events.ToastActivated;
//            toast.Dismissed += events.ToastDismissed;
//            toast.Failed += events.ToastFailed;
//
//            // Show the toast. Be sure to specify the AppUserModelId
//            // on your application's shortcut!
//            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
//        }
//
//        static void ShowImageToast(string appId, string title, string message, string image)
//        {
//            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
//
//            // Fill in the text elements
//            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
//            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
//            stringElements[1].AppendChild(toastXml.CreateTextNode(message));
//
//            // Specify the absolute path to an image
//            String imagePath = "file:///" + image;
//            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
//            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
//
//            // Create the toast and attach event listeners
//            ToastNotification toast = new ToastNotification(toastXml);
//
//            ToastEvents events = new ToastEvents();
//
//            toast.Activated += events.ToastActivated;
//            toast.Dismissed += events.ToastDismissed;
//            toast.Failed += events.ToastFailed;
//
//            // Show the toast. Be sure to specify the AppUserModelId
//            // on your application's shortcut!
//            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
//        }
    }
}