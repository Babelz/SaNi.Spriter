using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SaNi.Spriter
{
    public class SpriterReader : ContentTypeReader<SpriterModel>
    {
        protected override SpriterModel Read(ContentReader input, SpriterModel existingInstance)
        {
            existingInstance = new SpriterModel();
            return existingInstance;
        }

    }
}
