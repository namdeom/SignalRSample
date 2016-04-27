using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NotificationDto
{
    public class SaveNotificationDetailsModel
    {
        public NotificationTypeEnum NotificationTypeId { get; set; }
        public string NotificationNumber { get; set; }
        public int CompanyId { get; set; }
        public string NotificationStatus { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedUpdatedBy { get; set; }
        public string CreatedUpdatedByName { get; set; }
        public List<UserInfo> UserInfo { get; set; }
        public string DisplayMessage { get; set; }
    }

    public class Notifications
    {
        public List<GetNotificationDetailsModel> NotificationDetailList;
        public int Count;
    }

    public class ReadFlagNotification
    {
        public Guid NotificationId { get; set; }

        [XmlElement("IsRead")]
        public bool IsRead { get; set; }

        [XmlElement("RecipientId")]
        public Guid RecipientId { get; set; }
    }

    public class GetNotificationDetailsModel
    {
        public Guid NotificationId { get; set; }

        [XmlElement("NotificationType")]
        public string NotificationType { get; set; }

        [XmlElement("NotificationNumber")]
        public string NotificationNumber { get; set; }

        [XmlElement("CompanyId")]
        public int CompanyId { get; set; }

        [XmlElement("NotificationStatus")]
        public string NotificationStatus { get; set; }

        [XmlElement("IsActive")]
        public bool IsActive { get; set; }

        [XmlElement("CreatedUpdatedBy")]
        public Guid CreatedUpdatedBy { get; set; }

        [XmlElement("Message")]
        public string DisplayMessage { get; set; }

        [XmlElement("IsRead")]
        public bool IsRead { get; set; }

        [XmlElement("UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }
    }

    public enum NotificationTypeEnum
    {
        Ro = 1,
        Parts,
        Text,
        Info
    }

    public class UserInfo
    {
        public Nullable<Guid> UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    public class NotificationRecipients
    {
        public Guid NotificationId { get; set; }
        public Nullable<bool> Status { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
