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

namespace SS.HealthApp.Android
{

    public class About : BaseType.BaseFragment
    {

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.About, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //Set top bar title
            //((global::Android.Support.V7.App.AppCompatActivity)Activity).SupportActionBar.Title = Resources.GetString(Resource.String.menu_About);

            String version = string.Empty;

            try
            {
                global::Android.Content.PM.PackageInfo pInfo = Context.PackageManager.GetPackageInfo(Context.PackageName, global::Android.Content.PM.PackageInfoFlags.Activities);
                version = pInfo.VersionName;
            }
            catch (Exception)
            {
                //could not get version, ignore it
            }

            Activity.FindViewById<ImageView>(Resource.Id.ivLogo).Click += ivLogo_Click;
            //Activity.FindViewById<TextView>(Resource.Id.tvCopyright).Text = String.Format(GetString(Resource.String.AboutCopyright), GetString(Resource.String.client_name));
            //Activity.FindViewById<TextView>(Resource.Id.tvDevelopedBy).Click += tvDevelopedBy_Click;
            Activity.FindViewById<TextView>(Resource.Id.tvVersion).Text = String.Format(GetString(Resource.String.AboutVersion), version);

        }

        private void ivLogo_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.AddCategory(Intent.CategoryBrowsable);
            intent.SetData(global::Android.Net.Uri.Parse(GetString(Resource.String.client_url)));
            StartActivity(intent);
        }

        private void tvDevelopedBy_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.AddCategory(Intent.CategoryBrowsable);
            intent.SetData(global::Android.Net.Uri.Parse(GetString(Resource.String.developer_url)));
            StartActivity(intent);
        }

        #endregion

    }
}