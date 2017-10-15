using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.ZJH
{
    public class MatchPool
    {
        public List<string> m_waitings = new List<string>();
        public List<List<string>> m_matcheds = new List<List<string>>();
        private const int MATCH_COUNT = 2;

        public void Add(string id)
        {
            m_waitings.Add(id);
        }

        public void Update()
        {
            CheckMatch();
        }

        private void CheckMatch()
        {
            while (m_waitings.Count >= MATCH_COUNT)
            {
                var matched = new List<string>();
                for (int i = 0; i < MATCH_COUNT; i++)
                {
                    matched.Add(m_waitings[0]);
                    m_waitings.RemoveAt(0);
                }
                m_matcheds.Add(matched);
            }
        }

        public List<string> GetMatched()
        {
            var tmp = m_matcheds[0];
            m_matcheds.RemoveAt(0);
            return tmp;
        }
    }
}
