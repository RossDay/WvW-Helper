using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2NET;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    class WvwStats
    {
        private static readonly string[] TeamList = new string[] { "Red", "Green", "Blue" };
        private static readonly string[] MapList = new string[] { null, "Red", "Green", "Blue", "EBG" };
        private static readonly int[] Intervals = new int[] { 5, 10, 15, 20, 30, 60, -1, -2, 0 };
        public readonly Dictionary<string, Objective> Objectives = new Dictionary<string, Objective>();

        public string APIStatus { get { return MatchHistoryCollection.APIStatus; } }

        private ITeamMapGetter Getter { get; set; }
        private IniFile Ini { get; set; }

        public string LeftTracking { get; private set; }
        public string RightTracking { get; private set; }
        public string TierTracking { get; private set; }
        public string CurrentMapDetails { get; private set; }
        public string GetWorldByTeam(string team)
        {
            return CurrentHistory.GetWorldByTeam(team);
        }
        private Dictionary<string, string> CurrentWorlds { get { return CurrentHistory.CurrentWorlds; } }
        private Dictionary<string, string> CurrentTeams { get { return CurrentHistory.CurrentTeams; } }
        public string OurTeam { get { return Getter.Team; } }
        public string LeftTeam { get { return CurrentHistory.LeftTeam; } }
        public string RightTeam { get { return CurrentHistory.RightTeam; } }
        public string OurWorld { get { return CurrentHistory.OurWorld; } }
        public string LeftWorld { get { return CurrentHistory.LeftWorld; } }
        public string RightWorld { get { return CurrentHistory.RightWorld; } }

        private MatchHistoryCollection CurrentHistory;
        public Match GetHistoryMatch(int interval)
        {
            return CurrentHistory.GetHistoryMatch(interval);
        }

        public DateTime LastUpdateTime { get { return CurrentHistory.LastUpdateTime; } }
        public DateTime SkirmishTime { get { return CurrentHistory.SkirmishTime; } }
        public DateTime TimezoneTime { get { return CurrentHistory.TimezoneTime; } }

        #region Constructor / Initialize
        public WvwStats(ITeamMapGetter getter, IniFile ini)
        {
            Getter = getter;
            Ini = ini;
            MatchHistory.maybeCreateDatabase();
        }

        private async Task populateObjectives()
        {
            var mapIds = new int[] { 95, 96, 1099, 38 };
            try
            {
                var GW2 = new GW2Bootstrapper();
                var repo = GW2.V2.WorldVersusWorld.Objectives.ForDefaultCulture();
                var allObjs = await repo.FindAllAsync();
                var objs = allObjs.Where(o => mapIds.Contains(o.Value.MapId));
                foreach (var kv in objs)
                    if (!Objectives.ContainsKey(kv.Key))
                        Objectives.Add(kv.Key, kv.Value);
            }
            catch (Exception e)
            {
            }
        }

        private string iniRead(string key, string defaultValue = "", bool setIfMissing = false)
        {
            if (Ini.KeyExists(key, "GW2"))
                return Ini.Read(key, "GW2");
            if (setIfMissing)
                Ini.Write(key, defaultValue, "GW2");
            return defaultValue;
        }

        public async Task Initialize()
        {
            CurrentHistory = await MatchHistoryCollection.CreateAsync();
            CurrentHistory.Getter = this.Getter;
        }
        #endregion

        public async Task<bool> maybeUpdateStats()
        {
            var matchAdded = await CurrentHistory.maybeUpdate();
            if (matchAdded)
            {
                await writeStats();
                return true;
            }
            return false;
        }

        public async Task<bool> setMatchup(string matchupId)
        {
            if (CurrentHistory.CurrentMatchup.Equals(matchupId))
                return false;

            CurrentHistory.CurrentMatchup = matchupId;
            await writeStats();

            Ini.Write("OurWorld", CurrentHistory.OurWorld, "GW2");
            Ini.Write("LeftWorld", CurrentHistory.LeftWorld, "GW2");
            Ini.Write("RightWorld", CurrentHistory.RightWorld, "GW2");
            Ini.Write("RedWorld", CurrentHistory.GetWorldByTeam("Red"), "GW2");
            Ini.Write("GreenWorld", CurrentHistory.GetWorldByTeam("Green"), "GW2");
            Ini.Write("BlueWorld", CurrentHistory.GetWorldByTeam("Blue"), "GW2");

            return true;
        }

        private async Task writeStats()
        {
            if (Objectives.Count == 0)
                await populateObjectives();

            await Task.Run(() =>
            {
                foreach (var i in Intervals)
                {
                    writeRatioMessages(i);
                    writeKillDeathStats(i);
                    writeOurDeltaRatioMessages(i);
                    foreach (var m in MapList)
                    {
                        var details = writeMapDetailStats(m, i);

                        if (m != null && m.Equals(Getter.Map) && i == 10)
                            CurrentMapDetails = details;
                    }
                }
                writeOurMapRatioMessages();
                LeftTracking = writeTeamTracking(CurrentHistory.LeftTeam);
                RightTracking = writeTeamTracking(CurrentHistory.RightTeam);
                writeTierTracking();
            });
        }

        #region Writing OurStats
        private void writeKillDeathStats(int interval)
        {
            var currentMatch = CurrentHistory.GetHistoryMatch(0);

            Match deltaMatch = null;
            var intervalKey = "0";
            var intervalDesc = "Total";
            if (interval > 0)
            {
                deltaMatch = CurrentHistory.GetHistoryMatch(interval);
                intervalKey = interval.ToString();
                intervalDesc = "Last " + intervalKey + "m";
            }
            else if (interval == -1)
            {
                deltaMatch = CurrentHistory.SkirmishMatch;
                intervalKey = "Skirmish";
                intervalDesc = "Skirmish";
            }
            else if (interval == -2)
            {
                deltaMatch = CurrentHistory.TimezoneMatch;
                intervalKey = "Timezone";
                intervalDesc = "Timezone";
            }

            var builder = new StringBuilder();
            foreach (string map in MapList)
            {
                var key = "";
                if (map == null)
                {
                    builder.Append("All Maps");
                    key = "Total";
                }
                else if (map.Equals("EBG"))
                {
                    builder.Append("EBG");
                    key = "EBG";
                }
                else
                {
                    builder.Append(CurrentWorlds[map]).Append("BL");
                    key = CurrentTeams[map];
                }
                builder.Append(" ");
                builder.Append(intervalDesc);
                builder.Append(" Stats for ");
                builder.Append(OurWorld).Append(": Kills = ");
                builder.Append(currentMatch.GetDeltaKillsFor(deltaMatch, OurTeam, map));
                builder.Append(", Deaths = ");
                builder.Append(currentMatch.GetDeltaDeathsFor(deltaMatch, OurTeam, map));

                var msg = builder.ToString();
                builder.Clear();

                Ini.Write(key + "Stats" + intervalKey, "\"" + msg + "\"", "OurStats");
            }
        } 
        #endregion

        #region Writing Ratios
        private void writeRatioMessages(int interval)
        {
            var currentMatch = CurrentHistory.GetHistoryMatch(0);

            Match deltaMatch = null;
            var intervalKey = "0";
            var intervalDesc = "Total";
            if (interval > 0)
            {
                deltaMatch = CurrentHistory.GetHistoryMatch(interval);
                intervalKey = interval.ToString();
                intervalDesc = "Last " + intervalKey + "m";
            }
            else if (interval == -1)
            {
                deltaMatch = CurrentHistory.SkirmishMatch;
                intervalKey = "Skirmish";
                intervalDesc = "Skirmish";
            }
            else if (interval == -2)
            {
                deltaMatch = CurrentHistory.TimezoneMatch;
                intervalKey = "Timezone";
                intervalDesc = "Timezone";
            }

            var builder = new StringBuilder();
            foreach (string map in MapList)
            {
                var key = "";
                if (map == null)
                {
                    builder.Append("All Maps");
                    key = "Total";
                }
                else if (map.Equals("EBG"))
                {
                    builder.Append("EBG");
                    key = "EBG";
                }
                else
                {
                    builder.Append(CurrentWorlds[map]).Append("BL");
                    key = CurrentTeams[map];
                }
                builder.Append(" ");
                builder.Append(intervalDesc);
                builder.Append(" KDR: ");
                builder.Append(OurWorld).Append(" = ");
                builder.Append(currentMatch.GetDeltaKDR(deltaMatch, OurTeam, map));
                builder.Append(", ");
                builder.Append(LeftWorld).Append(" = ");
                builder.Append(currentMatch.GetDeltaKDR(deltaMatch, LeftTeam, map));
                builder.Append(", ");
                builder.Append(RightWorld).Append(" = ");
                builder.Append(currentMatch.GetDeltaKDR(deltaMatch, RightTeam, map));

                var msg = builder.ToString();
                builder.Clear();

                Ini.Write(key + "KDR" + intervalKey, "\"" + msg + "\"", "WVW");
            }
        }

        private void writeOurDeltaRatioMessages(int interval)
        {
            var currentMatch = CurrentHistory.GetHistoryMatch(0);
            //var deltaMatch = (interval == 0 ? null : GetHistoryMatch(interval));
            Match deltaMatch = null;
            var intervalKey = "0";
            var intervalDesc = "Total";
            if (interval > 0)
            {
                deltaMatch = CurrentHistory.GetHistoryMatch(interval);
                intervalKey = interval.ToString();
                intervalDesc = "Last " + intervalKey + "m";
            }
            else if (interval == -1)
            {
                deltaMatch = CurrentHistory.SkirmishMatch;
                intervalKey = "Skirmish";
                intervalDesc = "Skirmish";
            }
            else if (interval == -2)
            {
                deltaMatch = CurrentHistory.TimezoneMatch;
                intervalKey = "Timezone";
                intervalDesc = "Timezone";
            }

            var builder = new StringBuilder();
            builder.Append(OurWorld).Append(" ");
            builder.Append(intervalDesc);
            builder.Append(" KDR: ");
            foreach (string map in MapList)
            {
                if (map == null)
                    builder.Append("AllMaps");
                else if (map.Equals("EBG"))
                    builder.Append("EBG");
                else
                    builder.Append(CurrentWorlds[map]).Append("BL");
                builder.Append(" = ");
                builder.Append(currentMatch.GetDeltaKDR(deltaMatch, OurTeam, map));
                builder.Append(", ");
            }
            builder.Length -= 2;

            var msg = builder.ToString();
            builder.Clear();

            Ini.Write("OurKDR" + intervalKey, "\"" + msg + "\"", "OurDeltas");
        }

        private void writeOurMapRatioMessages()
        {
            var currentMatch = CurrentHistory.GetHistoryMatch(0);

            var builder = new StringBuilder();
            foreach (string map in MapList)
            {
                builder.Append(OurWorld).Append(" KDR on ");
                var key = "";
                if (map == null)
                {
                    builder.Append("All Maps");
                    key = "Total";
                }
                else if (map.Equals("EBG"))
                {
                    builder.Append("EBG");
                    key = "EBG";
                }
                else
                {
                    builder.Append(CurrentWorlds[map]).Append("BL");
                    key = CurrentTeams[map];
                }
                builder.Append(": ");
                foreach (var interval in Intervals)
                {
                    if (interval == 10 || interval == 20)
                        continue;

                    //var deltaMatch = (interval == 0 ? null : GetHistoryMatch(interval));
                    Match deltaMatch = null;
                    var intervalDesc = "Total";
                    if (interval > 0)
                    {
                        deltaMatch = CurrentHistory.GetHistoryMatch(interval);
                        intervalDesc = interval.ToString() + "m";
                    }
                    else if (interval == -1)
                    {
                        deltaMatch = CurrentHistory.SkirmishMatch;
                        intervalDesc = "Skirmish";
                    }
                    else if (interval == -2)
                    {
                        deltaMatch = CurrentHistory.TimezoneMatch;
                        intervalDesc = "Timezone";
                    }

                    builder.Append(intervalDesc);
                    builder.Append(" = ");
                    builder.Append(currentMatch.GetDeltaKDR(deltaMatch, OurTeam, map));
                    builder.Append(", ");
                }
                builder.Length -= 2;

                var msg = builder.ToString();
                builder.Clear();

                Ini.Write("Our" + key + "KDR", "\"" + msg + "\"", "OurMaps");
            }
        }
        #endregion

        private void writeTierTracking()
        {
            var builder = new StringBuilder();
            Match currentMatch;
            Match deltaMatch;

            for (var tier = 1; tier <= 3; tier++)
            {
                builder.Append("Tier ").AppendLine(tier.ToString()).AppendLine("-----------------------------");
                var m = CurrentHistory.GetMatchupFor("1-"+tier.ToString());
                currentMatch = m.GetHistoryMatch(0);
                deltaMatch = m.GetHistoryMatch(10);
                foreach (string map in MapList)
                {
                    if (map == null)
                        continue;

                    if (map.Equals("EBG"))
                        builder.Append("EBG");
                    else
                        builder.Append(map[0]).Append(" ").Append(m.CurrentWorlds[map]).Append("BL");
                    builder.Append(": ");
                    foreach (string team in TeamList)
                    {
                        builder.Append(m.CurrentWorlds[team]).Append(" = ");
                        builder.Append(currentMatch.GetDeltaKDR(deltaMatch, team, map));
                        builder.Append(", ");
                    }
                    builder.Length -= 2;
                    builder.AppendLine("");
                }
                builder.AppendLine("");
            }
            TierTracking = builder.ToString();
        }

        private String GetObjectiveShortName(string objectiveId)
        {
            Objective o;
            if (Objectives.TryGetValue(objectiveId, out o))
                return o.GetShortName();
            return objectiveId;
        }

        #region Writing Tracking
        private static string[] TrackedObjectiveTypes = new string[] { "Castle", "Keep", "Tower", "Camp" };
        private string writeTeamTracking(string team)
        {
            var match = CurrentHistory.GetHistoryMatch(0);
            var delta = CurrentHistory.GetHistoryMatch(10);
            var result = new StringBuilder();

            result.Append("Tracking " + CurrentWorlds[team] + " with 10m stats...");
            Ini.Write(CurrentTeams[team] + "1", "\"" + result.ToString() + "\"", "WVWTracking");
            result.AppendLine();

            var i = 2;
            foreach (var map in new string[] { "Red", "Green", "Blue", "EBG" })
            {
                var mapObjName = (map.Equals("EBG") ? "Center" : map + "Home");
                var objs = match.Maps.First(m => m.Type.Equals(mapObjName))
                    .Objectives.Where(o => TrackedObjectiveTypes.Contains(o.Type) && o.Owner.ToString().Equals(team) && o.GetMinutesHeld() < 15)
                    .OrderByDescending(o => Convert.ToDateTime(o.LastFlipped))
                    .Take(3).ToList();

                var mapName = (map.Equals("EBG") ? map : CurrentWorlds[map] + "BL");

                var kills = match.GetDeltaKillsFor(delta, team, map);
                var deaths = match.GetDeltaDeathsFor(delta, team, map);

                var s = new StringBuilder();
                s.Append(mapName)
                    .Append(": K=")
                    .Append(kills)
                    .Append(", D=")
                    .Append(deaths)
                    .Append(", Flips=");
                if (objs.Count == 0)
                    s.Append("None");
                else
                {
                    foreach (var o in objs)
                        s.Append(GetObjectiveShortName(o.ObjectiveId))
                            .Append(" (")
                            .Append(Math.Round(Convert.ToDecimal(o.GetMinutesHeld()), 1))
                            .Append("m), ");
                    s.Length -= 2;
                }
                result.AppendLine(s.ToString());

                Ini.Write(CurrentTeams[team] + (i++).ToString(), "\"" + s.ToString() + "\"", "WVWTracking");
            }

            return result.ToString();
        }
        #endregion

        #region Writing MapDetails
        private string writeMapDetailStats(string map, int interval)
        {
            var match = CurrentHistory.GetHistoryMatch(0);
            Match delta = null;
            var intervalKey = "0";
            var intervalDesc = "Total";
            if (interval > 0)
            {
                delta = CurrentHistory.GetHistoryMatch(interval);
                intervalKey = interval.ToString();
                intervalDesc = "Last " + intervalKey + "m";
            }
            else if (interval == -1)
            {
                delta = CurrentHistory.SkirmishMatch;
                intervalKey = "Skirmish";
                intervalDesc = "Skirmish";
            }
            else if (interval == -2)
            {
                delta = CurrentHistory.TimezoneMatch;
                intervalKey = "Timezone";
                intervalDesc = "Timezone";
            }


            var result = new StringBuilder();

            var section = "WVWMapDetails" + intervalKey.ToString();
            String key, mapName, mapObjName;
            if (String.IsNullOrEmpty(map))
            {
                key = "Total";
                mapName = "All Maps";
                mapObjName = null;
            }
            else if (map.Equals("EBG"))
            {
                key = "EBG";
                mapName = "EBG";
                mapObjName = "Center";
            }
            else
            {
                key = CurrentTeams[map];
                mapName = CurrentWorlds[map] + "BL";
                mapObjName = map + "Home";
            }

            result.Append(intervalDesc).Append(" details for " + mapName + "...");
            Ini.Write(key + "1", "\"" + result.ToString() + "\"", section);
            result.AppendLine();

            List<MatchObjective> allObjectives = null;
            if (mapObjName != null && interval > 0 && interval <= 15)
                allObjectives = match.Maps.First(m => m.Type.Equals(mapObjName))
                    .Objectives.Where(o => TrackedObjectiveTypes.Contains(o.Type) && o.GetMinutesHeld() < 15)
                    .OrderByDescending(o => Convert.ToDateTime(o.LastFlipped))
                    .ToList();

            var i = 2;
            foreach (var team in TeamList)
            {
                var kills = match.GetDeltaKillsFor(delta, team, map);
                var deaths = match.GetDeltaDeathsFor(delta, team, map);

                var s = new StringBuilder();
                s.Append(CurrentWorlds[team])
                    .Append(": K=")
                    .Append(kills)
                    .Append(", D=")
                    .Append(deaths);

                if (allObjectives != null)
                {
                    var objs = allObjectives.Where(o => o.Owner.ToString().Equals(team)).Take(3).ToList();

                    s.Append(", Flips=");
                    if (objs.Count == 0)
                        s.Append("None");
                    else
                    {
                        foreach (var o in objs)
                            s.Append(GetObjectiveShortName(o.ObjectiveId))
                                .Append(" (")
                                .Append(Math.Round(Convert.ToDecimal(o.GetMinutesHeld()), 1))
                                .Append("m), ");
                        s.Length -= 2;
                    }
                }
                else
                {
                    s.Append(", KDR=")
                        .Append(match.GetDeltaKDR(delta, team, map));
                }
                result.AppendLine(s.ToString());

                Ini.Write(key + (i++).ToString(), "\"" + s.ToString() + "\"", section);
            }

            return result.ToString();
        }

        #endregion
    }
}
