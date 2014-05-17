using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.menus
{
    class Image : UIElement
    {
        int width;
        int height;

        public Image(Texture2D nTexture, Vector2 nLoc, int nWidth, int nHeight)
        {

            texture = nTexture;
            location = nLoc;
            width = nWidth;
            height = nHeight;
        }

        public override List<MenuAction> click(Vector2 clickLoc)
        {
            List<MenuAction> result = new List<MenuAction>();
            return result;
        }

        public override Rectangle getRectangle()
        {
            return new Rectangle((int)location.X, (int)location.Y, width,height);
        }
    }
}
