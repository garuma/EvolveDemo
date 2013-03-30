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
		public static void SetImageDrawableAnimated (this ImageView view, Drawable drawable)
		{
			view.Animate ().Alpha (0).SetDuration (250).WithEndAction (new Runnable (() => {
				view.SetImageDrawable (drawable);
				view.Animate ().Alpha (1).SetDuration (500);
			}));
		}
	}
}

