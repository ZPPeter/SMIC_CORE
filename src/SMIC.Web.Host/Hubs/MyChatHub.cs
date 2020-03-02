using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Castle.Core.Logging;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace SMIC.Web.Host.Hubs
{    
    public class MyChatHub : Hub
    {
        //public ILogger Logger { get; set; }
        private MessageDealWidth _MessageDealWidth;
        public MyChatHub() {
            //Logger = NullLogger.Instance; // 不能用
            _MessageDealWidth = new MessageDealWidth();
        }
        /// <summary>
        /// 服务器端中转消息处理方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string message)
        {
            await _MessageDealWidth.DealWidth(message, this); 
        }
        
        /*
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }*/

        /// <summary>
        /// 用户连接方法重写
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        /// <summary>
        /// 用户断开连接方法重写
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var item = ConnectionManager.ConnectionUsers.Where(m => m.ConnectionId == Context.ConnectionId).FirstOrDefault();
                //移除相关联用户
                ConnectionManager.ConnectionUsers.Remove(item);
            }
            catch
            {
                //throw ex;
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
