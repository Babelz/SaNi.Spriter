using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            
        }
    }
}
