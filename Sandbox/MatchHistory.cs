using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    [DataContract]
    class MatchHistory : IEnumerable<KeyValuePair<DateTime, Match>>
    {
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

            var delta = Matches.First;
            for (int i = 0; i < interval; i++)
                if (delta.Next == null)
                    return delta.Value.Value;
                else
                    delta = delta.Next;
            return delta.Value.Value;
        }

        public async Task<bool> maybeAdd(Match match)
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
                if ((now - LastUpdateTime).TotalSeconds >= 60.0 || LastUpdateTime == DateTime.MinValue)
                {
                    if (Matches.Count == 61)
                        Matches.RemoveLast();
                    Matches.AddFirst(new KeyValuePair<DateTime, Match>(now, match));
                    needToSerialize = true;
                }
                if (needToSerialize)
                    Serialize();
                return needToSerialize;
            });
            return b;
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
            var adjCurrent = existing.AddHours(5);
            if (!adjExisting.Date.Equals(adjCurrent.Date))
                return false;

            var existingTimezone = adjExisting.Hour / 6;
            var currentTimezone = adjCurrent.Hour / 6;
            return existingTimezone.Equals(currentTimezone);
        }

    }
}
