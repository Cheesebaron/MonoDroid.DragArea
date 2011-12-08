using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MonoDroid.DragAreaSample
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class ManyDots : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            DragArea.DragArea dragArea = FindViewById<DragArea.DragArea>(Resource.Id.drag_area);
            TextView reportView = FindViewById<TextView>(Resource.Id.report_view);
            ViewGroup dots = FindViewById<ViewGroup>(Resource.Id.dots);

            for (int i = 0; i < dots.ChildCount; i++)
            {
                DraggableDot d = dots.GetChildAt(i) as DraggableDot;
                d.SetDragArea(dragArea, reportView);
            }
        }
    }
}

