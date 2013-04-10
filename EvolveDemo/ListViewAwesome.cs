
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

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

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.ListViewAwesomeLayout, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			ListView.Scroll += HandleScroll;
			var header = Activity.LayoutInflater.Inflate (Resource.Layout.ListHeaderLayout, ListView, false);
			ListView.AddHeaderView (header, null, false);
			ListAdapter = adapter = new GitHubActivityAdapter (Activity);
			FetchData (offset: currentOffset++);
		}

		void HandleScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			adapter.Scrolled = e.FirstVisibleItem > 0;
			if (loading || e.FirstVisibleItem + e.VisibleItemCount < e.TotalItemCount - 5)
				return;
			loading = true;
			FetchData (offset: currentOffset++);
		}

		async void FetchData (int offset = 0)
		{
			var client = new HttpClient ();
			var url = "https://api.github.com/orgs/xamarin/events";
			url += "?page=" + offset;
			var data = await client.GetStreamAsync (url).ConfigureAwait (false);
			var items = JsonSerializer.DeserializeFromStream<List<GitHubEvent>> (data);
			Activity.RunOnUiThread (() => { adapter.FeedData (items); loading = false; });
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.activity_menu, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			adapter.Clear ();
			currentOffset = 1;
			loading = true;
			FetchData (0);
			adapter.Scrolled = false;
			return true;
		}
	}
}

