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
		readonly RoundCornersDrawable EmptyAvatarDrawable;
		Context context;
		Handler handler;
		List<GitHubEvent> events = new List<GitHubEvent> ();
		ConcurrentDictionary<string, Task<Bitmap>> imageCache = new ConcurrentDictionary<string, Task<Bitmap>> ();
		Typeface octoicons;

		public GitHubActivityAdapter (Context context)
		{
			this.context = context;
			this.handler = new Handler (context.MainLooper);
			this.octoicons = Typeface.CreateFromAsset (context.Assets, "Octicons.ttf");
			EmptyAvatarDrawable = new RoundCornersDrawable (BitmapFactory.DecodeResource (context.Resources,
			                                                                              Resource.Drawable.github_default));
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

		public bool Scrolled {
			get;
			set;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = EnsureView (convertView);
			var versionNumber = view.VersionNumber = Interlocked.Increment (ref view.VersionNumber);
			var item = events [position];

			var authorAvatar = view.FindViewById<ImageView> (Resource.Id.AuthorAvatar);
			var authorText = view.FindViewById<TextView> (Resource.Id.AuthorText);
			var repoText = view.FindViewById<TextView> (Resource.Id.RepositoryText);
			var secondaryText = view.FindViewById<TextView> (Resource.Id.ActivitySecondary);
			var githubIcon = view.FindViewById<TextView> (Resource.Id.GitHubIcon);

			githubIcon.Typeface = octoicons;
			githubIcon.Text = GitHubIconsUtils.GetIconCharForEventType (item.Type).ToString ();
			authorText.Text = item.Actor.Login;
			repoText.Text = item.Repo.Name.Substring ("xamarin/".Length);

			var eventText = MakeTextForEvent (item);
			secondaryText.Text = eventText ?? string.Empty;

			authorAvatar.SetImageDrawable (EmptyAvatarDrawable);
			FetchAvatar (view, authorAvatar, item, versionNumber);

			var presentationLayout = view.FindViewById (Resource.Id.PresentationLayout);
			presentationLayout.Visibility = ViewStates.Visible;
			var actionLayout = view.FindViewById (Resource.Id.ActionLayout);
			actionLayout.LayoutParameters.Height = 1;
			var actions = new Tuple<int, char>[] {
				Tuple.Create (Resource.Id.Action1, '\uf06f'),
				Tuple.Create (Resource.Id.Action2, '\uf223'),
				Tuple.Create (Resource.Id.Action3, '\uf22a'),
			};
			foreach (var a in actions) {
				var actView = view.FindViewById<TextView> (a.Item1);
				actView.Typeface = octoicons;
				actView.Text = a.Item2.ToString ();
			}

			var extraInformation = view.FindViewById<TextView> (Resource.Id.ExtraInformation);
			var expandableMark = view.FindViewById<TextView> (Resource.Id.ExpandableMark);
			extraInformation.LayoutParameters.Height = 0;
			expandableMark.Visibility = ViewStates.Gone;
			view.Expandable = false;
			switch (item.Type) {
			case GitHubEventType.CommitCommentEvent:
			case GitHubEventType.IssueCommentEvent:
			case GitHubEventType.PullRequestReviewCommentEvent:
			case GitHubEventType.PushEvent:
			case GitHubEventType.PullRequestEvent:
			case GitHubEventType.IssuesEvent:
				expandableMark.Visibility = ViewStates.Visible;
				MakeExtra (item, extraInformation);
				view.Expandable = true;
				break;
			}

			if (!item.Consumed) {
				item.Consumed = true;
				if (Scrolled) {
					var animation = AnimationUtils.MakeInChildBottomAnimation (context);
					view.StartAnimation (animation);
				}
			}

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
			var url = GravatarHelper.MakeUrl (evt.Actor.Gravatar_Id, context.ToPixels (48));
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
				avatarView.SetImageDrawable (new RoundCornersDrawable (bmp.Result));
			else
				bmp.ContinueWith (t => {
					if (view.VersionNumber == versionNumber && t.Result != null)
						handler.Post (() => {
							if (view.VersionNumber == versionNumber)
								avatarView.SetImageDrawableAnimated (new RoundCornersDrawable (t.Result));
						});
				});
		}

		string MakeTextForEvent (GitHubEvent evt)
		{
			switch (evt.Type) {
			case GitHubEventType.PushEvent:
				return string.Format ("Pushed {0} commits", evt.Payload["size"].ToString ());
			case GitHubEventType.ForkEvent:
				return string.Format ("Forked to {0}", evt.Payload.Object ("forkee")["full_name"]);
			case GitHubEventType.IssuesEvent:
				return string.Format ("{0} issue {1}", evt.Payload["action"].ToTitleCase (), evt.Payload.Object ("issue")["number"]);
			case GitHubEventType.WatchEvent:
				return string.Format ("{0} watching", evt.Payload["action"].ToTitleCase ());
			case GitHubEventType.PullRequestEvent:
				return string.Format ("{0} pull request {1}", evt.Payload["action"].ToTitleCase (), evt.Payload["number"]);
			case GitHubEventType.IssueCommentEvent:
				return string.Format ("Commented on issue {0}", evt.Payload.Object ("issue")["number"]);
			case GitHubEventType.CreateEvent:
				return string.Format ("Created {0} {1}", evt.Payload ["ref_type"], evt.Payload ["ref"] ?? evt.Repo.Name);
			case GitHubEventType.CommitCommentEvent:
				return string.Format ("Commented on commit {0}", evt.Payload.Object ("comment")["commit_id"].Substring (0, 5));
			default:
				return null;
			}
		}

		void MakeExtra (GitHubEvent evt, TextView extraInfo)
		{
			extraInfo.Gravity = GravityFlags.Center;
			extraInfo.Typeface = Typeface.Default;
			extraInfo.SetSingleLine (true);
			switch (evt.Type) {
			case GitHubEventType.CommitCommentEvent:
			case GitHubEventType.IssueCommentEvent:
			case GitHubEventType.PullRequestReviewCommentEvent:
				var text = evt.Payload.Object ("comment") ["body"].SingleLineify ();
				text = '“' + text + '”';
				extraInfo.Text = text;
				extraInfo.Typeface = Typeface.Create (Typeface.Default, TypefaceStyle.Italic);
				break;
			case GitHubEventType.PushEvent:
				var commits = evt.Payload.ArrayObjects ("commits").Select (c => c ["sha"].Substring (0, 5) + " " + c ["message"].SingleLineify ().Ellipsize (21));
				extraInfo.SetSingleLine (false);
				extraInfo.Gravity = GravityFlags.Left;
				extraInfo.Typeface = Typeface.Monospace;
				extraInfo.Text = string.Join (System.Environment.NewLine, commits.Select (line => "   › " + line));
				break;
			case GitHubEventType.PullRequestEvent:
				text = evt.Payload.Object ("pull_request") ["body"].SingleLineify ();
				extraInfo.Text = text;
				break;
			case GitHubEventType.IssuesEvent:
				text = evt.Payload.Object ("issue") ["body"].SingleLineify ();
				extraInfo.Text = text;
				break;
			}
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

	static class GithubStringExtensions
	{
		public static string SingleLineify (this string input)
		{
			var space = " ";
			input = input.Replace ("\r\n", space);
			input = input.Replace ("\r", space);
			input = input.Replace ("\n", space);
			return input;
		}

		public static string Ellipsize (this string input, int maxSize = 30)
		{
			return input == null || input.Length < maxSize ? input : input.Substring (0, maxSize - 1) + '…';
		}
	}
}

