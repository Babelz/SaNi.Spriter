using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SaNi.Spriter.Data;

namespace SaNi.Spriter
{
    abstract public class SpriterLoader<R>
    {
        protected readonly Dictionary<FileReference, R> Resources;

        protected readonly Game Game;
        protected SpriterData Data;
        protected string Root;


        protected SpriterLoader(Game game, SpriterData data)
        {
            Game = game;
            Data = data;
            Resources = new Dictionary<FileReference, R>(100);
        }

        protected virtual void BeginLoading() {}
        protected virtual void FinishLoading() {}

        public void Load(string root)
        {
            if (!root.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                root += Path.DirectorySeparatorChar;
            }
            Root = root;
            BeginLoading();
            foreach (var folder in Data.Folders)
            {
                foreach (var file in folder.Files)
                {
                    FileReference f = new FileReference(folder.ID, file.ID);
                    Resources[f] = LoadResource(f);
                }
            }
        }

        protected abstract R LoadResource(FileReference fileReference);

        public R this[FileReference r]
        {
            get { return Resources[r]; }
        }
    }
}
