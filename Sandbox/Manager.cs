using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using GW2NET.MumbleLink;

namespace Sandbox
{
    class Manager : ITeamMapGetter
    {
        private static readonly Dictionary<int, string> _TeamColorDict = new Dictionary<int, string>()
        { { 9, "Blue" }, { 55, "Green" } };
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
            WvwStats = new WvwStats(this, Ini);
            Mumble = MumbleLinkFile.CreateOrOpen();

            Mode = Convert.ToInt32(iniRead("mode", "0"));
            MapId = Convert.ToInt32(iniRead("map_id", "0"));
            Map = iniRead("map");

            TeamColorId = Convert.ToInt32(iniRead("team_id", "0"));
            Team = iniRead("team");
            WvwStats.updateLeftRightWorldNames();

            SquadPin = iniRead("SquadPin");
            SquadMap = iniRead("SquadMap");
            SquadMessage = iniRead("SquadMsg");
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

        public bool maybeUpdateStats()
        {
            return WvwStats.maybeUpdateStats();
        }

        public bool maybeUpdateMumble()
        {

            var a = Mumble.Read();
            if (a == null || a.Context == null || a.Identity == null)
                return false;

            bool result = maybeUpdateMap(a.Context.MapId);
            return maybeUpdateTeam(a.Identity.TeamColorId) && result;
        }

        private bool maybeUpdateMap(int newMapId)
        {
            if (MapId.Equals(newMapId))
                return false;

            MapId = newMapId;

            string map;
            if (!_MapDict.TryGetValue(MapId, out map))
                map = "UNK";
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

        private bool maybeUpdateTeam(int newColorId)
        {
            if (TeamColorId.Equals(newColorId))
                return false;

            TeamColorId = newColorId;

            string teamColor;
            if (!_TeamColorDict.TryGetValue(TeamColorId, out teamColor))
                teamColor = "UNK";
            Team = teamColor;

            WvwStats.updateLeftRightWorldNames();
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
