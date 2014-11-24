using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SaNi.Spriter.Data;

namespace SaNi.Spriter.Renderer
{
    public abstract class SpriterRenderer<R>
    {
        protected SpriterLoader<R> Loader { get; set; }

        protected SpriterRenderer(SpriterLoader<R> loader)
        {
            Loader = loader;
        }

        public void DrawBones(SpriterAnimationPlayer player)
        {
            SetColor(Color.Red);
            BoneIterator it = player.BoneIterator();
            while (it.MoveNext())
            {
                Bone bone = it.Current;
                TimelineKey key = player.GetKeyFor(bone);
                if (!key.Active) continue;
                ObjectInfo info = player.GetObjectInfoFor(bone);
                Vector2 size = info.Size;
                DrawBone(bone, size);
            }
        }

        public void DrawBone(Bone bone, Vector2 size)
        {
            float halfHeight = size.Y/2f;
            float xx = bone.Position.X + (float) Math.Cos(MathHelper.ToRadians(bone.Angle))*size.Y;
            float yy = bone.Position.Y + (float) Math.Sin(MathHelper.ToRadians(bone.Angle))*size.Y;
            float x2 = (float) Math.Cos(MathHelper.ToRadians(bone.Angle + 90))*halfHeight*bone.Scale.Y;
            float y2 = (float)Math.Sin(MathHelper.ToRadians(bone.Angle + 90)) * halfHeight * bone.Scale.Y;

            float targetX = bone.Position.X + (float) Math.Cos(MathHelper.ToRadians(bone.Angle))*size.X*bone.Scale.X;
            float targetY = bone.Position.Y + (float)Math.Sin(MathHelper.ToRadians(bone.Angle)) * size.X * bone.Scale.X;

            float upperPointX = xx + x2;
            float upperPointY = yy + y2;

            Line(bone.Position.X, bone.Position.Y, upperPointX, upperPointY);
            Line(upperPointX, upperPointY, targetX, targetY);

            float lowerPointX = xx - x2;
            float lowerPointY = yy - y2;
            Line(bone.Position.X, bone.Position.Y, lowerPointX, lowerPointY);
            Line(lowerPointX, lowerPointY, targetX, targetY);
            Line(bone.Position.X, bone.Position.Y, targetX, targetY);
        }

        public void DrawBoxes(SpriterAnimationPlayer player)
        {
            SetColor(Color.Red);
            DrawBoneBoxes(player);
            DrawObjectBoxes(player);
            DrawPoints(player);
        }

        private void DrawPoints(SpriterAnimationPlayer player)
        {
            throw new NotImplementedException();
        }

        private void DrawObjectBoxes(SpriterAnimationPlayer player)
        {
            DrawObjectBoxes(player, player.ObjectIterator());
        }

        private void DrawObjectBoxes(SpriterAnimationPlayer player, ObjectIterator objectIterator)
        {
            while (objectIterator.MoveNext())
            {
                DrawBox(player.GetBox(objectIterator.Current));
            }
        }

        private void DrawBox(object getBox)
        {
            throw new NotImplementedException();
        }

        private void DrawBoneBoxes(SpriterAnimationPlayer player)
        {
            DrawBoneBoxes(player, player.BoneIterator());
        }

        public void DrawBoneBoxes(SpriterAnimationPlayer player, BoneIterator boneIterator)
        {
            while (boneIterator.MoveNext())
            {
                Bone bone = boneIterator.Current;
                DrawBox(player.GetBox(bone));
            }
        }

        public void Draw(SpriterAnimationPlayer player, SpriteBatch sb)
        {
            Draw(player, player.CharacterMaps, sb);
        }

        private void Draw(SpriterAnimationPlayer player, CharacterMap[] characterMaps, SpriteBatch sb)
        {
            Draw(player.ObjectIterator(), characterMaps, sb);
        }

        private void Draw(ObjectIterator objectIterator, CharacterMap[] characterMaps, SpriteBatch sb)
        {
            while (objectIterator.MoveNext())
            {
                SpriterObject obj = objectIterator.Current;
                if (obj.Ref.HasFile)
                {
                    if (characterMaps != null)
                    {
                        foreach (var characterMap in characterMaps)
                        {
                            if (characterMap != null)
                            {
                                obj.Ref.Set(characterMap[obj.Ref]);
                            }
                        }
                    }
                    Draw(obj, sb);
                }
            }
        }

        public abstract void Draw(SpriterObject obj, SpriteBatch sb);
        public abstract void Line(float pX, float pY, float targetX, float targetY);
        public abstract void SetColor(Color color);
    }
}
