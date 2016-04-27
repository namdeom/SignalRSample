using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NotificationDto;
using SignalRNotificationEF;

namespace SignalRWEBServer
{
    public class NotificationHub : Hub
    {
        private static IDictionary<Guid, string> _onlineUsersDictionary = new Dictionary<Guid, string>(); 
        public void Heartbeat()
        {
            Debug.WriteLine("Hub Heartbeat\n");
            Clients.All.heartbeat();
        }

        public async Task  SendNotificationDetails(SaveNotificationDetailsModel notificationDetailsModel)
        {
            try
            {
                var notificationDetailsRepository = new NotificationDetailsRepository();
                Guid noticiationId = await notificationDetailsRepository.SaveNotificationDetails(notificationDetailsModel);

                Debug.WriteLine(string.Format("Notification Recieved at server side for Notification type:{0} and Number:{1}",
                    notificationDetailsModel.NotificationTypeId.ToString(), notificationDetailsModel.NotificationNumber));

                //Notify to specific users from Connections
                foreach (var userDetails in notificationDetailsModel.UserInfo)
                {
                    Nullable<Guid> userId = userDetails.UserId;
                    var connectionIdsToNotify =
                        _onlineUsersDictionary.Where(r => r.Key == userId).Select(r => r.Value).ToList();
                    if (connectionIdsToNotify.Any())
                    {
                        var sendNotificationDetailsModel = new GetNotificationDetailsModel()
                        {
                            NotificationType = ((NotificationTypeEnum)notificationDetailsModel.NotificationTypeId).ToString(),
                            NotificationStatus = notificationDetailsModel.NotificationStatus,
                            NotificationId = noticiationId,
                            NotificationNumber = notificationDetailsModel.NotificationNumber,
                            CreatedUpdatedBy = notificationDetailsModel.CreatedUpdatedBy,
                            IsRead = false,
                            DisplayMessage = notificationDetailsModel.DisplayMessage 
                        };
                        Clients.Clients(connectionIdsToNotify).SendNotificationDetails(sendNotificationDetailsModel);
                    }
                }
            }
            catch (Exception ex)
            {
                //ToDO: add logger
                throw ex;
            }
        
        }

        public override Task OnConnected()
        {
            Debug.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            if (Context.QueryString != null && Context.QueryString["UserId"] != null)
            {
                Guid key;
                Guid.TryParse(Context.QueryString["UserId"].ToString(CultureInfo.InvariantCulture), out key);
                _onlineUsersDictionary[key] = Context.ConnectionId;
            }
            return (base.OnConnected());
        }

        public override Task OnDisconnected(bool isStoppedCall)
        {
            Debug.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            if (Context.QueryString != null && Context.QueryString["UserId"] != null)
            {
                Guid key;
                Guid.TryParse(Context.QueryString["UserId"].ToString(CultureInfo.InvariantCulture), out key);
                _onlineUsersDictionary.Remove(key);
            }
            return (base.OnDisconnected(isStoppedCall));
        }

      }
}