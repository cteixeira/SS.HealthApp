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
using SS.HealthApp.Android.Util;
using Plugin.Connectivity;

namespace SS.HealthApp.Android.News
{
    public class NewsList : BaseType.BaseFragment
    {

        ListView newsViewList = null;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.NewsList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetupToolbarTitle(Resources.GetString(Resource.String.menu_News));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            newsViewList = Activity.FindViewById<ListView>(Resource.Id.lvNews);

            LoadNewsListAsync();

        }

        #endregion

        private void NewsListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            NewsListScreenAdapter lvAdapter = lv.Adapter as NewsListScreenAdapter;
            Model.NewsModels.News selectedItem = lvAdapter[e.Position];

            var intent = new Intent(this.Context, typeof(NewsDetail));
            intent.PutExtra("selected_item_id", selectedItem.ID);
            StartActivity(intent);
        }

        private async void LoadNewsListAsync()
        {
            SS.HealthApp.PCL.Services.NewsService nService = new PCL.Services.NewsService();
            List<Model.NewsModels.News> news = await nService.GetItemsAsync();

            newsViewList.Visibility = ViewStates.Visible;
            Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar).Visibility = ViewStates.Gone;

            if (news != null && news.Count > 0)

            {
                newsViewList.Adapter = new NewsListScreenAdapter(this.Activity, news.ToArray());
                newsViewList.ItemClick += NewsListView_Click;
            }
            else
            {
                //no items to display
                Toast.MakeText(this.Context, Resources.GetString(Resource.String.no_records), ToastLength.Long).Show();
            }

        }
    }

    public class NewsListScreenAdapter : BaseAdapter<Model.NewsModels.News>
    {
        public Model.NewsModels.News[] items;
        Activity context;
        public NewsListScreenAdapter(Activity context, Model.NewsModels.News[] items) : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Model.NewsModels.News this[int position]
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
            view.FindViewById<TextView>(Resource.Id.tvDescription).Text = Java.Text.DateFormat.DateInstance.Format(items[position].Date.ToJavaDate());
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