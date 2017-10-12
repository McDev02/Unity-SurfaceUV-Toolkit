using UnityEngine;
using UnityEditor;
using System.IO;

namespace SurfaceMeshToolkit
{
	public class SurfaceMeshProcessor
	{
		public SurfaceModel Model;
		public MeshRenderer Modelrenderer;
		public LightmapData ModelLightmapData;

		public Texture2D infoTexture;
		Color[] infoTextureBlack;
		Color[] infoTextureCache;
		public Texture2D processingTexture;
		Color[] processingTextureCache;
		Color[] processingTextureBlack;
		Texture2D CachedLightmapTexture;

		public bool IsInPreviewMode;
		public bool IsInLightmapMode;
		Material previewMaterial;
		Material cachedMaterial;

		public bool HasActiveTexture { get; set; }

		public void ShowSeamTexture(int uvset, Texture2D BaseTexture, bool isLightmap)
		{
			IsInLightmapMode = isLightmap;
			if (IsInLightmapMode)
				uvset = 2;

			int width = 512;
			int height = 512;
			TextureFormat format = TextureFormat.RGB24;
			FilterMode filter = FilterMode.Bilinear;
			TextureWrapMode wrap = TextureWrapMode.Repeat;
			bool mipmap = false;
			bool linear = false;

			infoTexture = new Texture2D(width, height, format, mipmap, linear);
			infoTexture.filterMode = filter;
			infoTexture.wrapMode = wrap;
			infoTextureBlack = new Color[width * height];

			if (BaseTexture != null)
			{
				width = BaseTexture.width;
				height = BaseTexture.height;
				if (IsInLightmapMode)
					format = BaseTexture.format;
				else if (BaseTexture.format == TextureFormat.ARGB32 || BaseTexture.format == TextureFormat.RGBA32 || BaseTexture.format == TextureFormat.RGB24 || BaseTexture.format == TextureFormat.Alpha8)
					format = BaseTexture.format;
				else
				{
					if (BaseTexture.format == TextureFormat.DXT1 || BaseTexture.format == TextureFormat.DXT1Crunched)
						format = TextureFormat.RGB24;
					else if (BaseTexture.format == TextureFormat.DXT5 || BaseTexture.format == TextureFormat.DXT5Crunched)
						format = TextureFormat.RGBA32;
					else
						format = TextureFormat.RGBA32;
				}
				filter = BaseTexture.filterMode;
				wrap = BaseTexture.wrapMode;
				mipmap = BaseTexture.mipmapCount > 1;
				//linear = IsInLightmapMode;
			}

			processingTexture = new Texture2D(width, height, format, mipmap, linear);
			processingTexture.filterMode = filter;
			processingTexture.wrapMode = wrap;
			processingTextureBlack = new Color[width * height];
			if (BaseTexture != null)
			{
				if (false && IsInLightmapMode)
					Graphics.CopyTexture(BaseTexture, processingTexture);
				else
					processingTexture.SetPixels(BaseTexture.GetPixels());
			}
			processingTexture.Apply();
			previewMaterial = new Material(Shader.Find("Hidden/TexturePreview"));
			previewMaterial.SetTexture("_InfoTex", infoTexture);
			previewMaterial.SetTexture("_MainTex", processingTexture);
			previewMaterial.EnableKeyword(uvset == 2 ? "UVSET2" : "UVSET1");

			if (isLightmap)
			{
				var offset = Modelrenderer.lightmapScaleOffset;
				previewMaterial.SetTextureScale("_MainTex", new Vector2(offset.x, offset.y));
				previewMaterial.SetTextureOffset("_MainTex", new Vector2(offset.z, offset.w));
			}

			ShowTextureOnModel();
			DrawInfoSeams(uvset);
		}

