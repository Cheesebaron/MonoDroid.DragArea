using System;
using System.Collections.Generic;
using System.Linq;

using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Util;
using Android.Graphics;
using Android.Views;

namespace MonoDroid.DragArea
{
    public delegate void DragAreaListener(object sender, MotionEvent e);

    public class DragArea : FrameLayout
    {
        private Dictionary<OnDragListener, Droppable> mDroppables;

        private bool mTouching;
        private bool mDrag;

        private Bundle mDragBundle;

        private float mX;
        private float mY;

        private DragShadowBuilder mShadowBuilder;

        private void InitDragArea()
        {
            mDroppables = new Dictionary<OnDragListener, Droppable>();
            mTouching = false;
            mDrag = false;
            mX = 0;
            mY = 0;

            SetWillNotDraw(false);
        }

        public DragArea(Context context)
            : base(context)
        {
            InitDragArea();
        }

        public DragArea(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            InitDragArea();
        }

        public DragArea(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            InitDragArea();
        }

        public void StartDrag(Bundle dragBundle, DragShadowBuilder shadowBuilder)
        {
            DragStarted(dragBundle);

            if (mTouching)
            {
                mShadowBuilder = shadowBuilder;
                DragMoved();
            }
            else
            {
                DragAborted();
            }
        }

        public void AddDragListener(View view, OnDragListener listener)
        {
            mDroppables.Add(listener, new Droppable(listener, view));
        }

        public void RemoveDragListener(OnDragListener listener)
        {
            mDroppables.Remove(listener);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Move:
                    mX = ev.GetX();
                    mY = ev.GetY();
                    mTouching = true;
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    mTouching = false;
                    if (mDrag)
                        DragAborted();
                    break;
                default:
                    break;
            }

            return mDrag;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            bool handled = false;

            if (mDrag)
            {
                handled = true;

                switch (e.Action)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.Move:
                        mX = e.GetX();
                        mY = e.GetY();
                        mTouching = true;
                        break;
                    case MotionEventActions.Up:
                        mX = e.GetX();
                        mY = e.GetY();
                        DragDropped();
                        break;
                    case MotionEventActions.Cancel:
                        DragAborted();
                        break;
                    default:
                        handled = false;
                        break;
                }
            }

            return handled;
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);

            if (mDrag && (mShadowBuilder != null))
            {
                Point size = new Point();
                Point touchPoint = new Point();

                mShadowBuilder.OnProvideShadowMetrics(size, touchPoint);

                canvas.Save();
                canvas.Translate(mX - touchPoint.X, mY - touchPoint.Y);
                mShadowBuilder.OnDraw(canvas);
                canvas.Restore();
            }
        }

        private void DragStarted(Bundle dragBundle)
        {
            if (mDrag)
                DragAborted();

            mDragBundle = dragBundle;
            DragEvent dragStarted = new DragEvent(mDragBundle, DragEvent.ACTION_DRAG_STARTED, 0, 0);

            foreach (Droppable d in mDroppables.Values)
            {
                d.Listener.OnDrag(d.View, dragStarted);
            }
            mDrag = true;
        }

        private void DragAborted()
        {
            DragEvent dragEnded = new DragEvent(mDragBundle, DragEvent.ACTION_DRAG_ENDED, 0, 0);
            foreach (Droppable d in mDroppables.Values)
            {
                d.Listener.OnDrag(d.View, dragEnded);
            }
            mDrag = false;
            Invalidate();
        }

        private void DragMoved()
        {
            foreach (Droppable d in mDroppables.Values)
            {
                bool hit = IsHit(d, (int)mX, (int)mY);
                int ev = d.OnMoveEvent(hit);

                if (ev != 0)
                {
                    DragEvent dragEvent = new DragEvent(mDragBundle, ev, (int)mX, (int)mY);
                    d.Listener.OnDrag(d.View, dragEvent);
                }
            }
            Invalidate();
        }

        private void DragDropped()
        {
            foreach (Droppable d in mDroppables.Values)
            {
                bool hit = IsHit(d, (int)mX, (int)mY);
                int ev = d.OnUpEvent(hit);

                DragEvent dragEvent = new DragEvent(mDragBundle, ev, (int) mX, (int) mY);
                d.Listener.OnDrag(d.View, dragEvent);
            }
            Invalidate();
        }

        private bool IsHit(Droppable droppable, int x, int y)
        {
            Rect hitRect = new Rect(0, 0, droppable.View.Width, droppable.View.Height);
            OffsetDescendantRectToMyCoords(droppable.View, hitRect);

            return hitRect.Contains(x, y);
        }

        private class Droppable
        {
            public OnDragListener Listener;
            public View View;

            private bool mLastEventHit;

            public Droppable(OnDragListener listener, View view)
            {
                this.Listener = listener;
                this.View = view;

                mLastEventHit = false;
            }

            public int OnMoveEvent(bool eventHit)
            {
                int result;

                if (!mLastEventHit && eventHit)
                    result = DragEvent.ACTION_DRAG_ENTERED;
                else if (mLastEventHit && eventHit)
                    result = DragEvent.ACTION_DRAG_LOCATION;
                else if (mLastEventHit && !eventHit)
                    result = DragEvent.ACTION_DRAG_EXITED;
                else
                    result = 0;

                mLastEventHit = eventHit;

                return result;
            }

            public int OnUpEvent(bool eventHit)
            {
                mLastEventHit = false;

                if (eventHit)
                    return DragEvent.ACTION_DROP;
                else
                    return DragEvent.ACTION_DRAG_ENDED;
            }
        }
    }
}
