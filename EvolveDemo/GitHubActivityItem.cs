
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
	public class GitHubActivityItem : FrameLayout
	{
		public long VersionNumber;

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
			inflater.Inflate (Android.Resource.Layout.ActivityListItem, this, true);
		}
	}
}

