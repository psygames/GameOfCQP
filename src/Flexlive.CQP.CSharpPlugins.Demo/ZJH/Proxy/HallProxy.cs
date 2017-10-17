using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.ZJH
{
    public class HallProxy
    {
        private static HallProxy s_ins = new HallProxy();
        public static HallProxy instance { get { return s_ins; } }

        public List<Room> rooms = new List<Room>();
        private MatchPool matchPool = new MatchPool();

        public void Update()
        {
            matchPool.Update();
            RoomUpdate();
            CheckMatch();
            CheckRoomDismiss();
            UpdateSendMsg();
        }

        private void RoomUpdate()
        {
            foreach (Room room in rooms)
            {
                room.Update();
            }
        }

        public void CheckMatch()
        {
            var matched = matchPool.GetMatched();
            if (matched == null)
                return;
            var room = CreateRoom();
            rooms.Add(room);
            foreach (var pid in matched)
            {
                room.Join(pid);
            }
            room.Reset();
        }

        // 检查解散
        public void CheckRoomDismiss()
        {
            // XX逃跑了，为您重新匹配
            for (int k = rooms.Count - 1; k >= 0; k--)
            {
                var room = rooms[k];
                if (room.state == Room.State.Dismiss)
                {
                    for (int i = room.players.Count - 1; i >= 0; i--)
                    {
                        var player = room.players[i];

                        if (player.isReady)
                        {
                            matchPool.Add(player.id);
                            room.players.Remove(player);
                        }
                    }

                    if (room.players.Count <= 0)
                    {
                        rooms.Remove(room);
                    }
                }
            }
        }

        public void ProcessMsg(string playerID, string msg)
        {
            msg = msg.ToLower();

            if (msg == "zjh")
            {
                Join(playerID);
            }
            else if (msg == "rank")
            {
                var rply = "排行榜：\n";
                int maxCount = 10;
                int rank = 1;
                GetPlayerMoney(playerID);
                foreach (var pid in playerMoneyDic.Keys)
                {
                    var pname = GetName(pid);
                    rply += "排行.{0} : {1}  狗牌: {2}\n".FormatStr(rank++, pname, GetPlayerMoney(pid));
                    if (rank >= maxCount)
                        break;
                }
                rply += "你的 狗牌: {0}".FormatStr(GetPlayerMoney(playerID));
                SendMsgTo(playerID, rply);
            }

            if (!HasPlayer(playerID))
            {
                if (msg == "help" || msg == "?")
                {
                    string hp = "zjh --- 炸金花\nrank --- 狗牌榜\niamsx --- 免费狗牌";
                    SendMsgTo(playerID, hp);
                }
                else if (msg == "iamsx")
                {
                    AskForMoney(playerID);
                }
                return;
            }

            // ZJH HELP
            if (msg == "?" || msg == "help")
            {
                string hp = "ready --- 准备 \nfollow(fl) *[狗牌数量] --- 跟\n"
                    + "drop(dp) --- 弃牌\npk [序号] --- 比牌\n"
                    + "（ps:*[xxx] --- []内为参数名, *表示参数可不填）";
                SendMsgTo(playerID, hp);
            }

            Room room = GetPlayer(playerID).room;
            // Ready State
            if (room.state == Room.State.Ready || room.state == Room.State.Dismiss)
            {
                if (msg == "ready")
                {
                    room.Ready(playerID);
                }
                else if (msg == "quit")
                {
                    room.Quit(playerID);
                }
            }
            // Gaming State
            else if (room.state == Room.State.Gaming)
            {
                if (msg.StartsWith("fl") || msg.StartsWith("follow"))
                {
                    if (msg.Contains(" "))
                    {
                        string value = msg.Substring(msg.IndexOf(" ") + 1).Trim();
                        int iVal = 0;
                        if (int.TryParse(value, out iVal))
                            room.Follow(playerID, iVal);
                    }
                    else if (msg == "fl" || msg == "follow")
                    {
                        room.Follow(playerID);
                    }
                }
                else if (msg == "drop" || msg == "dp")
                {
                    room.Drop(playerID);
                }
                else if (msg.StartsWith("pk "))
                {
                    string param = msg.Substring(3);
                    string target = GetPlayerIDByNumOrName(room, param);
                    if (target != null)
                        room.PK(playerID, target);
                }
            }
        }

        public void Join(string playerID)
        {
            if (HasPlayer(playerID))
            {
                Log("hall join existed {0}", playerID);
                return;
            }

            matchPool.Add(playerID);
            SendMsgTo(playerID, "等待匹配中...");
        }


        public Player GetPlayer(string playerID)
        {
            foreach (var room in rooms)
            {
                var player = room.GetPlayer(playerID);
                if (player != null)
                    return player;
            }
            return null;
        }

        public bool HasPlayer(string playerID)
        {
            return rooms.Exists(a => a.GetPlayer(playerID) != null);
        }


        private void Log(string msg, params string[] parm)
        {
            Console.WriteLine(msg, parm);
        }


        private Room CreateRoom()
        {
            var room = new Room();
            room.state = Room.State.Ready;
            room.name = rooms.Count.ToString();
            return room;
        }




        private static Dictionary<string, int> playerMoneyDic = new Dictionary<string, int>();
        public static int GetPlayerMoney(string playerID)
        {
            int value = 0;
            if (!playerMoneyDic.TryGetValue(playerID, out value))
            {
                playerMoneyDic.Add(playerID, 1000);
            }
            return playerMoneyDic[playerID];
        }

        public static void SetPlayerMoney(string playerID, int money)
        {
            if (playerMoneyDic.ContainsKey(playerID))
            {
                playerMoneyDic[playerID] = money;
            }
        }

        public static void IncrPlayerMoney(string playerID, int money)
        {
            if (playerMoneyDic.ContainsKey(playerID))
            {
                playerMoneyDic[playerID] += money;
            }
        }

        public static void AskForMoney(string playerID)
        {
            int value = 500;
            if (!playerMoneyDic.ContainsKey(playerID))
            {
                playerMoneyDic.Add(playerID, value);
            }
            else if (playerMoneyDic[playerID] <= 100)
            {
                playerMoneyDic[playerID] = value;
            }
        }

        public class Msg
        {
            public string playerID;
            public string msg;
            public Msg(string id, string msg)
            {
                this.playerID = id;
                this.msg = msg;
            }
        }
        public Queue<Msg> msgQueue = new Queue<Msg>();
        public void SendMsgTo(string playerID, string msg)
        {
            msgQueue.Enqueue(new Msg(playerID, msg));
        }

        private float m_lastSend = 0;
        private float MIN_SEND_INTERVAL = 1f;
        private void UpdateSendMsg()
        {
            if (Time.current - m_lastSend > MIN_SEND_INTERVAL && msgQueue.Count > 0)
            {
                Msg msg = msgQueue.Dequeue();
                msg.msg = rand.Next(100, 999) + "\n" + msg.msg;
                CQX.SendPrivateMessage(long.Parse(msg.playerID), msg.msg);
                m_lastSend = Time.current;
            }
        }

        Random rand = new Random();
        Dictionary<string, string> m_nameCached = new Dictionary<string, string>();
        public string GetName(string playerID)
        {
            string name = "";
            if (!m_nameCached.TryGetValue(playerID, out name))
            {
                name = CQX.GetQQName(long.Parse(playerID));
                m_nameCached.Add(playerID, name);
            }
            return "【{0}】".FormatStr(name);
        }

        public string GetPlayerIDByNumOrName(Room room, string playerNameOrNum)
        {
            // num
            int num = 0;
            if (int.TryParse(playerNameOrNum.Trim(), out num))
            {
                num -= 1;
                if (num >= 0 && num < room.players.Count)
                {
                    return room.players[num].id;
                }
            }

            // name
            foreach (var kv in m_nameCached)
            {
                if (kv.Value == playerNameOrNum)
                    return kv.Key;
            }
            return null;
        }
    }
}
