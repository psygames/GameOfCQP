using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD
{
    public class Time
    {
        private static long startUpTicks = DateTime.Now.Ticks;
        public static float current { get { return (DateTime.Now.Ticks - startUpTicks) * 0.0000001f; } }
        public static float delta { get { return 1f; } }
    }
}