		public void ShowTextureOnModel()
		{
			IsInPreviewMode = true;
			if (IsInLightmapMode)
			{
				if (false)
				{
					var data = LightmapSettings.lightmaps;
					CachedLightmapTexture = data[Modelrenderer.lightmapIndex].lightmapColor;
					data[Modelrenderer.lightmapIndex].lightmapColor = processingTexture;
					LightmapSettings.lightmaps = data;
				}
				else
				{
					if (!IsInPreviewMode)
						cachedMaterial = Modelrenderer.sharedMaterial;
					Modelrenderer.sharedMaterial = previewMaterial;
					IsInPreviewMode = true;
				}
			}
			else
			{
				if (!IsInPreviewMode)
					cachedMaterial = Modelrenderer.sharedMaterial;
				previewMaterial = new Material(Modelrenderer.sharedMaterial);
				previewMaterial.SetTexture("_MainTex", processingTexture);
				Modelrenderer.sharedMaterial = previewMaterial;
				IsInPreviewMode = true;
			}
		}
		public void RestoreModelMaterial()
		{
			if (true || !IsInLightmapMode)
				Modelrenderer.sharedMaterial = cachedMaterial;

			IsInPreviewMode = false;
			//if (CachedLightmapTexture != null)
			//{
			//    var data = LightmapSettings.lightmaps[Modelrenderer.lightmapIndex];
			//    data.lightmapColor = CachedLightmapTexture;
			//    LightmapSettings.lightmaps[Modelrenderer.lightmapIndex] = data;
			//}
		}

