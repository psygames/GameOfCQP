using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipeChanel
{
    public class PipeMsg
    {
        public delegate void PipeMsgEventHandler(object sender, PipeMsgEventArgs e);
        public class PipeMsgEventArgs : EventArgs
        {
            public string receivedMsg;
            public PipeMsgEventArgs(String s)
            {
                this.receivedMsg = s;
            }
        }
    }
}
