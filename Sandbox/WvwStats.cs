using System;
using System.Collections.Generic;
using GW2NET;
using GW2NET.Worlds;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    class WvwStats
    {
        private static readonly string[] TeamList = new string[] { "Red", "Green", "Blue" };
        private static readonly string[] MapList = new string[] { null, "Red", "Green", "Blue", "EBG" };
        private readonly Dictionary<string, string> CurrentTeams = new Dictionary<string, string>();

        private GW2Bootstrapper GW2 { get; set; }
        private ITeamMapGetter Getter { get; set; }
        private IniFile Ini { get; set; }

        public string LeftTeam { get; private set; }
        public string RightTeam { get; private set; }
        public string LeftMapWorldName { get; private set; }
        public string RightMapWorldName { get; private set; }

        private MatchHistory MatchHistory = new MatchHistory();
        public Match GetHistoryMatch(int interval)
        {
            return MatchHistory.GetHistoryMatch(interval);
        }

        private Match CurrentMatch 
        {
            get { return GW2.V2.WorldVersusWorld.MatchesByWorld.Find(1008); }
        }

        public WvwStats(ITeamMapGetter getter, IniFile ini)
        {
            GW2 = new GW2Bootstrapper();
            Getter = getter;
            Ini = ini;
        }

        public void updateLeftRightWorldNames()
        {
            var matchWorlds = CurrentMatch.Worlds;

            var worldRepo = GW2.V2.Worlds.ForDefaultCulture();
            var worlds = new World[] {
                worldRepo.Find(matchWorlds.Red),
                worldRepo.Find(matchWorlds.Green),
                worldRepo.Find(matchWorlds.Blue)
            };

            var index = Array.IndexOf(TeamList, Getter.Team);

            LeftTeam = TeamList[(index - 1 + TeamList.Length) % TeamList.Length];
            RightTeam = TeamList[(index + 1) % TeamList.Length];
            LeftMapWorldName = worlds[(index - 1 + TeamList.Length) % TeamList.Length].AbbreviatedName;
            RightMapWorldName = worlds[(index + 1) % TeamList.Length].AbbreviatedName;
            Ini.Write("LeftWorld", LeftMapWorldName, "GW2");
            Ini.Write("RightWorld", RightMapWorldName, "GW2");

            CurrentTeams[Getter.Team] = "Our";
            CurrentTeams[LeftTeam] = "Left";
            CurrentTeams[RightTeam] = "Right";
        }

        public bool maybeUpdateStats()
        {
            if (MatchHistory.maybeAdd(CurrentMatch))
            { 
                writeStats();
                return true;
            }
            return true;
        }

        private void write(string key, string value, string section = "WVW")
        {
            Ini.Write(key, value, section);
        }
        private void write(string key, int value, string section = "WVW")
        {
            Ini.Write(key, value.ToString(), section);
        }
        private void write(string key, decimal value, string section = "WVW")
        {
            Ini.Write(key, value.ToString(), section);
        }

        private void writeStats()
        {
            writeCurrentScores();
            writeDeltaScores(10);
            writeDeltaScores(20);
            writeDeltaScores(30);
            writeDeltaScores(60);

            writeCurrentRatios();
            writeDeltaRatios(10);
            writeDeltaRatios(20);
            writeDeltaRatios(30);
            writeDeltaRatios(60);
        }

        private void writeCurrentScores()
        {
            var scores = MatchHistory.GetHistoryMatch(0).Scores;

            write("OurCurrentScore", scores.Get(Getter.Team));
            write("LeftCurrentScore", scores.Get(LeftTeam));
            write("RightCurrentScore", scores.Get(RightTeam));
        }

        private void writeDeltaScores(int interval)
        {
            var currentScores = MatchHistory.GetHistoryMatch(0).Scores;
            var deltaScores = GetHistoryMatch(interval).Scores;

            var intervalString = interval.ToString();

            var key = "OurScore" + intervalString;
            var current = currentScores.Get(Getter.Team);
            var delta = deltaScores.Get(Getter.Team);
            write(key, (current - delta).ToString());

            key = "LeftScore" + intervalString;
            current = currentScores.Get(LeftTeam);
            delta = deltaScores.Get(LeftTeam);
            write(key, (current - delta).ToString());

            key = "RightScore" + intervalString;
            current = currentScores.Get(RightTeam);
            delta = deltaScores.Get(RightTeam);
            write(key, (current - delta).ToString());
        }

        private void writeCurrentRatios()
        {
            var currentMatch = MatchHistory.GetHistoryMatch(0);

            foreach (string map in MapList)
            {
                foreach (var t in CurrentTeams)
                {
                    var key = t.Value + ( String.IsNullOrEmpty(map) ? "Total" : map ) + "KDR";
                    var value = currentMatch.GetKDR(t.Key, map);
                    write(key, value);
                }
            }
        }

        private void writeDeltaRatios(int interval)
        {
            var currentMatch = MatchHistory.GetHistoryMatch(0);
            var deltaMatch = GetHistoryMatch(interval);

            var intervalString = interval.ToString();

            foreach (string map in MapList)
            {
                foreach (var t in CurrentTeams)
                {
                    var key = t.Value + (String.IsNullOrEmpty(map) ? "Total" : map) + "KDR" + intervalString;
                    var value = currentMatch.GetDeltaKDR(deltaMatch, t.Key, map);
                    write(key, value);
                }
            }
        }

        private void Foo()
        {
            var currentMatch = MatchHistory.GetHistoryMatch(0);

            //currentMatch.GetMap("EBG").
        }

    }
}
