using Android.Content;
using Android.Graphics;
using Android.Hardware.Lights;
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
        private readonly float protein;
        private readonly float carbs;
        private readonly float fats;
        private readonly float fiber;
        private readonly Paint paint;

        public MacroDonutView(Context context, float protein, float carbs, float fats, float fiber) : base(context)
        {
            this.protein = protein;
            this.carbs = carbs;
            this.fats = fats;
            this.fiber = fiber;

            paint = new Paint { AntiAlias = true, StrokeWidth = 40, StrokeCap = Paint.Cap.Round, StrokeJoin = Paint.Join.Round };
            paint.SetStyle(Paint.Style.Stroke);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            float total = protein + carbs + fats + fiber;
            if (total <= 0) total = 1; // Avoid division by zero

            float startAngle = -90; // Start at top

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
