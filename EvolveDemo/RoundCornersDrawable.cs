using System;

using Android.Graphics;
using Android.Graphics.Drawables;

namespace EvolveDemo
{
	public class RoundCornersDrawable : Drawable
	{
		float mCornerRadius;
		RectF mRect = new RectF ();
		BitmapShader bitmapShader;
		Paint paint;
		int margin;

		public RoundCornersDrawable (Bitmap bitmap, float cornerRadius = 5, int margin = 3)
		{
			mCornerRadius = cornerRadius;

			bitmapShader = new BitmapShader (bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

			paint = new Paint () { AntiAlias = true };
			paint.SetShader (bitmapShader);

			this.margin = margin;
		}

		protected override void OnBoundsChange (Rect bounds)
		{
			base.OnBoundsChange (bounds);
			mRect.Set (margin, margin, bounds.Width () - margin, bounds.Height () - margin);
		}

		public override void Draw (Canvas canvas)
		{
			canvas.DrawRoundRect (mRect, mCornerRadius, mCornerRadius, paint);
		}

		public override int Opacity {
			get {
				return (int)Format.Translucent;
			}
		}

		public override void SetAlpha (int alpha)
		{
			paint.Alpha = alpha;
		}

		public override void SetColorFilter (ColorFilter cf)
		{
			paint.SetColorFilter (cf);
		}
	}
}

