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
    public class CarnageCircularProgress : CircularProgressIndicator
    {
        public CarnageCircularProgress(Context context)
            : base(context, null, Resource.Attribute.circleRadius) => Init();

        public CarnageCircularProgress(Context context, IAttributeSet attrs)
            : base(context, attrs, Resource.Attribute.circleRadius) => Init();

        public CarnageCircularProgress(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr) => Init();

        private void Init()
        {
            TrackThickness = Context.DpToPx(8);

            SetIndicatorColor(CarnageStyle.VividBlue);
            TrackColor = CarnageStyle.SlateGray;
        }

        public CarnageCircularProgress WithMax(int max)
        {
            Max = max;
            return this;
        }

        public CarnageCircularProgress WithProgress(int value)
        {
            Progress = value;
            return this;
        }

        public CarnageCircularProgress WithProgressRatio(float ratio)
        {
            Progress = (int)(ratio * Max);
            return this;
        }

        public CarnageCircularProgress WithThickness(int dp)
        {
            TrackThickness = Context.DpToPx(dp);
            return this;
        }

        public CarnageCircularProgress WithTrackColor(Color color)
        {
            TrackColor = color;
            return this;
        }
    }
}
