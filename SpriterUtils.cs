using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaNi.Spriter.Data;

namespace SaNi.Spriter
{
    public static class SpriterUtils
    {
        public static ObjectType GetObjectInfoFor(string name)
        {
            if (name == "bone") return ObjectType.Bone;
            if (name == "skin") return ObjectType.Skin;
            if (name == "box") return ObjectType.Box;
            if (name == "point") return ObjectType.Point;
            return ObjectType.Sprite;
        }
    }
}
