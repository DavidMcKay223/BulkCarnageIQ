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
    public class CarnageLinearProgress : LinearProgressIndicator
    {
        public CarnageLinearProgress(Context context)
            : base(context, null, Resource.Attribute.linearProgressIndicatorStyle) => Init();

        public CarnageLinearProgress(Context context, IAttributeSet attrs)
            : base(context, attrs, Resource.Attribute.linearProgressIndicatorStyle) => Init();

        public CarnageLinearProgress(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr) => Init();

        private void Init() => WithStyle(CarnageProgressStyle.Default);

        public CarnageLinearProgress WithStyle(CarnageProgressStyle style)
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
    }
}
