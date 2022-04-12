using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SS.HealthApp.Android.ViewModel
{
    public class AppNavigationMenuItem : ListViewMenuItem
    {
        public bool DisplayOnNavDrawer { get; set; }
        public bool DisplayOnHomeMenu { get; set; }
    }
}