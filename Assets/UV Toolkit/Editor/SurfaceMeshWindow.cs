using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SharpEXR;

namespace SurfaceMeshToolkit
{
	public class SurfaceMeshWindow : EditorWindow
	{
		SurfaceMeshProcessor MeshProcessor;
		protected static SurfaceMeshWindow window;
		private GameObject selectedObject = null;

		private MeshFilter currentMeshFilter;
		private MeshRenderer currentMeshRenderer;
		private Texture2D currentTexture;
		bool update;
		public bool HasMesh;

		bool drawTextureOnModel;
		int drawUVSet;
		string[] SelectUVStrings;
		public bool drawUV;
		public bool drawSplitUV1;
		public bool drawSplitUV2;
		public bool PaintOnTexture;

		int TargetMask;

		public SurfaceModel SurfaceModel;

		[MenuItem("Window/SurfaceAware Toolkit")]
		protected static void Start()
		{
			window = (SurfaceMeshWindow)EditorWindow.GetWindow(typeof(SurfaceMeshWindow));
			window.titleContent = new GUIContent("Surface Mesh Toolkit");
			window.autoRepaintOnSceneChange = true;
			window.minSize = new Vector2(256, 256);

			window.SelectUVStrings = new string[] { "No UV", "UV1", "UV2" };
			window.MeshProcessor = new SurfaceMeshProcessor();

			window.TargetMask = 1 << LayerMask.NameToLayer("Target");
		}

		// Update is called once per frame
		void UpdateMeshData(MeshFilter filter)
		{
			HasMesh = false;

			if (SurfaceModel == null)
				SurfaceModel = new SurfaceModel();
			else
				SurfaceModel.Clear();

			update = false;

			SurfaceModel.SetMesh(selectedObject.transform, currentMeshFilter.sharedMesh);
			HasMesh = true;

			if (MeshProcessor == null)
				MeshProcessor = new SurfaceMeshProcessor();
			MeshProcessor.SetModel(SurfaceModel, currentMeshRenderer);
		}

		void DrawLines()
		{
			if (!HasMesh)
				return;
			int drawDuration = 20;
			Color col;

			Vector3 vA, vB;
			if (drawSplitUV1)
			{
				col = Color.Lerp(Color.green, Color.yellow, 0.2f);
				SurfaceModel.Edge<SurfaceModel.Vertex2> edge;
				for (int i = 0; i < SurfaceModel.UV1BoarderEdges.Count; i++)
				{
					edge = SurfaceModel.UV1BoarderEdges[i];
					vA = SurfaceModel.Transform.localToWorldMatrix.MultiplyPoint(edge.A.Vertex.Position);
					vB = SurfaceModel.Transform.localToWorldMatrix.MultiplyPoint(edge.B.Vertex.Position);
					Debug.DrawLine(vA, vB, col);
				}
			}
			else if (drawSplitUV2)
			{
				col = Color.blue;
				SurfaceModel.Edge<SurfaceModel.Vertex2> edge;
				for (int i = 0; i < SurfaceModel.UV2BoarderEdges.Count; i++)
				{
					edge = SurfaceModel.UV2BoarderEdges[i];
					vA = SurfaceModel.Transform.localToWorldMatrix.MultiplyPoint(edge.A.Vertex.Position);
					vB = SurfaceModel.Transform.localToWorldMatrix.MultiplyPoint(edge.B.Vertex.Position);
					Debug.DrawLine(vA, vB, col);
				}
			}
		}

		private void OnSelectionChange()
		{
			CheckNewMesh();
			Repaint();
		}

		void OnEnable()
		{
			// Remove delegate listener if it has previously
			// been assigned.
			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
			// Add (or re-add) the delegate.
			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		}
		private void OnDestroy()
		{
			MeshProcessor.RestoreModelMaterial();
			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		}

