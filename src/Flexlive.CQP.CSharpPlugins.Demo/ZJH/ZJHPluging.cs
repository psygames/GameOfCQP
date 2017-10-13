using Flexlive.CQP.Framework;
using System;

namespace RD.ZJH
{
    public class ZJHPluging
    {
        public void PrivateMessage(int subType, int sendTime, long fromQQ, string msg, int font)
        {
            // 处理私聊消息。
            // CQX.SendPrivateMessage(fromQQ, String.Format("[{0}]你发的私聊消息是：{1}", CQX.ProxyType, msg));
            // var img = CQX.CQCode_Image(@"C:\Users\Yin\Desktop\图\2015-12-25\修改体验\hint_get_diamond_big.png");
            // CQX.SendPrivateMessage(fromQQ, img);

            Hall.instance.ProcessMsg(fromQQ.ToString(), msg);
        }

        public void FriendAdded(int subType, int sendTime, long fromQQ)
        {
            // 处理好友事件-好友已添加。
            // CQX.SendPrivateMessage(fromQQ, String.Format("[{0}]你好，我的朋友！", CQX.ProxyType));
        }
    }
}
