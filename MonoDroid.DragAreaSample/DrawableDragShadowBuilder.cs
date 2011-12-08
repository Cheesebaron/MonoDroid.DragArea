using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;

namespace MonoDroid.DragAreaSample
{
    class DrawableDragShadowBuilder: MonoDroid.DragArea.DragShadowBuilder
    {
        private View mView;
        private Drawable mDrawable;
        private Point mTouchPoint;

        public DrawableDragShadowBuilder(View view, Drawable drawable, Point touchPoint)
        {
            mView = view;
            mDrawable = drawable;
            mTouchPoint = touchPoint;
        }

        public View GetView()
        {
            return mView;
        }

        public void OnProvideShadowMetrics(Point shadowSize, Point touchPoint)
        {
            shadowSize.X = mDrawable.IntrinsicWidth;
            shadowSize.Y = mDrawable.IntrinsicHeight;

            touchPoint.X = mTouchPoint.X;
            touchPoint.Y = mTouchPoint.Y;

            mDrawable.SetBounds(0, 0, shadowSize.X, shadowSize.Y);
        }

        public void OnDraw(Canvas c)
        {
            mDrawable.Draw(c);
        }
    }
}