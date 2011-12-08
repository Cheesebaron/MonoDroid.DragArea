using System;
using System.Collections.Generic;
using System.Linq;

using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android;
using Android.Content;
using Android.Util;
using Android.OS;

namespace MonoDroid.DragAreaSample
{
    class DraggableDot: TextView
    {
        private Drawable mWhiteDot;
        private Drawable mRedDot;
        private Drawable mGreenDot;
        private Drawable mTranslucentDot;

        private void InitDraggableDot()
        {
            mWhiteDot = Resources.GetDrawable(Resource.Drawable.white_dot);
            mRedDot = Resources.GetDrawable(Resource.Drawable.red_dot);
            mGreenDot = Resources.GetDrawable(Resource.Drawable.green_dot);
            mTranslucentDot = Resources.GetDrawable(Resource.Drawable.translucent_dot);
        }

        public DraggableDot(Context context)
            : base(context)
        {
            InitDraggableDot();
        }

        public DraggableDot(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            InitDraggableDot();
        }

        public DraggableDot(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            InitDraggableDot();
        }

        public void SetDragArea(MonoDroid.DragArea.DragArea dragArea, TextView reportView)
        {
            this.Touch += (sender, e) => {
                if (e.E.Action == MotionEventActions.Down)
                {
                    Bundle data = new Bundle();
                    data.PutCharSequence("number", Text);
                    dragArea.StartDrag(data, new DrawableDragShadowBuilder(
                        this,
                        mTranslucentDot,
                        new Point(
                            (int)e.E.GetX() - PaddingLeft,
                            (int)e.E.GetY() - PaddingTop
                            )
                        )
                    );         
                }
            };

            dragArea.AddDragListener(this, new MyDragListener(this, reportView));
        }

        private class MyDragListener : DragArea.OnDragListener
        {
            DraggableDot mDraggableDot;
            TextView mReportView;

            public MyDragListener(DraggableDot draggableDot, TextView reportView)
            {
                mDraggableDot = draggableDot;
                mReportView = reportView;
            }

            public void OnDrag(View view, DragArea.DragEvent e)
            {
                switch (e.Action)
                {
                    case DragArea.DragEvent.ACTION_DRAG_STARTED:
                        mDraggableDot.SetBackgroundDrawable(mDraggableDot.mGreenDot);
                        break;
                    case DragArea.DragEvent.ACTION_DRAG_ENTERED:
                        mDraggableDot.SetBackgroundDrawable(mDraggableDot.mWhiteDot);
                        break;
                    case DragArea.DragEvent.ACTION_DRAG_EXITED:
                        mDraggableDot.SetBackgroundDrawable(mDraggableDot.mGreenDot);
                        break;
                    case DragArea.DragEvent.ACTION_DROP:
                        Bundle data = e.Data;
                        string dropText = data.GetCharSequence("number");

                        int a = int.Parse(dropText);
                        int b = int.Parse(mDraggableDot.Text);
                        mDraggableDot.SetBackgroundDrawable(mDraggableDot.mRedDot);
                        break;
                    case DragArea.DragEvent.ACTION_DRAG_ENDED:
                        mDraggableDot.SetBackgroundDrawable(mDraggableDot.mRedDot);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}