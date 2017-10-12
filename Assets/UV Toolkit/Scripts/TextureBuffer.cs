﻿using System;
using UnityEngine;

namespace SurfaceMeshToolkit.Utility
{
    /// <summary>
    /// Wraps a texture colors for faster access
    /// </summary>
    [Serializable]
    public class TextureBuffer
    {
        public enum EChannel
        {
            Red,
            Green,
            Blue,
            Alpha
        }

        private Color32[] m_Colors;
        private Color32[] m_ClearColors;
        public int m_Width;
        public int m_Height;
        public int PixelCount { get; private set; }
        private bool dirty;

        [SerializeField] private Texture2D m_Texture;

        public Texture2D Texture
        {
            get { return m_Texture; }
        }

        public TextureBuffer(int width, int height, TextureFormat format, bool mipmap, bool linear, FilterMode mode,
            TextureWrapMode warpMode)
        {
            m_Texture = new Texture2D(width, height, format, mipmap, linear);
            m_Texture.filterMode = mode;
            m_Texture.wrapMode = warpMode;

            m_Width = width;
            m_Height = height;
            PixelCount = m_Width * m_Height;
            m_Colors = new Color32[PixelCount];
            m_ClearColors = new Color32[PixelCount];
            dirty = true;

            for (int i = 0; i < m_ClearColors.Length; i++)
            {
                m_ClearColors[i] = Color.clear;
            }
        }

        internal void CopyFrom(Texture2D baseTexture)
        {
            Graphics.CopyTexture(baseTexture, m_Texture);
            dirty = true;
            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    m_Colors[y * m_Height + x] = m_Texture.GetPixel(x, y);
                }
            }
            Apply(true);
        }

        public void ClearPixels(Color32 col)
        {
            dirty = true;
            var colors = m_Colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = col;
            }
        }

        public void ClearPixels(Color[] col)
        {
            dirty = true;
            var colors = m_Colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = col[i >= col.Length ? col.Length - 1 : i];
            }
        }

        public void Clear()
        {
            ClearPixels(Color.clear);
            //dirty = true;								Doesn't work
            //var color = m_ClearColors;
            //m_Colors = color;
        }

        public void Resize(int width, int height)
        {
            if (width == m_Width && height == m_Height)
                return;

            m_Texture.Resize(width, height);

            m_Width = width;
            m_Height = height;
            PixelCount = m_Width * m_Height;
            m_Colors = new Color32[PixelCount];
            m_ClearColors = new Color32[PixelCount];
            dirty = true;

            for (int i = 0; i < m_ClearColors.Length; i++)
            {
                m_ClearColors[i] = Color.black;
            }
        }

        public void SetPixels(RectBounds rect, Color32 col)
        {
            dirty = true;
            rect.Clamp(m_Width, m_Height);
            var colors = m_Colors;
            for (int y = rect.Minimum.Y; y < rect.Maximum.Y; y++)
            {
                int yOff = y * m_Height;
                for (int x = rect.Minimum.X; x < rect.Maximum.X; x++)
                {
                    colors[yOff + x] = col;
                }
            }
        }
        public void SetPixels(RectBounds rect, Color32 col, byte alpha)
        {
            dirty = true;
            rect.Clamp(m_Width, m_Height);
            var colors = m_Colors;
            col.a = alpha;
            for (int y = rect.Minimum.Y; y < rect.Maximum.Y; y++)
            {
                int yOff = y * m_Height;
                for (int x = rect.Minimum.X; x < rect.Maximum.X; x++)
                {
                    colors[yOff + x] = col;
                }
            }
        }

        public void SetPixel(int index, Color32 col)
        {
            dirty = true;
            m_Colors[index] = col;
        }

        public void SetPixel(int x, int y, Color32 col)
        {
            dirty = true;
            m_Colors[y * m_Height + x] = col;
        }

        public void SetPixel(Vector2i index, Color32 col)
        {
            dirty = true;
            m_Colors[index.Y * m_Width + index.X] = col;
        }

        public void SetPixel(int index, Color32 col, byte alpha)
        {
            dirty = true;
            col.a = alpha;
            m_Colors[index] = col;
        }

        public void SetPixel(int x, int y, Color32 col, byte alpha)
        {
            dirty = true;
            col.a = alpha;
            m_Colors[y * m_Height + x] = col;
        }

        public void SetPixel(Vector2i index, Color32 col, byte alpha)
        {
            dirty = true;
            col.a = alpha;
            m_Colors[index.Y * m_Height + index.X] = col;
        }

        public void SetChannel(Vector2i index, float value, EChannel chanel)
        {
            dirty = true;
            Color col = m_Colors[index.Y * m_Height + index.X];
            col[(int)chanel] = value;
            m_Colors[index.Y * m_Height + index.X] = col;
        }

        public void SetChannel(Vector2i index, float value, EChannel chanel, float blend)
        {
            dirty = true;
            Color col = m_Colors[index.Y * m_Height + index.X];
            col[(int)chanel] = Mathf.Lerp(col[(int)chanel], value, blend);
            m_Colors[index.Y * m_Height + index.X] = col;
        }

        public void AddChannel(Vector2i index, float value, EChannel chanel)
        {
            dirty = true;
            Color col = m_Colors[index.Y * m_Height + index.X];
            col[(int)chanel] = Mathf.Clamp01(col[(int)chanel] + value);
            m_Colors[index.Y * m_Height + index.X] = col;
        }

        public void Apply(bool force = false)
        {
            if (dirty || force)
            {
                m_Texture.SetPixels32(m_Colors);
                m_Texture.Apply();
            }
        }
    }
}