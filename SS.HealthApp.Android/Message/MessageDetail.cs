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
using Plugin.Connectivity;
using SS.HealthApp.Android.Util;
using Java.Text;

namespace SS.HealthApp.Android.Message
{
    [Activity(Label = "MessageDetail", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class MessageDetail : BaseType.BaseActivity
    {

        LinearLayout llProgressBar = null;
        ListView conversationListView = null;

        string selectedItemId = null;
        string selectedItemTitle = null;
        string selectedItemDescription = null;

        #region activity lifecycle methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MessageDetail);
            base.SetupToolbar(Resources.GetString(Resource.String.menu_Messages));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            llProgressBar = FindViewById<LinearLayout>(Resource.Id.llProgressBar);
            conversationListView = FindViewById<ListView>(Resource.Id.lvConversation);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                selectedItemId = extras.GetString("selected_item_id");
                selectedItemTitle = extras.GetString("selected_item_title");
                selectedItemDescription = extras.GetString("selected_item_description");

                //fill header info
                TextView tvSubHeaderTitle = FindViewById<TextView>(Resource.Id.tvSubHeaderTitle);
                tvSubHeaderTitle.Text = selectedItemTitle;
                TextView tvSubHeaderDescription = FindViewById<TextView>(Resource.Id.tvSubHeaderDescription);
                tvSubHeaderDescription.Text = selectedItemDescription;
                
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            LoadViewDetail();

        }

        #endregion

        #region toolbar methods

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenuMessageDetail, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_reply)
            {
                var intent = new Intent(this, typeof(MessageNew));
                intent.PutExtra("message_id", selectedItemId);
                StartActivity(intent);

            }

            return base.OnOptionsItemSelected(item);

        }

        #endregion

        private async void LoadViewDetail()
        {

            llProgressBar.Visibility = ViewStates.Visible;
            conversationListView.Visibility = ViewStates.Gone;

            PCL.Services.MessageService mService = new PCL.Services.MessageService();
            List<Model.MessageModels.Message> conversation = await mService.OpenItemAsync(selectedItemId);

            conversationListView.Adapter = new ConversationListScreenAdapter(this, conversation.ToArray());

            conversationListView.Visibility = ViewStates.Visible;
            llProgressBar.Visibility = ViewStates.Gone;

            if (conversationListView.Count == 0)
            {
                //no items to display
                Toast.MakeText(this, Resources.GetString(Resource.String.no_records), ToastLength.Long).Show();
            }

        }

        class ConversationListScreenAdapter : BaseAdapter<Model.MessageModels.Message>
        {
            public Model.MessageModels.Message[] items;
            Activity context;
            public ConversationListScreenAdapter(Activity context, Model.MessageModels.Message[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override Model.MessageModels.Message this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.CustomListItemMessage, null);
                view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Detail;
                view.FindViewById<TextView>(Resource.Id.tvDescription).Text = String.Format("{1} {2}",
                    items[position].Name, Java.Text.DateFormat.DateInstance.Format(items[position].Moment.ToJavaDate()),
                    Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(items[position].Moment.ToJavaDate()));

                View messageContainer = (view as LinearLayout).GetChildAt(0);
                LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)messageContainer.LayoutParameters;

                if (items[position].Received)
                {
                    layoutParams.SetMarginsDp(this.context, 60, 0, 10, 0);
                }
                else
                {
                    layoutParams.SetMarginsDp(this.context, 10, 0, 60, 0);
                }
                messageContainer.LayoutParameters = layoutParams;

                return view;
            }

        }

    }
}