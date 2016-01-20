namespace SmarterDashboard_Drivers
{
    partial class DriversWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DriversWindow));
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage4 = new MetroFramework.Controls.MetroTabPage();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.metroStyleExtender1 = new MetroFramework.Components.MetroStyleExtender(this.components);
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager(this.components);
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.picNormal = new System.Windows.Forms.PictureBox();
            this.tmrFps = new System.Windows.Forms.Timer(this.components);
            this.lblRes = new MetroFramework.Controls.MetroLabel();
            this.tmrNetworkTables = new System.Windows.Forms.Timer(this.components);
            this.lblImgPrcsDelay = new MetroFramework.Controls.MetroLabel();
            this.pnlStreamData = new MetroFramework.Controls.MetroPanel();
            this.lblCompression = new MetroFramework.Controls.MetroLabel();
            this.lblBits = new MetroFramework.Controls.MetroLabel();
            this.lblFps = new MetroFramework.Controls.MetroLabel();
            this.pnlDashboard = new MetroFramework.Controls.MetroPanel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.tmrUpdatePictures = new System.Windows.Forms.Timer(this.components);
            this.lstboxAutonomousChooser = new MetroFramework.Controls.MetroComboBox();
            this.tileIRAngle = new MetroFramework.Controls.MetroTile();
            this.tileForklift = new MetroFramework.Controls.MetroTile();
            this.battery = new MetroFramework.Controls.MetroProgressBar();
            this.lblBattery = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.lblIRDistance_Left = new System.Windows.Forms.Label();
            this.tileIRDistance_Left = new MetroFramework.Controls.MetroTile();
            this.lblIRDistance_Right = new System.Windows.Forms.Label();
            this.tileIRDistance_Right = new MetroFramework.Controls.MetroTile();
            this.btnAutonomous = new MetroFramework.Controls.MetroButton();
            this.metroTabControl1.SuspendLayout();
            this.metroTabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNormal)).BeginInit();
            this.pnlStreamData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.metroTabPage4);
            this.metroTabControl1.Location = new System.Drawing.Point(20, 50);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(1060, 54);
            this.metroTabControl1.TabIndex = 2;
            this.metroTabControl1.UseSelectable = true;
            // 
            // metroTabPage4
            // 
            this.metroTabPage4.Controls.Add(this.metroTile1);
            this.metroTabPage4.HorizontalScrollbarBarColor = true;
            this.metroTabPage4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage4.HorizontalScrollbarSize = 10;
            this.metroTabPage4.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage4.Name = "metroTabPage4";
            this.metroTabPage4.Size = new System.Drawing.Size(1052, 15);
            this.metroTabPage4.TabIndex = 3;
            this.metroTabPage4.VerticalScrollbarBarColor = true;
            this.metroTabPage4.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage4.VerticalScrollbarSize = 10;
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.Enabled = false;
            this.metroTile1.Location = new System.Drawing.Point(661, 10);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(400, 4);
            this.metroTile1.TabIndex = 75;
            this.metroTile1.UseSelectable = true;
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.Owner = this;
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.Enabled = false;
            this.metroTile4.Location = new System.Drawing.Point(24, 95);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(640, 4);
            this.metroTile4.TabIndex = 12;
            this.metroTile4.UseSelectable = true;
            // 
            // picNormal
            // 
            this.picNormal.BackColor = System.Drawing.Color.WhiteSmoke;
            this.picNormal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picNormal.Location = new System.Drawing.Point(24, 98);
            this.picNormal.Margin = new System.Windows.Forms.Padding(2);
            this.picNormal.Name = "picNormal";
            this.picNormal.Size = new System.Drawing.Size(640, 480);
            this.picNormal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picNormal.TabIndex = 13;
            this.picNormal.TabStop = false;
            // 
            // tmrFps
            // 
            this.tmrFps.Interval = 1000;
            this.tmrFps.Tick += new System.EventHandler(this.timerFps_Tick);
            // 
            // lblRes
            // 
            this.lblRes.AutoSize = true;
            this.lblRes.Location = new System.Drawing.Point(3, 25);
            this.lblRes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRes.Name = "lblRes";
            this.lblRes.Size = new System.Drawing.Size(72, 19);
            this.lblRes.TabIndex = 25;
            this.lblRes.Text = "Resolution:";
            // 
            // tmrNetworkTables
            // 
            this.tmrNetworkTables.Interval = 50;
            this.tmrNetworkTables.Tick += new System.EventHandler(this.tmrNetworkTables_Tick);
            // 
            // lblImgPrcsDelay
            // 
            this.lblImgPrcsDelay.AutoSize = true;
            this.lblImgPrcsDelay.Location = new System.Drawing.Point(3, 109);
            this.lblImgPrcsDelay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblImgPrcsDelay.Name = "lblImgPrcsDelay";
            this.lblImgPrcsDelay.Size = new System.Drawing.Size(156, 19);
            this.lblImgPrcsDelay.TabIndex = 50;
            this.lblImgPrcsDelay.Text = "Image Proccesing Delay: ";
            // 
            // pnlStreamData
            // 
            this.pnlStreamData.Controls.Add(this.lblCompression);
            this.pnlStreamData.Controls.Add(this.lblBits);
            this.pnlStreamData.Controls.Add(this.lblFps);
            this.pnlStreamData.Controls.Add(this.lblImgPrcsDelay);
            this.pnlStreamData.Controls.Add(this.lblRes);
            this.pnlStreamData.HorizontalScrollbarBarColor = true;
            this.pnlStreamData.HorizontalScrollbarHighlightOnWheel = false;
            this.pnlStreamData.HorizontalScrollbarSize = 10;
            this.pnlStreamData.Location = new System.Drawing.Point(24, 584);
            this.pnlStreamData.Name = "pnlStreamData";
            this.pnlStreamData.Size = new System.Drawing.Size(187, 138);
            this.pnlStreamData.TabIndex = 55;
            this.pnlStreamData.VerticalScrollbarBarColor = true;
            this.pnlStreamData.VerticalScrollbarHighlightOnWheel = false;
            this.pnlStreamData.VerticalScrollbarSize = 10;
            // 
            // lblCompression
            // 
            this.lblCompression.AutoSize = true;
            this.lblCompression.Location = new System.Drawing.Point(3, 41);
            this.lblCompression.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCompression.Name = "lblCompression";
            this.lblCompression.Size = new System.Drawing.Size(89, 19);
            this.lblCompression.TabIndex = 54;
            this.lblCompression.Text = "Compression:";
            // 
            // lblBits
            // 
            this.lblBits.AutoSize = true;
            this.lblBits.Location = new System.Drawing.Point(3, 76);
            this.lblBits.Name = "lblBits";
            this.lblBits.Size = new System.Drawing.Size(95, 19);
            this.lblBits.TabIndex = 52;
            this.lblBits.Text = "MBi Received: ";
            // 
            // lblFps
            // 
            this.lblFps.AutoSize = true;
            this.lblFps.Location = new System.Drawing.Point(3, 9);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(38, 19);
            this.lblFps.TabIndex = 51;
            this.lblFps.Text = "FPS: ";
            // 
            // pnlDashboard
            // 
            this.pnlDashboard.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlDashboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDashboard.HorizontalScrollbarBarColor = true;
            this.pnlDashboard.HorizontalScrollbarHighlightOnWheel = false;
            this.pnlDashboard.HorizontalScrollbarSize = 10;
            this.pnlDashboard.Location = new System.Drawing.Point(685, 98);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(391, 624);
            this.pnlDashboard.TabIndex = 56;
            this.pnlDashboard.UseCustomBackColor = true;
            this.pnlDashboard.VerticalScrollbarBarColor = true;
            this.pnlDashboard.VerticalScrollbarHighlightOnWheel = false;
            this.pnlDashboard.VerticalScrollbarSize = 10;
            // 
            // picLogo
            // 
            this.picLogo.Image = global::SmarterDashboard_Drivers.Properties.Resources.logo;
            this.picLogo.Location = new System.Drawing.Point(24, 10);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(55, 67);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 62;
            this.picLogo.TabStop = false;
            // 
            // lstboxAutonomousChooser
            // 
            this.lstboxAutonomousChooser.FormattingEnabled = true;
            this.lstboxAutonomousChooser.ItemHeight = 23;
            this.lstboxAutonomousChooser.Location = new System.Drawing.Point(536, 599);
            this.lstboxAutonomousChooser.Name = "lstboxAutonomousChooser";
            this.lstboxAutonomousChooser.Size = new System.Drawing.Size(121, 29);
            this.lstboxAutonomousChooser.TabIndex = 0;
            this.lstboxAutonomousChooser.UseSelectable = true;
            this.lstboxAutonomousChooser.SelectedIndexChanged += new System.EventHandler(this.lstboxAutonomousChooser_SelectedIndexChanged);
            // 
            // tileIRAngle
            // 
            this.tileIRAngle.ActiveControl = null;
            this.tileIRAngle.Enabled = false;
            this.tileIRAngle.Location = new System.Drawing.Point(242, 647);
            this.tileIRAngle.Name = "tileIRAngle";
            this.tileIRAngle.Size = new System.Drawing.Size(120, 45);
            this.tileIRAngle.Style = MetroFramework.MetroColorStyle.Silver;
            this.tileIRAngle.TabIndex = 76;
            this.tileIRAngle.Text = "IR Angle: 0.0";
            this.tileIRAngle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileIRAngle.UseSelectable = true;
            // 
            // tileForklift
            // 
            this.tileForklift.ActiveControl = null;
            this.tileForklift.Enabled = false;
            this.tileForklift.Location = new System.Drawing.Point(385, 647);
            this.tileForklift.Name = "tileForklift";
            this.tileForklift.Size = new System.Drawing.Size(120, 46);
            this.tileForklift.Style = MetroFramework.MetroColorStyle.Silver;
            this.tileForklift.TabIndex = 77;
            this.tileForklift.Text = "Forklift Down";
            this.tileForklift.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileForklift.UseSelectable = true;
            // 
            // battery
            // 
            this.battery.Location = new System.Drawing.Point(536, 650);
            this.battery.Margin = new System.Windows.Forms.Padding(2);
            this.battery.Name = "battery";
            this.battery.Size = new System.Drawing.Size(121, 39);
            this.battery.Style = MetroFramework.MetroColorStyle.Green;
            this.battery.TabIndex = 78;
            this.battery.Value = 30;
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblBattery.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblBattery.Location = new System.Drawing.Point(533, 691);
            this.lblBattery.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(55, 25);
            this.lblBattery.TabIndex = 79;
            this.lblBattery.Text = "13.0v";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel3.Location = new System.Drawing.Point(938, 191);
            this.metroLabel3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(0, 0);
            this.metroLabel3.TabIndex = 86;
            // 
            // lblIRDistance_Left
            // 
            this.lblIRDistance_Left.BackColor = System.Drawing.Color.Transparent;
            this.lblIRDistance_Left.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIRDistance_Left.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblIRDistance_Left.Location = new System.Drawing.Point(249, 598);
            this.lblIRDistance_Left.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIRDistance_Left.Name = "lblIRDistance_Left";
            this.lblIRDistance_Left.Size = new System.Drawing.Size(106, 39);
            this.lblIRDistance_Left.TabIndex = 87;
            this.lblIRDistance_Left.Text = "8.92";
            this.lblIRDistance_Left.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tileIRDistance_Left
            // 
            this.tileIRDistance_Left.ActiveControl = null;
            this.tileIRDistance_Left.Enabled = false;
            this.tileIRDistance_Left.Location = new System.Drawing.Point(242, 589);
            this.tileIRDistance_Left.Name = "tileIRDistance_Left";
            this.tileIRDistance_Left.Size = new System.Drawing.Size(120, 58);
            this.tileIRDistance_Left.Style = MetroFramework.MetroColorStyle.Silver;
            this.tileIRDistance_Left.TabIndex = 91;
            this.tileIRDistance_Left.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileIRDistance_Left.UseSelectable = true;
            // 
            // lblIRDistance_Right
            // 
            this.lblIRDistance_Right.BackColor = System.Drawing.Color.Transparent;
            this.lblIRDistance_Right.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIRDistance_Right.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblIRDistance_Right.Location = new System.Drawing.Point(392, 598);
            this.lblIRDistance_Right.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIRDistance_Right.Name = "lblIRDistance_Right";
            this.lblIRDistance_Right.Size = new System.Drawing.Size(106, 39);
            this.lblIRDistance_Right.TabIndex = 92;
            this.lblIRDistance_Right.Text = "8.92";
            this.lblIRDistance_Right.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tileIRDistance_Right
            // 
            this.tileIRDistance_Right.ActiveControl = null;
            this.tileIRDistance_Right.Enabled = false;
            this.tileIRDistance_Right.Location = new System.Drawing.Point(385, 589);
            this.tileIRDistance_Right.Name = "tileIRDistance_Right";
            this.tileIRDistance_Right.Size = new System.Drawing.Size(120, 58);
            this.tileIRDistance_Right.Style = MetroFramework.MetroColorStyle.Silver;
            this.tileIRDistance_Right.TabIndex = 93;
            this.tileIRDistance_Right.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileIRDistance_Right.UseSelectable = true;
            // 
            // btnAutonomous
            // 
            this.btnAutonomous.Location = new System.Drawing.Point(242, 699);
            this.btnAutonomous.Name = "btnAutonomous";
            this.btnAutonomous.Size = new System.Drawing.Size(263, 23);
            this.btnAutonomous.TabIndex = 94;
            this.btnAutonomous.Text = "Autonomous Settings";
            this.btnAutonomous.UseSelectable = true;
            this.btnAutonomous.Click += new System.EventHandler(this.btnAutonomous_Click);
            // 
            // DriversWindow
            // 
            this.AccessibleName = "";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackImage = ((System.Drawing.Image)(resources.GetObject("$this.BackImage")));
            this.BackImagePadding = new System.Windows.Forms.Padding(25, 12, 0, 0);
            this.ClientSize = new System.Drawing.Size(1107, 731);
            this.Controls.Add(this.btnAutonomous);
            this.Controls.Add(this.lblIRDistance_Right);
            this.Controls.Add(this.tileIRDistance_Right);
            this.Controls.Add(this.pnlDashboard);
            this.Controls.Add(this.lblIRDistance_Left);
            this.Controls.Add(this.tileIRDistance_Left);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.lstboxAutonomousChooser);
            this.Controls.Add(this.lblBattery);
            this.Controls.Add(this.battery);
            this.Controls.Add(this.metroTile4);
            this.Controls.Add(this.tileForklift);
            this.Controls.Add(this.tileIRAngle);
            this.Controls.Add(this.pnlStreamData);
            this.Controls.Add(this.picNormal);
            this.Controls.Add(this.metroTabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DriversWindow";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.StyleManager = this.metroStyleManager1;
            this.Tag = "";
            this.Text = "SmarterDashboard™ - Drivers";
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.metroTabControl1.ResumeLayout(false);
            this.metroTabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNormal)).EndInit();
            this.pnlStreamData.ResumeLayout(false);
            this.pnlStreamData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage metroTabPage4;
        private MetroFramework.Components.MetroStyleExtender metroStyleExtender1;
        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
        private MetroFramework.Controls.MetroTile metroTile4;
        public System.Windows.Forms.PictureBox picNormal;
        private System.Windows.Forms.Timer tmrFps;
        public MetroFramework.Controls.MetroLabel lblRes;
        private System.Windows.Forms.Timer tmrNetworkTables;
        public MetroFramework.Controls.MetroLabel lblImgPrcsDelay;
        private MetroFramework.Controls.MetroPanel pnlStreamData;
        public MetroFramework.Controls.MetroPanel pnlDashboard;
        public MetroFramework.Controls.MetroLabel lblFps;
        public MetroFramework.Controls.MetroLabel lblBits;
        private System.Windows.Forms.PictureBox picLogo;
        public MetroFramework.Controls.MetroLabel lblCompression;
        private System.Windows.Forms.Timer tmrUpdatePictures;
        private MetroFramework.Controls.MetroTile metroTile1;
        public MetroFramework.Controls.MetroComboBox lstboxAutonomousChooser;
        private MetroFramework.Controls.MetroTile tileIRAngle;
        private MetroFramework.Controls.MetroTile tileForklift;
        public MetroFramework.Controls.MetroLabel lblBattery;
        private System.Windows.Forms.Label lblIRDistance_Left;
        public MetroFramework.Controls.MetroLabel metroLabel3;

        public MetroFramework.Controls.MetroProgressBar battery;
        private MetroFramework.Controls.MetroTile tileIRDistance_Left;
        private System.Windows.Forms.Label lblIRDistance_Right;
        private MetroFramework.Controls.MetroTile tileIRDistance_Right;
        private MetroFramework.Controls.MetroButton btnAutonomous;

    }
}