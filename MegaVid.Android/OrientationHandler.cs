using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MegaVid.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

using Android.Content.PM;
using Xamarin.Forms;
using MegaVid.Droid;

[assembly: Dependency(typeof(OrientationHandler))]
namespace MegaVid.Droid
{
    public class OrientationHandler : IOrientationHandler
    {
        public void SetLandscape()
        {
            var activity = (MainActivity)Forms.Context;
            activity.RequestedOrientation = ScreenOrientation.Landscape;
        }

        public void SetPortrait()
        {
            var activity = (MainActivity)Forms.Context;
            activity.RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}