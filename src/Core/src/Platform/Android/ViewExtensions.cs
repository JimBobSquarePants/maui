using Android.Graphics.Drawables;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Maui.Graphics;
using AView = Android.Views.View;

namespace Microsoft.Maui
{
	public static class ViewExtensions
	{
		const int DefaultAutomationTagId = -1;
		public static int AutomationTagId { get; set; } = DefaultAutomationTagId;

		public static void UpdateIsEnabled(this AView nativeView, IView view)
		{
			nativeView.Enabled = view.IsEnabled;
		}

		public static void UpdateVisibility(this AView nativeView, IView view)
		{
			nativeView.Visibility = view.Visibility.ToNativeVisibility();
		}

		public static void UpdateClip(this AView nativeView, IView view)
		{
			if (nativeView is WrapperView wrapper)
				wrapper.Clip = view.Clip;
		}

		public static ViewStates ToNativeVisibility(this Visibility visibility)
		{
			return visibility switch
			{
				Visibility.Hidden => ViewStates.Invisible,
				Visibility.Collapsed => ViewStates.Gone,
				_ => ViewStates.Visible,
			};
		}

		public static void UpdateBackground(this AView nativeView, IView view, Drawable? defaultBackgroundDrawable = null)
		{
			// Remove previous background gradient if any
			if (nativeView.Background is MauiDrawable mauiDrawable)
			{
				nativeView.Background = null;
				mauiDrawable.Dispose();
			}

			var paint = view.Background;

			if (paint.IsNullOrEmpty())
				nativeView.Background = defaultBackgroundDrawable;
			else
			{
				if (paint is SolidPaint solidPaint)
				{
					Color backgroundColor = solidPaint.Color;

					if (backgroundColor != null)
						nativeView.SetBackgroundColor(backgroundColor.ToNative());
				}
				else
					nativeView.Background = paint!.ToDrawable(nativeView.Context);
			}
		}
			
		public static void UpdateBorderBrush(this AView nativeView, IView view)
		{
			nativeView.GetMauiDrawable()?.SetBorderBrush(view.BorderBrush);
		}

		public static void UpdateBorderWidth(this AView nativeView, IView view)
		{
			nativeView.GetMauiDrawable()?.SetBorderWidth(view.BorderWidth);
		}

		public static void UpdateCornerRadius(this AView nativeView, IView view)
		{
			nativeView.GetMauiDrawable()?.SetCornerRadius(view.CornerRadius);
		}

		public static void UpdateOpacity(this AView nativeView, IView view)
		{
			nativeView.Alpha = (float)view.Opacity;
		}

		public static bool GetClipToOutline(this AView view)
		{
			return view.ClipToOutline;
		}

		public static void SetClipToOutline(this AView view, bool value)
		{
			view.ClipToOutline = value;
		}

		public static void UpdateAutomationId(this AView nativeView, IView view)
		{
			if (AutomationTagId == DefaultAutomationTagId)
			{
				AutomationTagId = Resource.Id.automation_tag_id;
			}

			nativeView.SetTag(AutomationTagId, view.AutomationId);
		}

		public static void UpdateSemantics(this AView nativeView, IView view)
		{
			var semantics = view.Semantics;

			if (semantics == null)
				return;

			ViewCompat.SetAccessibilityHeading(nativeView, semantics.IsHeading);
		}

		public static void InvalidateMeasure(this AView nativeView, IView view)
		{
			nativeView.RequestLayout();
		}

		public static void UpdateWidth(this AView nativeView, IView view)
		{
			// GetDesiredSize will take the specified Width into account during the layout
			if (!nativeView.IsInLayout)
			{
				nativeView.RequestLayout();
			}
		}

		public static void UpdateHeight(this AView nativeView, IView view)
		{
			// GetDesiredSize will take the specified Height into account during the layout
			if (!nativeView.IsInLayout)
			{
				nativeView.RequestLayout();
			}
		}

		public static void RemoveFromParent(this AView view)
		{
			if (view == null)
				return;
			if (view.Parent == null)
				return;
			((ViewGroup)view.Parent).RemoveView(view);
		}		
    
    internal static MauiDrawable? GetMauiDrawable(this AView view)
		{
			if (view.Background is MauiDrawable mauiDrawable)
				return mauiDrawable;

			return null;
		}
	}
}