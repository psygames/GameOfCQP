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
            foreach (Room room in rooms)
            {
                room.Update();
            }

            matchPool.Update();
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
        }

        // 检查解散
        public void CheckRoomDismiss()
        {

            // XX逃跑了，为您重新匹配

        }

        public void ProcessMsg(string playerID, string msg)
        {
            if (msg == "ZJH")
            {
                Join(playerID);
            }

            if (!HasPlayer(playerID))
            {
                return;
            }

            Room room = GetPlayer(playerID).room;
            // Ready State
            if (room.state == Room.State.Ready)
            {
                if (msg == "Ready")
                {

                }
            }
            // Gaming State
            else if (room.state == Room.State.Gaming)
            {
                if (msg.StartsWith("FL"))
                {
                    string value = msg.Substring(2).Trim();
                    if (string.IsNullOrEmpty(value))
                    {
                        room.Follow(playerID);
                    }
                    else
                    {
                        int iVal = 0;
                        if (int.TryParse(value, out iVal))
                            room.Follow(playerID, iVal);
                    }
                }
                else if (msg == "DROP")
                {
                    room.Drop(playerID);
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
            room.curPrice = 1;
            return room;
        }




        private static Dictionary<string, int> playerMoneyDic = new Dictionary<string, int>();
        public static int GetPlayerMoney(string playerID)
        {
            int value = 1000;
            if (!playerMoneyDic.TryGetValue(playerID, out value))
            {
                playerMoneyDic.Add(playerID, value);
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
    }
}
