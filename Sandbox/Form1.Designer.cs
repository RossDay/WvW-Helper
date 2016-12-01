namespace Sandbox
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.speechBox = new System.Windows.Forms.TextBox();
            this.speakButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.redBL = new System.Windows.Forms.Button();
            this.ebg = new System.Windows.Forms.Button();
            this.mapLabel = new System.Windows.Forms.Label();
            this.greenBL = new System.Windows.Forms.Button();
            this.blueBL = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.teamLabel = new System.Windows.Forms.Label();
            this.mumbleTimer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mapNameLabel = new System.Windows.Forms.Label();
            this.teamColorLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.joinSquadBox = new System.Windows.Forms.GroupBox();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.pinLabel = new System.Windows.Forms.Label();
            this.pinTextBox = new System.Windows.Forms.TextBox();
            this.squadMapLabel = new System.Windows.Forms.Label();
            this.squadMapTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.squadMessageBox = new System.Windows.Forms.TextBox();
            this.squadUpdateButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.joinSquadBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipTitle = "GW2";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Sandbox";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // speechBox
            // 
            this.speechBox.Location = new System.Drawing.Point(12, 15);
            this.speechBox.Name = "speechBox";
            this.speechBox.Size = new System.Drawing.Size(216, 20);
            this.speechBox.TabIndex = 0;
            // 
            // speakButton
            // 
            this.speakButton.Location = new System.Drawing.Point(234, 12);
            this.speakButton.Name = "speakButton";
            this.speakButton.Size = new System.Drawing.Size(75, 23);
            this.speakButton.TabIndex = 1;
            this.speakButton.Text = "Speak";
            this.speakButton.UseVisualStyleBackColor = true;
            this.speakButton.Click += new System.EventHandler(this.speakButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "mode";
            // 
            // redBL
            // 
            this.redBL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.redBL.Location = new System.Drawing.Point(114, 15);
            this.redBL.Margin = new System.Windows.Forms.Padding(15);
            this.redBL.Name = "redBL";
            this.redBL.Size = new System.Drawing.Size(70, 25);
            this.redBL.TabIndex = 3;
            this.redBL.Text = "Red";
            this.redBL.UseVisualStyleBackColor = true;
            this.redBL.Click += new System.EventHandler(this.mapButtonClick);
            // 
            // ebg
            // 
            this.ebg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ebg.Location = new System.Drawing.Point(114, 125);
            this.ebg.Margin = new System.Windows.Forms.Padding(15);
            this.ebg.Name = "ebg";
            this.ebg.Size = new System.Drawing.Size(70, 27);
            this.ebg.TabIndex = 4;
            this.ebg.Text = "EBG";
            this.ebg.UseVisualStyleBackColor = true;
            this.ebg.Click += new System.EventHandler(this.mapButtonClick);
            // 
            // mapLabel
            // 
            this.mapLabel.AutoSize = true;
            this.mapLabel.Location = new System.Drawing.Point(36, 6);
            this.mapLabel.Name = "mapLabel";
            this.mapLabel.Size = new System.Drawing.Size(27, 13);
            this.mapLabel.TabIndex = 5;
            this.mapLabel.Text = "map";
            // 
            // greenBL
            // 
            this.greenBL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.greenBL.Location = new System.Drawing.Point(15, 70);
            this.greenBL.Margin = new System.Windows.Forms.Padding(15);
            this.greenBL.Name = "greenBL";
            this.greenBL.Size = new System.Drawing.Size(69, 25);
            this.greenBL.TabIndex = 6;
            this.greenBL.Text = "Green";
            this.greenBL.UseVisualStyleBackColor = true;
            this.greenBL.Click += new System.EventHandler(this.mapButtonClick);
            // 
            // blueBL
            // 
            this.blueBL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blueBL.Location = new System.Drawing.Point(214, 70);
            this.blueBL.Margin = new System.Windows.Forms.Padding(15);
            this.blueBL.Name = "blueBL";
            this.blueBL.Size = new System.Drawing.Size(71, 25);
            this.blueBL.TabIndex = 7;
            this.blueBL.Text = "Blue";
            this.blueBL.UseVisualStyleBackColor = true;
            this.blueBL.Click += new System.EventHandler(this.mapButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Team: ";
            // 
            // teamLabel
            // 
            this.teamLabel.AutoSize = true;
            this.teamLabel.Location = new System.Drawing.Point(30, 7);
            this.teamLabel.Name = "teamLabel";
            this.teamLabel.Size = new System.Drawing.Size(27, 13);
            this.teamLabel.TabIndex = 9;
            this.teamLabel.Text = "Red";
            // 
            // mumbleTimer
            // 
            this.mumbleTimer.Interval = 1000;
            this.mumbleTimer.Tick += new System.EventHandler(this.mumbleTimer_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Map ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Team Color ID:";
            // 
            // mapNameLabel
            // 
            this.mapNameLabel.AutoSize = true;
            this.mapNameLabel.Location = new System.Drawing.Point(30, 6);
            this.mapNameLabel.Name = "mapNameLabel";
            this.mapNameLabel.Size = new System.Drawing.Size(38, 13);
            this.mapNameLabel.TabIndex = 12;
            this.mapNameLabel.Text = "map id";
            // 
            // teamColorLabel
            // 
            this.teamColorLabel.AutoSize = true;
            this.teamColorLabel.Location = new System.Drawing.Point(32, 6);
            this.teamColorLabel.Name = "teamColorLabel";
            this.teamColorLabel.Size = new System.Drawing.Size(41, 13);
            this.teamColorLabel.TabIndex = 13;
            this.teamColorLabel.Text = "team id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Current Map";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Mode:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer5, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.blueBL, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.ebg, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.greenBL, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.redBL, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer4, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 44);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 167);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(102, 58);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mapLabel);
            this.splitContainer1.Size = new System.Drawing.Size(94, 49);
            this.splitContainer1.SplitterDistance = 20;
            this.splitContainer1.TabIndex = 8;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Size = new System.Drawing.Size(93, 49);
            this.splitContainer2.SplitterDistance = 20;
            this.splitContainer2.TabIndex = 9;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Location = new System.Drawing.Point(3, 113);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.teamLabel);
            this.splitContainer3.Size = new System.Drawing.Size(93, 51);
            this.splitContainer3.SplitterDistance = 25;
            this.splitContainer3.TabIndex = 10;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Location = new System.Drawing.Point(202, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.mapNameLabel);
            this.splitContainer4.Size = new System.Drawing.Size(95, 49);
            this.splitContainer4.SplitterDistance = 20;
            this.splitContainer4.TabIndex = 11;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Location = new System.Drawing.Point(202, 113);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.teamColorLabel);
            this.splitContainer5.Size = new System.Drawing.Size(95, 51);
            this.splitContainer5.SplitterDistance = 25;
            this.splitContainer5.TabIndex = 12;
            // 
            // joinSquadBox
            // 
            this.joinSquadBox.Controls.Add(this.splitContainer6);
            this.joinSquadBox.Location = new System.Drawing.Point(12, 217);
            this.joinSquadBox.Name = "joinSquadBox";
            this.joinSquadBox.Size = new System.Drawing.Size(300, 101);
            this.joinSquadBox.TabIndex = 18;
            this.joinSquadBox.TabStop = false;
            this.joinSquadBox.Text = "Squad Announcement";
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(3, 16);
            this.splitContainer6.Name = "splitContainer6";
            this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.squadMessageBox);
            this.splitContainer6.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer6.Size = new System.Drawing.Size(294, 82);
            this.splitContainer6.SplitterDistance = 25;
            this.splitContainer6.TabIndex = 0;
            // 
            // pinLabel
            // 
            this.pinLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pinLabel.AutoSize = true;
            this.pinLabel.Location = new System.Drawing.Point(3, 0);
            this.pinLabel.Name = "pinLabel";
            this.pinLabel.Size = new System.Drawing.Size(38, 25);
            this.pinLabel.TabIndex = 0;
            this.pinLabel.Text = "Pin:";
            this.pinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pinTextBox
            // 
            this.pinTextBox.Location = new System.Drawing.Point(47, 3);
            this.pinTextBox.Name = "pinTextBox";
            this.pinTextBox.Size = new System.Drawing.Size(67, 20);
            this.pinTextBox.TabIndex = 1;
            // 
            // squadMapLabel
            // 
            this.squadMapLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.squadMapLabel.AutoSize = true;
            this.squadMapLabel.Location = new System.Drawing.Point(120, 0);
            this.squadMapLabel.Name = "squadMapLabel";
            this.squadMapLabel.Size = new System.Drawing.Size(38, 25);
            this.squadMapLabel.TabIndex = 2;
            this.squadMapLabel.Text = "Map:";
            this.squadMapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // squadMapTextBox
            // 
            this.squadMapTextBox.Location = new System.Drawing.Point(164, 3);
            this.squadMapTextBox.Name = "squadMapTextBox";
            this.squadMapTextBox.Size = new System.Drawing.Size(67, 20);
            this.squadMapTextBox.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Controls.Add(this.squadMapTextBox, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.squadMapLabel, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.pinLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.pinTextBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.squadUpdateButton, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(294, 25);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // squadMessageBox
            // 
            this.squadMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.squadMessageBox.Location = new System.Drawing.Point(4, 4);
            this.squadMessageBox.Multiline = true;
            this.squadMessageBox.Name = "squadMessageBox";
            this.squadMessageBox.Size = new System.Drawing.Size(286, 45);
            this.squadMessageBox.TabIndex = 0;
            // 
            // squadUpdateButton
            // 
            this.squadUpdateButton.Location = new System.Drawing.Point(237, 3);
            this.squadUpdateButton.Name = "squadUpdateButton";
            this.squadUpdateButton.Size = new System.Drawing.Size(54, 19);
            this.squadUpdateButton.TabIndex = 4;
            this.squadUpdateButton.Text = "Update";
            this.squadUpdateButton.UseVisualStyleBackColor = true;
            this.squadUpdateButton.Click += new System.EventHandler(this.squadUpdateButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 327);
            this.Controls.Add(this.joinSquadBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.speakButton);
            this.Controls.Add(this.speechBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel1.PerformLayout();
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.joinSquadBox.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            this.splitContainer6.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox speechBox;
        private System.Windows.Forms.Button speakButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button redBL;
        private System.Windows.Forms.Button ebg;
        private System.Windows.Forms.Label mapLabel;
        private System.Windows.Forms.Button greenBL;
        private System.Windows.Forms.Button blueBL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label teamLabel;
        private System.Windows.Forms.Timer mumbleTimer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label mapNameLabel;
        private System.Windows.Forms.Label teamColorLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox joinSquadBox;
        private System.Windows.Forms.SplitContainer splitContainer6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox squadMapTextBox;
        private System.Windows.Forms.Label squadMapLabel;
        private System.Windows.Forms.Label pinLabel;
        private System.Windows.Forms.TextBox pinTextBox;
        private System.Windows.Forms.TextBox squadMessageBox;
        private System.Windows.Forms.Button squadUpdateButton;
    }
}

