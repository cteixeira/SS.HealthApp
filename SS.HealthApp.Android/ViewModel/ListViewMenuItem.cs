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
    public class ListViewMenuItem
    {
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Icon { get; set; }
        public bool Hidden { get; set; }

    }
}