		public void DrawInfoSeams(int uvset)
		{
			if (Model == null || infoTexture == null)
				return;

			infoTexture.SetPixels(infoTextureBlack);
			int EdgeSamples = 2;
			var edges = uvset == 1 ? Model.UV1BoarderEdges : Model.UV2BoarderEdges;
			foreach (var edge in edges)
			{
				Vector2 pos, a, b;
				Vector2i coords;
				int id;

				a = edge.A.Coords;
				b = edge.B.Coords;
				a.x *= infoTexture.width;
				a.y *= infoTexture.height;
				b.x *= infoTexture.width;
				b.y *= infoTexture.height;

				float lx = b.x - a.x;
				float ly = b.y - a.y;

				int samples = 1 + EdgeSamples * (int)Mathf.Ceil(Mathf.Sqrt(lx * lx + ly * ly));

				for (int i = 0; i <= samples; i++)
				{
					pos = Vector2.Lerp(a, b, i / (float)samples);
					pos.x = Mathf.Clamp(pos.x, 0, infoTexture.width);
					pos.y = Mathf.Clamp(pos.y, 0, infoTexture.height);
					//coords = new Vector2i(pos.x, pos.y);
					//coords.Clamp(0, 0, infoTexture.width, infoTexture.height);
					//id = coords.X + coords.Y * infoTexture.width;
					//infoTexture.SetPixel(coords.X , coords.Y, Color.red);
					SetPixelBilinear(infoTexture, pos.x, pos.y, Color.red);
				}
			}
			infoTexture.Apply();

			infoTextureCache = infoTexture.GetPixels();
			processingTextureCache = processingTexture.GetPixels();
		}
		public void InterpolateUVSeams()
		{
			if (Model == null || processingTexture == null)
				return;
			
			int EdgeSamples = 2;
			var edges = IsInLightmapMode ? Model.UV2BoarderVertexEdges : Model.UV1BoarderVertexEdges;
			foreach (var edge in edges)
			{
				Vector2 uv1, uv2, a1, b1, a2, b2;
				a1 = b1 = a2 = b2 = Vector2.zero;

				if (IsInLightmapMode)
				{
					a1 = edge.UV2Edges[0].A.Coords;
					b1 = edge.UV2Edges[0].B.Coords;
					if (edge.UV2Edges[1].A.Vertex == edge.UV2Edges[0].A.Vertex)
					{
						a2 = edge.UV2Edges[1].A.Coords;
						b2 = edge.UV2Edges[1].B.Coords;
					}
					else
					{
						a2 = edge.UV2Edges[1].B.Coords;
						b2 = edge.UV2Edges[1].A.Coords;
					}
				}
				else
				{
					a1 = edge.UV1Edges[0].A.Coords;
					b1 = edge.UV1Edges[0].B.Coords;
					if (edge.UV1Edges[1].A.Vertex == edge.UV1Edges[0].A.Vertex)
					{
						a2 = edge.UV1Edges[1].A.Coords;
						b2 = edge.UV1Edges[1].B.Coords;
					}
					else
					{
						a2 = edge.UV1Edges[1].B.Coords;
						b2 = edge.UV1Edges[1].A.Coords;
					}
				}
				float lx1 = (b1.x - a1.x) * processingTexture.width;
				float ly1 = (b1.y - a1.y) * processingTexture.height;
				float lx2 = (b2.x - a2.x) * processingTexture.width;
				float ly2 = (b2.y - a2.y) * processingTexture.height;

				int samples = 1 + EdgeSamples * (int)((Mathf.Ceil(Mathf.Sqrt(lx1 * lx1 + ly1 * ly1)) + Mathf.Ceil(Mathf.Sqrt(lx2 * lx2 + ly2 * ly2))) / 2f);
				bool drawDot = false;

				for (int i = 0; i <= samples; i++)
				{
					uv1 = Vector2.Lerp(a1, b1, i / (float)samples);
					Vector2 baseUV1 = uv1;
					uv1.x = Mathf.Clamp(uv1.x, 0, 1);
					uv1.y = Mathf.Clamp(uv1.y, 0, 1);

					uv2 = Vector2.Lerp(a2, b2, i / (float)samples);
					Vector2 baseUV2 = uv2;
					uv2.x = Mathf.Clamp(uv2.x, 0, 1);
					uv2.y = Mathf.Clamp(uv2.y, 0, 1);

					if (IsInLightmapMode)
					{
						uv1 = GetLightmapUV(uv1);
						uv2 = GetLightmapUV(uv2);
					}

					//Interpolate
					float x, y;

					Color col = processingTexture.GetPixelBilinear(uv1.x, uv1.y);
					col += processingTexture.GetPixelBilinear(uv2.x, uv2.y);

					col /= 2f;
					float strength = 1;

					SetPixelBilinear(processingTexture, uv1, col, strength);
					SetPixelBilinear(processingTexture, uv2, col, strength);

					if (drawDot)
					{
						strength *= 0.4f;
						x = uv1.x * processingTexture.width;
						y = uv1.y * processingTexture.height;

						SetPixelBilinear(processingTexture, x - 1, y, col, strength);
						SetPixelBilinear(processingTexture, x + 1, y, col, strength);
						SetPixelBilinear(processingTexture, x, y - 1, col, strength);
						SetPixelBilinear(processingTexture, x, y + 1, col, strength);

						x = uv2.x * processingTexture.width;
						y = uv2.y * processingTexture.height;

						SetPixelBilinear(processingTexture, x - 1, y, col, strength);
						SetPixelBilinear(processingTexture, x + 1, y, col, strength);
						SetPixelBilinear(processingTexture, x, y - 1, col, strength);
						SetPixelBilinear(processingTexture, x, y + 1, col, strength);
					}
				}
			}
			processingTexture.Apply();
		}

		public Vector2 GetUV(Vector2 uv)
		{
			if (IsInLightmapMode)
				return GetLightmapUV(uv);
			else
				return uv;
		}
		public Vector2 GetLightmapUV(Vector2 uv)
		{
			var offset = Modelrenderer.lightmapScaleOffset;
			uv.x = uv.x * offset.x + offset.z;
			uv.y = uv.y * offset.y + offset.w;
			return uv;
		}

		void DrawDot(Color[] Colors, int width, int height, Vector2 pos, float radius, float falloff = 1)
		{
			int iradius = (int)Mathf.Round(radius);
			for (int y = -iradius; y <= iradius; y++)
			{
				for (int x = -iradius; x <= iradius; x++)
				{
					float strength = 1 - new Vector2(x, y).magnitude / radius;

				}
			}
		}

