using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    class MatchHistory : IEnumerable<KeyValuePair<DateTime, Match>>
    {
        private LinkedList<KeyValuePair<DateTime, Match>> Matches { get; set; }

        public async Task Initialize()
        {
            await Task.Run(() => Matches = Deserialize());
        }

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
                if ((now - LastUpdateTime).TotalSeconds >= 60.0 || LastUpdateTime == DateTime.MinValue)
                {
                    if (Matches.Count == 61)
                        Matches.RemoveLast();
                    Matches.AddFirst(new KeyValuePair<DateTime, Match>(now, match));
                    Serialize();

                    return true;
                }
                return false;
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
        private static readonly DataContractSerializer _Serializer = new DataContractSerializer(typeof(LinkedList<KeyValuePair<DateTime, Match>>), new Type[] { typeof(RedBorderlands), typeof(GreenBorderlands), typeof(BlueBorderlands), typeof(EternalBattlegrounds), typeof(Bloodlust) });

        private void Serialize()
        {
            using (FileStream fs = File.Create(MATCH_HISTORY_FILE))
            {
                _Serializer.WriteObject(fs, Matches);
            }
        }

        private static LinkedList<KeyValuePair<DateTime, Match>> Deserialize()
        {
            if (File.Exists(MATCH_HISTORY_FILE))
            {
                try
                {
                    using (FileStream fs = File.OpenRead(MATCH_HISTORY_FILE))
                    {
                        return (LinkedList<KeyValuePair<DateTime, Match>>)_Serializer.ReadObject(fs);
                    }
                }
                catch (SerializationException)
                {
                    return new LinkedList<KeyValuePair<DateTime, Match>>();
                }
            }
            else
            {
                return new LinkedList<KeyValuePair<DateTime, Match>>();
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
    }
}
