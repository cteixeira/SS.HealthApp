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

using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;

namespace SS.HealthApp.Android.BaseType
{
    public abstract class BaseActivity : AppCompatActivity
    {

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case Resource.Id.home:
                    OnBackPressed();
                    return true;
                case Resource.Id.homeAsUp:
                    OnBackPressed();
                    return true;
                case global::Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }


        protected void SetupToolbar()
        {
            SetupToolbar(Resources.GetString(Resource.String.app_name));
        }

        protected void SetupToolbar(string title)
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.top_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

    }
}