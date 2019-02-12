using System;
using Windows.UI.Notifications;

namespace Notifications.Models
{
    public class ToastEvents
    {
        internal void ToastActivated(ToastNotification sender, object e)
        {
            Console.WriteLine("User activated the toast");
        }

        internal void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            String outputText = "";
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    outputText = "The app hid the toast using ToastNotifier.Hide";
                    break;
                case ToastDismissalReason.UserCanceled:
                    outputText = "The user dismissed the toast";
                    break;
                case ToastDismissalReason.TimedOut:
                    outputText = "The toast has timed out";
                    break;
            }

            Console.WriteLine(outputText);
        }

        internal void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            Console.WriteLine("The toast encountered an error.");
        }
    }
}