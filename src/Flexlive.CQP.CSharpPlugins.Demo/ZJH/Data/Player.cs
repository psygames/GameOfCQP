using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.ZJH
{
    public class Player
    {
        public string id;
        private List<Card> cards = new List<Card>();
        public int money { get { return HallProxy.GetPlayerMoney(id); } }
        public Room room;
        public bool isDroped = false;
        public bool isReady { get; private set; }

        public List<Card> sortCards
        {
            get
            {
                var stCards = new List<Card>(cards);
                stCards.Sort((a, b) => { return b.id.CompareTo(a.id); });
                return stCards;
            }
        }

        public void Reset(int num)
        {
            isReady = false;
            cards.Clear();
            isDroped = false;
            this.num = num;
        }

        public bool PK(Player target)
        {
            return CheckPK(target);
        }

        public void Drop()
        {
            isDroped = true;
        }

        public void TakeCard(int cardID)
        {
            cards.Add(new Card(cardID));
        }

        public void Ready()
        {
            isReady = true;
        }

        public void SendMsg(string msg)
        {
            HallProxy.instance.SendMsgTo(id, msg);

        }

        public string name
        {
            get { return HallProxy.instance.GetName(id); }
        }

        public int num { get; private set; }

        public string cardsMsg
        {
            get
            {
                string msg = "";
                foreach (var card in sortCards)
                {
                    msg += "{0}   ".FormatStr(card.title);
                }
                return msg;
            }
        }

        public bool isTurn { get { return room.whoseTurn == this; } }

        public string commonStatus
        {
            get
            {
                if (room.state == Room.State.Ready)
                    return "序号.{0}, {1}, 狗牌: {2}".FormatStr(num, name, money);
                else if (room.state == Room.State.Gaming)
                    if (isDroped)
                        return "序号.{0}, {1}, 狗牌: {2}  【已放弃】".FormatStr(num, name, money);
                    else
                        return "序号.{0}, {1}, 狗牌: {2}".FormatStr(num, name, money);
                return "";
            }
        }

        public string privateStatus
        {
            get
            {
                if (room.state == Room.State.Gaming)
                    return "手牌: {0}".FormatStr(cardsMsg);
                return "";
            }
        }

        public string status
        {
            get
            {
                if (string.IsNullOrEmpty(privateStatus))
                {
                    return commonStatus;
                }
                return commonStatus + privateStatus;
            }
        }




        // CARDS CHECK
        bool isSameType
        {
            get
            {
                for (int i = 0; i < cards.Count - 1; i++)
                {
                    if (cards[i].type != cards[i + 1].type)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        bool isSameNum
        {
            get
            {
                for (int i = 0; i < cards.Count - 1; i++)
                {
                    if (cards[i].num != cards[i + 1].num)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        bool isProgression
        {
            get
            {
                for (int i = 0; i < sortCards.Count - 1; i++)
                {
                    if (cards[i].num - cards[i + 1].num != 1)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        bool isPair
        {
            get
            {
                for (int i = 0; i < sortCards.Count - 1; i++)
                {
                    if (cards[i].num == cards[i + 1].num)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        int maxId
        {
            get
            {
                return sortCards[0].id;
            }
        }

        int pairId
        {
            get
            {
                for (int i = 0; i < sortCards.Count - 1; i++)
                {
                    if (cards[i].num == cards[i + 1].num)
                    {
                        return cards[i].id;
                    }
                }
                return 0;
            }
        }

        int cardsValue
        {
            get
            {
                int value = 0;
                if (isSameNum && isSameType)
                {
                    value = 200000 + maxId;
                }
                else if (isSameNum)
                {
                    value = 100000 + maxId;
                }
                else if (isSameType && isProgression)
                {
                    value = 50000 + maxId;
                }
                else if (isSameType && isPair)
                {
                    value = 20000 + pairId * 100 + maxId;
                }
                else if (isSameType)
                {
                    value = 10000 + maxId;
                }
                else if (isProgression)
                {
                    value = 9000 + maxId;
                }
                else if (isPair)
                {
                    value = 100 * pairId + maxId;
                }
                else
                {
                    value = maxId;
                }
                return value;
            }
        }

        private bool CheckPK(Player target)
        {
            return cardsValue > target.cardsValue;
        }
    }
}