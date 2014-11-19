using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SaNi.Spriter.Data;

namespace SaNi.Spriter
{
    public class SpriterReader : ContentTypeReader<SpriterData>
    {
        protected override SpriterData Read(ContentReader input, SpriterData data)
        {
            
            string scmlversion = input.ReadString();
            string generator = input.ReadString();
            string generatorVersion = input.ReadString();
            int folderCount = 0;
            int entityCount = 0;
            data = new SpriterData(scmlversion, generator, generatorVersion, folderCount, entityCount);
            /*int[] files = new int[folderCount];

            for (int i = 0; i < folderCount; i++)
            {
                // filujen määrä per folder
                files[i] = input.ReadInt32();
            }*/

            return data;
        }

    }
}
