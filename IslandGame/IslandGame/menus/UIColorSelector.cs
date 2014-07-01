using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.menus
{
    class UIColorSelector : UIRadioGroup
    {
        public UIColorSelector(int screenWidth, int screenHeight)
        {
            children = new List<UIElement>();

            addColorButton(new Vector2((colorSwatchWidth()+10)*1,
                screenHeight - colorSwatchHeight()-10), 3);

            addColorButton(new Vector2((colorSwatchWidth() + 10) * 2,
                screenHeight - colorSwatchHeight() - 10), 67);

            addColorButton(new Vector2((colorSwatchWidth() + 10) * 3,
                screenHeight - colorSwatchHeight() - 10), 125);
        }

        private void addColorButton(Vector2 loc, byte color)
        {
            children.Add(new UIElement(new ColorSelection(color), getColorSwatchBox(), loc,
                1, ColorPallete.getColorFromByte(color), ColorPallete.getColorFromByte(color), "Select color"));
        }

        private Texture2D getColorSwatchBox()
        {
            return ContentDistributor.colorSwatchBox;
        }

        private int colorSwatchHeight()
        {
            return getColorSwatchBox().Height;
        }

        private int colorSwatchWidth()
        {
            return getColorSwatchBox().Width;
        }
    }
}
