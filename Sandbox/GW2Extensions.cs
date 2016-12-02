using System;
using System.Collections.Generic;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    static class GW2Extensions
    {
        private static readonly Dictionary<string, int> _MapDict = new Dictionary<string, int>()
        { { "Green", 95 }, { "Blue", 96 }, { "Red", 1099 }, { "EBG", 38 } };

        public static int Get(this Scoreboard scores, string key)
        {
            if (key.Equals("Red"))
                return scores.Red;
            if (key.Equals("Blue"))
                return scores.Blue;
            if (key.Equals("Green"))
                return scores.Green;
            return 0;
        }

        public static CompetitiveMap GetMap(this Match match, string map)
        {
            int mapId;
            if (!_MapDict.TryGetValue(map, out mapId))
                return null;

            foreach (var m in match.Maps)
                if (m.Id == mapId)
                    return m;

            return null;
        }

        public static int GetKillsFor(this Match match, string team, string map = null)
        {
            if (String.IsNullOrEmpty(map))
                return match.Kills.Get(team);
            else
                return match.GetMap(map).Kills.Get(team);
        }

        public static int GetDeathsFor(this Match match, string team, string map = null)
        {
            if (String.IsNullOrEmpty(map))
                return match.Deaths.Get(team);
            else
                return match.GetMap(map).Deaths.Get(team);
        }

        public static decimal GetKDR(this Match match, string team, string map = null)
        {
            var kills = match.GetKillsFor(team, map);
            var deaths = match.GetDeathsFor(team, map);

            if (deaths == 0)
                return Convert.ToDecimal(kills);
            return Math.Round(Convert.ToDecimal(kills) / Convert.ToDecimal(deaths), 1);
        }

        public static decimal GetDeltaKDR(this Match current, Match delta, string team, string map = null)
        {
            var killDelta = current.GetKillsFor(team, map) - delta.GetKillsFor(team, map);
            var deathDelta = current.GetDeathsFor(team, map) - delta.GetDeathsFor(team, map);

            if (deathDelta == 0)
                return Convert.ToDecimal(killDelta);
            return Math.Round(Convert.ToDecimal(killDelta) / Convert.ToDecimal(deathDelta), 1);
        }
    }
}
