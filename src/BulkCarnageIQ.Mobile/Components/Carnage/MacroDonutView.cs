using Android.Content;
using Android.Graphics;
using Android.Hardware.Lights;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.ListFormatter;

namespace BulkCarnageIQ.Mobile.Components.Carnage
{
    public class MacroDonutView : View
    {
        private float protein;
        private float carbs;
        private float fats;
        private float fiber;
        private Paint paint;

        // Constructor for programmatic creation
        public MacroDonutView(Context context, float protein, float carbs, float fats, float fiber) : base(context)
        {
            Init(protein, carbs, fats, fiber);
        }

        // Constructor for XML inflation
        public MacroDonutView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(0, 0, 0, 0); // Start with 0s until set
        }

        private void Init(float protein, float carbs, float fats, float fiber)
        {
            this.protein = protein;
            this.carbs = carbs;
            this.fats = fats;
            this.fiber = fiber;

            paint = new Paint { AntiAlias = true, StrokeWidth = 40, StrokeCap = Paint.Cap.Round, StrokeJoin = Paint.Join.Round };
            paint.SetStyle(Paint.Style.Stroke);
        }

        public void SetMacros(float protein, float carbs, float fats, float fiber)
        {
            this.protein = protein;
            this.carbs = carbs;
            this.fats = fats;
            this.fiber = fiber;
            Invalidate(); // Redraw
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            float total = protein + carbs + fats + fiber;
            if (total <= 0) total = 1;

            float startAngle = -90;
            RectF bounds = new RectF(40, 40, Width - 40, Height - 40);

            // Protein (Orange)
            paint.Color = Color.ParseColor("#FF9800");
            float sweep = (protein / total) * 360f;
            canvas.DrawArc(bounds, startAngle, sweep, false, paint);
            startAngle += sweep;

            // Carbs (Green)
            paint.Color = Color.ParseColor("#4CAF50");
            sweep = (carbs / total) * 360f;
            canvas.DrawArc(bounds, startAngle, sweep, false, paint);
            startAngle += sweep;

            // Fats (Blue)
            paint.Color = Color.ParseColor("#2196F3");
            sweep = (fats / total) * 360f;
            canvas.DrawArc(bounds, startAngle, sweep, false, paint);
            startAngle += sweep;

            // Fiber (Purple)
            paint.Color = Color.ParseColor("#9C27B0");
            sweep = (fiber / total) * 360f;
            canvas.DrawArc(bounds, startAngle, sweep, false, paint);
        }
    }
}
