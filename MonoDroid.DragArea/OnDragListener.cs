using Android.Views;

namespace MonoDroid.DragArea
{
    public interface OnDragListener
    {
        void OnDrag(View view, DragEvent e);
    }
}