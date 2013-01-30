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

namespace Shapes
    {
    public class Rect : Shape
    {
        public override Rectangle BoundingBox
        {
            get
            {
                int x = (int)Center.X, y = (int)Center.Y;
                int diameter = (int)mRadius * 2;

                return new Rectangle(
                    x, y,
                    x + diameter,
                    y + diameter
                );
            }
        }

        const float TestRectZCoordinate = 0;
        const int NumVertices = 4;

        public Rect(GraphicsDevice device, Vector3 center, Color[] colors, float radius)
            : base(device, center, colors, NumVertices, radius)
        {
            if (colors.Length < NumVertices)
                throw new IndexOutOfRangeException(string.Format("Color array passed to Rect constructor MUST have an element index size of 4. Current length passed is {0}", colors.Length));

            mShader.VertexColorEnabled = true;

            mVertexBuf.SetData<VertexPositionColor>(
                    new VertexPositionColor[]
                        {
                            new VertexPositionColor(new Vector3(Center.X - mRadius, Center.Y - mRadius, TestRectZCoordinate), Colors[2]),
                            new VertexPositionColor(new Vector3(Center.X + mRadius, Center.Y - mRadius, TestRectZCoordinate), Colors[1]),
                            new VertexPositionColor(new Vector3(Center.X + mRadius, Center.Y + mRadius, TestRectZCoordinate), Colors[0]),
                            new VertexPositionColor(new Vector3(Center.X - mRadius, Center.Y + mRadius, TestRectZCoordinate), Colors[3])
                        });

            mIndexBuf.SetData<short>(new short[] { 0, 1, 2, 0, 2, 3 });
        }

        public override void Update()
        {
            //TODO
        }

        public override void Draw()
        {
            var halfWidth = Device.Viewport.Width / 2f;
            var halfHeight = Device.Viewport.Height / 2f;

            // mShader.World = Matrix.CreateWorld(Center, Vector3.Forward, Vector3.Up);
            mShader.Projection = Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, halfHeight, -halfHeight, 0, 1);

            Device.SetVertexBuffer(mVertexBuf);
            Device.Indices = mIndexBuf;
            foreach (var pass in mShader.CurrentTechnique.Passes)
            {
                pass.Apply();

                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, NumVertices, 0, 2);
            }
        }
    }
}