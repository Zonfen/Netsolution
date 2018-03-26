using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

/**
 * @Author: Tony Pepic
 * @Date: 2018-03-19
 * 
 * Netsolution test - TMDB 
 * 
 * The following application is made to showcase how to communicate with third party API's.
 * The API will show a number of elements over HTTP. The number of API calls cannot be so few that the 
 * application can do it during startup.
 *
 * 
 * @ Copyright - Netsolution && Tony Pepic - All rights reserved 2018
 *
 * 
 */

namespace NetSolution
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
