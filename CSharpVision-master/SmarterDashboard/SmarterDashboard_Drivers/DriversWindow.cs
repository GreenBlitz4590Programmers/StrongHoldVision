using System;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using Renci.SshNet;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;


namespace SmarterDashboard_Drivers
{
    public partial class DriversWindow : MetroFramework.Forms.MetroForm
    {
        public static string VisionTargetsThresholdDataFile = "../../../../SmarterDashboard_Drivers/ThresholdData/Bins.txt";

        Stream stream;
        NetworkTable robotNetworkTable;

        private enum ForkliftState { Up, Middle, Down };
        ForkliftState forkliftState;
        double ForkliftHeight = 0;
        const double GoodIRAngle = 5.0;
        const double GoodIRDistance = 0.05;
        double currentIRDistanceRight = 0, currentIRDistanceLeft = 0;

        Dictionary<string, List<MetroFramework.Controls.MetroLabel>> networkTableVariables = new Dictionary<string, List<MetroFramework.Controls.MetroLabel>>();
        Dictionary<string, string[]> autonomousDict = new Dictionary<string, string[]>();
        //Ctor And Initializations
        public DriversWindow()
        {
            InitializeComponent();
            //Initialize Timers
            tmrFps.Start();
            tmrNetworkTables.Start();


            stream = new Stream(UpdateGuiOnNewFrame);

            InitNetworkTable();
            stream.LoadLastThreshData(VisionTargetsThresholdDataFile);

            stream.Init();
            autonomousDict.Add("AutoStupidBinFromStep", new string[] { "1", "UseIR:CheckBox", "IsStupid:CheckBox", "StartLocation:TextBox" });
            autonomousDict.Add("AutoSimple", new string[] { "2" });
            autonomousDict.Add("AuTote", new string[] { "3" });
            autonomousDict.Add("autoTwoTotesAndBinFromStep", new string[] { "4" });
            autonomousDict.Add("autoAllSets", new string[] { "5" });
            autonomousDict.Add("autoBin", new string[] { "6" });
            autonomousDict.Add("autoBoris", new string[] { "7" });
            autonomousDict.Add("AutoTwoBinsFromStepOnTote", new string[] { "8" });

            foreach (string s in new List<string>(autonomousDict.Keys))
            {
                lstboxAutonomousChooser.Items.Add(s);
            }
            lstboxAutonomousChooser.SelectedIndex = 1;
        }


        void InitNetworkTable()
        {
            robotNetworkTable = new NetworkTable(NetworkTableNewData);
            new Thread(() =>
            {
                while (!stream.isRunning) Thread.Sleep(100);
                robotNetworkTable.Start();
            }).Start();
        }

        //Stop The Stream When Form'S Closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (robotNetworkTable != null) robotNetworkTable.Close();
            stream.StopStream();
            Environment.Exit(0);
        }


        //Timer Event That Updates All The Values That Should Be Sent To The Network Table
        private void tmrNetworkTables_Tick(object sender, EventArgs e)
        {
            if (robotNetworkTable != null && stream.ProccessedCurrentFrame != null)
            {
                Size res = stream.ProccessedCurrentFrame.Size;

                //TODO: calculate stuff

                robotNetworkTable.SetVariable("SmarterDashboard::AngleToBin", stream.GetAngleToBin());
                //TODO: add more
            }
        }


