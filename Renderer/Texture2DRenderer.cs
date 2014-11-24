using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SaNi.Spriter.Data;

namespace SaNi.Spriter.Renderer
{
    /*
    public class Texture2DRenderer : SpriterRenderer<Texture2D>
    {
        private readonly SpriteBatch sb;

        public Texture2DRenderer(SpriterLoader<Texture2D> loader, SpriteBatch sb) : base(loader)
        {
            this.sb = sb;
        }

        public override void Draw(SpriterObject obj)
        {
            Texture2D texture = Loader[obj.Ref];
            float newPivotX = texture.Width*obj.Pivot.X;
            float newPivotY = texture.Height*obj.Pivot.Y;
            sb.Begin();
            sb.Draw(
                texture,
                new Vector2(100f, 100f),
                null,
                null,
                new Vector2(newPivotX, newPivotY),
                obj.Angle,
                obj.Scale,
                Color.White * obj.Alpha,
                SpriteEffects.None,
                0f
                );
            sb.End();
        }

        public override void Line(float pX, float pY, float targetX, float targetY)
        {
            
        }

        public override void SetColor(Color color)
        {
            
        }
    }*/
}
