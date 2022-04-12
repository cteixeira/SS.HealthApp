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
using System.Threading.Tasks;

namespace SS.HealthApp.Android.User
{
    public class UserSettingsList : BaseType.BaseFragment
    {

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.UserSettingsList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetupToolbarTitle(Resources.GetString(Resource.String.menu_PersonalData));

            LoadMenu();

        }

        #endregion

        private void ListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            UserSettingsListScreenAdapter lvAdapter = lv.Adapter as UserSettingsListScreenAdapter;
            ViewModel.ListViewActivityLinkMenuItem selectedItem = lvAdapter[e.Position];

            if (selectedItem.ActivityLinkType != null)
            {
                var intent = new Intent(this.Context, selectedItem.ActivityLinkType);
                StartActivity(intent);
            }

        }

        private void LoadMenu()
        {
            List<ViewModel.ListViewActivityLinkMenuItem> listItems = new List<ViewModel.ListViewActivityLinkMenuItem>{
                   new ViewModel.ListViewActivityLinkMenuItem {
                       Title = Resources.GetString(Resource.String.menu_PersonalData),
                       Description = Resources.GetString(Resource.String.menu_UserSettings_PersonalDataDescription),
                       ActivityLinkType = typeof(User.UserDetail)
                   },
                   new ViewModel.ListViewActivityLinkMenuItem {
                       Title = Resources.GetString(Resource.String.menu_UserSettings_ChangePassword),
                       Description = Resources.GetString(Resource.String.menu_UserSettings_ChangePasswordDescription),
                       ActivityLinkType = typeof(User.ChangePassword)
                   }//,
                   //new ViewModel.ListViewActivityLinkMenuItem {
                   //    Title = Resources.GetString(Resource.String.menu_UserSettings_Notifications),
                   //    Description = Resources.GetString(Resource.String.menu_UserSettings_NotificationsDescription),
                   //    ActivityLinkType = typeof(User.Notifications)
                   //}
            };

            ListView lvMenu = Activity.FindViewById<ListView>(Resource.Id.lvUserSettings);
            lvMenu.Adapter = new UserSettingsListScreenAdapter(this.Activity, listItems.ToArray());
            lvMenu.ItemClick += ListView_Click;

        }

        public class UserSettingsListScreenAdapter : BaseAdapter<ViewModel.ListViewActivityLinkMenuItem>
        {
            public ViewModel.ListViewActivityLinkMenuItem[] items;
            Activity context;
            public UserSettingsListScreenAdapter(Activity context, ViewModel.ListViewActivityLinkMenuItem[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override ViewModel.ListViewActivityLinkMenuItem this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.CustomListItemTwoLines, null);
                view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Title;
                view.FindViewById<TextView>(Resource.Id.tvDescription).Text = items[position].Description;
                return view;
            }

        }

    }
}