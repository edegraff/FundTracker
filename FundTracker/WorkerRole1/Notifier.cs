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
                    bool shouldNotify = notification.ShouldNotify(); // Avoid calculating twice.
                    if(shouldNotify && notification.IsEnabled)
                    {
                        SendNotification(notification);
                        notification.IsEnabled = false;
                    } 
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
            message.Body = "Hello,\n\nThe fund " + notification.FundEntity.name + " has exceeded its threshold\n\n"
                + "View notifications at https://ece493.azurewebsites.net/Notifications\n\nSincerely,\nFundTracker";
            smtp.Send(message);
        }

	}
}
