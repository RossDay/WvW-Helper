using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public int WorldId { get; private set; } = 1008;
        private async Task<Match> GetCurrentMatch()
        {
            try
            {
                var m = await GW2.V2.WorldVersusWorld.MatchesByWorld.FindAsync(WorldId);
                APIStatus = "Up";
                return m;
            }
            catch (Exception e)
            {
                APIStatus = "Down!\n" + e.Message;
                return null;
            }
        }

        private static readonly string[] TeamList = new string[] { "Red", "Green", "Blue" };

        [DataMember]
        private readonly Dictionary<string, string> CurrentTeams = new Dictionary<string, string>();
        [DataMember]
        private readonly Dictionary<string, string> CurrentWorlds = new Dictionary<string, string>();

        private ITeamMapGetter Getter { get; set; }
        public string OurTeam { get { return Getter.Team; } }

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

        public async Task<bool> maybeUpdate()
        {
            var match = await GetCurrentMatch();
            if (match == null)
                return false;

            Task t = null;
            var needToSerialize = false;
            if (OurWorld == null || match.Scores.Red < Matches.First.Value.Value.Scores.Red)
            {
                needToSerialize = true;
                t = maybeUpdateTeam(match);
            }

            needToSerialize = needToSerialize || (await maybeAdd(match));
            if (needToSerialize)
                await Task.Run(() => Serialize());

            if (t != null)
                await t;
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
                if (((now - LastUpdateTime).TotalSeconds >= 60.0 || LastUpdateTime == DateTime.MinValue)
                    && !match.IsEqualTo(Matches.First.Value.Value))
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

        private async Task maybeUpdateTeam(Match currentMatch = null)
        {
            if (currentMatch == null)
                currentMatch = await GetCurrentMatch();
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

        public void clear()
        {
            Matches.Clear();
            if (File.Exists(MATCH_HISTORY_FILE))
                File.Delete(MATCH_HISTORY_FILE);
        }
        
        public static readonly string MATCH_HISTORY_FILE = "C:\\rday\\wvwhistory.dat";
        private static readonly DataContractSerializer _Serializer = new DataContractSerializer(typeof(MatchHistory), new Type[] { typeof(Match), typeof(RedBorderlands), typeof(GreenBorderlands), typeof(BlueBorderlands), typeof(EternalBattlegrounds), typeof(Bloodlust) });

        private void Serialize()
        {
            using (FileStream fs = File.Create(MATCH_HISTORY_FILE))
            {
                _Serializer.WriteObject(fs, this);
            }
        }

        public static async Task<MatchHistory> CreateAsync()
        {
            var mh = await Task.Run(() => MatchHistory.Create());
            return mh;
        }
        private static MatchHistory Create()
        { 
            if (File.Exists(MATCH_HISTORY_FILE))
            {
                try
                {
                    using (FileStream fs = File.OpenRead(MATCH_HISTORY_FILE))
                    {
                        return (MatchHistory)_Serializer.ReadObject(fs);
                    }
                }
                catch (SerializationException)
                {
                    return new MatchHistory() { Matches = new LinkedList<KeyValuePair<DateTime, Match>>(), SkirmishTime = DateTime.MinValue, TimezoneTime = DateTime.MinValue };
                }
            }
            else
            {
                return new MatchHistory() { Matches = new LinkedList<KeyValuePair<DateTime, Match>>(), SkirmishTime = DateTime.MinValue, TimezoneTime = DateTime.MinValue };
            }
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

    }
}
