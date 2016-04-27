using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using NotificationDto;
using SignalRNotificationEF;

namespace SignalRNotificationAPIs.Controllers
{
    public class NotificationController : ApiController
    {   
        public async Task<Notifications> Get(Guid UserId,Guid? NotificationId = null)
        {
            var notificationDetailsRepository = new NotificationDetailsRepository();
            var notification = new Notifications();
            notification.NotificationDetailList = await notificationDetailsRepository.GetNotificationDetailsByUserId(UserId, NotificationId);
            if (notification.NotificationDetailList != null)
            notification.Count = notification.NotificationDetailList.Count(x => x.IsRead == false);
            return notification;
        }

        public async Task<bool> Put(ReadFlagNotification readNotificationObj)
        {
            var notificationDetailsRepository = new NotificationDetailsRepository();
            return await notificationDetailsRepository.UpdateNotificationReadStatus(readNotificationObj);
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
