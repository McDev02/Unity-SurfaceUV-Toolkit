using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SurfaceMeshToolkit.Utility
{
    public enum ESide
    {
        North = 1 >> 0,
        East = 1 >> 1,
        South = 1 >> 2,
        West = 1 >> 3
    }
    public enum ERectBoundsEdge
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [System.Serializable]
    public struct RectBounds : IEquatable<RectBounds>
    {
        [FormerlySerializedAs("Minimal")]
        public Vector2i Minimum;
        [FormerlySerializedAs("Maximal")]
        public Vector2i Maximum;

        public int X
        {
            get { return Minimum.X; }
            set
            {
                Maximum.X = value + Maximum.X - Minimum.X;
                Minimum.X = value;
            }
        }

        public int Y
        {
            get { return Minimum.Y; }
            set
            {
                Maximum.Y = value + Maximum.Y - Minimum.Y;
                Minimum.Y = value;
            }
        }
        public int Top
        {
            get { return Maximum.Y; }
        }
        public int Right
        {
            get { return Maximum.X; }
        }
        public int Bottom
        {
            get { return Minimum.Y; }
        }
        public int Left
        {
            get { return Minimum.X; }
        }

        public Vector2i TopLeft { get { return new Vector2i(Minimum.X, Maximum.Y); } }
        public Vector2i TopRight { get { return Maximum; } }
        
        public Vector2i BottomRight { get { return new Vector2i(Maximum.X, Minimum.Y); } }
        public Vector2i BottomLeft { get { return Minimum; } }

        public void Translate(Vector2i offset)
        {
            Minimum += offset;
            Maximum += offset;
        }

        public void Translate(int X, int Y)
        {

            Translate(new Vector2i(X, Y));
        }

        public int Width
        {
            get { return Maximum.X - Minimum.X; }
            set { Maximum.X = Minimum.X + value; }
        }

        public int Height
        {
            get { return Maximum.Y - Minimum.Y; }
            set { Maximum.Y = Minimum.Y + value; }
        }

        public Vector2i Size
        {
            get { return new Vector2i(Width, Height); }
            set { Width = value.X; Height = value.Y; }
        }

        public int Area { get { return Width * Height; } }

        public Vector2 Center
        {
            get { return new Vector2(Minimum.X + Width * 0.5f, Minimum.Y + Height * 0.5f); }
        }
        public Vector2i Centeri
        {
            get { return new Vector2i((int)(Minimum.X + Width * 0.5f), (int)(Minimum.Y + Height * 0.5f)); }
        }

        public RectBounds(Vector2i tile)
        {
            Minimum.X = tile.X;
            Minimum.Y = tile.Y;
            Maximum.X = tile.X + 1;
            Maximum.Y = tile.Y + 1;
        }

        public RectBounds(int x, int y, int width, int height)
        {
            Minimum.X = x;
            Minimum.Y = y;
            Maximum.X = x + width;
            Maximum.Y = y + height;
        }

        public RectBounds(Vector2i minimal, Vector2i maximal)
        {
            Minimum.X = Mathf.Min(minimal.X, maximal.X);
            Minimum.Y = Mathf.Min(minimal.Y, maximal.Y);
            Maximum.X = Mathf.Max(minimal.X, maximal.X);
            Maximum.Y = Mathf.Max(minimal.Y, maximal.Y);
        }

        public Vector2i GetEdge(ERectBoundsEdge edge)
        {
            switch (edge)
            {
                // top left
                case ERectBoundsEdge.TopLeft:
                    return new Vector2i(Minimum.X, Maximum.Y);
                // top right
                case ERectBoundsEdge.TopRight:
                    return Maximum;
                // bottom left
                case ERectBoundsEdge.BottomLeft:
                    return Minimum;
                // bottom right
                case ERectBoundsEdge.BottomRight:
                    return new Vector2i(Maximum.X, Minimum.Y);

                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Clamps the Rect on X and Y to a Quader from 0 to size
        /// </summary>
        /// <param name="size"></param>
        public void Clamp(int width, int height)
        {
            Minimum.X = Mathf.Clamp(Minimum.X, 0, width);
            Minimum.Y = Mathf.Clamp(Minimum.Y, 0, height);
            Maximum.X = Mathf.Clamp(Maximum.X, 0, width);
            Maximum.Y = Mathf.Clamp(Maximum.Y, 0, height);
        }
        /// <summary>
        /// Clamps the Rect on X and Y to a Quader from 0 to size
        /// </summary>
        /// <param name="size"></param>
        public void Clamp(int size)
        {
            Minimum.X = Mathf.Clamp(Minimum.X, 0, size);
            Minimum.Y = Mathf.Clamp(Minimum.Y, 0, size);
            Maximum.X = Mathf.Clamp(Maximum.X, 0, size);
            Maximum.Y = Mathf.Clamp(Maximum.Y, 0, size);
        }

        public void Extend(int size)
        {
            Minimum.X -= size;
            Minimum.Y -= size;
            Maximum.X += size;
            Maximum.Y += size;
        }

        public void Extend(int top, int right, int bottom, int left)
        {
            Minimum.X -= left;
            Minimum.Y -= bottom;
            Maximum.X += right;
            Maximum.Y += top;
        }

        public void Merge(RectBounds rect)
        {
            Minimum.X = Mathf.Min(Minimum.X, rect.Minimum.X);
            Minimum.Y = Mathf.Min(Minimum.Y, rect.Minimum.Y);
            Maximum.X = Mathf.Max(Maximum.X, rect.Maximum.X);
            Maximum.Y = Mathf.Max(Maximum.Y, rect.Maximum.Y);
        }

       public bool Intersects(ref RectBounds other)
        {
            return Intersects(ref this, ref other);
        }

        public bool Overlaps(ref RectBounds other)
        {
            return Overlaps(ref this, ref other);
        }

        public bool Contains(ref RectBounds other)
        {
            return Contains(ref this, ref other);
        }

        public bool Contains(ref Vector2i point)
        {
            return ContainsPoint(ref this, ref point);
        }

        public bool Contains(Vector2i point)
        {
            return ContainsPoint(ref this, ref point);
        }
        public bool ContainsVertex(ref Vector2i point)
        {
            return ContainsVertex(ref this, ref point);
        }

        public bool ContainsVertex(Vector2i point)
        {
            return ContainsVertex(ref this, ref point);
        }

        public bool Equals(RectBounds other)
        {
            return Minimum.X == other.Minimum.X
                && Minimum.Y == other.Minimum.Y
                && Maximum.X == other.Maximum.X
                && Maximum.Y == other.Maximum.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is RectBounds)
            {
                return Equals((RectBounds)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Minimum.GetHashCode() + Maximum.GetHashCode();
        }

        public override string ToString()
        {
            return "(Minimum: " + Minimum + ", Maximum: " + Maximum + ")";
        }

        public static implicit operator Rect(RectBounds bounds)
        {
            Rect rect = new Rect();
            rect.min = bounds.Minimum;
            rect.max = bounds.Maximum;
            return rect;
        }

        public static explicit operator RectBounds(Rect rect)
        {
            RectBounds bounds;
            bounds.Minimum = (Vector2i)rect.min;
            bounds.Maximum = (Vector2i)rect.max;
            return bounds;
        }

        public static bool operator ==(RectBounds left, RectBounds right)
        {
            return left.Minimum.X == right.Minimum.X
                && left.Minimum.Y == right.Minimum.Y
                && left.Maximum.X == right.Maximum.X
                && left.Maximum.Y == right.Maximum.Y;
        }

        public static bool operator !=(RectBounds left, RectBounds right)
        {
            return left.Minimum.X != right.Minimum.X
                || left.Minimum.Y != right.Minimum.Y
                || left.Maximum.X != right.Maximum.X
                || left.Maximum.Y != right.Maximum.Y;
        }

        // create a rect based on two indices
        public static RectBounds CreateRectBounds(Vector2i point0, Vector2i point1)
        {
            RectBounds result;
            result.Minimum.X = Math.Min(point0.X, point1.X);
            result.Minimum.Y = Math.Min(point0.Y, point1.Y);
            result.Maximum.X = Math.Max(point0.X, point1.X);
            result.Maximum.Y = Math.Max(point0.Y, point1.Y);
            return result;
        }

        public static RectBounds MaskOut(RectBounds source, RectBounds mask)
        {
            RectBounds result;
            result.Minimum.X = Math.Max(source.Minimum.X, mask.Minimum.X);
            result.Minimum.Y = Math.Max(source.Minimum.Y, mask.Minimum.Y);
            result.Maximum.X = Math.Min(source.Maximum.X, mask.Maximum.X);
            result.Maximum.Y = Math.Min(source.Maximum.Y, mask.Maximum.Y);
            return result;
        }

        public static void Extends(ref RectBounds rect, ESide side)
        {
            int top = ((int)side >> 0) & 1; // north
            int right = ((int)side >> 1) & 1; // east
            int bottom = ((int)side >> 2) & 1; // south
            int left = ((int)side >> 3) & 1; // west
            rect.Extend(top, right, bottom, left);
        }

        public static bool Intersects(ref RectBounds rect0, ref RectBounds rect1)
        {
            return !(rect0.Maximum.X < rect1.Minimum.X
                || rect0.Minimum.X > rect1.Maximum.X
                || rect0.Maximum.Y < rect1.Minimum.Y
                || rect0.Minimum.Y > rect1.Maximum.Y);
        }

        public static bool Overlaps(ref RectBounds rect0, ref RectBounds rect1)
        {
            return rect0.Maximum.X > rect1.Minimum.X
                && rect0.Minimum.X < rect1.Maximum.X
                && rect0.Maximum.Y > rect1.Minimum.Y
                && rect0.Minimum.Y < rect1.Maximum.Y;
        }

        public static bool Contains(ref RectBounds rect0, ref RectBounds rect1)
        {
            return rect0.Minimum.X <= rect1.Minimum.X
                && rect0.Minimum.Y <= rect1.Minimum.Y
                && rect0.Maximum.X >= rect1.Maximum.X
                && rect0.Maximum.Y >= rect1.Maximum.Y;
        }

        public static bool ContainsPoint(ref RectBounds rect, ref Vector2i point)
        {
            //if (rect.Maximum.Y == point.Y || rect.Maximum.X == point.X)
            //	Debug.LogError("This equation has been removed below, please check if this method is still correct!");

            return rect.Minimum.X <= point.X
                && rect.Minimum.Y <= point.Y
                && rect.Maximum.X > point.X
                && rect.Maximum.Y > point.Y;
        }
        public static bool ContainsVertex(ref RectBounds rect, ref Vector2i point)
        {
            return rect.Minimum.X <= point.X
                && rect.Minimum.Y <= point.Y
                && rect.Maximum.X >= point.X
                && rect.Maximum.Y >= point.Y;
        }
        /// <summary>
        /// Return the width or edge depending on the index
        /// It's used to make easy link to Vector2i where v2i[0]=x and v2i[1]=y;
        /// </summary>
        /// <param name="index">array index</param>
        /// <returns></returns>
        public int this[int index]
        {
            get
            {
                if (index == 0)
                    return Width;
                else
                    return Height;
            }

            set
            {
                if (index == 0)
                    Width = value;
                else
                    Height = value;
            }

        }

        internal Rect ToRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}