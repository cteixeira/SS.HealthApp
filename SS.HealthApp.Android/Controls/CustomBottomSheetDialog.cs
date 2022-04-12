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

using Android.Content.Res;
using Android.Support.V7.App;
using Android.Util;
using Android.Support.Design.Widget;

namespace SS.HealthApp.Android.Controls
{
    //RC: this class is a replica from android source code from https://android.googlesource.com/platform/frameworks/support.git/+/master/design/src/android/support/design/widget/BottomSheetDialog.java
    public class CustomBottomSheetDialog : AppCompatDialog
    {

        private BottomSheetBehavior mBehavior;
        bool mCancelable = true;
        private bool mCanceledOnTouchOutside = true;
        private bool mCanceledOnTouchOutsideSet;

        public CustomBottomSheetDialog(Context context) : this(context, 0) { }

        public CustomBottomSheetDialog(Context context, int theme) : base(context, getThemeResId(context, theme))
        {
            // We hide the title bar for any style configuration. Otherwise, there will be a gap
            // above the bottom sheet when it is expanded.
            SupportRequestWindowFeature((int)WindowFeatures.NoTitle);
        }

        protected CustomBottomSheetDialog(Context context, bool cancelable, IDialogInterfaceOnCancelListener cancelListener) : base(context, cancelable, cancelListener)
        {
            SupportRequestWindowFeature((int)WindowFeatures.NoTitle);
            mCancelable = cancelable;
        }

        public override void SetContentView(int layoutResId)
        {
            base.SetContentView(wrapInBottomSheet(layoutResId, null, null));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        public override void SetContentView(View view)
        {
            base.SetContentView(wrapInBottomSheet(0, view, null));
        }

        public override void SetContentView(View view, ViewGroup.LayoutParams lParams)
        {
            base.SetContentView(wrapInBottomSheet(0, view, lParams));
        }

        public override void SetCancelable(bool cancelable)
        {
            base.SetCancelable(cancelable);
            if (mCancelable != cancelable)
            {
                mCancelable = cancelable;
                if (mBehavior != null)
                {
                    mBehavior.Hideable = cancelable;
                }
            }
        }

        public override void SetCanceledOnTouchOutside(bool cancel)
        {
            base.SetCanceledOnTouchOutside(cancel);
            if (cancel && !mCancelable)
            {
                mCancelable = true;
            }
            mCanceledOnTouchOutside = cancel;
            mCanceledOnTouchOutsideSet = true;
        }

        private View wrapInBottomSheet(int layoutResId, View view, ViewGroup.LayoutParams lParams)
        {
            CoordinatorLayout coordinator = (CoordinatorLayout)View.Inflate(Context, Resource.Layout.design_bottom_sheet_dialog, null);
            if (layoutResId != 0 && view == null)
            {
                view = LayoutInflater.Inflate(layoutResId, coordinator, false);
            }
            FrameLayout bottomSheet = (FrameLayout)coordinator.FindViewById(Resource.Id.design_bottom_sheet);
            mBehavior = BottomSheetBehavior.From(bottomSheet);
            mBehavior.SetBottomSheetCallback(new CustomBottomSheetBehaviorCallback(this));
            mBehavior.Hideable = mCancelable;
            if (lParams == null)
            {
                bottomSheet.AddView(view);
            }
            else
            {
                bottomSheet.AddView(view, lParams);
            }
            coordinator.FindViewById(Resource.Id.touch_outside).Click += delegate {
                if (mCancelable && IsShowing && shouldWindowCloseOnTouchOutside())
                {
                    Cancel();
                }
            };

            return coordinator;
        }

        bool shouldWindowCloseOnTouchOutside()
        {
            if (!mCanceledOnTouchOutsideSet)
            {
                if ((int)Build.VERSION.SdkInt < 11)
                {
                    mCanceledOnTouchOutside = true;
                }
                else
                {
                    TypedArray a = Context.ObtainStyledAttributes(new int[] { global::Android.Resource.Attribute.WindowCloseOnTouchOutside });
                    mCanceledOnTouchOutside = a.GetBoolean(0, true);
                    a.Recycle();
                }
                mCanceledOnTouchOutsideSet = true;
            }
            return mCanceledOnTouchOutside;
        }

        private static int getThemeResId(Context context, int themeId)
        {
            if (themeId == 0)
            {
                // If the provided theme is 0, then retrieve the dialogTheme from our theme
                TypedValue outValue = new TypedValue();
                if (context.Theme.ResolveAttribute(
                        Resource.Attribute.bottomSheetDialogTheme, outValue, true))
                {
                    themeId = outValue.ResourceId;
                }
                else
                {
                    // bottomSheetDialogTheme is not provided; we default to our light theme
                    themeId = Resource.Style.Theme_Design_Light_BottomSheetDialog;
                }
            }
            return themeId;
        }

        private class CustomBottomSheetBehaviorCallback : BottomSheetBehavior.BottomSheetCallback
        {

            Dialog ContextDialog;

            public CustomBottomSheetBehaviorCallback(Dialog ContextDialog)
            {
                this.ContextDialog = ContextDialog;
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                if (newState == BottomSheetBehavior.StateHidden)
                {
                    ContextDialog.Cancel();
                }
            }

            public override void OnSlide(View bottomSheet, float slideOffset)
            {

            }

        }

    }
}