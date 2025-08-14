using Android.Content;
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

        private void Init() => WithStyle(CarnageProgressStyle.Default);

        public CarnageCircularProgress WithStyle(CarnageProgressStyle style)
        {
            switch (style)
            {
                case CarnageProgressStyle.Primary:
                    SetIndicatorColor(CarnageStyle.PrimaryColor);
                    break;
                case CarnageProgressStyle.Danger:
                    SetIndicatorColor(CarnageStyle.DangerColor);
                    break;
                case CarnageProgressStyle.Secondary:
                    SetIndicatorColor(CarnageStyle.SecondaryColor);
                    break;
                default:
                    SetIndicatorColor(CarnageStyle.PrimaryColor);
                    break;
            }
            return this;
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
    }
}
