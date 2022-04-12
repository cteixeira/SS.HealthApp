using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.Connectivity;
using Square.Picasso;
using SS.HealthApp.Android.Util;

namespace SS.HealthApp.Android.News
{
    [Activity(Label = "NewsDetail", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class NewsDetail : BaseType.BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.NewsDetail);
            base.SetupToolbar(Resources.GetString(Resource.String.menu_News));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                string selectedItemId = extras.GetString("selected_item_id");
                LoadViewDetail(selectedItemId);
            }

        }

        private void LoadViewDetail(string selectedItemId)
        {
            PCL.Services.NewsService nService = new PCL.Services.NewsService();
            Model.NewsModels.News news = nService.GetItem(selectedItemId);

            ImageView ivNewsImage = FindViewById<ImageView>(Resource.Id.ivNewsImage);
            TextView tvNewsTitle = FindViewById<TextView>(Resource.Id.tvNewsTitle);
            TextView tvNewsDate = FindViewById<TextView>(Resource.Id.tvNewsDate);
            TextView tvNewsDescription = FindViewById<TextView>(Resource.Id.tvNewsDescription);

            if (!string.IsNullOrEmpty(news.Image))
            {
                Picasso.With(this.ApplicationContext).Load(news.Image).Fit().Into(ivNewsImage);
            }
            else
            {
                ivNewsImage.Visibility = ViewStates.Gone;
            }

            tvNewsTitle.Text = news.Name;
            tvNewsDate.Text = Java.Text.DateFormat.DateInstance.Format(news.Date.ToJavaDate());
            tvNewsDescription.Text = news.Detail;
        }
       
    }
}