        delegate void NetworkTableNewDataEventArgs(string source, string key, object _value, bool isNew);
        //Called Each Time The NetworkTable Receives Data
        private void NetworkTableNewData(string source, string key, object _value, bool isNew)
        {
            // Thread Safety
            if (this.InvokeRequired)
            {
                this.Invoke((NetworkTableNewDataEventArgs)NetworkTableNewData, new object[] { source, key, _value, isNew });
                return;
            }



            // Thread Safe Now
            string value = _value.ToString();


            #region Network Table Arguments That Have Proprietary GUI ObjectS
            if (key == "Battery")
            {
                UpdateBattery(Convert.ToDouble(value));
            }
            else if (key == "Forklift::ms_Up")
            {
                if (value == "1.0")
                {
                    tileForklift.Text = "Forklift Up";
                    tileForklift.Style = MetroFramework.MetroColorStyle.Blue;
                    forkliftState = ForkliftState.Up;
                }
                else
                {
                    forkliftState = ForkliftState.Middle;
                }
            }
            else if (key == "Forklift::ms_Down")
            {
                if (value == "1.0")
                {
                    tileForklift.Text = "Forklift Down";
                    tileForklift.Style = MetroFramework.MetroColorStyle.Blue;
                    forkliftState = ForkliftState.Down;
                }
                else
                {
                    forkliftState = ForkliftState.Middle;
                }
            }
            else if (key == "Forklift::height")
            {
                ForkliftHeight = Convert.ToDouble(value);
            }
            else if (key == "Chassis::IRAngle")
            {
                if (Convert.ToDouble(value) < GoodIRAngle)
                {
                    tileIRAngle.Text = "Aligned To Tote";
                    tileIRAngle.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    tileIRAngle.Text = "IR Angle: " + value.Substring(0, Math.Min(5, value.Length));
                    tileIRAngle.Style = MetroFramework.MetroColorStyle.Silver;
                }
            }
            else if (key == "Chassis::IR_distanceRight")
            {
                currentIRDistanceRight = Convert.ToDouble(value);
            }
            else if (key == "Chassis::IR_distanceLeft")
            {
                currentIRDistanceLeft = Convert.ToDouble(value);
            }

            #endregion


            #region Every Other Network Table Arguments
            else
            {
                int colonIndex = key.IndexOf("::");
                string subsystem;
                if (colonIndex != -1)
                    subsystem = key.Substring(0, colonIndex);
                else
                {
                    subsystem = "Untitled";
                    key = "Untitled::" + key;
                    colonIndex = key.IndexOf("::");
                }
                if (isNew)
                {
                    if (!networkTableVariables.ContainsKey(subsystem))
                    {
                        networkTableVariables.Add(subsystem, new List<MetroFramework.Controls.MetroLabel>());
                        pnlDashboard.Controls.Add(GetNTableVarLabel(subsystem, subsystem + ":"));
                    }

                    MetroFramework.Controls.MetroLabel lbl = GetNTableVarLabel(key,
                        "    " + key.Substring(colonIndex + 2, key.Length - (colonIndex + 2)) + ": " + value.Substring(0, Math.Min(8, value.Length)));
                    networkTableVariables[subsystem].Add(lbl);
                    pnlDashboard.Controls.Add(lbl);

                    ReorderPnlDashboard();
                }
                else
                {
                    MetroFramework.Controls.MetroLabel lbl = (MetroFramework.Controls.MetroLabel)pnlDashboard.Controls.Find(key, false)[0];
                    lbl.Text = "    " + key.Substring(colonIndex + 2, key.Length - (colonIndex + 2)) + ": " + value.Substring(0, Math.Min(8, value.Length));
                }
            }
            #endregion


            string s = "" + currentIRDistanceRight;
            lblIRDistance_Right.Text = s.Substring(0, Math.Min(4, s.Length));
            tileIRDistance_Left.Style = currentIRDistanceRight < GoodIRDistance ? MetroFramework.MetroColorStyle.Blue : MetroFramework.MetroColorStyle.Silver;

            s = "" + currentIRDistanceLeft;
            lblIRDistance_Left.Text = s.Substring(0, Math.Min(4, s.Length));
            tileIRDistance_Right.Style = currentIRDistanceLeft < GoodIRDistance ? MetroFramework.MetroColorStyle.Blue : MetroFramework.MetroColorStyle.Silver;



            if (forkliftState == ForkliftState.Middle)
            {
                tileForklift.Style = MetroFramework.MetroColorStyle.Silver;
                string height = ForkliftHeight + "";
                tileForklift.Text = "Forklift: " + height.Substring(0, Math.Min(5, height.Length));
            }
        }

        #region Battery Consts
        const double BatteryMinVolts = 11.7;
        const double BatteryMaxVolts = 13.5;
        const double BatteryCriticalVolts = 12.5;
        #endregion
        private void UpdateBattery(double volts)
        {
            if (volts < BatteryMinVolts)
                battery.Value = 0;
            else
                battery.Value = (int)(((volts - BatteryMinVolts) / (BatteryMaxVolts - BatteryMinVolts)) * 100);

            battery.Style = volts <= BatteryCriticalVolts ? MetroFramework.MetroColorStyle.Red : MetroFramework.MetroColorStyle.Green;
            lblBattery.Text = ((double)(int)(volts * 100)) / 100 + "V";
        }

        private MetroFramework.Controls.MetroLabel GetNTableVarLabel(string name, string text)
        {
            MetroFramework.Controls.MetroLabel lbl = new MetroFramework.Controls.MetroLabel();
            lbl.AutoSize = true;
            lbl.UseCustomBackColor = true;
            lbl.BackColor = pnlDashboard.BackColor;
            lbl.Name = name;
            lbl.Text = text;

            return lbl;
        }

        private void ReorderPnlDashboard()
        {
            int index = 1;
            foreach (var subsystem in networkTableVariables)
            {
                pnlDashboard.Controls.Find(subsystem.Key, false)[0].Location = new Point(1, (++index - 2) * 20);


                foreach (MetroFramework.Controls.MetroLabel lbl in subsystem.Value)
                    lbl.Location = new Point(1, (++index - 2) * 20);


                index += 1;
            }
        }


