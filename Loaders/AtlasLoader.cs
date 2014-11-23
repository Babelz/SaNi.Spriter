using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SaNi.Spriter.Data;
using File = System.IO.File;

namespace SaNi.Spriter.Loaders
{
    public class AtlasLoader : SpriterLoader<Texture2D>
    {
        private readonly string atlasFile;

        public AtlasLoader(Game game, SpriterData data, string atlasFile) : base(game, data)
        {
            this.atlasFile = atlasFile;
        }

        protected override void BeginLoading()
        {
            string[] contents = File.ReadAllLines(Path.Combine(Root, atlasFile + ".atlas"));

            string filename = contents[1];
            string format = contents[2];
            string filter = contents[3];
            string repeat = contents[4];
            // filename
            // format
            // filter
            // repeat
            // file?
            //  first child
        }

        protected override Texture2D LoadResource(FileReference fileReference)
        {
            return null;
        }
    }
}
