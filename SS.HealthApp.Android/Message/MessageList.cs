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
    public class MessageList : BaseType.BaseFragment
    {

        LinearLayout llProgressBar = null;
        ListView messageListView = null;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.MessageList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //Set top bar title
            ((global::Android.Support.V7.App.AppCompatActivity)Activity).SupportActionBar.Title = Resources.GetString(Resource.String.menu_Messages);

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            llProgressBar = Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar);
            messageListView = Activity.FindViewById<ListView>(Resource.Id.lvMessages);
            messageListView.ItemClick += MessagesViewList_ItemClick;

        }

        public override void OnResume()
        {
            base.OnResume();

            //load list here, when an message is created and the user come back to this screen the appointment list will be refreshed
            LoadMessagesListAsync();

        }

        #endregion

        #region toolbar methods

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.TopToolbarMenuAddNew, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_new)
            {

                var intent = new Intent(this.Context, typeof(MessageNew));
                StartActivity(intent);

            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        private async void LoadMessagesListAsync()
        {
            messageListView.Visibility = ViewStates.Gone;
            llProgressBar.Visibility = ViewStates.Visible;

            SS.HealthApp.PCL.Services.MessageService mService = new PCL.Services.MessageService();
            List<Model.MessageModels.Message> messages = await mService.GetItemsAsync();

            messageListView.Adapter = new MessageListScreenAdapter(this.Activity, messages.ToArray());

            messageListView.Visibility = ViewStates.Visible;
            llProgressBar.Visibility = ViewStates.Gone;

            if (messages.Count == 0)
            {
                //no items to display
                Toast.MakeText(this.Context, Resources.GetString(Resource.String.no_records), ToastLength.Long).Show();
            }

        }

        private void MessagesViewList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            MessageListScreenAdapter lvAdapter = lv.Adapter as MessageListScreenAdapter;
            Model.MessageModels.Message selectedItem = lvAdapter[e.Position];

            var intent = new Intent(this.Context, typeof(MessageDetail));
            intent.PutExtra("selected_item_id", selectedItem.ID);
            intent.PutExtra("selected_item_title", selectedItem.Subject);
            intent.PutExtra("selected_item_description", MessageListScreenAdapter.FormateMessageDescription(selectedItem));
            StartActivity(intent);
        }

        class MessageListScreenAdapter : BaseAdapter<Model.MessageModels.Message>
        {
            public Model.MessageModels.Message[] items;
            Activity context;
            public MessageListScreenAdapter(Activity context, Model.MessageModels.Message[] items) : base()
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.CustomListItemTwoLinesNavigateIcon, null);
                view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Subject;
                view.FindViewById<TextView>(Resource.Id.tvDescription).Text = FormateMessageDescription(items[position]);
                return view;
            }

            public static string FormateMessageDescription(Model.MessageModels.Message Message)
            {
                return String.Format("{0} | {1} {2}",
                    Message.Name, Java.Text.DateFormat.DateInstance.Format(Message.Moment.ToJavaDate()),
                    Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(Message.Moment.ToJavaDate()));
            }

        }

    }
}