		public void SaveCurrentTexture()
		{
			if (IsInLightmapMode)
			{
				var data = LightmapSettings.lightmaps[Modelrenderer.lightmapIndex];
				string path = AssetDatabase.GetAssetPath(data.lightmapColor);
				path.Replace(".exr", ".png");
				var content = processingTexture.EncodeToPNG();
				var stream = File.Create(path);
				stream.Write(content, 0, content.Length);
				stream.Close();
				Debug.Log("Lightmap stored at " + path);
			}
			else { }
		}
		public void SaveCopyOfCurrentTexture()
		{
			if (IsInLightmapMode)
			{
				var data = LightmapSettings.lightmaps[Modelrenderer.lightmapIndex];
				string path = AssetDatabase.GetAssetPath(data.lightmapColor);
				path = path.Insert(path.Length - 4, "_copy");
				path.Replace(".exr", ".png");
				var content = processingTexture.EncodeToPNG();
				var stream = File.Create(path);
				stream.Write(content, 0, content.Length);
				stream.Close();
				Debug.Log("Copy of Lightmap stored at " + path);
			}
			else { }
		}

		public void SetModel(SurfaceModel surfaceModel, MeshRenderer renderer)
		{
			if (IsInPreviewMode)
				RestoreModelMaterial();
			Model = surfaceModel;
			Modelrenderer = renderer;
			cachedMaterial = Modelrenderer.sharedMaterial;
			if (renderer.lightmapIndex >= 0)
			{
				ModelLightmapData = LightmapSettings.lightmaps[renderer.lightmapIndex];
			}
		}

		struct Vector2i
		{
			public int X;
			public int Y;

			public Vector2i(float x, float y)
			{
				X = (int)x;
				Y = (int)y;
			}
			public Vector2i(int x, int y)
			{
				X = x;
				Y = y;
			}
			public void Clamp(int min, int max)
			{
				X = X < min ? min : X > max ? max : X;
				Y = Y < min ? min : Y > max ? max : Y;
			}
			public void Clamp(int minx, int miny, int maxx, int maxy)
			{
				X = X < minx ? minx : X > maxx ? maxx : X;
				Y = Y < miny ? miny : Y > maxy ? maxy : Y;
			}
		}
		/// <summary>
		/// Input Vector2 in UV Space (0-1)
		/// </summary>
		public void SetPixelBilinear(Texture2D texture, Vector2 uv, Color color, float strength = 1)
		{
			SetPixelBilinear(texture, uv.x * texture.width, uv.y * texture.height, color, strength);
		}
		/// <summary>
		/// Input Vector2 in Texture Pixel Space
		/// </summary>
		public void SetPixelBilinear(Texture2D texture, float x, float y, Color color, float strength = 1)
		{
			x -= 0.5f; y -= 0.5f;
			int x1 = (int)Mathf.Clamp(Mathf.Floor(x), 0, texture.width);
			int y1 = (int)Mathf.Clamp(Mathf.Floor(y), 0, texture.height);
			int x2 = Mathf.Clamp(x1 + 1, 0, texture.width);
			int y2 = Mathf.Clamp(y1 + 1, 0, texture.height);
			float dx = x - x1;
			float dy = y - y1;

			texture.SetPixel(x1, y1, Color.Lerp(texture.GetPixel(x1, y1), color, strength * ((1 - dx) * (1 - dy))));
			texture.SetPixel(x2, y1, Color.Lerp(texture.GetPixel(x2, y1), color, strength * (dx * (1 - dy))));
			texture.SetPixel(x1, y2, Color.Lerp(texture.GetPixel(x1, y2), color, strength * ((1 - dx) * dy)));
			texture.SetPixel(x2, y2, Color.Lerp(texture.GetPixel(x2, y2), color, strength * (dx * dy)));
		}

		public void ResetInfoTexture()
		{
			infoTexture.SetPixels(infoTextureCache);
		}
		public void ResetProcessingTexture()
		{
			processingTexture.SetPixels(processingTextureCache);
		}
		public void UpdateInfoTexture()
		{
			infoTextureCache = infoTexture.GetPixels();
		}
		public void UpdateProcessingTexture()
		{
			processingTextureCache = processingTexture.GetPixels();
			SaveCopyOfCurrentTexture();
		}

	}
}