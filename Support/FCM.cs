using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;

namespace TrueKeyServer.Support
{
    public class FCM
    {
        public FCM()
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("lemma-mobile-firebase-adminsdk-685w8-32ac7b19d5.json"),
            });
            Program.Logger.Log("FCM", "Created FCM object.");
        }

        public async void Push(List<string> mobileIds, string taskNumber, string typeOfPush, string subData, string commentNumber = "")
        {
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            //{
            //    Program.Logger.Log("FCM.Push", "In developnet enviroment FCM.Push not wroking.");
            //    return;
            //}
            Program.Logger.Log("FCM.Push", $"Try sending Push for task with number: {taskNumber}");
            try
            {
                Program.Logger.Log("FCM.Push", "Creating push message.");
                string title;
                string body;
                string typeValue;
                switch (typeOfPush)
                {
                    case "task":
                        body = subData switch
                        {
                            "InProgress" => $"Мы приступили к работе по заявке {taskNumber}.",
                            "WaitForResponse" => $"В заявке {taskNumber} требуется уточнение.",
                            "Resolved" => $"Заявка {taskNumber} выполнена. Если проблема сохраняется - дайте нам знать.",
                            _ => string.Empty,
                        };
                        title = $"Изменение статуса задачи {taskNumber}.";
                        typeValue = taskNumber;
                        break;
                    case "comment":
                        title = $"Новый комментарий к заявке {taskNumber}.";
                        body = $"Комментарий:  {subData}";
                        typeValue = commentNumber;
                        break;
                    default:
                        title = string.Empty;
                        body = string.Empty;
                        typeValue = string.Empty;
                        break;
                }
                Program.Logger.Log("FCM.Push", $"Push type: {typeOfPush}.");
                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(body))
                {
                    MulticastMessage messages = new MulticastMessage()
                    {
                        Tokens = mobileIds,
                        Notification = new Notification()
                        {
                            Title = title,
                            Body = body
                        },
                        Data = new Dictionary <string, string>()
                        {
                            { "taskNumber", taskNumber},
                            { "typeValue", typeValue},
                            { "type", typeOfPush}
                        }
                    };
                    Program.Logger.Log("FCM.Push", $"Sending push for {mobileIds.Count} MobileIds.");
                    string mobiles = string.Empty;
                    foreach (string str in mobileIds)
                    {
                        mobiles += str + ';';
                    }
                    Program.Logger.Log("FCM.Push", $"Sending push for {mobiles}");
                    if (mobileIds.Count == 0) return;
                    BatchResponse resp = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(messages).ConfigureAwait(false);
                    if (resp.FailureCount > 0)
                    {
                        Program.Logger.Log("FCM.Push", $"Sending errors: {resp.FailureCount}.");

                        foreach (SendResponse fails in resp.Responses.Where(rs => !rs.IsSuccess))
                        {
                            Program.Logger.Log("FCM.Push", $"Error: {fails.Exception.Message}");
                        }
                        /*
                        List<string> failTokens = resp.Responses.Where(rs => !rs.IsSuccess).Select(rs => mobileIds[resp.Responses.ToList().IndexOf(rs)]).ToList();
                        for (int i = 0; i < 3; i++)
                        {
                            Program.Logger.Log("FCM.Push", $"Attempt {i} to send a push for an errors ID: {failTokens}.");
                            Push(failTokens, typeOfPush, taskNumber);
                            Thread.Sleep(1000 * i);
                        }
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("FCM.Push", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
        }
    }
}
