using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaNi.Spriter.Data
{
    public class File
    {
        public readonly int ID;
        public readonly string Name;
        public readonly Vector2 Size;
        public readonly Vector2 Pivot;

        public File(int id, string name, Vector2 size, Vector2 pivot)
        {
            ID = id;
            Name = name;
            Size = size;
            Pivot = pivot;
        }

        /// <summary>
        /// Returns whether this file is a sprite, i.e. an image which is going to be animated, or not.
        /// </summary>
        public bool IsSprite
        {
            get
            {
                return Size != Vector2.Zero;
            }
        }
    }
}
