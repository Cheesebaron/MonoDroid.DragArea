using Android.OS;

namespace MonoDroid.DragArea
{
    public class DragEvent
    {
        public const int ACTION_DRAG_STARTED = 1;
        public const int ACTION_DRAG_LOCATION = 2;
        public const int ACTION_DROP = 3;
        public const int ACTION_DRAG_ENDED = 4;
        public const int ACTION_DRAG_ENTERED = 5;
        public const int ACTION_DRAG_EXITED = 6;

        private int mX;
        private int mY;
        private int mAction;

        private Bundle mData;

        public DragEvent(Bundle data, int action, int x, int y)
        {
            mData = data;
            mAction = action;
            mX = x;
            mY = y;
        }

        public Bundle Data
        {
            get { return mData; }
        }

        public int Action
        {
            get { return mAction; }
        }

        public int X
        {
            get { return mX; }
        }

        public int Y
        {
            get { return mY; }
        }
    }
}