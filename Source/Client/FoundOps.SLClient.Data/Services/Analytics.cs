using System;
using System.Windows.Browser;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Manages analytics.
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Tracks the event in Totango
        /// </summary>
        /// <param name="activity">The tracked activity</param>
        public static void Track(string activity)
        {
            try
            {
                //Call Totango API (for every event except Manifest Option)
                HtmlPage.Window.Eval("analytics.track('" + activity + "');");
            }
            //Do not stop executing code if there is an error
            catch (Exception) { }
        }
    }
}