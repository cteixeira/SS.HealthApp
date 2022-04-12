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

using SS.HealthApp.Android.Util;
using SS.HealthApp.Model.DeclarationModels;
using Plugin.Connectivity;

namespace SS.HealthApp.Android.Declaration
{
    public class DeclarationList : BaseType.BaseFragment
    {

        ListView declarationListView = null;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.DeclarationList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetupToolbarTitle(Resources.GetString(Resource.String.menu_Declarations));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            declarationListView = Activity.FindViewById<ListView>(Resource.Id.lvDeclaration);

            LoadDeclarationListAsync();

        }

        #endregion

        private async void LoadDeclarationListAsync()
        {
            SS.HealthApp.PCL.Services.DeclarationService dService = new PCL.Services.DeclarationService();
            List<PresenceDeclaration> declarations = await dService.GetItemsAsync();

            declarationListView.Visibility = ViewStates.Visible;
            Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar).Visibility = ViewStates.Gone;

            if (declarations != null && declarations.Count > 0)
            {
                declarationListView.Adapter = new DeclatationListScreenAdapter(this.Activity, declarations.ToArray());
                declarationListView.ItemClick += DeclarationListView_Click;
            }
            else
            {
                //no items to display
                Toast.MakeText(this.Context, Resources.GetString(Resource.String.no_records), ToastLength.Long).Show();
            }

        }

        private void DeclarationListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            DeclatationListScreenAdapter lvAdapter = lv.Adapter as DeclatationListScreenAdapter;
            PresenceDeclaration selectedItem = lvAdapter[e.Position];

            OpenDeclarationAsync(selectedItem);

        }

        private async void OpenDeclarationAsync(PresenceDeclaration Declaration)
        {

            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.DeclarationService dService = new PCL.Services.DeclarationService();
            string savePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDownloads).Path;
            string filepath = await dService.DownloadPresenceDeclarationAsync(Declaration.ID, savePath);

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
                //declaration not downloaded
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrOpenDeclaration), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();

        }

    }

    public class DeclatationListScreenAdapter : BaseAdapter<PresenceDeclaration>
    {
        public PresenceDeclaration[] items;
        Activity context;
        public DeclatationListScreenAdapter(Activity context, PresenceDeclaration[] items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override PresenceDeclaration this[int position]
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
            view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Facility;
            view.FindViewById<TextView>(Resource.Id.tvDescription).Text = String.Format("{0} | {1}", items[position].Appointment, Java.Text.DateFormat.DateInstance.Format(items[position].Moment.ToJavaDate()));
            ImageView iv = view.FindViewById<ImageView>(Resource.Id.ivCustomListImage);
            return view;
        }

    }

}