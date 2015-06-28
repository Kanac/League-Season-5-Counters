using Microsoft.AdMediator.WindowsPhone81;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace League_of_Legends_Counterpicks.Common
{
    public class AdError
    {
        public void Ad_Error(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        {
            var adName = (sender as AdMediatorControl).Id;
            Debug.WriteLine("AdMediatorError for " + adName + ":" + e.Error + " " + e.ErrorCode);
        }
    }
}
