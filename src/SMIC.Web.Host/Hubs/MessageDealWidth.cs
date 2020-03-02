using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Castle.Core.Logging;
using Abp;
using NLog;

namespace SMIC.Web.Host.Hubs
{
    /// <summary>
    /// 消息处理
    /// </summary>
    public class MessageDealWidth   // : AbpServiceBase 加上 Logger 也无法用
    {
        public MessageDealWidth() {
        }
        public async Task DealWidth(string message, MyChatHub chatHub) 
        {
            // AbpServiceBase 的 Logger 在此模块不正常工作
            // 引用 NLog.Web.AspNetCore
            //Logger logger = LogManager.GetCurrentClassLogger();
            //logger.Info("This is OK");

            await Task.Run(() =>
            {
                try
                {
                    MessageData data = JsonConvert.DeserializeObject<MessageData>(message);
                    if (data != null)
                    {
                        ConnectionUser connectionUser = null;
                        MessageData sendMsg = null;
                        switch (data.MessageType)
                        {
                            case MessageType.Line: //上线了
                                connectionUser = ConnectionManager.ConnectionUsers.Where(m => m.ConnectionId == chatHub.Context.ConnectionId).FirstOrDefault();
                                //处理连接消息
                                if (connectionUser == null)
                                {
                                    connectionUser = new ConnectionUser();
                                    connectionUser.ConnectionId = chatHub.Context.ConnectionId;
                                    connectionUser.UserId = data.SendUserId;
                                    ConnectionManager.ConnectionUsers.Add(connectionUser);
                                }
                                // 处理发送回执消息
                                sendMsg = new MessageData();
                                sendMsg.MessageBody = data.SendUserId;
                                sendMsg.MessageType = MessageType.LineReceipt; // 98 上线通知
                                sendMsg.SendUserId = "0";

                                // 发给自己
                                // chatHub.Clients.Client(chatHub.Context.ConnectionId).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(sendMsg));

                                // 上线通知
                                foreach (ConnectionUser user in ConnectionManager.ConnectionUsers)
                                {
                                    //Logger 不运行
                                    //Logger.Info("Signalr:" + user.ConnectionId + "," + chatHub.Context.ConnectionId);
                                    //Logger.Info(JsonConvert.SerializeObject(sendMsg));
                                    if (user.ConnectionId != chatHub.Context.ConnectionId) // 不发送给自己
                                        chatHub.Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(sendMsg));
                                }
                                break;
                            case MessageType.NewMessage:// 处理新消息通知 不发给自己
                                foreach (ConnectionUser user in ConnectionManager.ConnectionUsers)
                                {
                                    if (user.ConnectionId != chatHub.Context.ConnectionId) // 不发送给自己
                                    chatHub.Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(data));
                                }
                                break;
                            case MessageType.HomeInfoDataChanged:
                                // 首页数字刷新
                                ChatCore.SendMessage(chatHub, data);
                                break;
                            case MessageType.LineReceipt:
                                //处理连接回执消息
                                ChatCore.SendMessage(chatHub, data);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //Logger.Error(ex.Message);
                }
            });
        }
    }
}