		void OnSceneGUI(SceneView sceneView)
		{
			if (!(PaintOnTexture))// && Input.GetKey(KeyCode.LeftControl)
				return;
			Texture2D texture = MeshProcessor.processingTexture;
			if (drawTextureOnModel && MeshProcessor.IsInPreviewMode)
			{
				bool DrawOrPreview = true;
				var mousePos = Event.current.mousePosition;
				mousePos.y = Camera.current.pixelRect.height - mousePos.y;
				RaycastHit hit;
				Color col = Color.blue;

				bool drawDot = false;

				if (!DrawOrPreview)
					MeshProcessor.ResetProcessingTexture();

				if (Physics.Raycast(Camera.current.ScreenPointToRay(mousePos), out hit, 100, TargetMask))
				{
					Vector2 uv = MeshProcessor.IsInLightmapMode ? hit.textureCoord2 : hit.textureCoord;
					var tris = SurfaceModel.GetOpposingTriangles(MeshProcessor.IsInLightmapMode ? SurfaceModel.MeshData.UV2 : SurfaceModel.MeshData.UV1, hit.triangleIndex, uv, 1);//1f / 128f
					uv = MeshProcessor.GetUV(uv);

					float x, y;

					int colCount = 1;
					col = texture.GetPixelBilinear(uv.x, uv.y);
					//Draw Average
					if (DrawOrPreview)
					{
						if (tris != null)
						{
							for (int i = 0; i < tris.Count; i++)
							{
								var myuv = MeshProcessor.GetUV(tris[i].Position);
								col += texture.GetPixelBilinear(myuv.x, myuv.y);
								colCount++;
							}
						}
						col /= colCount;
						float strength = 0.1f;

						MeshProcessor.SetPixelBilinear(texture, uv, col, strength);

						if (drawDot)
						{
							x = uv.x * texture.width;
							y = uv.y * texture.height;

							MeshProcessor.SetPixelBilinear(texture, x - 1, y, col, strength);
							MeshProcessor.SetPixelBilinear(texture, x + 1, y, col, strength);
							MeshProcessor.SetPixelBilinear(texture, x, y - 1, col, strength);
							MeshProcessor.SetPixelBilinear(texture, x, y + 1, col, strength);
						}

						if (tris != null)
						{
							for (int i = 0; i < tris.Count; i++)
							{
								var myuv = MeshProcessor.GetUV(tris[i].Position);
								MeshProcessor.SetPixelBilinear(texture, myuv, col, strength);

								if (drawDot)
								{
									x = myuv.x * texture.width;
									y = myuv.y * texture.height;

									MeshProcessor.SetPixelBilinear(texture, x - 1, y, col, strength);
									MeshProcessor.SetPixelBilinear(texture, x + 1, y, col, strength);
									MeshProcessor.SetPixelBilinear(texture, x, y - 1, col, strength);
									MeshProcessor.SetPixelBilinear(texture, x, y + 1, col, strength);
								}
							}
						}
					}
					//Draw Preview
					else
					{
						col = Color.blue;
						MeshProcessor.SetPixelBilinear(texture, uv, col);

						if (drawDot)
						{
							x = uv.x * texture.width;
							y = uv.y * texture.height;

							MeshProcessor.SetPixelBilinear(texture, x - 1, y, col);
							MeshProcessor.SetPixelBilinear(texture, x + 1, y, col);
							MeshProcessor.SetPixelBilinear(texture, x, y - 1, col);
							MeshProcessor.SetPixelBilinear(texture, x, y + 1, col);
						}

						col = Color.Lerp(Color.red, Color.yellow, 0.34f);
						if (tris != null)
						{
							for (int i = 0; i < tris.Count; i++)
							{
								var myuv = MeshProcessor.GetUV(tris[i].Position);
								MeshProcessor.SetPixelBilinear(texture, myuv, col);

								if (drawDot)
								{
									x = myuv.x * texture.width;
									y = myuv.y * texture.height;

									MeshProcessor.SetPixelBilinear(texture, x - 1, y, col);
									MeshProcessor.SetPixelBilinear(texture, x + 1, y, col);
									MeshProcessor.SetPixelBilinear(texture, x, y - 1, col);
									MeshProcessor.SetPixelBilinear(texture, x, y + 1, col);
								}
							}
						}
					}
				}
				texture.Apply();
			}
		}

