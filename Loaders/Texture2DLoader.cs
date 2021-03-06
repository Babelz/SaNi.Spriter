﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SaNi.Spriter.Data;


namespace SaNi.Spriter.Loaders
{
    public class Texture2DLoader : SpriterLoader<Texture2D>
    {
        public Texture2DLoader(Game game, SpriterData data) : base(game, data)
        {
        }

        protected override Texture2D LoadResource(FileReference fileReference)
        {
            string file = Data.GetFile(fileReference).Name.Replace(".png", "");

            file = Root + file.Replace('/', Path.DirectorySeparatorChar);

            if (file.StartsWith("\\"))
            {
                file = file.Substring(1);
            }

              return Game.Content.Load<Texture2D>(file);
        }
    }
}
