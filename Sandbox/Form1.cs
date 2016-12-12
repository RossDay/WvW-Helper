using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    public partial class Form1 : Form
    {
        private Manager Manager;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Manager = new Manager();

            populateStatsTableLabels();
            statsTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            updateStatsTableTeamsAndMaps();
            if (Manager.WvwStats.GetHistoryMatch(0) != null)
                updateStatsTable();

            label1.Text = Manager.Mode.ToString();
            mapCurrentNameLabel.Text = Manager.Map;
            teamLabel.Text = Manager.Team;

            pinTextBox.Text = Manager.SquadPin;
            squadMapTextBox.Text = Manager.SquadMap;
            squadMessageBox.Text = Manager.SquadMessage;

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
            Manager.maybeUpdateMode();
            label1.Text = Manager.Mode.ToString();
            label1.Refresh();

            if (e.KeyChar.Equals('s'))
                Manager.Speak(speechBox.Text);
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

        private void mumbleTimer_Tick(object sender, EventArgs e)
        {
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
            Task t = null;
            if (Manager.maybeUpdateStats())
            {
                t = Task.Run(() => updateStatsTable());
                timerLabel.Text = "59";
                updateTrackingTab();
            }
            else
                timerLabel.Text = (Convert.ToInt32(timerLabel.Text) - 1).ToString();
            timerLabel.Refresh();
            if (t != null)
                t.Wait();
        }

        private void updateTrackingTab()
        {
            trackingLabel.Text = Manager.WvwStats.LastUpdateTime.ToString("yyyyMMdd HH:mm:ss") + "\n\n" + Manager.WvwStats.LeftTracking + "\n\n" + Manager.WvwStats.RightTracking;
        }

        private void squadUpdateButton_Click(object sender, EventArgs e)
        {
            Manager.SquadPin = pinTextBox.Text;
            Manager.SquadMap = squadMapTextBox.Text;
            Manager.SquadMessage = squadMessageBox.Text;
            Manager.updateSquad();
        }

        private void populateStatsTableLabels()
        {
            var anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left);

            var p = new Panel();
            p.Anchor = anchor;
            p.BackColor = System.Drawing.Color.Black;
            p.Margin = Padding.Empty;
            statsTable.Controls.Add(p, 1, 0);
            statsTable.SetRowSpan(p, 17);

            p = new Panel();
            p.Anchor = anchor;
            p.BackColor = System.Drawing.Color.Black;
            p.Margin = Padding.Empty;
            statsTable.Controls.Add(p, 5, 0);
            statsTable.SetRowSpan(p, 17);

            foreach (var r in new int[] { 1, 5, 9, 13 })
            {
                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = System.Drawing.Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 0, r);

                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = System.Drawing.Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 2, r);
                statsTable.SetColumnSpan(p, 3);

                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = System.Drawing.Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 6, r);
                statsTable.SetColumnSpan(p, 3);
            }

            for (var r = 2; r <= 16; r++)
                if (r != 5 && r != 9 && r != 13)
                    for (var c = 2; c <= 8; c++)
                    {
                        if (c == 5)
                            continue;

                        var l = new Label();
                        l.Text = $"{r}x{c}";
                        l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        l.Anchor = anchor;
                        statsTable.Controls.Add(l, c, r);
                    }
        }

        private void updateStatsTableTeamsAndMaps()
        {
            redTeamLabel.Text = Manager.WvwStats.GetWorldByTeam("Red") + "\nRed";
            greenTeamLabel.Text = Manager.WvwStats.GetWorldByTeam("Green") + "\nGreen";
            blueTeamLabel.Text = Manager.WvwStats.GetWorldByTeam("Blue") + "\nBlue";
            redTeamLabel2.Text = Manager.WvwStats.GetWorldByTeam("Red") + "\nRed";
            greenTeamLabel2.Text = Manager.WvwStats.GetWorldByTeam("Green") + "\nGreen";
            blueTeamLabel2.Text = Manager.WvwStats.GetWorldByTeam("Blue") + "\nBlue";

            redWorldLabel.Text = Manager.WvwStats.GetWorldByTeam("Red") + "BL\nRed";
            greenWorldLabel.Text = Manager.WvwStats.GetWorldByTeam("Green") + "BL\nGreen";
            blueWorldLabel.Text = Manager.WvwStats.GetWorldByTeam("Blue") + "BL\nBlue";
            statsTable.Refresh();
        }

        private static readonly string[] maps = new string[] { null, null, "Red", "Red", "Red", null,"Green", "Green", "Green", null, "Blue", "Blue", "Blue", null, "EBG", "EBG", "EBG" };
        private static readonly string[] teams = new string[] { "", "", "Red", "Green", "Blue", "", "Red", "Green", "Blue" };
        private static readonly Func<Match, Match, string, string, Decimal> getKills = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaKillsFor(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal> getDeaths = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaDeathsFor(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal> getKDR = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaKDR(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal>[] funcs = new Func<Match, Match, string, string, Decimal>[] { null, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR };

        private void updateStatsTable()
        {
            var match = Manager.WvwStats.GetHistoryMatch(0);
            var delta5 = Manager.WvwStats.GetHistoryMatch(5);
            var delta15 = Manager.WvwStats.GetHistoryMatch(15);
            var deltas = new Match[] { null, null, delta5, delta5, delta5, null, delta15, delta15, delta15 };

            for (var r = 2; r <= 16; r++)
                if (r != 5 && r != 9 && r != 13)
                    for (var c = 2; c <= 8; c++)
                        if (c != 5)
                            ((Label)statsTable.GetControlFromPosition(c, r)).Text = funcs[r](match, deltas[c], teams[c], maps[r]).ToString();

            statsTable.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
