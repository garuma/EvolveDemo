using System;
using System.Reflection;
using System.Linq.Expressions;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace EvolveDemo
{
	[Activity (Label = "EvolveDemo", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light.DarkActionBar",
	           ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation)]
	public class Activity1 : Activity
	{
		ListViewAwesome listViewFragment = null;
		CollectionViewAwesome collectionViewFragment = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			DensityExtensions.Initialize (this);

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			AddTabWithFragment<ListViewAwesome> ("ListView", () => listViewFragment, hasOptionsMenu: true);
			AddTabWithFragment<CollectionViewAwesome> ("CollectionView", () => collectionViewFragment);
		}

		void AddTabWithFragment<TFragment> (string label, Expression<Func<TFragment>> field, bool hasOptionsMenu = false) where TFragment : Fragment
		{
			var body = ((MemberExpression)field.Body);
			var backingField = body.Member as FieldInfo;

			var tab = ActionBar.NewTab ().SetText (label);
			tab.TabSelected += (_, e) => {
				var value = (TFragment)backingField.GetValue (this);
				if (value == null) {
					var frag = Fragment.Instantiate (this, Java.Lang.Class.FromType (typeof (TFragment)).Name);
					e.FragmentTransaction.Add (Android.Resource.Id.Content, frag, label);
					if (hasOptionsMenu)
						frag.SetHasOptionsMenu (true);
					backingField.SetValue (this, frag);
				} else {
					e.FragmentTransaction.Attach (value);
				}
			};
			tab.TabUnselected += (_, e) => {
				var value = (TFragment)backingField.GetValue (this);
				if (value != null)
					e.FragmentTransaction.Detach (value);
			};
			ActionBar.AddTab (tab);
		}
	}
}


