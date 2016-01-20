using System;
using edu.wpi.first.wpilibj.networktables;
using edu.wpi.first.wpilibj.tables;
using edu.wpi.first.smartdashboard.robot;
using MetroFramework.Controls;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace SmarterDashboard_Programmers
{
    //TODO: Check if the DLL should be changed to 2015
    enum Types { String, Double, Bool, Object }

    class NetworkTable : ITableListener
    {
        public delegate void NewDataEventHandler(string source, string key, object value, bool isNew);

        private edu.wpi.first.wpilibj.networktables.NetworkTable table;
        NewDataEventHandler updateGuiOnNewData;
        Thread networkTableThread;
        bool shouldStop = false;
        bool isRoborio;


        Dictionary<string, GenericType> outVariables = new Dictionary<string,GenericType>();



        public NetworkTable(NewDataEventHandler updateGuiOnNewData, bool isRoborio)
        {
            this.updateGuiOnNewData = updateGuiOnNewData;
            this.isRoborio = isRoborio;
            networkTableThread = new Thread(SendDataToNetworkTable);
        }

        public void Start()
        {
            //TODO: check if works with Roborio. Maybe Robot.setHost("roborio-4590.local");
            if (isRoborio) 
                Robot.setHost("roborio-4590.local");
            Robot.setTeam(4590);

            if (table == null)
            {
                table = Robot.getTable() as edu.wpi.first.wpilibj.networktables.NetworkTable;
                table.addTableListener(this);
            }
            if (!networkTableThread.IsAlive)
            {
                shouldStop = false;
                networkTableThread.Start();
            }
        }

        // Nicely Asks Thread To Stop
        public void Stop()
        {
            shouldStop = true;
        }

        // Abort Thread
        public void ForceStop()
        {
            shouldStop = true;
            networkTableThread.Abort();
        }

        public void Close()
        {
            ForceStop();
            if (table != null)
            {
                table.removeTableListener(this);
                table = null;
            }
        }



        //Worker Thread That Sends The Data To The Network Tables; Putdata Is Blocking.
        private void SendDataToNetworkTable()
        {
            while (!shouldStop)
            {
                foreach (KeyValuePair<string, GenericType> v in outVariables)
                {
                    switch (v.Value.type)
                    {
                        case Types.Double:
                            PutValue(v.Key, (double)v.Value.value);
                            break;
                        case Types.String:
                            PutValue(v.Key, (string)v.Value.value);
                            break;
                        case Types.Bool:
                            PutValue(v.Key, (bool)v.Value.value);
                            break;
                        default:
                            PutValue(v.Key, v.Value.value);
                            break;
                    }
                }

                Thread.Sleep(50);
            }
        }



        //Listener Function
        public void valueChanged(ITable source, string key, object value, bool isNew)
        {
            updateGuiOnNewData(source.ToString(), key, value, isNew);
        }

        public bool isConnected()
        {
            return table.isConnected();
        }




        public void SetVariable(string key, string value)
        {
            if (!outVariables.ContainsKey(key))
                outVariables.Add(key, new GenericType(value, Types.String));
            else
                outVariables[key].value = value;
        }
        public void SetVariable(string key, double value)
        {
            if (!outVariables.ContainsKey(key))
                outVariables.Add(key, new GenericType(value, Types.Double));
            else
                outVariables[key].value = value;
        }
        public void SetVariable(string key, bool value)
        {
            if (!outVariables.ContainsKey(key))
                outVariables.Add(key, new GenericType(value, Types.Bool));
            else
                outVariables[key].value = value;
        }
        public void SetVariable(string key, object value)
        {
            if (!outVariables.ContainsKey(key))
                outVariables.Add(key, new GenericType(value, Types.Object));
            else
                outVariables[key].value = value;
        }

        //Send A Value To The Network Table
        public void PutValue(string key, object value)
        {
            if (isConnected())
                table.putValue(key, value);
        }
        public void PutValue(string key, double value)
        {
            if (isConnected())
                table.putNumber(key, value);
        }
        public void PutValue(string key, string value)
        {
            if (isConnected())
                table.putString(key, value);
        }
        public void PutValue(string key, bool value)
        {
            if (isConnected())
                table.putBoolean(key, value);
        }
    }

    class GenericType
    {
        public object value;
        public Types type;
        public GenericType(object value, Types type)
        {
            this.value = value;
            this.type = type;
        }
    }
}