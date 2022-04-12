using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SS.HealthApp.Android.Controls;

using CoordinatorLayout = global::Android.Support.Design.Widget.CoordinatorLayout;
using BottomSheetBehavior = global::Android.Support.Design.Widget.BottomSheetBehavior;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Java.Lang;
using System.Threading.Tasks;

namespace SS.HealthApp.Android.Appointment
{
    [global::Android.App.Activity(Label = "AppointmentNew", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class AppointmentNew : BaseType.BaseActivity
    {

        TabLayout tabLayout = null;
        ViewPagerAdapter adapter = null;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AppointmentNew);

            base.SetupToolbar(Resources.GetString(Resource.String.appointment_NewAppointment));
            SupportActionBar.Elevation = 0;

            ViewPager viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
            tabLayout = (TabLayout)FindViewById(Resource.Id.tabs);

            ResetAppointmentBook(Model.Enum.AppointmentType.C);
            string facilityId = null;

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                facilityId = extras.GetString("facilityId");
            }

            await LoadPickersData(facilityId);
            
            //only show tabs after appointment data loaded from server
            setupViewPager(viewPager);
            

        }

        private void setupViewPager(ViewPager viewPager)
        {
            adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.addFragment(new AppointmentNewStep0(Model.Enum.AppointmentType.C), Resources.GetString(Resource.String.appointment_type0));
            adapter.addFragment(new AppointmentNewStep0(Model.Enum.AppointmentType.E), Resources.GetString(Resource.String.appointment_type1));
            adapter.addFragment(new AppointmentNewStep0(Model.Enum.AppointmentType.O), Resources.GetString(Resource.String.appointment_type2));
            viewPager.OffscreenPageLimit = 3;
            viewPager.Adapter = adapter;
            viewPager.PageSelected += ViewPager_PageSelected;
            tabLayout.SetupWithViewPager(viewPager);
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            AppointmentNewStep0 fragment = adapter.GetItem(e.Position) as AppointmentNewStep0;
            ResetAppointmentBook(fragment.TypeOfAppointment);
            fragment.RefreshFormData();
        }

        private void ResetAppointmentBook(Model.Enum.AppointmentType Type) {
            PCL.Services.AppointmentService.ApptBook = new Model.AppointmentModels.AppointmentBook();
            PCL.Services.AppointmentService.ApptBook.Type = Type;
            PCL.Services.AppointmentService.ApptBook.Moment = DateTime.Now.AddDays(1).Date;
        }

        private async Task LoadPickersData(string FacilityId)
        {
            SS.HealthApp.PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();
            Model.AppointmentModels.AppointmentData aData = await aService.GetAllDataAsync();

            if (!string.IsNullOrEmpty(FacilityId))
            {
                //select the facility
                Model.PickerItem facilityPickerItem = aData.Facilities.FirstOrDefault(f => f.ID == FacilityId);
                if(facilityPickerItem != null)
                {
                    PCL.Services.AppointmentService.ApptBook.Facility = facilityPickerItem;
                }
                
            }

            tabLayout.Visibility = ViewStates.Visible;
            FindViewById<LinearLayout>(Resource.Id.llProgressBar).Visibility = ViewStates.Gone;
            return;
        }

    }

    class ViewPagerAdapter : FragmentPagerAdapter
    {
        private List<Fragment> fragmentList = new List<Fragment>();
        private List<string> fragmentTitleList = new List<string>();

        public override int Count
        {
            get
            {
                return fragmentList.Count;
            }
        }

        public ViewPagerAdapter(FragmentManager manager) : base(manager)
        {
        }

        public override Fragment GetItem(int position)
        {
            return fragmentList[position];
        }

        public int GetCount()
        {
            return fragmentList.Count;
        }

        public void addFragment(Fragment fragment, string title)
        {
            fragmentList.Add(fragment);
            fragmentTitleList.Add(title);
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(fragmentTitleList[position]);
        }

    }

}