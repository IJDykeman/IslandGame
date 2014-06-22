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
        protected Color color = Color.White;
        protected Color mousedOverColor = new Color(140, 88, 58);
        protected string toolTip = "";

        public enum buttonInteractionState
        {
            none,
            mousedOver,
            pressed
        }

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

        public UIElement(MenuAction naction, Texture2D ntexture, Vector2 nLoc, float nScale, Color nColor, string ntoolTip)
        {
            toolTip = ntoolTip;
            action = naction;
            texture = ntexture;
            location = nLoc;
            scale = nScale;
            color = nColor;
        }

        public virtual List<MenuAction> click(Vector2 clickLoc)
        {
            List<MenuAction> result = new List<MenuAction>();
            if (locIsWithinElement( clickLoc)){
            
                result.Add(action);
            }
            return result;
        }

        public virtual List<MenuAction> getAction()
        {
            List<MenuAction> result = new List<MenuAction>();
            result.Add(action);
            return result;
        }

        public bool locIsWithinElement(Vector2 clickLoc){
        
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

        public Color getColor(Vector2 clickLoc)
        {
            if (locIsWithinElement(clickLoc))
            {
                return mousedOverColor;
            }
            else
            {
                return color;
            }
        }

        public bool hasToolTip()
        {
            return !toolTip.Equals("");
        }

        public string getToolTip()
        {
            return toolTip;
        }


        public virtual void draw(SpriteBatch spriteBatch, Vector2 mouseLocation)
        {
            spriteBatch.Draw(getTexture(), getRectangle(), getColor(mouseLocation));
        }

        public virtual void draw(SpriteBatch spriteBatch, buttonInteractionState interactionState)
        {
            Color colorToUse = color;
            if (interactionState == buttonInteractionState.mousedOver)
            {
                colorToUse = mousedOverColor;
            }

            spriteBatch.Draw(getTexture(), getRectangle(), colorToUse);
        }

        public virtual List<MenuAction> incrementSelection()
        {
            return new List<MenuAction>();
        }

        public virtual List<MenuAction> decrementSelection()
        {
            return new List<MenuAction>();
        }
    }

}
