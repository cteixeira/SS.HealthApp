using System;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DrawerLayout = Android.Support.V4.Widget.DrawerLayout;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Content.Res;
using SS.HealthApp.Android.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Android.Content;

namespace SS.HealthApp.Android
{
    [Activity(Label = "MainActivity", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout drawerLayout;
        private ListView drawerList;
        ActionBarDrawerToggle drawerToggle;

        private List<AppNavigationMenuItem> navigationMenuItems = null;
        public List<AppNavigationMenuItem> NavDrawerMenuItems { get { return navigationMenuItems.Where(i => i.DisplayOnNavDrawer).ToList(); } }
        public List<AppNavigationMenuItem> HomeMenuItems { get { return navigationMenuItems.Where(i => i.DisplayOnHomeMenu).ToList(); } }

        #region activity methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            
            // set the toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.top_toolbar);
            //Toolbar will now take on default Action Bar characteristics
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Resources.GetString(Resource.String.app_name);

            // set the navigation drawer
            SetupNavDrawer(bundle, toolbar);

        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            // Sync the toggle state after onRestoreInstanceState has occurred.
            drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            // Pass any configuration change to the drawer toggles
            drawerToggle.OnConfigurationChanged(newConfig);
        }

        #endregion

        #region toolbar methods

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenuHome, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_personalData)
            {

                int navDrawerItemPosition = NavDrawerMenuItems.FindIndex(i => i.Title == Resources.GetString(Resource.String.menu_PersonalData));

                drawerList.PerformItemClick(drawerList.Adapter.GetView(navDrawerItemPosition, null, null), navDrawerItemPosition, navDrawerItemPosition);
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region drawer methods

        private void SetupNavDrawer(Bundle bundle, Toolbar toolbar)
        {

            InitNavDrawerSections();

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerList = FindViewById<ListView>(Resource.Id.drawer_menu);

            drawerToggle = new ActionBarDrawerToggle(this,
                                            drawerLayout,
                                            toolbar,
                                            Resource.String.drawer_open,
                                            Resource.String.drawer_close);

            drawerLayout.SetDrawerListener(drawerToggle);

            // Set the adapter for the list view
            drawerList.Adapter = new NavDrawerMenuAdapter(this, NavDrawerMenuItems.ToArray());

            // Set the list's click listener
            drawerList.ItemClick += (sender, args) => DrawerList_ItemClick(args.Position);

            //if first time you will want to go ahead and click first item.
            if (bundle == null)
            {
                DrawerList_ItemClick(0);
            }
        }

        private void InitNavDrawerSections()
        {
            navigationMenuItems = new List<AppNavigationMenuItem>();

            //Home
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "home",
                Title = Resources.GetString(Resource.String.menu_Home),
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = false
            });

