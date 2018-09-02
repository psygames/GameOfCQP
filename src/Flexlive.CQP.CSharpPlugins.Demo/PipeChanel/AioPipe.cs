using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;

namespace PipeChanel
{
    /// <summary>
    /// 进程间通信
    /// </summary>
    public class Pipechanel
    {
        private static int _maxNumberInstance;
        private static string _sendpipeName;
        private static string _recvPipeName;

        /// <summary>
        /// 初始化管道并开启异步接收模式
        /// </summary>
        /// <param name="maxNumberInstance">最大管道实例数</param>
        /// <param name="sendpipeName">发送管道名</param>
        /// <param name="recvPipeName">接收管道名</param>
        public Pipechanel(int maxNumberInstance, string sendpipeName, string recvPipeName)
        {
            _maxNumberInstance = maxNumberInstance;
            _sendpipeName = sendpipeName;
            _recvPipeName = recvPipeName;
            pipeStream = new NamedPipeServerStream(_recvPipeName, PipeDirection.InOut, _maxNumberInstance,
            PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            waitforConnect();
        }

        /// <summary>
        /// 发送一条数据
        /// </summary>
        public  void Send(string info)
        {
            Char[] chars = new Char[10];
            using (NamedPipeClientStream pipeStream =
                    new NamedPipeClientStream(_sendpipeName))
            {
                try
                {
                    //防止因Connect阻塞导致cpu占用较高
                    pipeStream.Connect(10);
                    WritePipeStream(info, pipeStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 从管道读取数据
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="pipeStream"></param>
        private static string ReadPipeStream(NamedPipeServerStream pipeStream)
        {
            Char[] chars = new Char[10];
            Byte[] bytes = new Byte[10];
            Decoder decoder = Encoding.UTF8.GetDecoder();
            pipeStream.ReadMode = PipeTransmissionMode.Message;
            int numBytes;
            string message = "";
            do
            {
                numBytes = pipeStream.Read(bytes, 0, bytes.Length);
                int numChars = decoder.GetChars(bytes, 0, numBytes, chars, 0);
                message += new String(chars, 0, numChars);
            } while (!pipeStream.IsMessageComplete);
            decoder.Reset();
            pipeStream.Disconnect();
            return message;
        }

        private static NamedPipeServerStream pipeStream; 

        /// <summary>
        /// 异步等待连接 读取管道数据
        /// </summary>
        private static void waitforConnect()
        {
            pipeStream.BeginWaitForConnection((o) =>
            {
                NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                pServer.EndWaitForConnection(o);
                string recv = ReadPipeStream(pipeStream);
                msgReceived(null, new PipeMsg.PipeMsgEventArgs(recv));
                waitforConnect();
            }, pipeStream);
        }

        /// <summary>
        /// 写入管道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pipeStream"></param>
        /// <returns></returns>
        private static void WritePipeStream(string message, NamedPipeClientStream pipeStream)
        {
            Byte[] bytes;
            UTF8Encoding encoding = new UTF8Encoding();
            bytes = encoding.GetBytes(message);
            pipeStream.Write(bytes, 0, bytes.Length);
            pipeStream.Flush();
        }
        /// <summary>
        /// 接收消息事件
        /// </summary>
        public static event PipeMsg.PipeMsgEventHandler msgReceived;
    }
}
