using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnageAndroid
{
    public class CarnageButtonIcon : FrameLayout
    {
        private readonly ImageView _iconView;
        private readonly TextView _textView;
        private readonly LinearLayout _layout;

        public CarnageButtonIcon(Context context) : base(context)
        {
            _layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Vertical
            };
            _layout.SetGravity(GravityFlags.Center);

            _iconView = new ImageView(context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(Context.DpToPx(48), Context.DpToPx(48))
            };

            _textView = new TextView(context)
            {
                Gravity = GravityFlags.Center,
                TextSize = 14
            };

            _layout.AddView(_iconView);
            _layout.AddView(_textView);

            AddView(_layout);

            var gd = new GradientDrawable();
            gd.SetShape(ShapeType.Rectangle);
            gd.SetCornerRadius(Context.DpToPx(8));
            gd.SetColor(Color.Transparent);
            Background = gd;

            SetPadding(Context.DpToPx(8), Context.DpToPx(8), Context.DpToPx(8), Context.DpToPx(8));
        }

        public CarnageButtonIcon WithIcon(CarnageIcon icon, Color color)
        {
            var drawableId = CarnageIconResolver.GetResource(icon);
            _iconView.SetImageResource(drawableId);
            _iconView.SetColorFilter(color, PorterDuff.Mode.SrcIn);
            return this;
        }

        public CarnageButtonIcon WithText(string text, Color color)
        {
            _textView.Text = text;
            _textView.SetTextColor(color);
            return this;
        }

        public CarnageButtonIcon WithBoxColor(Color color)
        {
            if (Background is GradientDrawable gd)
                gd.SetColor(color);
            return this;
        }

        public CarnageButtonIcon OnClick(Action action)
        {
            Click += (s, e) => action();
            return this;
        }

        public CarnageButtonIcon WithPosition(CarnageIconPosition position)
        {
            _layout.RemoveAllViews();
            switch (position)
            {
                case CarnageIconPosition.Top:
                    _layout.Orientation = Android.Widget.Orientation.Vertical;
                    _layout.AddView(_iconView);
                    _layout.AddView(_textView);
                    break;
                case CarnageIconPosition.Bottom:
                    _layout.Orientation = Android.Widget.Orientation.Vertical;
                    _layout.AddView(_textView);
                    _layout.AddView(_iconView);
                    break;
                case CarnageIconPosition.Left:
                    _layout.Orientation = Android.Widget.Orientation.Horizontal;
                    _layout.AddView(_iconView);
                    _layout.AddView(_textView);
                    break;
                case CarnageIconPosition.Right:
                    _layout.Orientation = Android.Widget.Orientation.Horizontal;
                    _layout.AddView(_textView);
                    _layout.AddView(_iconView);
                    break;
            }
            return this;
        }
    }
}
