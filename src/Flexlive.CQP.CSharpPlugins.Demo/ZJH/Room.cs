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
		const int MIN_MEMBERS = 3;
		const int MAX_MEMBERS = 5;

		public string name;
		public State state = State.Ready;
		public List<Player> players = new List<Player>();

		public int curPrice = 0;
		public int totalPrice = 0;

		private float createTime = 0;

		public Room()
		{
			createTime = Time.current;
		}

		public void Join(string playerID)
		{
			var player = CreatePlayer(playerID);
			players.Add(player);
			whoseTurn = GetPlayer(playerID);
			ReplyStatusToAll();
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
		}

		public void Drop(string playerID)
		{
			GetPlayer(playerID).Drop();
			if (whoseTurn.id == playerID)
				TurnNext();
		}


		public void Update()
		{

		}

		public enum State
		{
			Ready,
			Playing,
			End,
		}

		private void ReplyStatusToAll()
		{
			foreach (var player in players)
			{
				player.Reply(status);
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
