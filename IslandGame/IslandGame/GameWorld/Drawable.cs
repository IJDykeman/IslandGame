using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    public interface Drawable
    {
        void draw(GraphicsDevice device, Effect effect);
    }
}
