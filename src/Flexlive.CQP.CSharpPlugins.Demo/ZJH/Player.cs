﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.ZJH
{
    public class Player
    {
        public string id;
        public List<Card> cards = new List<Card>();
        public int money { get { return Hall.GetPlayerMoney(id); } }
        public Room room;
        public bool isDroped = false;

        public void Follow(int price)
        {
            room.curPrice = price;
            room.totalPrice += price;
            Hall.SetPlayerMoney(id, money - price);
        }

        public void Win(int price)
        {
            Hall.SetPlayerMoney(id, money + price);

            room.curPrice = 0;
            room.totalPrice = 0;
        }

        public bool PK(Player target)
        {
            room.totalPrice += room.curPrice * 2;
            Hall.SetPlayerMoney(id, money - room.curPrice * 2);
            return false;
        }

        public void Update()
        {

        }

        public void Drop()
        {
            isDroped = true;
        }

        public void TakeCard(int cardID)
        {
            cards.Add(new Card(cardID));
        }

        public void Reply(string msg)
        {
            CQX.SendPrivateMessage(idL, msg);

        }

        public string name
        {
            get { return CQX.GetQQName(idL); }
        }

        private long idL
        {
            get { return long.Parse(id); }
        }
    }
}