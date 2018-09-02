using Flexlive.CQP.Framework;
using System;
using MMUtils;
using Newtonsoft.Json;

namespace RD
{
    public class CSDNPlugin
    {
        PipeChanel.Pipechanel pipeclient;

        public void Init()
        {
            pipeclient = new PipeChanel.Pipechanel(2, "PipeClient", "PipeServer");
            PipeChanel.Pipechanel.msgReceived += new PipeChanel.PipeMsg.PipeMsgEventHandler(Pipechanel_msgReceived);
        }

        private void Pipechanel_msgReceived(object sender, PipeChanel.PipeMsg.PipeMsgEventArgs e)
        {
            PipeMsg msg = JsonConvert.DeserializeObject<PipeMsg>(e.receivedMsg);
            string str = "";
            if (msg.isDownloadSuccess)
            {
                str += "下载成功： " + msg.downloadFileName;
                str += "\n" + msg.downloadUrl;
            }
            else
                str += "下载失败： " + msg.dowloadErrorInfo;
            CQX.SendMessage(msg.fromQQType, msg.fromGroup, msg.fromQQ, str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">21/好友 2/群 4/讨论组</param>
        /// <param name="subType"></param>
        /// <param name="sendTime"></param>
        /// <param name="fromQQ"></param>
        /// <param name="msg"></param>
        /// <param name="font"></param>
        public void OnMessage(int type, int subType, int sendTime, long groupID, long fromQQ, string msg, int font)
        {
            // 处理私聊消息。
            // CQX.SendPrivateMessage(fromQQ, String.Format("[{0}]你发的私聊消息是：{1}", CQX.ProxyType, msg));
            // var img = CQX.CQCode_Image(@"C:\Users\Yin\Desktop\图\2015-12-25\修改体验\hint_get_diamond_big.png");
            // CQX.SendPrivateMessage(fromQQ, img);
            if (msg.StartsWith("下载"))
            {
                var url = "http" + msg.GetAfter("http").TrimEnd();
                PipeMsg pMsg = new PipeMsg();
                pMsg.fromGroup = groupID;
                pMsg.fromUrl = url;
                pMsg.fromQQType = type;
                pMsg.fromQQ = fromQQ;
                var s_pMsg = JsonConvert.SerializeObject(pMsg);
                pipeclient.Send(s_pMsg);
            }
        }


        public class PipeMsg
        {
            public int fromQQType;
            public long fromGroup;
            public long fromQQ;
            public string fromUrl;
            public bool isDownloadSuccess;
            public string downloadFileName;
            public string downloadUrl;
            public string dowloadErrorInfo;
        }
    }
}
