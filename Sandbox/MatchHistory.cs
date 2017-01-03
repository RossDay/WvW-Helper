using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GW2NET;
using GW2NET.Worlds;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    [DataContract]
    class MatchHistory : IEnumerable<KeyValuePair<DateTime, Match>>
    {
        public static string APIStatus { get; private set; } = "N/A";
        private GW2Bootstrapper GW2 { get; set; } = new GW2Bootstrapper();

        private static readonly string[] TeamList = new string[] { "Red", "Green", "Blue" };

        public MatchHistory(ITeamMapGetter getter, String matchupId)
        {
            Getter = getter;
            MatchupId = matchupId;
            Matches = new LinkedList<KeyValuePair<DateTime, Match>>();
        }

        [DataMember]
        public Dictionary<string, string> CurrentTeams { get; private set; } = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> CurrentWorlds { get; private set; } = new Dictionary<string, string>();

        public ITeamMapGetter Getter { get; set; }
        public string OurTeam { get { return Getter.Team; } }

        [DataMember]
        public string MatchupId { get; set; }
        [DataMember]
        public string LeftTeam { get; private set; }
        [DataMember]
        public string RightTeam { get; private set; }
        [DataMember]
        public string OurWorld { get; private set; }
        [DataMember]
        public string LeftWorld { get; private set; }
        [DataMember]
        public string RightWorld { get; private set; }

        [DataMember]
        private LinkedList<KeyValuePair<DateTime, Match>> Matches { get; set; }
        [DataMember]
        public DateTime SkirmishTime { get; private set; }
        [DataMember]
        public Match SkirmishMatch { get; private set; }
        [DataMember]
        public DateTime TimezoneTime { get; private set; }
        [DataMember]
        public Match TimezoneMatch { get; private set; }

        public DateTime LastUpdateTime
        {
            get
            {
                if (Matches.Count == 0)
                    return DateTime.MinValue;
                return Matches.First.Value.Key;
            }
        }

        public string GetWorldByTeam(string team)
        {
            string world;
            if (CurrentWorlds.TryGetValue(team, out world))
                return world;
            return team;
        }

        public Match GetHistoryMatch(int interval)
        {
            if (Matches.Count == 0)
                return null;

            if (interval == 0)
                return Matches.First.Value.Value;

            var delta = Matches.First;
            var intervalTime = delta.Value.Key.AddMinutes(-1 * interval);
            var currentMatch = delta.Value.Value;

            while (delta.Next != null && delta.Next.Value.Key > intervalTime)
                delta = delta.Next;

            if (delta.Next == null)
                return delta.Value.Value;

            if ((intervalTime - delta.Next.Value.Key) < (delta.Value.Key - intervalTime))
                return delta.Next.Value.Value;
            else
                return delta.Value.Value;
        }

        private Task dbTask = null;
        public async Task<bool> maybeUpdate(Match match)
        {
            Task t = null;
            var needToSerialize = false;
            if (OurWorld == null || match.Scores.Red < Matches.First.Value.Value.Scores.Red)
            {
                needToSerialize = true;
                var now = DateTime.Now;
                t = maybeUpdateTeam(match);
                Matches.Clear();
                Matches.AddFirst(new KeyValuePair<DateTime, Match>(now, match));
                SkirmishTime = now;
                SkirmishMatch = match;
                TimezoneTime = now;
                TimezoneMatch = match;
            }
            else
                needToSerialize = await maybeAdd(match);

            if (t != null)
                await t;

            if (needToSerialize)
            {
                if (dbTask != null && !dbTask.IsCompleted)
                {
                    dbTask.Wait();
                    dbTask.Dispose();
                }
                dbTask = Task.Run(() => writeToDatabase());
            }

            return needToSerialize;
        }

        private async Task<bool> maybeAdd(Match match)
        {
            var now = DateTime.Now;
            var b = await Task.Run(() =>
            {
                var needToSerialize = false;
                if (!AreSameSkirmish(SkirmishTime, now))
                {
                    SkirmishTime = now;
                    SkirmishMatch = match;
                    needToSerialize = true;
                }
                if (!AreSameTimezone(TimezoneTime, now))
                {
                    TimezoneTime = now;
                    TimezoneMatch = match;
                    needToSerialize = true;
                }
                if (Matches.Count == 0 || ((now - LastUpdateTime).TotalSeconds >= 60.0 && !match.IsEqualTo(Matches.First.Value.Value)))
                {
                    if (Matches.Count == 61)
                        Matches.RemoveLast();
                    Matches.AddFirst(new KeyValuePair<DateTime, Match>(now, match));
                    needToSerialize = true;
                }
                return needToSerialize;
            });
            return b;
        }

        private async Task maybeUpdateTeam(Match currentMatch)
        {
            var matchWorlds = currentMatch.Worlds;

            var worldRepo = GW2.V2.Worlds.ForDefaultCulture();
            var worlds = new World[] {
                await worldRepo.FindAsync(matchWorlds.Red),
                await worldRepo.FindAsync(matchWorlds.Green),
                await worldRepo.FindAsync(matchWorlds.Blue)
            };

            CurrentWorlds["Red"] = worlds[0].AbbreviatedName;
            CurrentWorlds["Green"] = worlds[1].AbbreviatedName;
            CurrentWorlds["Blue"] = worlds[2].AbbreviatedName;

            var index = Array.IndexOf(TeamList, OurTeam);

            OurWorld = worlds[index].AbbreviatedName;
            LeftTeam = TeamList[(index - 1 + TeamList.Length) % TeamList.Length];
            RightTeam = TeamList[(index + 1) % TeamList.Length];
            LeftWorld = worlds[(index - 1 + TeamList.Length) % TeamList.Length].AbbreviatedName;
            RightWorld = worlds[(index + 1) % TeamList.Length].AbbreviatedName;

            CurrentTeams[OurTeam] = "Our";
            CurrentTeams[LeftTeam] = "Left";
            CurrentTeams[RightTeam] = "Right";
        }

        public void Clear()
        {
            Matches.Clear();
        }
        
        public IEnumerator<KeyValuePair<DateTime, Match>> GetEnumerator()
        {
            return Matches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private static bool AreSameSkirmish(DateTime existing, DateTime current)
        {
            if (existing == null || existing.Year < 2000)
                return false;

            var utcExisting = existing.ToUniversalTime();
            var utcCurrent = current.ToUniversalTime();
            if (!utcExisting.Date.Equals(utcCurrent.Date))
                return false;

            var existingSkirmish = utcExisting.Hour / 2;
            var currentSkirmish = utcCurrent.Hour / 2;
            return existingSkirmish.Equals(currentSkirmish);
        }

        private static bool AreSameTimezone(DateTime existing, DateTime current)
        {
            if (existing == null || existing.Year < 2000)
                return false;

            var adjExisting = existing.AddHours(5);
            var adjCurrent = current.AddHours(5);
            if (!adjExisting.Date.Equals(adjCurrent.Date))
                return false;

            var existingTimezone = adjExisting.Hour / 6;
            var currentTimezone = adjCurrent.Hour / 6;
            return existingTimezone.Equals(currentTimezone);
        }

        #region SQLite Database
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
            var match = GetHistoryMatch(0);
            var ts = LastUpdateTime;

            var row = new object[90];

            var i = 0;
            row[i++] = ts.Date.ToString("yyyyMMdd");
            row[i++] = ts.ToString("HH:mm:ss");
            row[i++] = MatchupId;
            row[i++] = CurrentWorlds["Red"];
            row[i++] = CurrentWorlds["Green"];
            row[i++] = CurrentWorlds["Blue"];
            row[i++] = match.Scores.Red;
            row[i++] = match.Scores.Green;
            row[i++] = match.Scores.Blue;
            // red team stats
            row[i++] = match.Kills.Red;
            row[i++] = match.Deaths.Red;
            row[i++] = match.GetMap("Red").Scores.Red;
            row[i++] = match.GetMap("Red").Kills.Red;
            row[i++] = match.GetMap("Red").Deaths.Red;
            row[i++] = match.GetMap("Green").Scores.Red;
            row[i++] = match.GetMap("Green").Kills.Red;
            row[i++] = match.GetMap("Green").Deaths.Red;
            row[i++] = match.GetMap("Blue").Scores.Red;
            row[i++] = match.GetMap("Blue").Kills.Red;
            row[i++] = match.GetMap("Blue").Deaths.Red;
            row[i++] = match.GetMap("EBG").Scores.Red;
            row[i++] = match.GetMap("EBG").Kills.Red;
            row[i++] = match.GetMap("EBG").Deaths.Red;
            // green team stats
            row[i++] = match.Kills.Green;
            row[i++] = match.Deaths.Green;
            row[i++] = match.GetMap("Red").Scores.Green;
            row[i++] = match.GetMap("Red").Kills.Green;
            row[i++] = match.GetMap("Red").Deaths.Green;
            row[i++] = match.GetMap("Green").Scores.Green;
            row[i++] = match.GetMap("Green").Kills.Green;
            row[i++] = match.GetMap("Green").Deaths.Green;
            row[i++] = match.GetMap("Blue").Scores.Green;
            row[i++] = match.GetMap("Blue").Kills.Green;
            row[i++] = match.GetMap("Blue").Deaths.Green;
            row[i++] = match.GetMap("EBG").Scores.Green;
            row[i++] = match.GetMap("EBG").Kills.Green;
            row[i++] = match.GetMap("EBG").Deaths.Green;
            // blue team stats
            row[i++] = match.Kills.Blue;
            row[i++] = match.Deaths.Blue;
            row[i++] = match.GetMap("Red").Scores.Blue;
            row[i++] = match.GetMap("Red").Kills.Blue;
            row[i++] = match.GetMap("Red").Deaths.Blue;
            row[i++] = match.GetMap("Green").Scores.Blue;
            row[i++] = match.GetMap("Green").Kills.Blue;
            row[i++] = match.GetMap("Green").Deaths.Blue;
            row[i++] = match.GetMap("Blue").Scores.Blue;
            row[i++] = match.GetMap("Blue").Kills.Blue;
            row[i++] = match.GetMap("Blue").Deaths.Blue;
            row[i++] = match.GetMap("EBG").Scores.Blue;
            row[i++] = match.GetMap("EBG").Kills.Blue;
            row[i++] = match.GetMap("EBG").Deaths.Blue;

            var redObjs = match.GetObjectiveCounts("Red");
            var greenObjs = match.GetObjectiveCounts("Green");
            var blueObjs = match.GetObjectiveCounts("Blue");
            var ebgObjs = match.GetObjectiveCounts("EBG");

            // red team objectives
            row[i++] = redObjs["Red"]["Keep"];
            row[i++] = redObjs["Red"]["Tower"];
            row[i++] = redObjs["Red"]["Camp"];
            row[i++] = greenObjs["Red"]["Keep"];
            row[i++] = greenObjs["Red"]["Tower"];
            row[i++] = greenObjs["Red"]["Camp"];
            row[i++] = blueObjs["Red"]["Keep"];
            row[i++] = blueObjs["Red"]["Tower"];
            row[i++] = blueObjs["Red"]["Camp"];
            row[i++] = ebgObjs["Red"]["Castle"];
            row[i++] = ebgObjs["Red"]["Keep"];
            row[i++] = ebgObjs["Red"]["Tower"];
            row[i++] = ebgObjs["Red"]["Camp"];
            // green team objectives
            row[i++] = redObjs["Green"]["Keep"];
            row[i++] = redObjs["Green"]["Tower"];
            row[i++] = redObjs["Green"]["Camp"];
            row[i++] = greenObjs["Green"]["Keep"];
            row[i++] = greenObjs["Green"]["Tower"];
            row[i++] = greenObjs["Green"]["Camp"];
            row[i++] = blueObjs["Green"]["Keep"];
            row[i++] = blueObjs["Green"]["Tower"];
            row[i++] = blueObjs["Green"]["Camp"];
            row[i++] = ebgObjs["Green"]["Castle"];
            row[i++] = ebgObjs["Green"]["Keep"];
            row[i++] = ebgObjs["Green"]["Tower"];
            row[i++] = ebgObjs["Green"]["Camp"];
            // blue team objectives
            row[i++] = redObjs["Blue"]["Keep"];
            row[i++] = redObjs["Blue"]["Tower"];
            row[i++] = redObjs["Blue"]["Camp"];
            row[i++] = greenObjs["Blue"]["Keep"];
            row[i++] = greenObjs["Blue"]["Tower"];
            row[i++] = greenObjs["Blue"]["Camp"];
            row[i++] = blueObjs["Blue"]["Keep"];
            row[i++] = blueObjs["Blue"]["Tower"];
            row[i++] = blueObjs["Blue"]["Camp"];
            row[i++] = ebgObjs["Blue"]["Castle"];
            row[i++] = ebgObjs["Blue"]["Keep"];
            row[i++] = ebgObjs["Blue"]["Tower"];
            row[i++] = ebgObjs["Blue"]["Camp"];

            for (var j = 0; j < 90; j++)
                if (row[j] is String)
                    row[j] = "'" + row[j] + "'";

            return String.Join(", ", row);
        }

        public static void maybeCreateDatabase()
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
                            , matchup varchar(10) NOT NULL
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
        #endregion
    }
}
