using System;
using System.Collections.Generic;
using System.Linq;
using GW2NET.Common.Drawing;
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
            return Math.Round(Convert.ToDecimal(kills) / Convert.ToDecimal(deaths), 2);
        }

        public static int GetDeltaKillsFor(this Match match, Match delta, string team, string map = null)
        {
            var currentKills = match.GetKillsFor(team, map);

            if (delta == null)
                return currentKills;

            return currentKills - delta.GetKillsFor(team, map);
        }

        public static int GetDeltaDeathsFor(this Match match, Match delta, string team, string map = null)
        {
            var currentDeaths = match.GetDeathsFor(team, map);

            if (delta == null)
                return currentDeaths;

            return currentDeaths - delta.GetDeathsFor(team, map);
        }

        public static decimal GetDeltaKDR(this Match current, Match delta, string team, string map = null)
        {
            if (delta == null)
                return current.GetKDR(team, map);

            var killDelta = current.GetDeltaKillsFor(delta, team, map);
            var deathDelta = current.GetDeltaDeathsFor(delta, team, map);

            if (deathDelta == 0)
                return Convert.ToDecimal(killDelta);
            return Math.Round(Convert.ToDecimal(killDelta) / Convert.ToDecimal(deathDelta), 2);
        }

        public static Boolean IsEqualTo(this Match current, Match other)
        {
            if (!current.Kills.Equals(other.Kills) || !current.Deaths.Equals(other.Deaths))
                return false;

            var currentLastFlip = current.Maps.Max(m => m.Objectives.Max(o => Convert.ToDateTime(o.LastFlipped)));
            var otherLastFlip = other.Maps.Max(m => m.Objectives.Max(o => Convert.ToDateTime(o.LastFlipped)));

            return currentLastFlip.Equals(otherLastFlip);
        }

        public static Dictionary<string, Dictionary<string, int>> GetObjectiveCounts(this Match match, string map)
        {
            var objectives = new Dictionary<string, Dictionary<string, int>>();

            Dictionary<string, int> temp;

            temp = new Dictionary<string, int>();
            temp.Add("Castle", 0);
            temp.Add("Keep", 0);
            temp.Add("Tower", 0);
            temp.Add("Camp", 0);
            objectives.Add("Red", temp);
            temp = new Dictionary<string, int>();
            temp.Add("Castle", 0);
            temp.Add("Keep", 0);
            temp.Add("Tower", 0);
            temp.Add("Camp", 0);
            objectives.Add("Green", temp);
            temp = new Dictionary<string, int>();
            temp.Add("Castle", 0);
            temp.Add("Keep", 0);
            temp.Add("Tower", 0);
            temp.Add("Camp", 0);
            objectives.Add("Blue", temp);

            foreach (var o in match.GetMap(map).Objectives)
            {
                if (o.Owner == TeamColor.Unknown || o.Owner == TeamColor.Neutral || o.Type.Equals("Ruins"))
                    continue;
                objectives[o.Owner.ToString()][o.Type] += 1;
            }

            return objectives;
        }


        private static readonly Dictionary<string, Vector2D> MapCenters = new Dictionary<string, Vector2D>
        {
            { "RedHome", new Vector2D(10728, 10688) },
            { "GreenHome", new Vector2D(6906, 13530) },
            { "BlueHome", new Vector2D(14088, 12932) },
            { "Center", new Vector2D(10626, 14575) }
        };

        private static readonly Dictionary<string, string> EBGShortNames = new Dictionary<string, string>
        {
            { "Aldon's Ledge", "Aldon" },
            { "Overlook", "RedKeep" },
            { "Langor Gulch", "Langor" },
            { "Mendon's Gap", "Mendon" },
            { "Danelon Passage", "Danelon" },
            { "Stonemist Castle", "SMC" },
            { "Pangloss Rise", "Pangloss" },
            { "Durios Gulch", "Durios" },
            { "Veloka Slope", "Veloka" },
            { "Klovan Gully", "Klovan" },
            { "Jerrifer's Slough", "Jerri" },
            { "Speldan Clearcut", "Speldan" },
            { "Valley", "BlueKeep" },
            { "Wildcreek Run", "WC" },
            { "Quentin Lake", "QL" },
            { "Bravost Escarpment", "Bravost" },
            { "Ogrewatch Cut", "OW" },
            { "Golanta Clearing", "Golanta" },
            { "Umberglade Woods", "Umber" },
            { "Rogue's Quarry", "Rogue" },
            { "Anzalias Pass", "Anz" },
            { "Lowlands", "GreenKeep" },
        };

        public static string GetShortName(this Objective objective)
        {
            if (objective.MapType.EndsWith("Home"))
            {
                if (objective.Type.Equals("Tower"))
                    return GetTowerShortName(objective);
                else if (objective.Type.Equals("Camp"))
                    return GetCampShortName(objective);
                else if (objective.Type.Equals("Keep"))
                    return GetKeepShortName(objective);
                return objective.Name;
            }
            else
                return EBGShortNames[objective.Name];
        }

        private static string GetTowerShortName(Objective objective)
        {
            var center = MapCenters[objective.MapType];

            // North vs South
            var result = ((objective.MapCoordinates.Y < center.Y) ? "N" : "S");

            // East vs West
            result += ((objective.MapCoordinates.X < center.X) ? "W" : "E");

            return result + "T";
        }

        private static string GetCampShortName(Objective objective)
        {
            var center = MapCenters[objective.MapType];

            // North vs South
            var result = ((objective.MapCoordinates.Y < center.Y) ? "N" : "S");

            // is it N camp or S camp?
            if (Math.Abs(center.X - objective.MapCoordinates.X) < 200)
                return result + "C";

            // East vs West
            result += ((objective.MapCoordinates.X < center.X) ? "W" : "E");

            return result + "C";
        }

        private static string GetKeepShortName(Objective objective)
        {
            var center = MapCenters[objective.MapType];

            // is it garri?
            if (Math.Abs(center.X - objective.MapCoordinates.X) < 200)
                return "Garri";

            // East vs West
            return ((objective.MapCoordinates.X < center.X) ? "Bay" : "Hills");
        }

        public static double GetMinutesHeld(this MatchObjective objective)
        {
            return (DateTime.Now - Convert.ToDateTime(objective.LastFlipped)).TotalMinutes;
        }
    }
}