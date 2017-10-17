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
        private const int MATCH_COUNT_MAX = 4;
        private const int MATCH_COUNT_MIN = 2;
        public const float USE_MATCH_MIN_CD = 60;

        private float m_useMinCD = USE_MATCH_MIN_CD;

        public void Add(string id)
        {
            if (!m_waitings.Contains(id))
                m_waitings.Add(id);
            m_useMinCD = 60;
        }

        public void Update()
        {
            CheckMatch();
        }

        private void CheckMatch()
        {
            m_useMinCD -= Time.delta;
            CheckMatch(MATCH_COUNT_MAX);
            if (m_useMinCD <= 0)
            {
                CheckMatch(MATCH_COUNT_MIN);
            }
        }

        private void CheckMatch(int count)
        {
            while (m_waitings.Count >= count)
            {
                var matched = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    matched.Add(m_waitings[0]);
                    m_waitings.RemoveAt(0);
                }
                m_matcheds.Add(matched);
            }
        }

        public List<string> GetMatched()
        {
            if (m_matcheds.Count > 0)
            {
                var tmp = m_matcheds[0];
                m_matcheds.RemoveAt(0);
                return tmp;
            }
            return null;
        }
    }
}
