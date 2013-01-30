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
    public abstract class Shape
    {
        public Vector3 Center;
        public Color[] Colors;
        public bool SetColorsOnUpdate;

        public virtual Rectangle BoundingBox { get; set; }
        public float Radius { get { return mRadius; } }

        public GraphicsDevice Device { get; private set; }

        protected VertexBuffer mVertexBuf;
        protected IndexBuffer mIndexBuf;
        protected float mRadius;
        protected BasicEffect mShader;

        public Shape(GraphicsDevice device, Vector3 center, Color[] colors, int numVertices, float radius)
        {
            Device = device;
            Colors = colors;
            Center = center;
            mVertexBuf = new VertexBuffer(device, typeof(VertexPositionColor), numVertices, BufferUsage.WriteOnly);
            mIndexBuf = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            SetColorsOnUpdate = true;
            mShader = new BasicEffect(device);
            mRadius = radius;
        }

        public abstract void Update();
        public abstract void Draw();
    }
}