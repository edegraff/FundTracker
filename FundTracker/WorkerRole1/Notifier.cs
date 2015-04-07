using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace FundService
{
	public class Notifier
	{
		public void Notify()
		{
            CheckNotifications();
		}

        private void CheckNotifications()
        {
            using (var db = new DatabaseContext())
            {
                foreach (var notification in db.Notifications)
                {
                    // 3.2.3.1 Users will be able to select specific mutual funds to be notified if they pass specified thresholds. (Checks if the threshhold has been exceeded and notifies correspondingly)
                    bool shouldNotify = notification.ShouldNotify(); // Avoid calculating twice.
                    if(shouldNotify && notification.IsEnabled)
                    {
                        SendNotification(notification);
                        // 3.2.3.2.1 Manual Reset: A notification is sent once the specified criteria matches, but is not sent again until the User resets the notification to trigger again.
                        notification.IsEnabled = false;
                    }
                    // 3.2.3.2.2 Automatic Reset: Once the fund matches the criteria a notification is sent. As soon as the fund no longer matches the criteria the state will be reset and it will be sent again once the criteria matches.
                    else if(!shouldNotify && notification.AutoReset)
                    {
                        // Reset notification
                        notification.IsEnabled = true;
                    }
                }
                db.SaveChanges();
            }
        }

        private void SendNotification(Notification notification)
        {
            Console.WriteLine(notification.NotificationId);
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("fundtracker.ece493@gmail.com", "rtznxkzviwcucthi")
            };
            MailMessage message = new MailMessage("fundtracker.ece493@gmail.com", notification.UserProfile.Email);
            message.Subject = "Notification from FundTracker!";
            message.Body = "Hello,\n\nThe fund " + notification.FundEntity.Name + " has exceeded its threshold\n\n"
                + "View notifications at https://ece493.azurewebsites.net/Notifications\n\nSincerely,\nFundTracker";
            smtp.Send(message);
        }

	}
}
