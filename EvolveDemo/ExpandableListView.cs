
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace EvolveDemo
{
	public class ExpandableListView : ListView, ExpandHelper.Callback
	{
		public interface IExpandableItem
		{
			bool Expandable { get; }
			bool Expanded { get; set; }
		}

		ExpandHelper expandHelper;
		bool expandoRequested;
		bool wasExpanding;

		public ExpandableListView (Context context) :
			base (context)
		{
			Initialize ();
			InnerViewId = -1;
		}

		public ExpandableListView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
			InnerViewId = attrs.GetAttributeResourceValue (null, "innerViewId", -1);
		}

		public ExpandableListView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
			InnerViewId = attrs.GetAttributeResourceValue (null, "innerViewId", -1);
		}

		void Initialize ()
		{
			expandHelper = new ExpandHelper (Context, this, 0, Context.ToPixels (48));
			expandHelper.SetEventSource (this);
		}

		public int InnerViewId {
			get;
			set;
		}

		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			expandoRequested = expandHelper.OnInterceptTouchEvent (ev);
			return expandoRequested;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			expandHelper.OnTouchEvent (e);
			if (expandHelper.Progressing || e.PointerCount == 2) {
				wasExpanding = true;
				return true;
			}

			return base.OnTouchEvent (e);
		}

		public override bool PerformItemClick (View view, int position, long id)
		{
			if (wasExpanding) {
				wasExpanding = false;
				return false;
			}
			//expandHelper.OnClick (InnerViewId == -1 ? view : view.FindViewById (InnerViewId));
			var activityItem = view as GitHubActivityItem;
			activityItem.DoLongClick ();
			return true;
			//return base.PerformItemClick (view, position, id);
		}

		public View GetChildAtRawPosition (float x, float y)
		{
			var index = GetItemPositionFromRawYCoordinates ((int)y);
			if (index == -1)
				return null;
			var view = GetChildAt (index);
			return InnerViewId == -1 ? view : view.FindViewById (InnerViewId);
		}

		public View GetChildAtPosition (float x, float y)
		{
			return GetChildAtRawPosition (x, y);
		}

		public bool CanChildBeExpanded (View v)
		{
			var activityItem = GetItemFromChildView (v);
			return activityItem != null && activityItem.Expandable;
		}

		public bool SetUserExpandedChild (View v, bool userxpanded)
		{
			var activityItem = GetItemFromChildView (v);
			if (activityItem != null)
				activityItem.Expanded = userxpanded;
			return activityItem != null;
		}

		IExpandableItem GetItemFromChildView (View v)
		{
			if (v == null)
				return null;
			IExpandableItem item = v as IExpandableItem;
			if (item != null)
				return item;
			var parent = v.Parent;
			while (parent != null && (item = parent as IExpandableItem) == null)
				parent = parent.Parent;
			return item;
		}

		int GetItemPositionFromRawYCoordinates (int rawY)
		{
			int total = LastVisiblePosition - FirstVisiblePosition + 1;

			int[] coords = new int[2];
			for (int i = 0; i < total; i++) {
				var child = GetChildAt (i);
				child.GetLocationOnScreen (coords);
				int top = coords[1];
				int bottom = top + child.Height;
				if ((rawY >= top) && (rawY <= bottom))
					return i;
			}

			return -1;
		}
	}
}

