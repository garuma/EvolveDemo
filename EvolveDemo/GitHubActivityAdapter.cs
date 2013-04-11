using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;

using ServiceStack.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Animation;
using Android.Views.Animations;

namespace EvolveDemo
{
	public class GitHubActivityAdapter : BaseAdapter
	{
		Context context;
		Handler handler;
		List<GitHubEvent> events = new List<GitHubEvent> ();
		ConcurrentDictionary<string, Task<Bitmap>> imageCache = new ConcurrentDictionary<string, Task<Bitmap>> ();

		readonly Bitmap EmptyAvatarDrawable;

		public GitHubActivityAdapter (Context context)
		{
			this.context = context;
			this.handler = new Handler (context.MainLooper);
			EmptyAvatarDrawable = BitmapFactory.DecodeResource (context.Resources, Resource.Drawable.github_default);
		}

		public void FeedData (IEnumerable<GitHubEvent> newEvents)
		{
			events.AddRange (newEvents);
			NotifyDataSetChanged ();
		}

		public void Clear ()
		{
			events.Clear ();
			NotifyDataSetChanged ();
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = EnsureView (convertView);
			var versionNumber = view.VersionNumber = Interlocked.Increment (ref view.VersionNumber);
			var item = events [position];

			var authorAvatar = view.FindViewById<ImageView> (Android.Resource.Id.Icon);
			var text = view.FindViewById<TextView> (Android.Resource.Id.Text1);

			text.Text = item.Actor.Login + " on " + item.Repo.Name.Substring ("xamarin/".Length);

			authorAvatar.SetImageBitmap (EmptyAvatarDrawable);
			FetchAvatar (view, authorAvatar, item, versionNumber);

			return view;
		}

		GitHubActivityItem EnsureView (View convertView)
		{
			var item = convertView as GitHubActivityItem;
			if (item == null)
				item = new GitHubActivityItem (context);

			return item;
		}

		void FetchAvatar (GitHubActivityItem view, ImageView avatarView, GitHubEvent evt, long versionNumber)
		{
			var url = GravatarHelper.MakeUrl (evt.Actor.Gravatar_Id, context.ToPixels (24));
			var bmp = imageCache.GetOrAdd (url, u => SerialScheduler.Factory.StartNew (() => {
				var wc = new WebClient ().Setup (ref u);
				try {
					var data = wc.DownloadData (u);
					return BitmapFactory.DecodeByteArray (data, 0, data.Length);
				} catch {
					return null;
				}
			}));

			if (bmp.IsCompleted && bmp.Result != null)
				avatarView.SetImageBitmap (bmp.Result);
			else
				bmp.ContinueWith (t => {
					if (view.VersionNumber == versionNumber && t.Result != null)
						handler.Post (() => {
							if (view.VersionNumber == versionNumber)
								avatarView.SetImageBitmap (t.Result);
						});
				});
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return new Java.Lang.String (events [position].Id);
		}

		public override long GetItemId (int position)
		{
			return events [position].Id.GetHashCode ();
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}

		public override int ViewTypeCount {
			get {
				return 1;
			}
		}

		public override int Count {
			get {
				return events.Count;
			}
		}
	}
}

