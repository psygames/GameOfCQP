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
            None = 0,
            Fan = 1,
            Mei = 2,
            Hon = 3,
            Hei = 4,
            BIG_JK = 5,
            SMALL_JK = 6,
        }










        /// <summary>
        /// 1-54 [2-A(方，梅，红, 黑)]，JK_SMALL，JK_BIG
        /// </summary>
        /// <param name="id"></param>
        public Card(int id)
        {
            this.id = id;
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
                num = (Num)((id - 1) / 4 + 1);
                type = (Type)((id - 1) % 4 + 1);
            }
        }

        public string typeImgCode
        {
            get
            {
                if (type == Type.Hei)
                    return CQX.CQCode_Image("spade.png");
                else if (type == Type.Hon)
                    return CQX.CQCode_Image("heart.png");
                else if (type == Type.Mei)
                    return CQX.CQCode_Image("club.png");
                else if (type == Type.Fan)
                    return CQX.CQCode_Image("diamond.png");
                return type.ToString();
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
                    return typeImgCode + num.ToString().Trim('_');
                }
            }
        }
    }
}
