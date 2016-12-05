using System;
using System.Windows.Forms;

namespace Sandbox
{
    public partial class Form1 : Form
    {
        private Manager Manager;

        public Form1()
        {
            InitializeComponent();

            Manager = new Manager();

            label1.Text = Manager.Mode.ToString();
            mapCurrentNameLabel.Text = Manager.Map;
            teamLabel.Text = Manager.Team;

            pinTextBox.Text = Manager.SquadPin;
            squadMapTextBox.Text = Manager.SquadMap;
            squadMessageBox.Text = Manager.SquadMessage;

            mumbleTimer.Start();
        }

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
                Refresh();
            }
            if (Manager.maybeUpdateStats())
            {
                updateWvwStatsTab();
                updateHistoryTab();
            }
        }

        private void updateHistoryTab()
        {
            historyLabel.Text = Manager.WvwStats.getStringDump();
        }

        private void updateWvwStatsTab()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            string map = null;

            var match = Manager.WvwStats.GetHistoryMatch(0);
            var delta10 = Manager.WvwStats.GetHistoryMatch(5);
            var delta30 = Manager.WvwStats.GetHistoryMatch(15);

            builder.Append("Scores: JQ = ");
            builder.Append(match.Scores.Get(Manager.Team));
            builder.Append(", ");
            builder.Append(Manager.WvwStats.LeftWorld);
            builder.Append(" = ");
            builder.Append(match.Scores.Get(Manager.WvwStats.LeftTeam));
            builder.Append(", ");
            builder.Append(Manager.WvwStats.RightWorld);
            builder.Append(" = ");
            builder.Append(match.Scores.Get(Manager.WvwStats.RightTeam));
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("Totals");
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", "JQ", match.GetKDR(Manager.Team, map), match.GetKillsFor(Manager.Team, map), match.GetDeathsFor(Manager.Team, map), match.GetDeltaKDR(delta10, Manager.Team, map), match.GetDeltaKDR(delta30, Manager.Team, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.LeftWorld, match.GetKDR(Manager.WvwStats.LeftTeam, map), match.GetKillsFor(Manager.WvwStats.LeftTeam, map), match.GetDeathsFor(Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.LeftTeam, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.RightWorld, match.GetKDR(Manager.WvwStats.RightTeam, map), match.GetKillsFor(Manager.WvwStats.RightTeam, map), match.GetDeathsFor(Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.RightTeam, map));
            builder.AppendLine();
            builder.AppendLine("Red");
            map = "Red";
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", "JQ", match.GetKDR(Manager.Team, map), match.GetKillsFor(Manager.Team, map), match.GetDeathsFor(Manager.Team, map), match.GetDeltaKDR(delta10, Manager.Team, map), match.GetDeltaKDR(delta30, Manager.Team, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.LeftWorld, match.GetKDR(Manager.WvwStats.LeftTeam, map), match.GetKillsFor(Manager.WvwStats.LeftTeam, map), match.GetDeathsFor(Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.LeftTeam, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.RightWorld, match.GetKDR(Manager.WvwStats.RightTeam, map), match.GetKillsFor(Manager.WvwStats.RightTeam, map), match.GetDeathsFor(Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.RightTeam, map));
            builder.AppendLine();
            builder.AppendLine("Green");
            map = "Green";
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", "JQ", match.GetKDR(Manager.Team, map), match.GetKillsFor(Manager.Team, map), match.GetDeathsFor(Manager.Team, map), match.GetDeltaKDR(delta10, Manager.Team, map), match.GetDeltaKDR(delta30, Manager.Team, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.LeftWorld, match.GetKDR(Manager.WvwStats.LeftTeam, map), match.GetKillsFor(Manager.WvwStats.LeftTeam, map), match.GetDeathsFor(Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.LeftTeam, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.RightWorld, match.GetKDR(Manager.WvwStats.RightTeam, map), match.GetKillsFor(Manager.WvwStats.RightTeam, map), match.GetDeathsFor(Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.RightTeam, map));
            builder.AppendLine();
            builder.AppendLine("Blue");
            map = "Blue";
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", "JQ", match.GetKDR(Manager.Team, map), match.GetKillsFor(Manager.Team, map), match.GetDeathsFor(Manager.Team, map), match.GetDeltaKDR(delta10, Manager.Team, map), match.GetDeltaKDR(delta30, Manager.Team, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.LeftWorld, match.GetKDR(Manager.WvwStats.LeftTeam, map), match.GetKillsFor(Manager.WvwStats.LeftTeam, map), match.GetDeathsFor(Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.LeftTeam, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.RightWorld, match.GetKDR(Manager.WvwStats.RightTeam, map), match.GetKillsFor(Manager.WvwStats.RightTeam, map), match.GetDeathsFor(Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.RightTeam, map));
            builder.AppendLine();
            builder.AppendLine("EBG");
            map = "EBG";
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", "JQ", match.GetKDR(Manager.Team, map), match.GetKillsFor(Manager.Team, map), match.GetDeathsFor(Manager.Team, map), match.GetDeltaKDR(delta10, Manager.Team, map), match.GetDeltaKDR(delta30, Manager.Team, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.LeftWorld, match.GetKDR(Manager.WvwStats.LeftTeam, map), match.GetKillsFor(Manager.WvwStats.LeftTeam, map), match.GetDeathsFor(Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.LeftTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.LeftTeam, map));
            builder.AppendFormat("{0}: KDR = {1} ({2} / {3}), Last10 = {4}, Last30 = {5}\n", Manager.WvwStats.RightWorld, match.GetKDR(Manager.WvwStats.RightTeam, map), match.GetKillsFor(Manager.WvwStats.RightTeam, map), match.GetDeathsFor(Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta10, Manager.WvwStats.RightTeam, map), match.GetDeltaKDR(delta30, Manager.WvwStats.RightTeam, map));
            builder.AppendLine();

            wvwTabLabel.Text = builder.ToString();
        }

        private void squadUpdateButton_Click(object sender, EventArgs e)
        {
            Manager.SquadPin = pinTextBox.Text;
            Manager.SquadMap = squadMapTextBox.Text;
            Manager.SquadMessage = squadMessageBox.Text;
            Manager.updateSquad();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
