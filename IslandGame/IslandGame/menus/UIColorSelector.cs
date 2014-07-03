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

            float xSpaceBetweenColorSwatched = (colorSwatchWidth() + 10);

            int numButtons=0;

            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 5);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 6);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 7);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 8);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 32);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 35);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 39);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 59);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 60);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 51);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 49);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 50);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 154);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 97);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 86);
            addColorButton(screenHeight, screenWidth, xSpaceBetweenColorSwatched, numButtons++, 254);

        }

        private void addColorButton(int screenHeight, int screenWidth, float xSpaceBetweenColorSwatched, int numButtonsPrevious, byte color)
        {
            addColorButton(new Vector2(xSpaceBetweenColorSwatched * numButtonsPrevious+10,
                screenHeight - colorSwatchHeight() - 10), color);
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
