using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.ZJH
{
    public class Card
    {
        public int id;
        public Type type { get; private set; }
        public Num num { get; private set; }

        public enum Num
        {
            None, _2, _3, _4, _5, _6, _7, _8, _9, _10, J, Q, K, A
        }

        public enum Type
        {
            Hon = 1,
            Mei = 2,
            Hei = 3,
            Fan = 4,
            BIG_JK = 5,
            SMALL_JK = 6,
        }












        /// <summary>
        /// 1-54 黑{2-A}，红{2-A}，梅{2-A}，方{2-A}，{JK2}，{JK1}
        /// </summary>
        /// <param name="id"></param>
        public Card(int id)
        {
            if (id == 53)
            {
                type = Type.SMALL_JK;
            }
            else if (id == 54)
            {

                type = Type.BIG_JK;
            }
            else
            {
                type = (Type)((id - 1) / 13 + 1);
                num = (Num)((id - 1) % 13 + 1);
            }
        }

        public string title
        {
            get
            {
                if (type == Type.BIG_JK)
                    return "BIG_JK";
                else if (type == Type.SMALL_JK)
                    return "SMALL_JK";
                else
                {
                    return type + " " + num.ToString().Trim('_');
                }
            }
        }
    }
}
