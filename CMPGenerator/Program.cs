using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMPGenerator
{
    static class Program
    {
        public static DataHandler dh { get; set; }
        public static FormMain fm { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            dh = new DataHandler();
            fm = new FormMain(dh);

            Application.Run(fm);
        }
    }
}
