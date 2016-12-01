using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    class MapLinks
    {
        public interface ITeamMapGetter
        {
            String Team { get; }
            String Map { get; }
        }

        private Dictionary<String, Dictionary<String, String>> _WaypointLinks = new Dictionary<string, Dictionary<string, string>>();
        private ITeamMapGetter _Getter;
        private String Team { get { return _Getter.Team; } }
        private String Map { get { return _Getter.Map; } }

        public MapLinks(ITeamMapGetter getter)
        {
            _Getter = getter;

            var links = new Dictionary<String, String>();
            links["Citadel"] = "[&BNYEAAA=]";
            links["Garrison"] = "[&BNUEAAA=]";
            links["Bay"] = "[&BNMEAAA=]";
            links["Hills"] = "[&BNcEAAA=]";
            links["Red"] = "[&BNQEAAA=]";
            links["Green"] = "[&BNgEAAA=]";
            _WaypointLinks["Blue"] = links;

            links = new Dictionary<String, String>();
            links["Citadel"] = "[&BNwEAAA=]";
            links["Garrison"] = "[&BNsEAAA=]";
            links["Bay"] = "[&BNkEAAA=]";
            links["Hills"] = "[&BN0EAAA=]";
            links["Red"] = "[&BN4EAAA=]";
            links["Blue"] = "[&BNoEAAA=]";
            _WaypointLinks["Green"] = links;

            links = new Dictionary<String, String>();
            links["Citadel"] = "[&BOQIAAA=]";
            links["Garrison"] = "[&BP0IAAA=]";
            links["Bay"] = "[&BK0IAAA=]";
            links["Hills"] = "[&BGYIAAA=]";
            links["Green"] = "[&BMcIAAA=]";
            links["Blue"] = "[&BIsIAAA=]";
            _WaypointLinks["Red"] = links;

            links = new Dictionary<String, String>();
            links["Red"] = "[&BPsDAAA=]";
            links["Green"] = "[&BPwDAAA=]";
            links["Blue"] = "[&BPoDAAA=]";
            links["RedKeep"] = "[&BL4EAAA=]";
            links["GreenKeep"] = "[&BMAEAAA=]";
            links["BlueKeep"] = "[&BL8EAAA=]";
            links["SMC"] = "[&BL0EAAA=]";
            _WaypointLinks["EBG"] = links;
        }

        public String Get(String location, String map = null)
        {
            if (map == null)
                map = Map;
            return _WaypointLinks[map][location];
        }

        public String GetGarrison()
        {
            return _WaypointLinks[Team]["Garrison"];
        }

        public String GetCurrentKeep()
        {
            if (Team.Equals(Map))
                return GetGarrison();
            if (Map.Equals("EBG"))
                return _WaypointLinks[Map][Team + "Keep"];
            return GetSpawn();
        }

        public String GetSpawn(String map = null)
        {
            map = map ?? Map;
            if (Team.Equals(map))
                return _WaypointLinks[map]["Citadel"];
            return _WaypointLinks[map][Team];
        }

        public String GetLeftMapColor()
        {
            var teams = new String[] { "Red", "Green", "Blue" };
            return teams[(Array.IndexOf(teams, Team) - 1 + teams.Length) % teams.Length];
        }

        public String GetLeftSpawn()
        {
            return GetSpawn(GetLeftMapColor());
        }

        public String GetRightMapColor()
        {
            var teams = new String[] { "Red", "Green", "Blue" };
            return teams[(Array.IndexOf(teams, Team) + 1) % teams.Length];
        }

        public String GetRightSpawn()
        {
            return GetSpawn(GetRightMapColor());
        }
    }
}
