using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.menus
{
    class UIRadioGroup : UIElementWithChildren
    {
        //will be used for color selector and first person tool selector
        int currentlySelectedIndex = 0;

        public UIRadioGroup()
        {
        }

        public UIRadioGroup(List<UIElement> nList)
        {
            children = nList;
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, buttonInteractionState state)
        {
            for (int i = 0; i < children.Count; i++)
            {
                state = buttonInteractionState.none;
                if (i == currentlySelectedIndex)
                {
                    state = buttonInteractionState.mousedOver;
                }
                children[i].draw(spriteBatch, state);
            }
        }

        public override List<MenuAction> decrementSelection()
        {
            currentlySelectedIndex++;
            currentlySelectedIndex %= children.Count;
            List<MenuAction> result = new List<MenuAction>();
            result.AddRange(children[currentlySelectedIndex].getAction());
            return result;
        }
    
        public override List<MenuAction> incrementSelection()
        {
            currentlySelectedIndex--;
            if (currentlySelectedIndex < 0)
            {
                currentlySelectedIndex = children.Count - 1;
            }
            List<MenuAction> result = new List<MenuAction>();
            result.AddRange(children[currentlySelectedIndex].getAction());
            return result;
        }



    }
}
