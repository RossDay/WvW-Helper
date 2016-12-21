using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using GW2NET.MumbleLink;

namespace Sandbox
{
    class Manager : ITeamMapGetter
    {
        private static readonly Dictionary<int, string> _TeamColorDict = new Dictionary<int, string>()
        { { 9, "Blue" }, { 55, "Green" }, { 376, "Red" } };
        private static readonly Dictionary<int, string> _MapDict = new Dictionary<int, string>()
        { { 95, "Green" }, { 96, "Blue" }, { 1099, "Red" }, { 38, "EBG" } };

        private SpeechSynthesizer Synth { get; set; }
        private IniFile Ini { get; set; }
        private MapLinks Links { get; set; }
        public WvwStats WvwStats { get; private set; }
        private MumbleLinkFile Mumble { get; set; }

        public int Mode { get; set; }
        public string SquadPin { get; set; }
        public string SquadMap { get; set; }
        public string SquadMessage { get; set; }

        public int TeamColorId { get; private set; }
        public int CurrentTeamColorId { get; private set; }
        public string Team { get; private set; }
        public int MapId { get; private set; }
        public string Map { get; private set; }

        private string iniRead(string key, string defaultValue = "", bool setIfMissing = false)
        {
            if (Ini.KeyExists(key, "GW2"))
                return Ini.Read(key, "GW2");
            if (setIfMissing)
                Ini.Write(key, defaultValue, "GW2");
            return defaultValue;
        }

        public Manager()
        {
            Synth = new SpeechSynthesizer();
            Synth.SetOutputToDefaultAudioDevice();

            Ini = new IniFile("C:\\rday\\rday.ini");
            Links = new MapLinks(this, Ini);
            Mumble = MumbleLinkFile.CreateOrOpen();
        }

        public async Task Initialize()
        {
            WvwStats = new WvwStats(this, Ini);

            Mode = Convert.ToInt32(iniRead("mode", "0"));
            MapId = Convert.ToInt32(iniRead("map_id", "0"));
            Map = iniRead("map");

            TeamColorId = Convert.ToInt32(iniRead("team_id", "0"));
            Team = iniRead("team");

            SquadPin = iniRead("SquadPin");
            SquadMap = iniRead("SquadMap");
            SquadMessage = iniRead("SquadMsg");

            await WvwStats.Initialize();
        }

        public void maybeUpdateMode()
        {
            Mode = Convert.ToInt32(iniRead("mode", "0"));
        }

        public void Speak(string message)
        {
            Synth.Speak(message);
        }

        public void resetMumble()
        {
            Mumble.Dispose();
            Mumble = MumbleLinkFile.CreateOrOpen();
        }

        public async Task<bool> maybeUpdateStats()
        {
            var b = await WvwStats.maybeUpdateStats();
            return b;
        }

        public async Task<bool> maybeUpdateMumble()
        {
            var b = await Task.Run(async () =>
            {
                var a = Mumble.Read();
                if (a == null || a.Context == null || a.Identity == null)
                    return false;

                bool result = maybeUpdateMap(a.Context.MapId);
                return result || (await maybeUpdateTeam(a.Identity.TeamColorId));
            });
            return b;
        }

        private bool maybeUpdateMap(int newMapId)
        {
            if (MapId.Equals(newMapId))
                return false;

            string map;
            if (!_MapDict.TryGetValue(newMapId, out map))
                return false;

            MapId = newMapId;
            Map = map;

            updateMapBasedLinks();

            return true;
        }

        private void updateMapBasedLinks()
        {
            Ini.Write("map_id", MapId.ToString(), "GW2");
            Ini.Write("map", Map, "GW2");
            Ini.Write("team_id", TeamColorId.ToString(), "GW2");
            Ini.Write("team", Team, "GW2");
            Links.updateMapBasedLinks();
        }

        private async Task<bool> maybeUpdateTeam(int newColorId)
        {
            CurrentTeamColorId = newColorId;
            if (TeamColorId.Equals(newColorId))
                return false;

            string teamColor;
            if (!_TeamColorDict.TryGetValue(newColorId, out teamColor))
                return false;

            TeamColorId = newColorId;
            Team = teamColor;

            await WvwStats.reset();
            Links.updateTeamBasedLinks();

            return true;
        }

        public void updateSquad()
        {
            Ini.Write("SquadPin", SquadPin, "GW2");
            Ini.Write("SquadMap", SquadMap, "GW2");
            Ini.Write("SquadMsg", SquadMessage, "GW2");
        }
    }
}
