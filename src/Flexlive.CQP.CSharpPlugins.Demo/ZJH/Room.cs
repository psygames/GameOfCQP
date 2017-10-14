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
		const int MIN_MEMBERS = 2;
		const int MAX_MEMBERS = 5;
		const int TURN_CD = 20;

		public string name;
		public State state = State.Ready;
		public List<Player> players = new List<Player>();

		public int curPrice = 0;
		public int totalPrice = 0;

		private float createTime = 0;
		private float m_turnCD = 0;

		public Room()
		{
			createTime = Time.current;
		}

		public void Join(string playerID)
		{
			var player = CreatePlayer(playerID);
			players.Add(player);
			whoseTurn = GetPlayer(playerID);
			SendStatusToAll();
		}

		public Player whoseTurn { get; private set; }

		public void Follow(string playerID)
		{
			Follow(playerID, curPrice);
		}

		public void Follow(string playerID, int price)
		{
			if (whoseTurn.id != playerID || whoseTurn.money < price)
				return;
			whoseTurn.Follow(price);
			TurnNext();
		}

		public void PK(string fromID, string toID)
		{
			var from = GetPlayer(fromID);
			var to = GetPlayer(toID);

			if (whoseTurn != from || to == null || from.money < curPrice * 2)
				return;

			bool isWin = from.PK(to);
			if (isWin) to.Drop();
			else from.Drop();
			CheckEnd();
			if (state != State.End)
			{
				TurnNext();
			}
		}

		private void CheckEnd()
		{
			if (players.Count(a => !a.isDroped) > 1)
				return;
			state = State.End;
			var winner = players.Find(a => a.isDroped = false);
			winner.Win(totalPrice);
		}

		private void TurnNext()
		{
			var playingPlayers = players.FindAll(a => a.isDroped = false);
			int index = playingPlayers.IndexOf(whoseTurn);
			index = (index + 1) % playingPlayers.Count;
			whoseTurn = playingPlayers[index];
			m_turnCD = TURN_CD;

			if (state == State.Playing)
			{
				SendMsgToAll(whoseTurn.name + " Turn");
			}
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


		private void CheckStart()
		{
			if (Time.current - createTime > 60 && players.Count >= MIN_MEMBERS
				|| players.Count >= MAX_MEMBERS)
			{
				state = State.Playing;
				SendCards();
			}
		}

		Random rand = new Random();
		private void SendCards()
		{
			List<int> ids = new List<int>();
			for (int i = 1; i <= 54; i++) ids.Add(i);

			for (int i = 0; i < 3; i++)
			{
				foreach (var player in players)
				{
					var index = rand.Next(0, ids.Count);
					player.TakeCard(ids[index]);
					ids.RemoveAt(index);
				}
			}
			foreach (var player in players)
			{
				ShowCards(player.id);
			}
		}


		public void ShowCards(string playerID)
		{
			var p = GetPlayer(playerID);
			p.SendMsg(p.cardsMsg);
		}

		public void ShowPK(Player from, Player to)
		{
			string msg = string.Format("{0} PK {1}\n({2} PK {3})\n", from.name, to.name, from.cardsMsg, to.cardsMsg);
			if (from.CheckPK(to))
				msg += "WIN";
			else
				msg += "LOSE";
			from.SendMsg(msg);
			to.SendMsg(msg);
		}

		public void Drop(string playerID)
		{
			GetPlayer(playerID).Drop();
			if (whoseTurn.id == playerID)
				TurnNext();
		}

		private float m_lastTurnCD = 0;
		public void Update()
		{
			if (state != State.Playing)
				return;
			m_turnCD -= Time.delta;

			if (m_turnCD < 10
				&& (int)m_lastTurnCD != (int)m_turnCD
				&& ((int)m_turnCD % 2) == 0)
			{
				string msg = whoseTurn.name + " LeftTime: " + (int)m_turnCD; e
				SendMsgToAll(msg);
			}

			if (m_turnCD < 0)
			{
				TurnNext();
			}

			m_lastTurnCD = m_turnCD;
		}

		public enum State
		{
			Ready,
			Playing,
			End,
		}

		private void SendStatusToAll()
		{
			SendMsgToAll(status);
		}

		public void SendMsgToAll(string msg)
		{
			foreach (var player in players)
			{
				player.SendMsg(msg);
			}
		}

		public string status
		{
			get
			{
				if (state == State.Ready)
				{
					return string.Format("waiting({0})...", players.Count);
				}
				else if (state == State.Playing)
				{
					return string.Format(
						"TURN: {0}\nAward: {1}\nPrice: {2}",
						whoseTurn.name,
						totalPrice,
						curPrice
						);
				}
				else if (state == State.End)
				{
					return "end.";
				}
				return "error status.";
			}
		}

	}
}
