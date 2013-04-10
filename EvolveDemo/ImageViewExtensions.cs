using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Animation;

using Java.Lang;

namespace EvolveDemo
{
	public static class ImageViewExtensions
	{
		const int LongAnimTime = Android.Resource.Integer.ConfigLongAnimTime;
		const int MediumAnimTime = Android.Resource.Integer.ConfigMediumAnimTime;

		public static void SetImageDrawableAnimated (this ImageView view, Drawable drawable)
		{
			var longAnim = view.Resources.GetInteger (LongAnimTime);
			var medAnim = view.Resources.GetInteger (MediumAnimTime);

			view.Animate ().Alpha (0).SetDuration (medAnim).WithEndAction (new Runnable (() => {
				view.SetImageDrawable (drawable);
				view.Animate ().Alpha (1).SetDuration (longAnim);
			}));
		}
	}
}

