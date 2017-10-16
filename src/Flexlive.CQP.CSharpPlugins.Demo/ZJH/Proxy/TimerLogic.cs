using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RD.ZJH
{
    public class TimerLogic
    {
        private static TimerLogic s_ins = new TimerLogic();
        public static TimerLogic instance { get { return s_ins; } }
        private Timer m_timer = null;
        public void Init()
        {
            m_timer = new Timer(new TimerCallback(Tick));
            m_timer.Change(0, (int)(Time.delta * 1000));
        }

        private void Tick(object state)
        {
            // 重构基类
            HallProxy.instance.Update();
        }

    }
}
