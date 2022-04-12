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

using Fragment = Android.Support.V4.App.Fragment;

namespace SS.HealthApp.Android.BaseType
{
    public abstract class BaseFragment : Fragment
    {

        #region activity lifecycle methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        protected void SetupToolbarTitle(string title)
        {
            ((global::Android.Support.V7.App.AppCompatActivity)Activity).SupportActionBar.Title = title;
        }

        #endregion

        #region top toolbar methdos

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            menu.Clear();
            base.OnCreateOptionsMenu(menu, inflater);
        }

        #endregion

    }
}