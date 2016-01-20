using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmarterDashboard_Programmers
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                Application.Run(new ProgrammersWindow());
                return;
            }

            // It's possible that args[0] will contain the app's name. Not the case currently
            switch (args[0].ToLower())
            {
                case "driver":
                    Application.Run(new SmarterDashboard_Drivers.DriversWindow()); // false for IsWithButton
                    break;
                case "programmer":
                    Application.Run(new ProgrammersWindow());
                    break;
                case "driverandbuttons":
                    Application.Run(new SmarterDashboard_Drivers.DriversWindow()); // true for IsWithButton
                    break;
                default:
                    MessageBox.Show("The first parameter must be one of the following: driver, programmer or driveAndButtons.");
                    break;
            }
        }
    }
}
