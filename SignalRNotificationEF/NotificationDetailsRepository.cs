using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NotificationDto;
using SignalRNotificationEF.Entity;

namespace SignalRNotificationEF
{
    public class NotificationDetailsRepository
    {
        public async Task<Guid> SaveNotificationDetails(SaveNotificationDetailsModel notificationDetailsModel)
        {
            using (var notificationContext = new ServiceEdgeCommonEntities())
            {
                var notificationDetail = new NotificationDetail
                {
                    NotificationId = Guid.NewGuid(),
                    NotificationTypeId = (int)notificationDetailsModel.NotificationTypeId,
                    NotificationNumber = notificationDetailsModel.NotificationNumber,
                    NotificationStatus = notificationDetailsModel.NotificationStatus,
                    CompanyId = notificationDetailsModel.CompanyId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    IsActive = notificationDetailsModel.IsActive,
                    CreatedUpdatedBy = notificationDetailsModel.CreatedUpdatedBy,
                    CreatedUpdatedByName = notificationDetailsModel.CreatedUpdatedByName
                };
                notificationContext.NotificationDetails.Add(notificationDetail);
                foreach (var userDetails in notificationDetailsModel.UserInfo)
                {
                    var userInfo = new NotificationRecipient
                    {
                        IsRead = false,
                        NotificationId = notificationDetail.NotificationId,
                        NotificationExpiry = DateTime.Now.AddDays(2),
                        UserId = userDetails.UserId,
                        UserName = userDetails.UserName,
                        Role = userDetails.Role
                    };
                    notificationContext.NotificationRecipients.Add(userInfo);

                }

                var messages = new NotificationMessage()
                {
                    DisplayText = notificationDetailsModel.DisplayMessage,
                    NotificationId = notificationDetail.NotificationId,
                    NotificationMessageId = Guid.NewGuid()
                };

                notificationContext.NotificationMessages.Add(messages);

                await notificationContext.SaveChangesAsync();
                return notificationDetail.NotificationId;
            }
        }

        public async Task<List<GetNotificationDetailsModel>> GetNotificationDetailsByUserId(Guid userId,Guid? notificationId)
        {
            using (var notificationContext = new ServiceEdgeCommonEntities())
            {
                if (notificationId.HasValue)
                {
                    var query = (from notificationDetail in notificationContext.NotificationDetails
                        where notificationDetail.NotificationId == notificationId
                        select new {cretedDate = notificationDetail.UpdatedOn}).FirstOrDefault();
                    if (query != null)
                    {
                        DateTime createdDateTime = (DateTime) query.cretedDate;
                        var innerJoinQuery =
                            (from notificationDetail in notificationContext.NotificationDetails
                                join notificationRecipient in notificationContext.NotificationRecipients
                                    on notificationDetail.NotificationId equals notificationRecipient.NotificationId
                                join notificationMessages in notificationContext.NotificationMessages
                                    on notificationDetail.NotificationId equals notificationMessages.NotificationId
                                where
                                    notificationRecipient.UserId == userId &&
                                    notificationDetail.UpdatedOn > createdDateTime
                                select new GetNotificationDetailsModel
                                {
                                    NotificationType =
                                        ((NotificationTypeEnum) notificationDetail.NotificationTypeId).ToString(),
                                    NotificationStatus = notificationDetail.NotificationStatus,
                                    NotificationId = notificationDetail.NotificationId,
                                    NotificationNumber = notificationDetail.NotificationNumber,
                                    CreatedUpdatedBy = notificationDetail.CreatedUpdatedBy,
                                    IsRead = notificationRecipient.IsRead ?? false,
                                    UpdatedOn = notificationDetail.UpdatedOn,
                                    DisplayMessage = notificationMessages.DisplayText
                                }).OrderByDescending(x => x.UpdatedOn).Take(10);
                        ;
                        return await innerJoinQuery.ToListAsync();
                    }
                    

                }
                else
                {
                    var innerJoinQuery =
                        (from notificationDetail in notificationContext.NotificationDetails
                         join notificationRecipient in notificationContext.NotificationRecipients
                             on notificationDetail.NotificationId equals notificationRecipient.NotificationId
                         join notificationMessages in notificationContext.NotificationMessages
                             on notificationDetail.NotificationId equals notificationMessages.NotificationId
                         where
                             notificationRecipient.UserId == userId
                         select new GetNotificationDetailsModel
                         {
                             NotificationType =
                                 ((NotificationTypeEnum)notificationDetail.NotificationTypeId).ToString(),
                             NotificationStatus = notificationDetail.NotificationStatus,
                             NotificationId = notificationDetail.NotificationId,
                             NotificationNumber = notificationDetail.NotificationNumber,
                             CreatedUpdatedBy = notificationDetail.CreatedUpdatedBy,
                             IsRead = notificationRecipient.IsRead ?? false,
                             UpdatedOn = notificationDetail.UpdatedOn,
                             DisplayMessage = notificationMessages.DisplayText
                         }).OrderByDescending(x => x.UpdatedOn).Take(10);
                    ;
                    return await innerJoinQuery.ToListAsync();
                }
            }
            return null;
        }

        public async Task<bool> UpdateNotificationReadStatus(ReadFlagNotification readNotificationObj)
        {
            try
            {
                using (var notificationContext = new ServiceEdgeCommonEntities())
                {
                    var query = (from c in notificationContext.NotificationRecipients
                                 where c.NotificationId == readNotificationObj.NotificationId && c.UserId == readNotificationObj.RecipientId
                                 select c).First();
                    query.IsRead = readNotificationObj.IsRead;
                    await notificationContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}