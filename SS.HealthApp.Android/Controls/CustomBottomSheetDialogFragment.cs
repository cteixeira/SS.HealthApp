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
using Android.Support.V7.App;

namespace SS.HealthApp.Android.Controls
{
    //RC: this class is a replica from android source code from https://android.googlesource.com/platform/frameworks/support.git/+/master/design/src/android/support/design/widget/BottomSheetDialogFragment.java
    public class CustomBottomSheetDialogFragment : AppCompatDialogFragment
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new CustomBottomSheetDialog(Context, Theme);
        }

    }
}