		void OnGUI()
		{
			//CheckNewMesh();
			bool updateDrawing = update;
			bool updateTexture = update;
			if (selectedObject == null)
			{
				GUI.color = Color.gray;
				EditorGUILayout.HelpBox("Select the object...", MessageType.Warning);
				return;
			}
			if (SurfaceModel == null || currentMeshFilter == null || currentMeshFilter.sharedMesh == null || currentMeshRenderer == null)
			{
				GUI.color = Color.gray;
				EditorGUILayout.HelpBox("Object has no mesh data", MessageType.Warning);
				return;
			}

			GUILayout.Label(selectedObject.name + " Selected");

			GUILayout.BeginHorizontal();
			GUILayout.Label("Mesh Vertecies: " + SurfaceModel.UnityMesh.vertexCount.ToString());
			GUILayout.Label("Vertecies: " + SurfaceModel.VertexCount.ToString());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Triangles: " + SurfaceModel.TriangleCount.ToString());
			GUILayout.Label("Edges: " + SurfaceModel.EdgeCount.ToString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Edge Boarders: " + SurfaceModel.BoarderEdges.Count.ToString());
			GUILayout.Label("Normal Edge Boarders: " + SurfaceModel.NormalBoarderEdges.Count.ToString());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("UV1 Edge Boarders: " + SurfaceModel.UV1BoarderEdges.Count.ToString());
			GUILayout.Label("UV2 Edge Boarders: " + SurfaceModel.UV2BoarderEdges.Count.ToString());
			GUILayout.EndHorizontal();

			GUILayout.Label("Lightmap Index: " + (currentMeshRenderer.lightmapIndex >= 0 ? currentMeshRenderer.lightmapIndex.ToString() : "None"));

			//Draw UV Set
			bool tmp, drawOnModelChanged;
			GUILayout.BeginHorizontal();
			tmp = GUILayout.Toggle(drawTextureOnModel, "Edit Texture");
			if (tmp)
			{
				if (GUILayout.Button("Blur Seams"))
					MeshProcessor.InterpolateUVSeams();
			}
			drawOnModelChanged = tmp != drawTextureOnModel;
			drawTextureOnModel = tmp;

			tmp = GUILayout.Toggle(PaintOnTexture, "Paint On Texture");
			if (PaintOnTexture != tmp && tmp)
				MeshProcessor.UpdateProcessingTexture();
			PaintOnTexture = tmp;

			if (GUILayout.Toggle(false, "Save texture"))
			{
				MeshProcessor.SaveCurrentTexture();
				MeshProcessor.RestoreModelMaterial();
			}

			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			drawUV = EditorGUILayout.Toggle("Draw UV", drawUV);
			tmp = EditorGUILayout.Toggle("Draw SplitUV1", drawSplitUV1);
			if (!updateDrawing && tmp != drawSplitUV1) updateDrawing = true; drawSplitUV1 = tmp;
			tmp = EditorGUILayout.Toggle("Draw SplitUV2", drawSplitUV2);
			if (!updateDrawing && tmp != drawSplitUV2) updateDrawing = true; drawSplitUV2 = tmp;
			GUILayout.EndHorizontal();
			int oldUV = GUILayout.SelectionGrid(drawUVSet, SelectUVStrings, SelectUVStrings.Length);

			updateTexture = drawOnModelChanged || (oldUV > 0) && (oldUV != drawUVSet);
			drawUVSet = oldUV;
			if (updateDrawing) DrawLines();

			if (drawOnModelChanged || updateTexture)
				currentTexture = GetModelTexture(drawUVSet);
			else if (drawTextureOnModel)
				currentTexture = MeshProcessor.processingTexture;

			if (drawOnModelChanged)
			{
				if (drawTextureOnModel)
				{
					MeshProcessor.ShowSeamTexture(drawUVSet, currentTexture, drawUVSet == 2);
				}
				else
					MeshProcessor.RestoreModelMaterial();
			}

			if (oldUV > 0)
				DrawUV();
		}

		Texture2D GetModelTexture(int id)
		{
			Texture2D tex;
			if (id == 1)
			{
				var mat = currentMeshRenderer.sharedMaterial;
				return mat.GetTexture("_MainTex") as Texture2D;
			}
			else if (currentMeshRenderer.lightmapIndex >= 0)
			{
				if (LightmapSettings.lightmaps.Length > currentMeshRenderer.lightmapIndex)
				{
					var data = LightmapSettings.lightmaps[currentMeshRenderer.lightmapIndex];
					var path = AssetDatabase.GetAssetPath(data.lightmapColor);
					var lightmap = data.lightmapColor;
					Debug.Log("Load file: " + path);

					var exrFile = EXRFile.FromFile(path);
					var part = exrFile.Parts[0];
					// open part while reading pixel data in parallel
					part.OpenParallel(path);
					var floats = part.GetFloats(ChannelConfiguration.RGB, true, GammaEncoding.Linear, true);

					tex = new Texture2D(lightmap.width, lightmap.height, TextureFormat.RGBAFloat, false);
					var fulllength = lightmap.width * lightmap.height;
					Color[] cols = new Color[fulllength];
					for (int y = 0; y < lightmap.height; y++)
					{
						for (int x = 0; x < lightmap.width; x++)
						{
							int pid = y * lightmap.width + x;
							cols[id] = new Color(floats[pid], floats[pid + fulllength], floats[pid + fulllength * 2], floats[pid + fulllength * 3]);
						}
					}
					tex.SetPixels(cols);
					//tex.LoadImage(bytes);

					part.Close();
					Debug.Log("File Loaded");
					return tex;
				}
			}
			return null;
		}

		void DrawUV()
		{
			float top = 170;
			int pad = 5;

			float width = position.width;
			float height = position.height;

			float sw = width - 2 * pad;
			float sh = height - 2 * pad - top;
			float screenRatio = sw / sh;
			float texRatio = 1;

			if (currentTexture != null)
				texRatio = (float)currentTexture.height / (float)currentTexture.width;
			float ratio = screenRatio * texRatio;
			if (ratio <= 1) { sh = sw * texRatio; }
			else { sw = sh / texRatio; }

			var m = Matrix4x4.TRS(new Vector2(pad, top + sh), Quaternion.identity, new Vector3(sw, -sh, 1));

			Color line = Color.blue;// new Color(0.4f, 0.4f, 0.4f, 1);
			Color border = Color.green;
			Handles.BeginGUI();
			Handles.color = line;

			Vector2 tbase = new Vector2(0, 0);
			Vector2 tsize = new Vector2(1, 1);
			if (drawUVSet == 2)
			{
				//We have to offset the Lightmap
				var offset = currentMeshRenderer.lightmapScaleOffset;
				tbase = new Vector2(-offset.z / offset.x, -offset.w / offset.y);
				tsize = new Vector2(1f / offset.x, 1f / offset.y);
			}
			Vector3 A = m.MultiplyPoint(tbase);
			Vector3 B = m.MultiplyPoint(tbase + tsize) - A;
			//if (currentTexture != null)
			//    GUI.DrawTexture(new Rect(A.x, A.y + B.y, B.x, -B.y), currentTexture);

			if (drawUV)
			{
				Handles.color = Color.gray;
				Handles.DrawLine(m.MultiplyPoint(new Vector2(0, 0)), m.MultiplyPoint(new Vector2(1, 0)));
				Handles.DrawLine(m.MultiplyPoint(new Vector2(1, 0)), m.MultiplyPoint(new Vector2(1, 1)));
				Handles.DrawLine(m.MultiplyPoint(new Vector2(1, 1)), m.MultiplyPoint(new Vector2(0, 1)));
				Handles.DrawLine(m.MultiplyPoint(new Vector2(0, 1)), m.MultiplyPoint(new Vector2(0, 0)));

				List<SurfaceModel.Edge<SurfaceModel.Vertex2>> edges = drawUVSet == 1 ? SurfaceModel.UV1Edges : SurfaceModel.UV2Edges;
				foreach (var edge in edges)
				{
					Handles.color = edge.IsBoundary ? border : line;
					Handles.DrawLine(m.MultiplyPoint(edge.A.Coords), m.MultiplyPoint(edge.B.Coords));
				}
			}
			Handles.EndGUI();
		}

		void CheckNewMesh()
		{
			update = selectedObject != Selection.activeGameObject;
			selectedObject = Selection.activeGameObject;
			bool updateDrawing = update;

			if (selectedObject == null)
				return;

			currentMeshFilter = selectedObject.GetComponent<MeshFilter>();
			currentMeshRenderer = selectedObject.GetComponent<MeshRenderer>();

			if (currentMeshFilter == null || currentMeshRenderer == null)
				update = false;

			if (update)
				UpdateMeshData(currentMeshFilter);
		}
	}
}