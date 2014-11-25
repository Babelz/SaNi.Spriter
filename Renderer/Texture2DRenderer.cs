using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SaNi.Spriter.Data;
using Point = SaNi.Spriter.Data.Point;

namespace SaNi.Spriter.Renderer
{

    public class Texture2DRenderer : SpriterRenderer<Texture2D>
    {

        public Texture2DRenderer(SpriterLoader<Texture2D> loader) : base(loader)
        {
        }

        public override void Draw(SpriterObject obj, SpriteBatch sb)
        {
            Texture2D texture = Loader[obj.@ref];
            float newPivotX = texture.Width*obj.Pivot.X;
            float newPivotY = texture.Height*(1f - obj.Pivot.Y);
            Point x = obj.Position;
            Vector2 origin = new Vector2(newPivotX, newPivotY);

            sb.Draw(
                texture,
                new Vector2(400f) + new Vector2(x.X, -x.Y) - origin,
                null,
                null,
                origin,
                MathHelper.ToRadians(360f - obj.Angle),
                new Vector2(obj.Scale.X, obj.Scale.Y),
                Color.White*obj.Alpha,
                SpriteEffects.None,
                0f
                );
        }

        public override void Line(float pX, float pY, float targetX, float targetY)
        {

        }

        public override void SetColor(Color color)
        {

        }
    }
}
