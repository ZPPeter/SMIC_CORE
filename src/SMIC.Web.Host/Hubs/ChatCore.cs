using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
namespace SMIC.Web.Host.Hubs
{
    public class ChatCore
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageData"></param>
        /// <param name="hub"></param>
        public static void SendMessage(MyChatHub chatHub, MessageData messageData)
        {     
            var sendMsg = JsonConvert.SerializeObject(messageData);
            foreach (ConnectionUser user in ConnectionManager.ConnectionUsers)
            {
                chatHub.Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", sendMsg);
            }
        }
    }
}
