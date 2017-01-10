using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW2NET.WorldVersusWorld;

namespace Sandbox
{
    public partial class StatsTableControl : UserControl
    {
        private SynchronizationContext SyncContext;

        public static string TimerSeconds { get; set; }

        public StatsTableControl()
        {
            InitializeComponent();

            SyncContext = WindowsFormsSynchronizationContext.Current;

            populateStatsTableLabels();
            statsTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        public void updateTimerLabel()
        {
            var timerText = TimerSeconds.ToString();
            SyncContext.Post(new SendOrPostCallback(o =>
            {
                timerLabel.Text = o.ToString();
                timerLabel.Refresh();

            }), timerText);
        }

        private void populateStatsTableLabels()
        {
            var anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left);

            var p = new Panel();
            p.Anchor = anchor;
            p.BackColor = Color.Black;
            p.Margin = Padding.Empty;
            statsTable.Controls.Add(p, 1, 0);
            statsTable.SetRowSpan(p, 17);

            p = new Panel();
            p.Anchor = anchor;
            p.BackColor = Color.Black;
            p.Margin = Padding.Empty;
            statsTable.Controls.Add(p, 5, 0);
            statsTable.SetRowSpan(p, 17);

            p = new Panel();
            p.Anchor = anchor;
            p.BackColor = Color.Black;
            p.Margin = Padding.Empty;
            statsTable.Controls.Add(p, 9, 0);
            statsTable.SetRowSpan(p, 17);

            foreach (var r in new int[] { 1, 5, 9, 13 })
            {
                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 0, r);

                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 2, r);
                statsTable.SetColumnSpan(p, 3);

                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 6, r);
                statsTable.SetColumnSpan(p, 3);

                p = new Panel();
                p.Anchor = anchor;
                p.BackColor = Color.Black;
                p.Margin = Padding.Empty;
                statsTable.Controls.Add(p, 10, r);
                statsTable.SetColumnSpan(p, 3);
            }

            for (var r = 2; r <= 16; r++)
                if (r != 5 && r != 9 && r != 13)
                    for (var c = 2; c <= 12; c++)
                    {
                        if (c == 5 || c == 9)
                            continue;

                        var l = new Label();
                        l.Text = $"{r}x{c}";
                        l.TextAlign = ContentAlignment.MiddleRight;
                        l.Anchor = anchor;
                        statsTable.Controls.Add(l, c, r);
                    }
        }

        private static readonly string[] maps = new string[] { null, null, "Red", "Red", "Red", null, "Green", "Green", "Green", null, "Blue", "Blue", "Blue", null, "EBG", "EBG", "EBG" };
        private static readonly string[] teams = new string[] { "", "", "Red", "Green", "Blue", "", "Red", "Green", "Blue", "", "Red", "Green", "Blue" };
        private static readonly Func<Match, Match, string, string, Decimal> getKills = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaKillsFor(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal> getDeaths = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaDeathsFor(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal> getKDR = new Func<Match, Match, string, string, Decimal>((c, d, t, m) => c.GetDeltaKDR(d, t, m));
        private static readonly Func<Match, Match, string, string, Decimal>[] funcs = new Func<Match, Match, string, string, Decimal>[] { null, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR, null, getKills, getDeaths, getKDR };

        public void updateStatsTable(Match match, Match delta5, Match delta10, Match delta15)
        {
            var deltas = new Match[] { null, null, delta5, delta5, delta5, null, delta10, delta10, delta10, null, delta15, delta15, delta15 };

            for (var r = 2; r <= 16; r++)
                if (r != 5 && r != 9 && r != 13)
                    for (var c = 2; c <= 12; c++)
                        if (c != 5 && c != 9)
                        {
                            SyncContext.Post(new SendOrPostCallback(o =>
                            {
                                var n = (int)o;
                                var r2 = n / 100;
                                var c2 = n % 100;
                                var l = ((Label)statsTable.GetControlFromPosition(c2, r2));
                                l.Text = funcs[r2](match, deltas[c2], teams[c2], maps[r2]).ToString();
                                l.Refresh();
                            }), r * 100 + c);
                        }
        }

        internal void updateStatsTableTeamsAndMaps(MatchHistory mh)
        {
            redTeamLabel.Text = mh.GetWorldByTeam("Red") + "\nRed";
            greenTeamLabel.Text = mh.GetWorldByTeam("Green") + "\nGreen";
            blueTeamLabel.Text = mh.GetWorldByTeam("Blue") + "\nBlue";
            redTeamLabel2.Text = mh.GetWorldByTeam("Red") + "\nRed";
            greenTeamLabel2.Text = mh.GetWorldByTeam("Green") + "\nGreen";
            blueTeamLabel2.Text = mh.GetWorldByTeam("Blue") + "\nBlue";
            redTeamLabel3.Text = mh.GetWorldByTeam("Red") + "\nRed";
            greenTeamLabel3.Text = mh.GetWorldByTeam("Green") + "\nGreen";
            blueTeamLabel3.Text = mh.GetWorldByTeam("Blue") + "\nBlue";

            redWorldLabel.Text = mh.GetWorldByTeam("Red") + "BL\nRed";
            greenWorldLabel.Text = mh.GetWorldByTeam("Green") + "BL\nGreen";
            blueWorldLabel.Text = mh.GetWorldByTeam("Blue") + "BL\nBlue";
        }
    }
}
