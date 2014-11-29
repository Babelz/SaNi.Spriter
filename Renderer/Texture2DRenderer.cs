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

        public override void Draw(SpriterAnimationPlayer player, SpriterObject obj, SpriteBatch sb)
        {
            Texture2D texture = Loader[obj.@ref];
            Vector2 flip = new Vector2(obj.Scale.X < 0f ? -1.0f : 1.0f, obj.Scale.Y < 0f ? -1.0f : 1.0f);

            float newPivotX = texture.Width*obj.Pivot.X;
            float newPivotY = texture.Height*(1f - obj.Pivot.Y );
            
            Point x = obj.Position;
            Vector2 playerPos = new Vector2(player.X, player.Y);
            Vector2 objPosition = new Vector2(x.X, x.Y);


            // normal case scenario
            Vector2 diff = objPosition - playerPos;
            diff.Y = -diff.Y;
            
            
            Vector2 origin = new Vector2(newPivotX, newPivotY);

            SpriteEffects effects = SpriteEffects.None;

            bool flipx = flip.X < 0f;
            bool flipy = flip.Y < 0f;

            float rotation = -MathHelper.ToRadians(obj.Angle);

            if (flipx)
            {
                effects = SpriteEffects.FlipHorizontally;
                origin.X = texture.Width - origin.X;

                diff = playerPos - objPosition;
                
            }

            // TODO EI TOIMI
            if (flipy)
            {
                effects |= SpriteEffects.FlipVertically;
                origin.Y = texture.Height - origin.Y;
                diff.Y = Math.Abs(diff.Y);
            }

            Vector2 position = playerPos + Vector2.Multiply(diff, flip);

            sb.Draw(
                texture,
                position,
                null,
                null,
                origin,
                rotation,
                new Vector2(Math.Abs(obj.Scale.X), Math.Abs(obj.Scale.Y)),
                Color.White * obj.Alpha,
                effects,
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
