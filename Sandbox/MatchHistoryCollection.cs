using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GW2NET;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    [DataContract]
    class MatchHistoryCollection : IEnumerable<KeyValuePair<DateTime, Match>>, ITeamMapGetter
    {
        public static string APIStatus { get; private set; } = "N/A";
        private GW2Bootstrapper GW2 = new GW2Bootstrapper();
        [DataMember]
        private Dictionary<string, MatchHistory> Matchups;
        [DataMember]
        public string CurrentMatchup { get; set; }

        private async Task<Match> GetCurrentMatchFor(string matchupId)
        {
            try
            {
                var m = await GW2.V2.WorldVersusWorld.Matches.FindAsync(matchupId);
                APIStatus = "Up";
                return m;
            }
            catch (Exception e)
            {
                APIStatus = "Down!\n" + e.Message + "\n" + e.StackTrace;
                return null;
            }
        }

        public MatchHistory GetMatchupFor(string matchupId)
        {
            return Matchups[matchupId];
        }

        public MatchHistoryCollection()
        {
            Matchups = new Dictionary<string, MatchHistory>();
            Matchups.Add("1-1", new MatchHistory(this, "1-1"));
            Matchups.Add("1-2", new MatchHistory(this, "1-2"));
            Matchups.Add("1-3", new MatchHistory(this, "1-3"));
            CurrentMatchup = "1-2";
        }

        public ITeamMapGetter Getter { get; set; }
        public string Team { get { return Getter.Team; } }
        public string Map { get { return Getter.Map; } }
        public string OurTeam { get { return Getter.Team; } }

        private MatchHistory CurrentHistory { get { return Matchups[CurrentMatchup]; } }
        public Dictionary<string, string> CurrentTeams { get { return CurrentHistory.CurrentTeams; } }
        public Dictionary<string, string> CurrentWorlds { get { return CurrentHistory.CurrentWorlds; } }
        public string LeftTeam { get { return CurrentHistory.LeftTeam; } }
        public string RightTeam { get { return CurrentHistory.RightTeam; } }
        public string OurWorld { get { return CurrentHistory.OurWorld; } }
        public string LeftWorld { get { return CurrentHistory.LeftWorld; } }
        public string RightWorld { get { return CurrentHistory.RightWorld; } }
        public DateTime LastUpdateTime { get { return CurrentHistory.LastUpdateTime; } }
        public DateTime SkirmishTime { get { return CurrentHistory.SkirmishTime; } }
        public DateTime TimezoneTime { get { return CurrentHistory.TimezoneTime; } }
        public Match SkirmishMatch { get { return CurrentHistory.SkirmishMatch; } }
        public Match TimezoneMatch { get { return CurrentHistory.TimezoneMatch; } }

        public Match GetHistoryMatch(int interval)
        {
            return CurrentHistory.GetHistoryMatch(interval);
        }

        public string GetWorldByTeam(string team)
        {
            return CurrentHistory.GetWorldByTeam(team);
        }

        private Task SerializerTask = null;
        public async Task<bool> maybeUpdate()
        {
            var needToSerialize = false;
            var isCurrentUpdated = false;

            foreach (var kv in Matchups)
            {
                var now = DateTime.Now;
                if ((now - kv.Value.LastUpdateTime).TotalSeconds < 55)
                    continue;

                var match = await GetCurrentMatchFor(kv.Key);
                if (match == null)
                    continue;

                if (SerializerTask != null)
                {
                    if (!SerializerTask.IsCompleted)
                        SerializerTask.Wait();
                    SerializerTask.Dispose();
                    SerializerTask = null;
                }

                var temp = await kv.Value.maybeUpdate(match);
                if (kv.Key.Equals(CurrentMatchup))
                    isCurrentUpdated = temp;
                needToSerialize = needToSerialize || temp;
            }

            if (needToSerialize)
            {
                if (SerializerTask != null)
                {
                    if (!SerializerTask.IsCompleted)
                        SerializerTask.Wait();
                    SerializerTask.Dispose();
                    SerializerTask = null;
                }
                SerializerTask = Task.Run(() => Serialize());
            }

            return isCurrentUpdated;
        }

        public void Clear()
        {
            foreach (var kv in Matchups)
                kv.Value.Clear();
            if (File.Exists(MATCH_HISTORY_FILE))
                File.Delete(MATCH_HISTORY_FILE);
        }

        #region Serialization
        public static readonly string MATCH_HISTORY_FILE = "C:\\rday\\wvwhistory.dat";
        private static readonly DataContractSerializer _Serializer = new DataContractSerializer(typeof(MatchHistoryCollection), new Type[] { typeof(MatchHistory), typeof(Match), typeof(RedBorderlands), typeof(GreenBorderlands), typeof(BlueBorderlands), typeof(EternalBattlegrounds), typeof(Bloodlust) });

        private void Serialize()
        {
            using (FileStream fs = File.Create(MATCH_HISTORY_FILE))
            {
                _Serializer.WriteObject(fs, this);
            }
        }

        public static async Task<MatchHistoryCollection> CreateAsync()
        {
            var mh = await Task.Run(() => MatchHistoryCollection.Create());
            return mh;
        }
        private static MatchHistoryCollection Create()
        {
            if (File.Exists(MATCH_HISTORY_FILE))
            {
                try
                {
                    using (FileStream fs = File.OpenRead(MATCH_HISTORY_FILE))
                    {
                        var mhc = (MatchHistoryCollection)_Serializer.ReadObject(fs);
                        mhc.GW2 = new GW2Bootstrapper();
                        return mhc;
                    }
                }
                catch (SerializationException)
                {
                    return new MatchHistoryCollection();
                }
            }
            else
            {
                return new MatchHistoryCollection();
            }
        } 
        #endregion

        public IEnumerator<KeyValuePair<DateTime, Match>> GetEnumerator()
        {
            return CurrentHistory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
