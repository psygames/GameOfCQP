using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.ZJH
{
    public class Hall
    {
        private static Hall s_ins = new Hall();
        public static Hall instance { get { return s_ins; } }

        public List<Room> rooms = new List<Room>();

        public bool HasPlayer(string playerID)
        {
            return rooms.Exists(a => a.players.Exists(b => b.id == playerID));
        }

        private void Log(string msg, params string[] parm)
        {
            Console.WriteLine(msg, parm);
        }

        public void Join(string playerID)
        {
            if (HasPlayer(playerID))
            {
                Log("hall join existed {0}", playerID);
                return;
            }

            var room = rooms.Find(a => a.state == Room.State.Ready);
            if (room == null)
            {
                room = CreateRoom();
                rooms.Add(room);
            }
            room.Join(playerID);
        }

        private Room CreateRoom()
        {
            var room = new Room();
            room.state = Room.State.Ready;
            room.name = rooms.Count.ToString();
            room.curPrice = 10;
            return room;
        }

        public void Update()
        {
            foreach (Room room in rooms)
            {
                room.Update();

                foreach (var player in room.players)
                {
                    player.Update();
                }
            }
        }

        public Player GetPlayer(string playerID)
        {
            foreach (var room in rooms)
            {
                return room.players.Find(a => a.id == playerID);
            }
            return null;
        }

        public void ProcessMsg(string playerID, string msg)
        {
            if (msg == "ZJH")
            {
                Join(playerID);
            }

            if (!HasPlayer(playerID))
                return;

            Room room = GetPlayer(playerID).room;

            if (room.state != Room.State.Playing)
                return;

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
                        room.Follow(playerID,iVal);
                }
            }

            if (msg == "DROP")
            {
                room.Drop(playerID);
            }
        }










        private static Dictionary<string, int> playerMoneyDic = new Dictionary<string, int>();
        public static int GetPlayerMoney(string playerID)
        {
            int value = 1000;
            if (!playerMoneyDic.TryGetValue(playerID, out value))
            {
                playerMoneyDic.Add(playerID, value);
            }
            return value;
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
