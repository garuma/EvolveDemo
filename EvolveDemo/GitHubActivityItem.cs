
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

