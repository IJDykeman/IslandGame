using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace IslandGame.menus
{
    public class UIElement
    {
        protected Texture2D texture;
        protected  MenuAction action;
        protected Vector2 location;
        protected float scale = 1;

        public UIElement() { }

        public UIElement(MenuAction nAction, Texture2D nTexture, Vector2 nLoc)
        {
            action = nAction;
            texture = nTexture;
            location = nLoc;
        }

        public UIElement(MenuAction naction, Texture2D ntexture, Vector2 nLoc, float nScale)
        {
            action = naction;
            texture = ntexture;
            location = nLoc;
            scale = nScale;
        }

        public virtual List<MenuAction> click(Vector2 clickLoc)
        {
            List<MenuAction> result = new List<MenuAction>();
            if (clickIsWithinElement( clickLoc)){
            
                result.Add(action);
            }
            return result;
        }

        public bool clickIsWithinElement(Vector2 clickLoc){
        
            return getRectangle().Contains(new Point((int)clickLoc.X, (int)clickLoc.Y));
        }

        public virtual Rectangle getRectangle()
        {
            return new Rectangle((int)location.X, (int)location.Y, (int)(texture.Width*scale), (int)(texture.Height*scale));
        }

        public Texture2D getTexture()
        {
            return texture;
        }
    }

}
