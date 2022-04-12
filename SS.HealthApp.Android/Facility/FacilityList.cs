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
using Square.Picasso;
using Java.Lang;
using Plugin.Connectivity;

namespace SS.HealthApp.Android.Facility
{
    public class FacilityList : BaseType.BaseFragment
    {

        ListView facilitiesViewList = null;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.FacilityList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetupToolbarTitle(Resources.GetString(Resource.String.menu_Facility));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            facilitiesViewList = Activity.FindViewById<ListView>(Resource.Id.lvFacilities);

            LoadFacilitiesListAsync();

        }

        #endregion

        private void FacilitiesListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            //int position = e.Position;
            ListView lv = sender as ListView;
            FacilityListScreenAdapter lvAdapter = lv.Adapter as FacilityListScreenAdapter;
            Model.FacilityModels.Facility selectedItem = lvAdapter[e.Position];

            var intent = new Intent(this.Context, typeof(FacilityDetail));
            intent.PutExtra("selected_item_id", selectedItem.ID);
            StartActivity(intent);
        }

        private async void LoadFacilitiesListAsync()
        {
            SS.HealthApp.PCL.Services.FacilityService fService = new PCL.Services.FacilityService();
            List<Model.FacilityModels.Facility> facilities = await fService.GetItemsAsync();

            facilitiesViewList.Visibility = ViewStates.Visible;
            Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar).Visibility = ViewStates.Gone;

            if (facilities != null && facilities.Count > 0)
            {
                facilitiesViewList.Adapter = new FacilityListScreenAdapter(this.Activity, facilities.ToArray());
                facilitiesViewList.ItemClick += FacilitiesListView_Click;
            }

        }

    }

    public class FacilityListScreenAdapter : BaseAdapter<Model.FacilityModels.Facility>
    {
        public Model.FacilityModels.Facility[] items;
        Activity context;
        public FacilityListScreenAdapter(Activity context, Model.FacilityModels.Facility[] items) : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Model.FacilityModels.Facility this[int position]
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
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomListItemImageTwoLines, null);
            view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Name;
            ImageView iv = view.FindViewById<ImageView>(Resource.Id.ivCustomListImage);
            if (!string.IsNullOrEmpty(items[position].Image))
            {
                Picasso.With(context).Load(items[position].Image)
                    .Placeholder(Resource.Drawable.list_placeholder)
                    .Error(Resource.Drawable.list_placeholder).Fit().CenterCrop().Into(iv);
            }
            else
            {
                Picasso.With(context).Load(Resource.Drawable.list_placeholder).Fit().CenterCrop().Into(iv);
            }
            return view;
        }

    }

}