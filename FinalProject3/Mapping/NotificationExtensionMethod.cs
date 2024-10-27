using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System;
using System.Linq;

namespace FinalProject3.Mapping
{
    public static class NotificationExtensionMethod
    {
        public static async Task<Notification> AddNotification(this NotificationNew newNotification, FP3Context _context)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                ReferenceId = newNotification.ReferenceId,
                Seen = false,
                Hidden = false,
                Type = newNotification.Type,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm"),
                NotifierId = newNotification.NotifierId
            };
            var Notified = await _context.Users.FindAsync(newNotification.NotifiedId);
            if (Notified != null)
            {
                notification.Notified = Notified;
            }
            return notification;
        }

        public static NotificationDisplay ToDisplay(this Notification notification)
        {
            NotificationDisplay Displaynotification = new NotificationDisplay()
            {
                Id = notification.Id,
                ReferenceId = notification.ReferenceId,
                Seen = notification.Seen,
                Hidden = notification.Hidden,
                Type = notification.Type,
                Date = notification.Date,
                NotifierId = notification.NotifierId,
            };
            return Displaynotification;

        }

        public static Notification AgeOut (this Notification notification)
        {
            if (notification.Date is not null)
            {
                DateTime NoteDateTime;
                bool parsed = DateTime.TryParseExact(notification.Date, "yyyy-MM-dd-HH-mm",
                                                     null, DateTimeStyles.None, out NoteDateTime);
                if (parsed)
                {
                    TimeSpan timeDifference = DateTime.UtcNow - NoteDateTime;
                    if (timeDifference.TotalDays >= 5)
                    {
                        notification.Viewed();
                    }
                    if (timeDifference.TotalDays >= 30)
                    {
                        notification.Hide();
                    }
                }
            }
            return notification;
        }

    }
}
