using System;

namespace SMIC.Web.Host.Hubs
{
    /// <summary>
    /// 消息类型
    /// 1.发送连接消息 2.新消息通知 3.未定义 90.首页数字刷新 98.连接回执消息
    /// </summary>
    public enum MessageType
    {
        Line = 1,             // 发送连接消息 - 上线通知
        NewMessage = 2,       // 新消息通知
        HomeInfoDataChanged = 90, // 首页数字刷新
        LineReceipt = 98,     // 连接回执消息
    }
}
