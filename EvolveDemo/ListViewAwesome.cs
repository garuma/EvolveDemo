
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using ServiceStack.Text;

namespace EvolveDemo
{
	public class ListViewAwesome : ListFragment
	{
		GitHubActivityAdapter adapter;
		bool loading;
		int currentOffset = 1;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.ListViewAwesomeLayout, container, false);
		}

		public override void OnAttach (Activity activity)
		{
			base.OnAttach (activity);
			ListAdapter = adapter = new GitHubActivityAdapter (Activity);
			FetchData (currentOffset++);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			ListView.Scroll += HandleScroll;
		}

		void HandleScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			adapter.Scrolled = e.FirstVisibleItem > 0;
			if (loading || e.FirstVisibleItem + e.VisibleItemCount < e.TotalItemCount - 5)
				return;
			loading = true;
			FetchData (currentOffset++);
		}

		void FetchData (int offset)
		{
			var client = new WebClient ();
			var url = "https://api.github.com/orgs/xamarin/events";
			url += "?page=" + offset;
			client = client.Setup (ref url);
			Task.Factory.StartNew (() => client.DownloadString (url)).ContinueWith (t => {
				var data = t.Result;
				var items = JsonSerializer.DeserializeFromString<List<GitHubEvent>> (data);
				Activity.RunOnUiThread (() => {
					adapter.FeedData (items);
					Activity.RunOnUiThread (() => ListView.Animate ().Alpha (1).SetDuration (1000));
					loading = false;
				});
			});
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.activity_menu, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			loading = true;
			adapter.Clear ();
			currentOffset = 1;
			FetchData (currentOffset++);
			adapter.Scrolled = false;
			return true;
		}
	}
}

