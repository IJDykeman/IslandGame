using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IslandGame.GameWorld;

namespace IslandGame
{
    public class Box
    {
        public Vector2 texturePos;
        protected Vector3 XlYlZl, XhYhZh, XhYhZl, XlYhZh, XlYhZl, XlYlZh, XhYlZl, XhYlZh;
        protected Vector3[] points;
        public Vector3 loc;
        public Vector2 frontTexturePos;
        public Vector2 topTexturePos;
        public Vector2 bottomTexturePos;
        public float height = 1;
        public float width = 1;
        public float length = 1;
         
        Color color;


        protected int count = 0;


        protected VertexPostitionColorPaintNormal[] vertices;
        protected int[] indices;

        public Box() { }

        public Box(Vector3 nloc, float nwidth, float nheight, float nlength)
        {
            loc = nloc;
            width = nwidth;
            height = nheight;
            length = nlength;


            color = Color.Red;


            vertices = new VertexPostitionColorPaintNormal[36];
            indices = new int[36];

            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            resetPoints();



        }


        protected void resetPoints()
        {
            XhYhZh = new Vector3(width / 2, height / 2, length / 2);
            XhYhZl = new Vector3(width / 2, height / 2, -length / 2);
            XhYlZl = new Vector3(width / 2, -height / 2, -length / 2);
            XlYlZl = new Vector3(-width / 2, -height / 2, -length / 2);
            XlYhZh = new Vector3(-width / 2, height / 2, length / 2);
            XlYlZh = new Vector3(-width / 2, -height / 2, length / 2);
            XhYlZh = new Vector3(width / 2, -height / 2, length / 2);
            XlYhZl = new Vector3(-width / 2, height / 2, -length / 2);

            points = new Vector3[8];
            points[0] = XhYhZh;
            points[1] = XhYhZl;
            points[2] = XhYlZl;
            points[3] = XlYlZl;
            points[4] = XlYhZh;
            points[5] = XlYlZh;
            points[6] = XhYlZh;
            points[7] = XlYhZl;
        }


        public Vector3 getAverageLocation()
        {
            Vector3 result = new Vector3();
            for (int i = 0; i < vertices.Length; i++)
            {
                result += vertices[i].Position;
            }
            result /= vertices.Length;
            return result;
        }

        public virtual void draw()
        {



            count = 0;



            transformPoints(Matrix.CreateTranslation(loc));
            drawSides();
            resetPoints();

            

           
           
            
            

            foreach (EffectPass pass in Compositer.effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //device.vertexDeclaration = texturedVertexDeclaration;
                Compositer.device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPostitionColorPaintNormal.VertexDeclaration);

            }


        }


        public List<VertexPostitionColorPaintNormal> getVertices()
        {



            count = 0;

            // Matrix matrix = Matrix.CreateTranslation(bodyOffset) * Matrix.CreateRotationY(bodyHeading_rotY) * Matrix.CreateRotationX(bodyrotX) * Matrix.CreateRotationZ(bodyrotZ);

            // Matrix jointMatrix = Matrix.CreateTranslation(joint) * (Matrix.CreateRotationX(swingRadians + xRest + yTilt) * Matrix.CreateRotationZ(zRest) * Matrix.CreateRotationY(yRest)) * Matrix.CreateTranslation(-joint);

            Matrix matrix = Matrix.CreateTranslation(loc);


            transformPoints(Matrix.CreateTranslation(loc));





            drawSides();
            resetPoints();


            return new List<VertexPostitionColorPaintNormal>(vertices);

        }


        protected void drawSides()
        {
            drawFront();
            drawBack();
            drawTop();
            drawBottom();
            drawLeft();
            drawRight();
        }

        protected virtual void transformPoints(Matrix matrix)//, Matrix jointMatrix)
        {


            for (int i = 0; i < points.Length; i++)
            {

                //points[i] = Vector3.Transform(points[i], jointMatrix);
                points[i] = Vector3.Transform(points[i], matrix);

            }

            XhYhZh = points[0];
            XhYhZl = points[1];
            XhYlZl = points[2];
            XlYlZl = points[3];
            XlYhZh = points[4];
            XlYlZh = points[5];
            XhYlZh = points[6];
            XlYhZl = points[7];
        }



        protected void drawFront()
        {

            Vector3 thisNormal = new Vector3(0, 0, -1);

            this.vertices[count].Position = (XlYhZl);
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;

            count++;

            this.vertices[count].Position = XlYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;

            count++;

            this.vertices[count].Position = XhYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;





            this.vertices[count].Position = XhYhZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYhZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XhYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


        }

        protected void drawBack()
        {


            Vector3 thisNormal = new Vector3(0, 0, 1);
            //Vector3 thisNormal = new Vector3(3f, -.5f, 1f);
            this.vertices[count].Position = XlYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYhZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XhYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;



            this.vertices[count].Position = XlYhZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XhYhZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XhYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


        }
        protected void drawRight()
        {

            Vector3 thisNormal = new Vector3(-1, 0, 0);
            //Vector3 thisNormal = new Vector3(-.6f, -.5f, 1f);
            this.vertices[count].Position = XlYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYhZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYhZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XlYhZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


        }
        protected void drawLeft()
        {


            Vector3 thisNormal = new Vector3(1, 0, 0);
            this.vertices[count].Position = XhYhZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XhYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XhYhZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


            this.vertices[count].Position = XhYhZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XhYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XhYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


        }
        protected void drawBottom()
        {
            Vector3 thisNormal = new Vector3(0, -1, 0);
            this.vertices[count].Position = XhYlZh;

            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XlYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


            this.vertices[count].Position = XhYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XlYlZl;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XhYlZh;
            //this.vertices[count].Color = sideColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


        }

        protected void drawTop()
        {

            //setupVertices();

            Vector3 thisNormal = new Vector3(0, 1, 0);
            this.vertices[count].Position = XlYhZl;
            //this.vertices[count].Color = topColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;


            count++;
            this.vertices[count].Position = XhYhZh;
            //this.vertices[count].Color = topColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;

            this.vertices[count].Position = XlYhZh;
            //this.vertices[count].Color = topColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


            this.vertices[count].Position = XlYhZl;
            //this.vertices[count].Color = topColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XhYhZl;
            // this.vertices[count].Color = topColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;
            this.vertices[count].Position = XhYhZh;
            // this.vertices[count].Color = topColor;
            this.vertices[count].Normal = thisNormal;
            this.vertices[count].Color = color;
            count++;


        }
    }

}