        //Each Second Update Frame Rate
        private void timerFps_Tick(object sender, EventArgs e)
        {
            if (stream != null)
            {
                stream.UpdateStreamMonitoringVariables();
                lblFps.Text = "FPS: " + stream.Fps;
                lblBits.Text = "MBi Received: " + stream.Bits / 1000000.0;
                lblImgPrcsDelay.Text = "Image Proccesing Delay: " + stream.Delay;

                lblRes.Text = string.Format("Resolution: {0}x{1}", stream.DefaultResolution.Width, stream.DefaultResolution.Height);
                lblCompression.Text = "Compression: " + stream.DefaultCompression;
            }
        }

        void UpdateGuiOnNewFrame(Image proccessed)
        {
            //Thread Safety
            if (this.picNormal.InvokeRequired)
            {
                Stream.UpdateGuiOnNewFrameFunction d = new Stream.UpdateGuiOnNewFrameFunction(UpdateGuiOnNewFrame);
                this.Invoke(d, new object[] { proccessed });
            }
            else
            {
                this.picNormal.Image = proccessed;
            }
        }

        private void btnAutonomous_Click(object sender, EventArgs e)
        {
            if (lstboxAutonomousChooser.SelectedItem == null || lstboxAutonomousChooser.SelectedItem.ToString().Length == 0) return;
            MetroFramework.Forms.MetroForm autoForm = new MetroFramework.Forms.MetroForm();
            MetroFramework.Controls.MetroPanel p = new MetroFramework.Controls.MetroPanel();
            p.Name = "panel";

            string[] autoParams = autonomousDict[lstboxAutonomousChooser.SelectedItem.ToString()];
            if (autoParams.Length < 2) return;
            for (int i = 1; i < autoParams.Length; i++)
            {
                MetroFramework.Controls.MetroLabel lbl = new MetroFramework.Controls.MetroLabel();
                lbl.Text = autoParams[i].Substring(0, autoParams[i].IndexOf(":"));
                lbl.Location = new Point(10, 30 + 30 * i);
                p.Controls.Add(lbl);
                Control c;
                string ControlText = autoParams[i].Substring(autoParams[i].IndexOf(":") + 1);
                switch (ControlText)
                {
                    case "TextBox":
                        c = new MetroFramework.Controls.MetroTextBox();
                        c.Size = new Size(100, lbl.Height);
                        break;
                    case "CheckBox":
                        c = new MetroFramework.Controls.MetroCheckBox();
                        break;
                    default:
                        MessageBox.Show("Unsupported Control");
                        return;
                }
                c.Location = new Point(10 + lbl.Width, 30 + 30 * i);
                p.Controls.Add(c);
            }
            p.Top = 55;
            p.Width = autoForm.Width;
            p.Height = autoForm.Height - p.Top;
            autoForm.Controls.Add(p);
            autoForm.Text = lstboxAutonomousChooser.SelectedItem.ToString();
            autoForm.PerformLayout();
            autoForm.Show();
            autoForm.FormClosing += autoForm_FormClosing;
        }

        void autoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Thread Safety
            if (this.InvokeRequired)
            {
                this.Invoke((FormClosingEventHandler)autoForm_FormClosing, new object[] { sender, e });
                return;
            }

            foreach (Control _c in ((MetroFramework.Forms.MetroForm)sender).Controls)
            {
                if (_c.Name == "panel")
                {
                    for (int i = 2; i < _c.Controls.Count; i++)
                    {

                        string key = ((MetroFramework.Forms.MetroForm)sender).Text + "::" + ((MetroFramework.Controls.MetroLabel)_c.Controls[i]).Text;
                        i++;
                        if (_c.Controls[i] as CheckBox != null)
                            robotNetworkTable.SetVariable(key, ((CheckBox)_c.Controls[i]).Checked);

                        if (_c.Controls[i] as MetroFramework.Controls.MetroTextBox != null)
                        {
                            int val;
                            bool isInt = int.TryParse(((MetroFramework.Controls.MetroTextBox)_c.Controls[i]).Text, out val);
                            if (isInt)
                            {
                                robotNetworkTable.SetVariable(key, val);
                            }
                            else
                            {
                                robotNetworkTable.SetVariable(key, ((TextBox)_c.Controls[i]).Text);
                            }
                        }


                    }
                    break;
                }
            }

        }

        private void lstboxAutonomousChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            robotNetworkTable.SetVariable("AutonomousCommand", int.Parse(autonomousDict[lstboxAutonomousChooser.SelectedItem.ToString()][0]));
        }


    }
}
