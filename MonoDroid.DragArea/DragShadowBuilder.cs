using Android.Graphics;
using Android.Views;

namespace MonoDroid.DragArea
{
    public interface DragShadowBuilder
    {
        View GetView();

        void OnProvideShadowMetrics(Point shadowSize, Point shadowTouchPoint);

        void OnDraw(Canvas c);
    }
}