            //Appointment
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "appointment",
                Title = Resources.GetString(Resource.String.menu_Appointment),
                Description = Resources.GetString(Resource.String.menu_AppointmentDescription),
                Icon = Resource.Drawable.ic_event_white_36dp,
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = true
            });

            //Messages
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "messages",
                Title = Resources.GetString(Resource.String.menu_Messages),
                Description = Resources.GetString(Resource.String.menu_MessagesDescription),
                Icon = Resource.Drawable.ic_question_answer_white_36dp,
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = true
            });

            //AccountStatment
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "accountStatment",
                Title = Resources.GetString(Resource.String.menu_AccountStatment),
                Description = Resources.GetString(Resource.String.menu_AccountStatmentDescription),
                Icon = Resource.Drawable.ic_euro_symbol_white_36dp,
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = true
            });

            //News
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "news",
                Title = Resources.GetString(Resource.String.menu_News),
                Description = Resources.GetString(Resource.String.menu_NewsDescription),
                Icon = Resource.Drawable.ic_import_contacts_white_36dp,
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = true
            });

            //facility
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "facility",
                Title = Resources.GetString(Resource.String.menu_Facility),
                Description = Resources.GetString(Resource.String.menu_FacilityDescription),
                Icon = Resource.Drawable.ic_pin_drop_white_36dp,
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = true
            });

            //Declarations
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "declarations",
                Title = Resources.GetString(Resource.String.menu_Declarations),
                Description = Resources.GetString(Resource.String.menu_DeclarationsDescription),
                Icon = Resource.Drawable.ic_insert_drive_file_white_36dp,
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = true
            });

            //Personal Data
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "personaldata",
                Title = Resources.GetString(Resource.String.menu_PersonalData),
                Description = Resources.GetString(Resource.String.menu_PersonalData),
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = false
            });

            //About
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "about",
                Title = Resources.GetString(Resource.String.menu_About),
                Description = Resources.GetString(Resource.String.menu_About),
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = false
            });

            //Logout
            navigationMenuItems.Add(new ViewModel.AppNavigationMenuItem {
                Tag = "logout",
                Title = Resources.GetString(Resource.String.menu_Logout),
                Description = Resources.GetString(Resource.String.menu_Logout),
                DisplayOnNavDrawer = true,
                DisplayOnHomeMenu = false
            });

        }

        private void DrawerList_ItemClick(int Position)
        {

            NavDrawerMenuAdapter adapter = drawerList.Adapter as NavDrawerMenuAdapter;
            ListViewMenuItem item = adapter[Position];

            if (item == null)
            {
                return;
            }

            Fragment fragment = null;
            string fragmentTag = null;

            switch (item.Tag)
            {
                case "home":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new Home.Home();
                    }
                    break;
                case "appointment":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new Appointment.AppointmentList();
                    }
                    break;
                case "messages":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new Message.MessageList();
                    }
                    break;
                case "accountStatment":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new Account.AccountList();
                    }
                    break;
                case "news":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new News.NewsList();
                    }
                    break;
                case "facility":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new Facility.FacilityList();
                    }
                    break;
                case "declarations":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new Declaration.DeclarationList();
                    }
                    break;
                case "personaldata":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new User.UserSettingsList();
                    }
                    break;
                case "about":
                    fragment = SupportFragmentManager.FindFragmentByTag(item.Tag);
                    if (fragment == null)
                    {
                        fragment = new About();
                    }
                    break;
                case "logout":
                    Logout();
                    return;
                default:
                    break;
            }
            
            ChangeFragment(fragment, fragmentTag);

            this.drawerList.SetItemChecked(Position, true);
            this.drawerLayout.CloseDrawer(this.drawerList);

        }

        #endregion

        #region utils

        private void ChangeFragment(Fragment NewFragment, string Tag)
        {

            SupportFragmentManager.PopBackStackImmediate("toplevel_fragment", (int)PopBackStackFlags.Inclusive);

            if (NewFragment.GetType() == typeof(Home.Home))
            {
                SupportFragmentManager.BeginTransaction()
                   .Replace(Resource.Id.content_frame, NewFragment, Tag)
                   .Commit();

            }
            else
            {
                SupportFragmentManager.BeginTransaction()
                .AddToBackStack("toplevel_fragment")
                .Replace(Resource.Id.content_frame, NewFragment, Tag)
                   .Commit();
            }

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (SupportFragmentManager.BackStackEntryCount == 0)
            {
                this.drawerList.SetItemChecked(0, true);
            }
        }

        private async void Logout()
        {
            if (await new PCL.Services.UserService().Logout())
            {
                Intent loginIntent = new Intent(Application.Context, typeof(User.Login));
                loginIntent.AddFlags(ActivityFlags.ClearTask); //clear the activity popback stack
                loginIntent.AddFlags(ActivityFlags.NewTask);
                StartActivity(loginIntent);
            }

        }

        #endregion

        public class NavDrawerMenuAdapter : BaseAdapter<ListViewMenuItem>
        {
            ListViewMenuItem[] items;
            Activity context;
            public NavDrawerMenuAdapter(Activity context, ListViewMenuItem[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override ListViewMenuItem this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.NavDrawerMenuItem, null);
                view.FindViewById<TextView>(Resource.Id.itemNavDrawerText).Text = items[position].Title;
                return view;
            }

        }

    }
}