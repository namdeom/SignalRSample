using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using NotificationDto;


namespace SignalRClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SendMessageToNotificationServer("222", "dispatch");
            Console.ReadLine();

        }
        private static void SendMessageToNotificationServer(string roNumber, string roStatus)
        {
            var hubConnection = new HubConnection("http://localhost:56888/");
            //hubConnection.TraceLevel = TraceLevels.All;
            //hubConnection.TraceWriter = Console.Out;
            IHubProxy notificationHubProxy = hubConnection.CreateHubProxy("NotificationHub");

            notificationHubProxy.On("heartbeat", () => Console.Write("Recieved heartbeat \n"));
            notificationHubProxy.On<SaveNotificationDetailsModel>("SendNotificationDetails", nod => Console.Write("Recieved sendNotificationDetails {0}, {1} \n", nod.NotificationTypeId.ToString(), nod.NotificationNumber));

            hubConnection.Start().Wait();

            var advisorUser = new UserInfo
            {
                Role = "Advisor",
                UserId = new Guid("2704CEBB-C1D3-4E37-98B1-343B3B58ECB9"),
                UserName = "ashtest1"
            };
            var technicianUser = new UserInfo
            {
                Role = "Technician",
                UserId = new Guid("EE4A852B-252D-46EF-808D-2AC3D333DC24"),
                UserName = "technician"
            };
            var userList = new List<UserInfo> {advisorUser, technicianUser};
            var notificationDetails = new SaveNotificationDetailsModel
            {
                NotificationTypeId = NotificationTypeEnum.Ro,
                CompanyId = 3002,
                CreatedUpdatedBy = new Guid("D95A2B40-5F9D-465B-9014-EF299968B48C"),
                CreatedUpdatedByName = "Test name",
                IsActive = true,
                NotificationNumber = "123456",
                NotificationStatus = "Inspection",
                UserInfo = userList
            };

            notificationHubProxy.Invoke<SaveNotificationDetailsModel>("SendNotificationDetails", notificationDetails).ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception != null)
                {
                    Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                }

            }).Wait();

            

        }
    }
}
