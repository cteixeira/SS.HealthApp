using System;
using Android.Content;
using Android.Views;
using Android.Widget;

using static Android.Views.GestureDetector;
using Android.Views.Animations;

namespace SS.HealthApp.Android.Util.ViewFlipper
{
    /// <summary>
    /// Used with banner slider to detect and handle the swipe gesture
    /// </summary>
    public class ViewFlipperSwipeGestureDetector : SimpleOnGestureListener
    {

        //static int SWIPE_MIN_DISTANCE = 120;
        //static int SWIPE_THRESHOLD_VELOCITY = 200;

        protected global::Android.Widget.ViewFlipper viewFlipper;
        protected Context context;


        public ViewFlipperSwipeGestureDetector(Context Context, global::Android.Widget.ViewFlipper ViewFlipper)
        {
            this.context = Context;
            this.viewFlipper = ViewFlipper;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            try
            {
                if (e1.GetX() - e2.GetX() > 0/* SWIPE_MIN_DISTANCE && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY*/)
                {
                    viewFlipper.InAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.left_in);
                    viewFlipper.OutAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.left_out);
                    //// controlling animation
                    //mViewFlipper.InAnimation.SetAnimationListener(mAnimationListener);
                    viewFlipper.StopFlipping();
                    viewFlipper.ShowNext();
                    return true;
                }
                else if (e2.GetX() - e1.GetX() > 0/*SWIPE_MIN_DISTANCE && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY*/)
                {
                    viewFlipper.InAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.right_in);
                    viewFlipper.OutAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.right_out);
                    // controlling animation
                    //mViewFlipper.InAnimation.SetAnimationListener(mAnimationListener);
                    viewFlipper.StopFlipping();
                    viewFlipper.ShowPrevious();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            //return false;
            return base.OnFling(e1, e2, velocityX, velocityY);
        }
    }

    public class ImageLinkViewFlipperGestureDetector : ViewFlipperSwipeGestureDetector
    {
        public ImageLinkViewFlipperGestureDetector(Context Context, global::Android.Widget.ViewFlipper ViewFlipper) : base(Context, ViewFlipper) { }

        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            View iv = ((ViewGroup)viewFlipper.CurrentView).GetChildAt(0);
            Console.WriteLine(iv.GetType().ToString());
            iv.CallOnClick();
            return true;
        }
    }

}