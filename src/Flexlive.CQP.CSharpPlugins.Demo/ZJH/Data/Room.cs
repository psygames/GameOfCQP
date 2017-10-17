using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace RD.ZJH
{
    public class Room
    {
        const int TURN_CD = 30;

        public string name;
        public State state = State.Ready;
        public List<Player> players = new List<Player>();

        private float createTime = 0;
        private float m_turnCD = 0;
        private MoneyPool moneyPool = new MoneyPool();

        public Room()
        {
            createTime = Time.current;
        }

        public void Reset()
        {
            state = State.Ready;
            int num = 1;
            foreach (var player in players)
            {
                player.Reset(num++);
            }

            SendRoundBegin();
        }

        public void Join(string playerID)
        {
            var player = CreatePlayer(playerID);
            players.Add(player);
        }

        public Player whoseTurn { get; private set; }

        public void Ready(string playerID)
        {
            if (state == State.Ready
                || state == State.Dismiss)
            {
                var player = GetPlayer(playerID);
                player.Ready();
                SendReady(player);
                CheckStart();
            }
        }

        public void Quit(string playerID)
        {
            if (state == State.Ready
                || state == State.Dismiss)
            {
                var quiter = GetPlayer(playerID);
                SendQuit(quiter);
                players.Remove(quiter);
                state = State.Dismiss;
            }
        }

        public void Follow(string playerID)
        {
            Follow(playerID, -1);
        }

        public void Follow(string playerID, int price)
        {
            if (whoseTurn.id != playerID || whoseTurn.money < price)
                return;
            if (price <= moneyPool.price)
                price = -1;
            else if (price >= MoneyPool.MAX_PRICE)
                price = MoneyPool.MAX_PRICE;

            moneyPool.Follow(playerID, price);
            SendFollow(whoseTurn, moneyPool.price);
            TurnNext();
        }

        public void PK(string fromID, string toID)
        {
            if (state != State.Gaming)
                return;

            var from = GetPlayer(fromID);
            var to = GetPlayer(toID);

            int cost = moneyPool.price * 2;
            if (whoseTurn != from || to == null || to.isDroped || from.money < cost)
                return;

            bool isWin = from.PK(to);
            if (isWin) to.Drop();
            else from.Drop();
            SendPK(from, to);
            CheckEnd();
            TurnNext();
        }

        private void CheckStart()
        {
            if (state == State.Ready
                && players.All(a => a.isReady))
            {
                Start();
            }
        }

        private void Start()
        {
            state = State.Gaming;
            moneyPool.Reset();
            moneyPool.TakeBase(this);
            whoseTurn = players.First();
            m_turnCD = TURN_CD;
            SendCards();
            SendStatusToAll();
        }

        private void CheckEnd()
        {
            if (state != State.Gaming || players.Count(a => !a.isDroped) > 1)
                return;
            var winner = players.Find(a => a.isDroped == false);
            SendResult(winner, moneyPool.total);
            moneyPool.RewardTo(winner.id);
            Reset();
        }

        private void TurnNext()
        {
            if (state != State.Gaming)
                return;

            var playingPlayers = players.FindAll(a => a.isDroped == false);
            int index = playingPlayers.IndexOf(whoseTurn);
            index = (index + 1) % playingPlayers.Count;
            whoseTurn = playingPlayers[index];
            m_turnCD = TURN_CD;
            SendStatusToAll();
        }

        public Player GetPlayer(string playerID)
        {
            return players.Find(a => a.id == playerID);
        }

        private Player CreatePlayer(string id)
        {
            Player player = new Player();
            player.id = id;
            player.room = this;
            return player;
        }



        Random rand = new Random();
        private void SendCards()
        {
            List<int> ids = new List<int>();

            //TODO: jk
            for (int i = 1; i <= 52; i++) ids.Add(i);

            for (int i = 0; i < 3; i++)
            {
                foreach (var player in players)
                {
                    var index = rand.Next(0, ids.Count);
                    player.TakeCard(ids[index]);
                    ids.RemoveAt(index);
                }
            }
        }


        public void Drop(string playerID)
        {
            var player = GetPlayer(playerID);
            player.Drop();
            SendDrop(player);
            if (whoseTurn == player)
                TurnNext();
            CheckEnd();
        }

        public void Update()
        {
            CheckStart();
            CheckEnd();
            UpdateTurn();
        }

        private float m_lastTurnCD = 0;

        private void UpdateTurn()
        {
            if (state != State.Gaming)
                return;
            m_turnCD -= Time.delta;

            if (m_turnCD < 10
                && (int)m_lastTurnCD != (int)m_turnCD
                && ((int)m_turnCD % 2) == 0)
            {
                SendTurnCD((int)m_turnCD);
            }

            if (m_turnCD < 0)
            {
                //Atuo Drop
                Drop(whoseTurn.id);
            }

            m_lastTurnCD = m_turnCD;
        }

        public enum State
        {
            Ready,
            Gaming,
            Dismiss,
        }

        private void SendStatusToAll()
        {
            foreach (var player in players)
            {
                string msg = status;
                if (state == State.Gaming)
                    msg += "\nYour Cards: " + player.cardsMsg;
                foreach (var _tp in players)
                {
                    msg += "\n{0}".FormatStr(_tp.commonStatus);
                }
                player.SendMsg(msg);
            }
        }

        public void SendMsgToAll(string msg)
        {
            foreach (var player in players)
            {
                player.SendMsg(msg);
            }
        }
        public void SendPK(Player from, Player to)
        {
            string msg = "{0} PK {1} : ".FormatStr(from.name, to.name);
            if (from.PK(to))
                msg += "WIN";
            else
                msg += "LOSE";
            SendMsgToAll(msg);

            string privateMsg = "{0} Cards: {1}\n{2} Cards: {3}"
                .FormatStr(from.name, from.cardsMsg, to.name, to.cardsMsg);
            from.SendMsg(privateMsg);
            to.SendMsg(privateMsg);
        }

        public void SendFollow(Player player, int price)
        {
            SendMsgToAll("{0} Follow {1}.".FormatStr(player.name, price));
        }

        public void SendDrop(Player player)
        {
            SendMsgToAll("{0} Droped.".FormatStr(player.name));
        }
        public void SendResult(Player winner, int reward)
        {
            SendMsgToAll("{0} Win this Round! Get Reward:{1}".FormatStr(winner.name, reward));
        }

        private void SendRoundBegin()
        {
            SendStatusToAll();
            SendMsgToAll("please ready or quit.");
        }

        private void SendReady(Player player)
        {
            SendMsgToAll("{0} is Ready!".FormatStr(player.name));
        }

        public void SendQuit(Player player)
        {
            foreach (var _p in players)
            {
                if (_p == player)
                {
                    _p.SendMsg("Quit Succeed. zjh to rematch...");
                }
                else
                {
                    string msg = "{0} Quit the Game, if you are Ready we will rematch game for you!"
                        .FormatStr(player.name);
                    _p.SendMsg(msg);
                }
            }
        }

        public void SendTurnCD(int cd)
        {
            string msg = whoseTurn.name + " Turn CD: " + cd;
            SendMsgToAll(msg);
        }

        public string status
        {
            get
            {
                if (state == State.Ready)
                {
                    return "Room[{0}] Wait For Ready...".FormatStr(name);
                }
                else if (state == State.Gaming)
                {
                    return "TURN: {0}\nMONEY POOL ** {1} **"
                    .FormatStr(whoseTurn.name, moneyPool.status);
                }
                else if (state == State.Dismiss)
                {
                    return "Room[{0}] Dismiss.".FormatStr(name);
                }
                return "error status.";
            }
        }



    }

    public class MoneyPool
    {
        const int BASE_PRICE = 1;
        public const int MAX_PRICE = 100;
        public int price { get; private set; }
        public int total { get; private set; }

        public void Reset()
        {
            price = BASE_PRICE;
        }

        public void TakeBase(Room room)
        {
            foreach (var player in room.players)
            {
                Follow(player.id, BASE_PRICE);
            }
        }

        public void Follow(string playerID, int price = -1)
        {
            if (price == -1)
            {
                HallProxy.IncrPlayerMoney(playerID, -this.price);
            }
            else
            {
                this.price = price;
                HallProxy.IncrPlayerMoney(playerID, -this.price);
            }

            total += this.price;
        }

        public void PK(string fromID)
        {
            HallProxy.IncrPlayerMoney(fromID, -price * 2);
            total += price * 2;
        }

        public void RewardTo(string playerID)
        {
            HallProxy.IncrPlayerMoney(playerID, total);
            total = 0;
        }

        public string status
        {
            get
            {
                return "TOTAL: {0}   PRICE: {1}".FormatStr(total, price);
            }
        }
    }
}
