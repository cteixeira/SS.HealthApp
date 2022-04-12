using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Support.V4.Content;
using SS.HealthApp.Android.Util;
using SS.HealthApp.Model.AccountModels;
using Plugin.Connectivity;
using SS.HealthApp.Model.DeclarationModels;
using Android.Content;

namespace SS.HealthApp.Android.Account
{
    public class AccountList : BaseType.BaseFragment
    {

        ListView accountListView = null;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.AccountList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetupToolbarTitle(Resources.GetString(Resource.String.menu_AccountStatment));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            accountListView = Activity.FindViewById<ListView>(Resource.Id.lvAccount);

            LoadAccountListAsync();

        }

        #endregion

        private async void LoadAccountListAsync()
        {
            SS.HealthApp.PCL.Services.AccountService aService = new PCL.Services.AccountService();
            List<AccountStatement> accountsStaments = await aService.GetItemsAsync();

            accountListView.Visibility = ViewStates.Visible;
            Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar).Visibility = ViewStates.Gone;

            if (accountsStaments != null && accountsStaments.Count > 0)
            {
                accountListView.Adapter = new AccountListScreenAdapter(this.Activity, accountsStaments.ToArray());
                accountListView.ItemClick += AccountListView_Click;
            }
            else
            {
                //no items to display
                Toast.MakeText(this.Context, Resources.GetString(Resource.String.no_records), ToastLength.Long).Show();
            }

        }

        private void AccountListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            AccountListScreenAdapter lvAdapter = lv.Adapter as AccountListScreenAdapter;
            AccountStatement selectedItem = lvAdapter[e.Position];

            OpenAccountStatmentAsync(selectedItem);

        }

        private async void OpenAccountStatmentAsync(AccountStatement AccountStatement)
        {

            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.AccountService aService = new PCL.Services.AccountService();
            string savePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDownloads).Path;
            string filepath = await aService.DownloadDocumentAsync(AccountStatement.ID, savePath);

            if (!string.IsNullOrEmpty(filepath))
            {
                global::Android.Net.Uri fileUri = null;
                using (Java.IO.File file = new Java.IO.File(filepath))
                {
                    file.SetReadable(true);
                    fileUri = global::Android.Net.Uri.FromFile(file);
                }
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(fileUri, "application/pdf");
                intent.SetFlags(ActivityFlags.NoHistory);

                try
                {
                    StartActivity(intent);
                }
                catch (Exception)
                {
                    Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrNoAppViewFile), ToastLength.Long).Show();
                }

            }
            else
            {
                //document not downloaded
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrOpenDocument), ToastLength.Long).Show();
            }
            
            progressDialog.Dismiss();

        }

    }

    public class AccountListScreenAdapter : BaseAdapter<AccountStatement>
    {
        public AccountStatement[] items;
        Activity context;
        public AccountListScreenAdapter(Activity context, AccountStatement[] items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override AccountStatement this[int position]
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
                view = context.LayoutInflater.Inflate(Resource.Layout.AccountListItem, null);
            view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Description;
            //view.FindViewById<TextView>(Resource.Id.tvDescription).Text = String.Format("{0} | {1}", items[position].Facility, Java.Text.DateFormat.DateInstance.Format(items[position].Date.ToJavaDate()));
            view.FindViewById<TextView>(Resource.Id.tvDescription).Text = Java.Text.DateFormat.DateInstance.Format(items[position].Date.ToJavaDate());
            TextView tvValue = view.FindViewById<TextView>(Resource.Id.tvValue);
            tvValue.Text = String.Format("{0} {1}", items[position].Value.ToString(), PCL.Settings.CurrencySymbol);

            //Payed/not payed not used
            //if (items[position].Payed)
            //{
            //tvValue.SetTextColor(context.Resources.GetColor(Resource.Color.colorPrimary));
            //}
            //else
            //{
            //    tvValue.SetTextColor(context.Resources.GetColor(Resource.Color.colorAccent));
            //}
            ImageView iv = view.FindViewById<ImageView>(Resource.Id.ivCustomListImage);
            return view;
        }

    }
}