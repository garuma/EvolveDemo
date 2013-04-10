using System;

using Android.Hardware;
using Android.Util;
using Android.Content;
using Android.Runtime;
using Android.Views;

namespace EvolveDemo
{
	public static class DensityExtensions
	{
		static readonly DisplayMetrics displayMetrics = new DisplayMetrics ();

		public static int ToPixels (this Context ctx, int dp)
		{
			var wm = ctx.GetSystemService (Context.WindowService).JavaCast<IWindowManager> ();
			wm.DefaultDisplay.GetMetrics (displayMetrics);

			var density = displayMetrics.Density;
			return (int)(dp * density + 0.5f);
		}
	}
}

