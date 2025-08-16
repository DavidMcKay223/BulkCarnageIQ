using Android.Content;
using Android.Graphics;
using Android.Util;
using Google.Android.Material.ProgressIndicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnageAndroid
{
    public class CarnageLinearProgress : LinearProgressIndicator
    {
        public CarnageLinearProgress(Context context)
            : base(context, null, Resource.Attribute.linearProgressIndicatorStyle) => Init();

        public CarnageLinearProgress(Context context, IAttributeSet attrs)
            : base(context, attrs, Resource.Attribute.linearProgressIndicatorStyle) => Init();

        public CarnageLinearProgress(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            TrackCornerRadius = Context.DpToPx(CarnageStyle.CornerRadius);
            TrackThickness = Context.DpToPx(8);

            WithStyle(CarnageProgressStyle.Default);
        }

        public CarnageLinearProgress WithStyle(CarnageProgressStyle style)
        {
            Color indicatorColor;
            Color trackColor;

            switch (style)
            {
                case CarnageProgressStyle.Primary:
                    indicatorColor = CarnageStyle.PrimaryColor;
                    trackColor = CarnageStyle.PrimaryColor.SetAlpha(60);
                    break;
                case CarnageProgressStyle.Danger:
                    indicatorColor = CarnageStyle.DangerColor;
                    trackColor = CarnageStyle.DangerColor.SetAlpha(60);
                    break;
                case CarnageProgressStyle.Secondary:
                    indicatorColor = CarnageStyle.SecondaryColor;
                    trackColor = CarnageStyle.SecondaryColor.SetAlpha(60);
                    break;
                default:
                    indicatorColor = CarnageStyle.PrimaryColor;
                    trackColor = CarnageStyle.PrimaryColor.SetAlpha(60);
                    break;
            }

            SetIndicatorColor(indicatorColor);
            TrackColor = trackColor;

            return this;
        }

        public CarnageLinearProgress WithMax(int max)
        {
            Max = max;
            return this;
        }

        public CarnageLinearProgress WithProgress(int value)
        {
            Progress = value;
            return this;
        }

        public CarnageLinearProgress WithProgressRatio(float ratio)
        {
            Progress = (int)(ratio * Max);
            return this;
        }

        public CarnageLinearProgress WithThickness(int dp)
        {
            TrackThickness = Context.DpToPx(dp);
            return this;
        }

        public CarnageLinearProgress WithCorners(float dp)
        {
            TrackCornerRadius = Context.DpToPx(dp);
            return this;
        }

        public CarnageLinearProgress WithTrackColor(Color color)
        {
            TrackColor = color;
            return this;
        }
    }
}
