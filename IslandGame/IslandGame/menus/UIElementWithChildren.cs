using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.menus
{
    class UIElementWithChildren : UIElement
    {
        protected List<UIElement> children;

        public override bool locIsWithinElement(Microsoft.Xna.Framework.Vector2 clickLoc)
        {
            foreach (UIElement child in children)
            {
                if (child.locIsWithinElement(clickLoc))
                {
                    return true;
                }
            }
            return false;
        }

        public override List<MenuAction> click(Microsoft.Xna.Framework.Vector2 clickLoc)
        {
            List<MenuAction> result = new List<MenuAction>();
            foreach (UIElement child in children)
            {
                result.AddRange(child.click(clickLoc));
            }
            return result;
        }
    }
}
