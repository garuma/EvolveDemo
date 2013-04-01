
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
using Android.Graphics;

namespace EvolveDemo
{
	public class GitHubActivityItem : FrameLayout, ExpandableListView.IExpandableItem
	{
		public GitHubActivityItem (Context context) :
			base (context)
		{
			Initialize ();
		}

		public GitHubActivityItem (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public GitHubActivityItem (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			var inflater = Context.GetSystemService (Context.LayoutInflaterService).JavaCast<LayoutInflater> ();
			inflater.Inflate (Resource.Layout.GitHubActivityItemLayout, this, true);
		}

		public void DoLongClick ()
		{
			var presentationLayout = FindViewById (Resource.Id.PresentationLayout);
			var actionLayout = FindViewById (Resource.Id.ActionLayout);
			if (presentationLayout.Visibility == ViewStates.Gone) {
				presentationLayout.Visibility = ViewStates.Visible;
				var lp = new LinearLayout.LayoutParams (actionLayout.LayoutParameters) {
					Height = 1,
				};
				actionLayout.LayoutParameters = lp;
			} else {
				var lp = new LinearLayout.LayoutParams (actionLayout.LayoutParameters) {
					Height = ViewGroup.LayoutParams.WrapContent,
					Gravity = GravityFlags.Center
				};
				actionLayout.LayoutParameters = lp;
				presentationLayout.Visibility = ViewStates.Gone;
			}
		}

		public long VersionNumber;

		/*public Bitmap ReusedBitmap {
			get;
			set;
		}

		public int BitmapOwned;*/

		public bool Expandable {
			get;
			set;
		}

		public bool Expanded {
			get;
			set;
		}
	}
}

