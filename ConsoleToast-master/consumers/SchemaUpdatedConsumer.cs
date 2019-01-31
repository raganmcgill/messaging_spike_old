﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using message_types.events;
using MassTransit;
using Notifications.Models;

namespace Notifications.consumers
{
    public class SchemaUpdatedConsumer : IConsumer<SchemaChanged>
    {
        private const string AppId = "RedgateNotifications.App";

        public SchemaUpdatedConsumer()
        {
            Models.DatabaseRegisteredConsumer.ShortCutCreator.TryCreateShortcut("RedgateNotifications.App", "RedgateNotifications");
        }

        [MTAThread]
        public Task Consume(ConsumeContext<SchemaChanged> context)
        {
            var title = "Database NotificationB";
            var message = "A schema has been updated";
            var imagePath = Path.GetFullPath("img/search.png");

            ShowToast(AppId, title, message, imagePath);

            return Task.CompletedTask;
        }

        static void ShowToast(string appId, string title, string message, string image)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            if (!string.IsNullOrWhiteSpace(image))
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

                String imagePath = "file:///" + image;
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
            }

            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            var toast = new ToastNotification(toastXml);

            var events = new ToastEvents();

            toast.Activated += events.ToastActivated;
            toast.Dismissed += events.ToastDismissed;
            toast.Failed += events.ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }
    }
}