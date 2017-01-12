using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    public partial class Form1 : Form
    {
        private Manager Manager;
        private SynchronizationContext SyncContext;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            SyncContext = WindowsFormsSynchronizationContext.Current;
            Manager = new Manager();

            Initialize();
        }

        private async void Initialize()
        {
            await Manager.Initialize();

            Task t = null;
            if (Manager.WvwStats.CurrentMatchup.GetHistoryMatch(0) != null)
                t = Task.Run(() => updateStatsTable());

            updateStatsTableTeamsAndMaps();

            label1.Text = Manager.Mode.ToString();
            mapCurrentNameLabel.Text = Manager.Map;
            teamLabel.Text = Manager.Team;

            pinTextBox.Text = Manager.SquadPin;
            squadMapTextBox.Text = Manager.SquadMap;
            squadMessageBox.Text = Manager.SquadMessage;

            if (t != null)
                await t;

            mumbleTimer.Start();
        }

        /*
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                return cp;
            }
        }
        */

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('s'))
            {
                Manager.Speak(speechBox.Text);

                Task.Run(() =>
                {
                    Manager.maybeUpdateMode();
                    SyncContext.Post(new SendOrPostCallback(o =>
                    {
                        label1.Text = o.ToString();
                        label1.Refresh();
                    }), Manager.Mode.ToString());
                });
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void speakButton_Click(object sender, EventArgs e)
        {
            Manager.Speak(speechBox.Text);
        }

        private async void mumbleTimer_Tick(object sender, EventArgs e)
        {
            var statsUpdateTask = Manager.maybeUpdateStats();

            if (Manager.maybeUpdateMumble())
            {
                mumbleTimer.Stop();
                Manager.resetMumble();
                mumbleTimer.Start();

                mapIdLabel.Text = Manager.MapId.ToString();
                mapCurrentNameLabel.Text = Manager.Map;
                teamColorIdLabel.Text = Manager.TeamColorId.ToString();
                teamLabel.Text = Manager.Team;
                updateStatsTableTeamsAndMaps();
                Refresh();
            }
            teamColorIdLabel.Text = Manager.CurrentTeamColorId.ToString();

            var statsUpdated = await statsUpdateTask;

            var timerText = ( statsUpdated ? "59" : (Convert.ToInt32(StatsTableControl.TimerSeconds) - 1).ToString());
            StatsTableControl.TimerSeconds = timerText;
            statsTableCurrent.updateTimerLabel();
            statsTableTier1.updateTimerLabel();
            statsTableTier2.updateTimerLabel();
            statsTableTier3.updateTimerLabel();

            if (statsUpdated)
            {
                await Task.Run(() =>
                {
                    updateStatsTable();
                    updateTrackingTab();
                });
            }
        }

        private void updateTrackingTab()
        {
            trackingLabel.Text = "Last Update: " + Manager.WvwStats.CurrentMatchup.LastUpdateTime.ToString("yyyyMMdd HH:mm:ss") + "\n" 
                + "Skirmish Started: " + Manager.WvwStats.CurrentMatchup.SkirmishTime.ToString("yyyyMMdd HH:mm:ss") + "\n"
                + "Timezone Started: " + Manager.WvwStats.CurrentMatchup.TimezoneTime.ToString("yyyyMMdd HH:mm:ss") + "\n" 
                + "API Status: " + Manager.WvwStats.APIStatus + "\n\n"
                + Manager.WvwStats.LeftTracking + "\n\n" 
                + Manager.WvwStats.RightTracking + "\n\n" 
                + Manager.WvwStats.CurrentMapDetails;

            trackingLabel2.Text = Manager.WvwStats.TierTracking;
        }

        private void squadUpdateButton_Click(object sender, EventArgs e)
        {
            Manager.SquadPin = pinTextBox.Text;
            Manager.SquadMap = squadMapTextBox.Text;
            Manager.SquadMessage = squadMessageBox.Text;
            Manager.updateSquad();
        }


        private void updateStatsTableTeamsAndMaps()
        {
            var current = Manager.WvwStats.CurrentMatchup;
            statsTableCurrent.updateStatsTableTeamsAndMaps(current);
 
            var tier1 = Manager.WvwStats.GetMatchupFor("1-1");
            statsTableTier1.updateStatsTableTeamsAndMaps(tier1);

            var tier2 = Manager.WvwStats.GetMatchupFor("1-2");
            statsTableTier2.updateStatsTableTeamsAndMaps(tier2);

            var tier3 = Manager.WvwStats.GetMatchupFor("1-3");
            statsTableTier3.updateStatsTableTeamsAndMaps(tier3);
        }

        private static readonly string[] maps = new string[] { null, null, "Red", "Red", "Red", null,"Green", "Green", "Green", null, "Blue", "Blue", "Blue", null, "EBG", "EBG", "EBG" };
        private static readonly string[] teams = new string[] { "", "", "Red", "Green", "Blue", "", "Red", "Green", "Blue", "", "Red", "Green", "Blue" };
        private static readonly Func<Match, Match, string, string, Decimal> getKills = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaKillsFor(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal> getDeaths = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaDeathsFor(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal> getKDR = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaKDR(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal>[] funcs = new Func<Match, Match, string, string, Decimal>[] { null, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR };

        private void updateStatsTable()
        {
            var match = Manager.WvwStats.CurrentMatchup.GetHistoryMatch(0);
            var delta5 = Manager.WvwStats.CurrentMatchup.GetHistoryMatch(5);
            var delta10 = Manager.WvwStats.CurrentMatchup.GetHistoryMatch(10);
            var delta15 = Manager.WvwStats.CurrentMatchup.GetHistoryMatch(15);
            statsTableCurrent.updateStatsTable(match, delta5, delta10, delta15);

            var tier1 = Manager.WvwStats.GetMatchupFor("1-1");
            var tier1match = tier1.GetHistoryMatch(0);
            var tier1delta5 = tier1.GetHistoryMatch(5);
            var tier1delta10 = tier1.GetHistoryMatch(10);
            var tier1delta15 = tier1.GetHistoryMatch(15);
            statsTableTier1.updateStatsTable(tier1match, tier1delta5, tier1delta10, tier1delta15);

            var tier2 = Manager.WvwStats.GetMatchupFor("1-2");
            var tier2match = tier2.GetHistoryMatch(0);
            var tier2delta5 = tier2.GetHistoryMatch(5);
            var tier2delta10 = tier2.GetHistoryMatch(10);
            var tier2delta15 = tier2.GetHistoryMatch(15);
            statsTableTier2.updateStatsTable(tier2match, tier2delta5, tier2delta10, tier2delta15);

            var tier3 = Manager.WvwStats.GetMatchupFor("1-3");
            var tier3match = tier3.GetHistoryMatch(0);
            var tier3delta5 = tier3.GetHistoryMatch(5);
            var tier3delta10 = tier3.GetHistoryMatch(10);
            var tier3delta15 = tier3.GetHistoryMatch(15);
            statsTableTier3.updateStatsTable(tier3match, tier3delta5, tier3delta10, tier3delta15);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var matchupId = comboBox1.SelectedItem.ToString();

            await Manager.WvwStats.setMatchup(matchupId);

            updateStatsTableTeamsAndMaps();
            updateStatsTable();
            updateTrackingTab();
        }
    }
}
