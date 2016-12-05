using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

        public string OurTeam { get { return Getter.Team; } }
        public string LeftTeam { get; private set; }
        public string RightTeam { get; private set; }
        public string OurWorld { get; private set; }
        public string LeftWorld { get; private set; }
        public string RightWorld { get; private set; }
        public string RedWorld { get; private set; }
        public string GreenWorld { get; private set; }
        public string BlueWorld { get; private set; }

        private MatchHistory MatchHistory;
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
            MatchHistory = new MatchHistory();
            maybeCreateDatabase();
            initialize();
        }

        public void reset()
        {
            MatchHistory.clear();
            initialize();
        }

        private void initialize()
        {
            var matchWorlds = CurrentMatch.Worlds;

            var worldRepo = GW2.V2.Worlds.ForDefaultCulture();
            var worlds = new World[] {
                worldRepo.Find(matchWorlds.Red),
                worldRepo.Find(matchWorlds.Green),
                worldRepo.Find(matchWorlds.Blue)
            };

            RedWorld = worlds[0].AbbreviatedName;
            GreenWorld = worlds[1].AbbreviatedName;
            BlueWorld = worlds[2].AbbreviatedName;

            var index = Array.IndexOf(TeamList, OurTeam);

            OurWorld = worlds[index].AbbreviatedName;
            LeftTeam = TeamList[(index - 1 + TeamList.Length) % TeamList.Length];
            RightTeam = TeamList[(index + 1) % TeamList.Length];
            LeftWorld = worlds[(index - 1 + TeamList.Length) % TeamList.Length].AbbreviatedName;
            RightWorld = worlds[(index + 1) % TeamList.Length].AbbreviatedName;
            Ini.Write("LeftTeam", LeftTeam, "GW2");
            Ini.Write("RightTeam", RightTeam, "GW2");
            Ini.Write("LeftWorld", LeftWorld, "GW2");
            Ini.Write("RightWorld", RightWorld, "GW2");

            CurrentTeams[OurTeam] = "Our";
            CurrentTeams[LeftTeam] = "Left";
            CurrentTeams[RightTeam] = "Right";
        }

        public bool maybeUpdateStats()
        {
            if (MatchHistory.maybeAdd(CurrentMatch))
            {
                writeStats();
                writeToDatabase();
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

            write("OurCurrentScore", scores.Get(OurTeam));
            write("LeftCurrentScore", scores.Get(LeftTeam));
            write("RightCurrentScore", scores.Get(RightTeam));
        }

        private void writeDeltaScores(int interval)
        {
            var currentScores = MatchHistory.GetHistoryMatch(0).Scores;
            var deltaScores = GetHistoryMatch(interval).Scores;

            var intervalString = interval.ToString();

            var key = "OurScore" + intervalString;
            var current = currentScores.Get(OurTeam);
            var delta = deltaScores.Get(OurTeam);
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

        public String getStringDump()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            var i = 0;
            foreach (var m in MatchHistory)
            {
                builder.AppendFormat("Match #{0}: {1}\n", i, m.Key);
                builder.AppendFormat(" {0}: K={1}, D={2}, KDR={3}\n", OurWorld, m.Value.Kills.Get(OurTeam), m.Value.Deaths.Get(OurTeam), m.Value.GetKDR(OurTeam));
                builder.AppendFormat(" {0}: K={1}, D={2}, KDR={3}\n", LeftWorld, m.Value.Kills.Get(LeftTeam), m.Value.Deaths.Get(LeftTeam), m.Value.GetKDR(LeftTeam));
                builder.AppendFormat(" {0}: K={1}, D={2}, KDR={3}\n", RightWorld, m.Value.Kills.Get(RightTeam), m.Value.Deaths.Get(RightTeam), m.Value.GetKDR(RightTeam));
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private void writeToDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=C:\rday\wvwhistory.sqlite;Version=3"))
            {
                conn.Open();
                var sql = String.Format("INSERT INTO stats VALUES ( {0} )", GetDatabaseRowValues());
                using (SQLiteCommand comm = new SQLiteCommand(sql, conn))
                {
                    comm.ExecuteNonQuery();
                }
            }
        }

        private string GetDatabaseRowValues()
        {
            var match = MatchHistory.GetHistoryMatch(0);
            var ts = MatchHistory.LastUpdateTime;

            var row = new object[89];

            row[0] = ts.Date.ToString("yyyyMMdd");
            row[1] = ts.ToString("HH:mm:ss");
            row[2] = RedWorld;
            row[3] = GreenWorld;
            row[4] = BlueWorld;
            row[5] = match.Scores.Red;
            row[6] = match.Scores.Green;
            row[7] = match.Scores.Blue;
            // red team stats
            row[8] = match.Kills.Red;
            row[9] = match.Deaths.Red;
            row[10] = match.GetMap("Red").Scores.Red;
            row[11] = match.GetMap("Red").Kills.Red;
            row[12] = match.GetMap("Red").Deaths.Red;
            row[13] = match.GetMap("Green").Scores.Red;
            row[14] = match.GetMap("Green").Kills.Red;
            row[15] = match.GetMap("Green").Deaths.Red;
            row[16] = match.GetMap("Blue").Scores.Red;
            row[17] = match.GetMap("Blue").Kills.Red;
            row[18] = match.GetMap("Blue").Deaths.Red;
            row[19] = match.GetMap("EBG").Scores.Red;
            row[20] = match.GetMap("EBG").Kills.Red;
            row[21] = match.GetMap("EBG").Deaths.Red;
            // green team stats
            row[22] = match.Kills.Green;
            row[23] = match.Deaths.Green;
            row[24] = match.GetMap("Red").Scores.Green;
            row[25] = match.GetMap("Red").Kills.Green;
            row[26] = match.GetMap("Red").Deaths.Green;
            row[27] = match.GetMap("Green").Scores.Green;
            row[28] = match.GetMap("Green").Kills.Green;
            row[29] = match.GetMap("Green").Deaths.Green;
            row[30] = match.GetMap("Blue").Scores.Green;
            row[31] = match.GetMap("Blue").Kills.Green;
            row[32] = match.GetMap("Blue").Deaths.Green;
            row[33] = match.GetMap("EBG").Scores.Green;
            row[34] = match.GetMap("EBG").Kills.Green;
            row[35] = match.GetMap("EBG").Deaths.Green;
            // blue team stats
            row[36] = match.Kills.Blue;
            row[37] = match.Deaths.Blue;
            row[38] = match.GetMap("Red").Scores.Blue;
            row[39] = match.GetMap("Red").Kills.Blue;
            row[40] = match.GetMap("Red").Deaths.Blue;
            row[41] = match.GetMap("Green").Scores.Blue;
            row[42] = match.GetMap("Green").Kills.Blue;
            row[43] = match.GetMap("Green").Deaths.Blue;
            row[44] = match.GetMap("Blue").Scores.Blue;
            row[45] = match.GetMap("Blue").Kills.Blue;
            row[46] = match.GetMap("Blue").Deaths.Blue;
            row[47] = match.GetMap("EBG").Scores.Blue;
            row[48] = match.GetMap("EBG").Kills.Blue;
            row[49] = match.GetMap("EBG").Deaths.Blue;


            var redObjs = match.GetObjectiveCounts("Red");
            var greenObjs = match.GetObjectiveCounts("Green");
            var blueObjs = match.GetObjectiveCounts("Blue");
            var ebgObjs = match.GetObjectiveCounts("EBG");

            // red team objectives
            row[50] = redObjs["Red"]["Keep"];
            row[51] = redObjs["Red"]["Tower"];
            row[52] = redObjs["Red"]["Camp"];
            row[53] = greenObjs["Red"]["Keep"];
            row[54] = greenObjs["Red"]["Tower"];
            row[55] = greenObjs["Red"]["Camp"];
            row[56] = blueObjs["Red"]["Keep"];
            row[57] = blueObjs["Red"]["Tower"];
            row[58] = blueObjs["Red"]["Camp"];
            row[59] = ebgObjs["Red"]["Castle"];
            row[60] = ebgObjs["Red"]["Keep"];
            row[61] = ebgObjs["Red"]["Tower"];
            row[62] = ebgObjs["Red"]["Camp"];
            // green team objectives
            row[63] = redObjs["Green"]["Keep"];
            row[64] = redObjs["Green"]["Tower"];
            row[65] = redObjs["Green"]["Camp"];
            row[66] = greenObjs["Green"]["Keep"];
            row[67] = greenObjs["Green"]["Tower"];
            row[68] = greenObjs["Green"]["Camp"];
            row[69] = blueObjs["Green"]["Keep"];
            row[70] = blueObjs["Green"]["Tower"];
            row[71] = blueObjs["Green"]["Camp"];
            row[72] = ebgObjs["Green"]["Castle"];
            row[73] = ebgObjs["Green"]["Keep"];
            row[74] = ebgObjs["Green"]["Tower"];
            row[75] = ebgObjs["Green"]["Camp"];
            // blue team objectives
            row[76] = redObjs["Blue"]["Keep"];
            row[77] = redObjs["Blue"]["Tower"];
            row[78] = redObjs["Blue"]["Camp"];
            row[79] = greenObjs["Blue"]["Keep"];
            row[80] = greenObjs["Blue"]["Tower"];
            row[81] = greenObjs["Blue"]["Camp"];
            row[82] = blueObjs["Blue"]["Keep"];
            row[83] = blueObjs["Blue"]["Tower"];
            row[84] = blueObjs["Blue"]["Camp"];
            row[85] = ebgObjs["Blue"]["Castle"];
            row[86] = ebgObjs["Blue"]["Keep"];
            row[87] = ebgObjs["Blue"]["Tower"];
            row[88] = ebgObjs["Blue"]["Camp"];

            for (var i = 0; i < 89; i++)
                if (row[i] is String)
                    row[i] = "'" + row[i] + "'";

            return String.Join(", ", row);
        }

        private void maybeCreateDatabase()
        {
            var db = @"C:\rday\wvwhistory.sqlite";
            if (!System.IO.File.Exists(db))
            {
                SQLiteConnection.CreateFile(db);
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=C:\rday\wvwhistory.sqlite;Version=3"))
                {
                    conn.Open();
                    var sql = @"
                        CREATE TABLE stats
                        (
                            dt varchar(8) NOT NULL
                            , ts varchar(20) NOT NULL
                            , red_world varchar(5) NOT NULL
                            , green_world varchar(5) NOT NULL
                            , blue_world varchar(5) NOT NULL
                            , red_total_score int NOT NULL
                            , green_total_score int NOT NULL
                            , blue_total_score int NOT NULL
                            , red_total_kills int NOT NULL
                            , red_total_deaths int NOT NULL
                            , red_red_score int NOT NULL
                            , red_red_kills int NOT NULL
                            , red_red_deaths int NOT NULL
                            , red_green_score int NOT NULL
                            , red_green_kills int NOT NULL
                            , red_green_deaths int NOT NULL
                            , red_blue_score int NOT NULL
                            , red_blue_kills int NOT NULL
                            , red_blue_deaths int NOT NULL
                            , red_ebg_score int NOT NULL
                            , red_ebg_kills int NOT NULL
                            , red_ebg_deaths int NOT NULL
                            , green_total_kills int NOT NULL
                            , green_total_deaths int NOT NULL
                            , green_red_score int NOT NULL
                            , green_red_kills int NOT NULL
                            , green_red_deaths int NOT NULL
                            , green_green_score int NOT NULL
                            , green_green_kills int NOT NULL
                            , green_green_deaths int NOT NULL
                            , green_blue_score int NOT NULL
                            , green_blue_kills int NOT NULL
                            , green_blue_deaths int NOT NULL
                            , green_ebg_score int NOT NULL
                            , green_ebg_kills int NOT NULL
                            , green_ebg_deaths int NOT NULL
                            , blue_total_kills int NOT NULL
                            , blue_total_deaths int NOT NULL
                            , blue_red_score int NOT NULL
                            , blue_red_kills int NOT NULL
                            , blue_red_deaths int NOT NULL
                            , blue_green_score int NOT NULL
                            , blue_green_kills int NOT NULL
                            , blue_green_deaths int NOT NULL
                            , blue_blue_score int NOT NULL
                            , blue_blue_kills int NOT NULL
                            , blue_blue_deaths int NOT NULL
                            , blue_ebg_score int NOT NULL
                            , blue_ebg_kills int NOT NULL
                            , blue_ebg_deaths int NOT NULL
                            , red_red_keeps int NOT NULL
                            , red_red_towers int NOT NULL
                            , red_red_camps int NOT NULL
                            , red_green_keeps int NOT NULL
                            , red_green_towers int NOT NULL
                            , red_green_camps int NOT NULL
                            , red_blue_keeps int NOT NULL
                            , red_blue_towers int NOT NULL
                            , red_blue_camps int NOT NULL
                            , red_ebg_castles int NOT NULL
                            , red_ebg_keeps int NOT NULL
                            , red_ebg_towers int NOT NULL
                            , red_ebg_camps int NOT NULL
                            , green_red_keeps int NOT NULL
                            , green_red_towers int NOT NULL
                            , green_red_camps int NOT NULL
                            , green_green_keeps int NOT NULL
                            , green_green_towers int NOT NULL
                            , green_green_camps int NOT NULL
                            , green_blue_keeps int NOT NULL
                            , green_blue_towers int NOT NULL
                            , green_blue_camps int NOT NULL
                            , green_ebg_castles int NOT NULL
                            , green_ebg_keeps int NOT NULL
                            , green_ebg_towers int NOT NULL
                            , green_ebg_camps int NOT NULL
                            , blue_red_keeps int NOT NULL
                            , blue_red_towers int NOT NULL
                            , blue_red_camps int NOT NULL
                            , blue_green_keeps int NOT NULL
                            , blue_green_towers int NOT NULL
                            , blue_green_camps int NOT NULL
                            , blue_blue_keeps int NOT NULL
                            , blue_blue_towers int NOT NULL
                            , blue_blue_camps int NOT NULL
                            , blue_ebg_castles int NOT NULL
                            , blue_ebg_keeps int NOT NULL
                            , blue_ebg_towers int NOT NULL
                            , blue_ebg_camps int NOT NULL
                            , PRIMARY KEY ( dt, ts )
                        )
                    ";
                    using (SQLiteCommand comm = new SQLiteCommand(sql, conn))
                    {
                        comm.ExecuteNonQuery();
                    }
                }

            }

        }

        private void foo()
        {
        }

    }
}
