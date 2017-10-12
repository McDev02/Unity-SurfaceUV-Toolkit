using SurfaceMeshToolkit.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceMeshToolkit
{
    public class SurfaceModel
    {
        public Mesh UnityMesh;
        public Transform Transform;

        public enum MeshData { Surface, Normal, UV1, UV2 }

        public List<Vertex> Vertecies;
        public List<Triangle> Triangles;

        public List<Vertex3> Normals;
        public List<Vertex2> UV1;
        public List<Vertex2> UV2;

        //Lookups
        const int LookupLength = 40;
        public Dictionary<Vector3, Vertex> VertexPositionLookup;
        public LookupTable<Vertex> VerteciesLookup;
        public LookupTable<Triangle> TrianglesLookup;
        public LookupTable<Vertex3> NormalsLookup;
        public LookupTable<Vertex2> UV1Lookup;
        public LookupTable<Vertex2> UV2Lookup;

        public List<EdgeVertex> Edges;
        public List<Edge<Vertex3>> NormalEdges;
        public List<Edge<Vertex2>> UV1Edges;
        public List<Edge<Vertex2>> UV2Edges;

        public List<EdgeVertex> BoarderEdges;
        public List<Edge<Vertex3>> NormalBoarderEdges;
        public List<Edge<Vertex2>> UV1BoarderEdges;
        public List<Edge<Vertex2>> UV2BoarderEdges;

        public List<EdgeVertex> NormalBoarderVertexEdges;
        public List<EdgeVertex> UV1BoarderVertexEdges;
        public List<EdgeVertex> UV2BoarderVertexEdges;

        public bool HasNormals;
        public bool HasUV1;
        public bool HasUV2;

        public int TotalVertexCount { get { return UnityMesh == null ? 0 : UnityMesh.vertexCount; } }
        public int VertexCount { get { return Vertecies == null ? 0 : Vertecies.Count; } }
        public int TriangleCount { get { return Triangles == null ? 0 : Triangles.Count; } }
        public int EdgeCount { get { return Edges == null ? 0 : Edges.Count; } }

        public struct BarycentricData
        {
            public Triangle Triangle;
            public Vector3 Position;

            public BarycentricData(Triangle triangle, Vector3 position)
            {
                Triangle = triangle;
                Position = position;
            }
        }
        public SurfaceModel()
        {
            Triangles = new List<Triangle>();
            Vertecies = new List<Vertex>();
            Normals = new List<Vertex3>();
            Edges = new List<EdgeVertex>();
            UV1 = new List<Vertex2>();
            UV2 = new List<Vertex2>();

            VertexPositionLookup = new Dictionary<Vector3, Vertex>();
            VerteciesLookup = new LookupTable<Vertex>();
            TrianglesLookup = new LookupTable<Triangle>();
            NormalsLookup = new LookupTable<Vertex3>();
            UV1Lookup = new LookupTable<Vertex2>();
            UV2Lookup = new LookupTable<Vertex2>();

            NormalEdges = new List<Edge<Vertex3>>();
            UV1Edges = new List<Edge<Vertex2>>();
            UV2Edges = new List<Edge<Vertex2>>();

            BoarderEdges = new List<EdgeVertex>();
            NormalBoarderEdges = new List<Edge<Vertex3>>();
            UV1BoarderEdges = new List<Edge<Vertex2>>();
            UV2BoarderEdges = new List<Edge<Vertex2>>();

            NormalBoarderVertexEdges = new List<EdgeVertex>();
            UV1BoarderVertexEdges = new List<EdgeVertex>();
            UV2BoarderVertexEdges = new List<EdgeVertex>();
        }

        public void SetMesh(Transform transform, Mesh sharedMesh)
        {
            Transform = transform;
            UnityMesh = sharedMesh;
            UpdateMeshData();
        }

        public void Clear()
        {
            UnityMesh = null;

            Triangles.Clear();
            Vertecies.Clear();
            Normals.Clear();
            Edges.Clear();
            UV1.Clear();
            UV2.Clear();
            NormalEdges.Clear();
            UV1Edges.Clear();
            UV2Edges.Clear();

            VertexPositionLookup.Clear();
            VerteciesLookup.Clear();
            TrianglesLookup.Clear();
            NormalsLookup.Clear();
            UV1Lookup.Clear();
            UV2Lookup.Clear();

            BoarderEdges.Clear();
            NormalBoarderEdges.Clear();
            UV1BoarderEdges.Clear();
            UV2BoarderEdges.Clear();

            NormalBoarderVertexEdges.Clear();
            UV1BoarderVertexEdges.Clear();
            UV2BoarderVertexEdges.Clear();

        }

        private void UpdateMeshData()
        {
            Debug.Log("UpdateMeshData");

            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            var meshTriangles = UnityMesh.triangles;
            var meshVertices = UnityMesh.vertices;
            var meshNormal = UnityMesh.normals;
            var meshUV0 = UnityMesh.uv;
            var meshUV1 = UnityMesh.uv2;

            HasNormals = meshNormal != null && meshNormal.Length > 0;
            HasUV1 = meshUV0 != null && meshUV0.Length > 0;
            HasUV2 = meshUV1 != null && meshUV1.Length > 0;

            //Construct Surface
            int trisID;
            Vertex curVertex;
            Triangle curTriangle = null;
            for (int i = 0; i < meshTriangles.Length; i++)
            {
                if (watch.ElapsedMilliseconds >= 10 * 1000)
                    break;
                int vid = meshTriangles[i];
                trisID = i % 3;
                if (trisID == 0)
                {
                    Triangles.Add(new Triangle(Triangles.Count));
                    curTriangle = Triangles[Triangles.Count - 1];
                }

                int vertexID;
                if (!VertexPositionLookup.TryGetValue(meshVertices[vid], out curVertex))
                {
                    vertexID = Vertecies.Count;
                    curVertex = new Vertex(Vertecies.Count, meshVertices[vid]);
                    Vertecies.Add(curVertex);
                    VertexPositionLookup.Add(curVertex.Position, curVertex);
                }

                int normId;
                int uv1Id;
                int uv2Id;
                if (HasNormals)
                {
                    if (!VertexHasNormal(curVertex, meshNormal[vid], out normId))
                        normId = AddNormal(curVertex, meshNormal[vid]);
                    curTriangle.Normals[trisID] = Normals[normId];
                }
                if (HasUV1)
                {
                    if (!VertexHasUV1(curVertex, meshUV0[vid], out uv1Id))
                        uv1Id = AddUV1(curVertex, meshUV0[vid]);
                    curTriangle.UV1s[trisID] = UV1[uv1Id];
                }
                if (HasUV2)
                {
                    if (!VertexHasUV2(curVertex, meshUV1[vid], out uv2Id))
                        uv2Id = AddUV2(curVertex, meshUV1[vid]);
                    curTriangle.UV2s[trisID] = UV2[uv2Id];
                }
                curTriangle.Vertecies[trisID] = curVertex;
            }

            Debug.Log("Construct Surface: " + watch.ElapsedMilliseconds.ToString() + "ms");
            var elapsed = watch.ElapsedMilliseconds;

            //ConstructEdges
            Triangle triangle;
            for (int i = 0; i < Triangles.Count; i++)
            {
                triangle = Triangles[i];

                for (int e = 0; e < 3; e++)
                {
                    //Search for Surface Edges
                    var va = triangle.Vertecies[e];
                    var vb = triangle.Vertecies[(e + 1) % 3];

                    EdgeVertex newEdge;
                    if (!va.HasEdgeWith(vb, out newEdge))
                    {
                        newEdge = new EdgeVertex(Edges.Count, va, vb);
                        Edges.Add(newEdge);
                    }
                    newEdge.Triangles.Add(triangle);
                    triangle.Edges.Add(newEdge);
                    va.Edges.Add(newEdge);
                    vb.Edges.Add(newEdge);

                    //Search for Normal Edges
                    if (HasNormals)
                    {
                        var na = triangle.Normals[e];
                        var nb = triangle.Normals[(e + 1) % 3];

                        Edge<Vertex3> edgen;
                        if (!va.HasNormalEdgeWith(na, nb, out edgen))
                        {
                            edgen = new Edge<Vertex3>(Edges.Count, na, nb, newEdge);
                            newEdge.NormalEdges.Add(edgen);
                            NormalEdges.Add(edgen);
                        }
                        edgen.Triangles.Add(triangle);
                        triangle.NormalEdges.Add(edgen);
                        va.NormalEdges.Add(edgen);
                        vb.NormalEdges.Add(edgen);
                    }
                    //Search for UV1 Edges
                    if (HasUV1)
                    {
                        var uva = triangle.UV1s[e];
                        var uvb = triangle.UV1s[(e + 1) % 3];

                        Edge<Vertex2> edgeuv;
                        if (!va.HasUV1EdgeWith(uva, uvb, out edgeuv))
                        {
                            edgeuv = new Edge<Vertex2>(Edges.Count, uva, uvb, newEdge);
                            newEdge.UV1Edges.Add(edgeuv);
                            UV1Edges.Add(edgeuv);
                        }

                        edgeuv.Triangles.Add(triangle);
                        triangle.UV1Edges.Add(edgeuv);
                        va.UV1Edges.Add(edgeuv);
                        vb.UV1Edges.Add(edgeuv);
                    }
                    //Search for UV2 Edges
                    if (HasUV2)
                    {
                        var uva = triangle.UV2s[e];
                        var uvb = triangle.UV2s[(e + 1) % 3];

                        Edge<Vertex2> edgeuv;
                        if (!va.HasUV2EdgeWith(uva, uvb, out edgeuv))
                        {
                            edgeuv = new Edge<Vertex2>(Edges.Count, uva, uvb, newEdge);
                            newEdge.UV2Edges.Add(edgeuv);
                            UV2Edges.Add(edgeuv);
                        }

                        edgeuv.Triangles.Add(triangle);
                        triangle.UV2Edges.Add(edgeuv);
                        va.UV2Edges.Add(edgeuv);
                        vb.UV2Edges.Add(edgeuv);
                    }
                }
            }
            Debug.Log("ConstructEdges: " + (watch.ElapsedMilliseconds - elapsed).ToString() + "ms");
            elapsed = watch.ElapsedMilliseconds;

            //Find Boundary Edges
            foreach (var edge in Edges)
            {
                if (edge.Triangles.Count != 1) continue;
                edge.IsBoundary = true;
                BoarderEdges.Add(edge);
            }
            foreach (var edge in NormalEdges)
            {
                if (edge.Triangles.Count != 1) continue;
                edge.IsBoundary = true;
                NormalBoarderEdges.Add(edge);
                NormalBoarderVertexEdges.Add(edge.BaseEdge);
            }
            foreach (var edge in UV1Edges)
            {
                if (edge.Triangles.Count != 1) continue;
                edge.IsBoundary = true;
                UV1BoarderEdges.Add(edge);
                UV1BoarderVertexEdges.Add(edge.BaseEdge);
            }
            foreach (var edge in UV2Edges)
            {
                if (edge.Triangles.Count != 1) continue;
                edge.IsBoundary = true;
                UV2BoarderEdges.Add(edge);
                UV2BoarderVertexEdges.Add(edge.BaseEdge);
            }

            Debug.Log("Find Boundary Edges: " + (watch.ElapsedMilliseconds - elapsed).ToString() + "ms");
            elapsed = watch.ElapsedMilliseconds;

            Debug.Log("Total: " + watch.ElapsedMilliseconds.ToString() + "ms");
            Debug.Log("---------------------------");
        }


        public List<BarycentricData> GetOpposingTriangles(MeshData uvSet, int triangleIndex, Vector2 textureCoord, float bias = 1)
        {
            List<BarycentricData> neighbours = new List<BarycentricData>();
            if (Triangles.Count <= triangleIndex) return null;
            var triangle = Triangles[triangleIndex];

            Vector3 mainbarycentric = Vector3.zero;
            if (uvSet == MeshData.UV1)
                mainbarycentric = SurfaceMath.GetBarycentric(textureCoord, triangle.UV1s[0].Coords, triangle.UV1s[1].Coords, triangle.UV1s[2].Coords);
            if (uvSet == MeshData.UV2)
                mainbarycentric = SurfaceMath.GetBarycentric(textureCoord, triangle.UV2s[0].Coords, triangle.UV2s[1].Coords, triangle.UV2s[2].Coords);

            for (int t = 0; t < triangle.Edges.Count; t++)
            {
                var e = triangle.Edges[t];
                var uvEdge = triangle.UV1Edges[t];
                if (uvSet == MeshData.UV1)
                    uvEdge = triangle.UV1Edges[t];

                if (!uvEdge.IsBoundary)
                    continue;

                var dist = SurfaceMath.LinePointDistance(uvEdge.A.Coords, (uvEdge.B.Coords - uvEdge.A.Coords).normalized, textureCoord);
                if (dist > bias)
                    continue;

                var opTriangle = e.GetOtherTriangle(triangle);

                Vector2 a, b, c;
                int aid, bid, cid;
                aid = bid = cid = -1;
                a = b = c = Vector2.zero;
                List<int> ids = new List<int>() { 0, 1, 2 };
                for (int i = 0; i < 3; i++)
                {
                    if (opTriangle.Vertecies[i] == triangle.Vertecies[0])
                    {
                        aid = i; ids.Remove(i);
                    }
                    if (opTriangle.Vertecies[i] == triangle.Vertecies[1])
                    {
                        bid = i; ids.Remove(i);
                    }
                    if (opTriangle.Vertecies[i] == triangle.Vertecies[2])
                    {
                        cid = i; ids.Remove(i);
                    }
                }
                if (aid < 0)
                    aid = ids[0];
                if (bid < 0)
                    bid = ids[0];
                if (cid < 0)
                    cid = ids[0];

                if (uvSet == MeshData.UV1)
                {
                    a = opTriangle.UV1s[aid].Coords;
                    b = opTriangle.UV1s[bid].Coords;
                    c = opTriangle.UV1s[cid].Coords;
                }
                if (uvSet == MeshData.UV2)
                {
                    a = opTriangle.UV2s[aid].Coords;
                    b = opTriangle.UV2s[bid].Coords;
                    c = opTriangle.UV2s[cid].Coords;
                }

                //if (uvSet == MeshData.UV1)
                //    c = opTriangle.GetOtherVertexUV2(uvEdge);
                //if (uvSet == MeshData.UV2)
                //    c = opTriangle.GetOtherVertexUV2(uvEdge);

                //mainbarycentric = new Vector3(0.333f, 0.333f, 0.333f);
                if (uvSet == MeshData.UV1)
                    neighbours.Add(new BarycentricData(opTriangle,
                    a * mainbarycentric.x +
                    b * mainbarycentric.y +
                    c * mainbarycentric.z));
                if (uvSet == MeshData.UV2)
                    neighbours.Add(new BarycentricData(opTriangle,
                    a * mainbarycentric.x +
                    b * mainbarycentric.y +
                    c * mainbarycentric.z));
            }
            return neighbours;
        }

        int AddNormal(Vertex vertex, Vector3 normal)
        {
            var norm = new Vertex3(Normals.Count, normal, vertex);
            Normals.Add(norm);
            Vertecies[vertex.ID].Normals.Add(norm);
            return norm.ID;
        }
        int AddUV1(Vertex vertex, Vector2 uv)
        {
            var nuv = new Vertex2(UV1.Count, uv, vertex);
            UV1.Add(nuv);
            Vertecies[vertex.ID].UV1s.Add(nuv);
            return nuv.ID;
        }
        int AddUV2(Vertex vertex, Vector2 uv)
        {
            var nuv = new Vertex2(UV2.Count, uv, vertex);
            UV2.Add(nuv);
            Vertecies[vertex.ID].UV2s.Add(nuv);
            return nuv.ID;
        }

        bool DoesVectorMatch(Vector2 a, Vector2 b, float bias)
        {
            return (b - a).sqrMagnitude < (bias * bias);
        }
        bool DoesVectorMatch(Vector3 a, Vector3 b, float bias)
        {
            return (b - a).sqrMagnitude < (bias * bias);
        }

        bool EdgeExists(MeshData data, int vA, int vB, out int i)
        {
            switch (data)
            {
                case MeshData.Surface:
                    foreach (var edge in Edges)
                    {
                        if ((edge.A.ID == vA && edge.B.ID == vB) || (edge.B.ID == vA && edge.A.ID == vB))
                        {
                            i = edge.ID;
                            return true;
                        }
                    }
                    break;
                case MeshData.Normal:
                    foreach (var edge in NormalEdges)
                    {
                        if ((edge.A.ID == vA && edge.B.ID == vB) || (edge.B.ID == vA && edge.A.ID == vB))
                        {
                            i = edge.ID;
                            return true;
                        }
                    }
                    break;
                case MeshData.UV1:
                    foreach (var edge in UV1Edges)
                    {
                        if ((edge.A.ID == vA && edge.B.ID == vB) || (edge.B.ID == vA && edge.A.ID == vB))
                        {
                            i = edge.ID;
                            return true;
                        }
                    }
                    break;
                case MeshData.UV2:
                    foreach (var edge in UV2Edges)
                    {
                        if ((edge.A.ID == vA && edge.B.ID == vB) || (edge.B.ID == vA && edge.A.ID == vB))
                        {
                            i = edge.ID;
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            i = -1;
            return false;
        }

        bool VertexHasNormal(Vertex vert, Vector3 norm, out int i)
        {
            for (i = 0; i < vert.Normals.Count; i++)
            {
                if (vert.Normals[i].Coords == norm)
                {
                    i = vert.Normals[i].ID;
                    return true;
                }

            }
            return false;
        }
        bool VertexHasUV1(Vertex vert, Vector2 uv, out int i)
        {
            for (i = 0; i < vert.UV1s.Count; i++)
            {
                if (vert.UV1s[i].Coords == uv)
                {
                    i = vert.UV1s[i].ID;
                    return true;
                }
            }
            return false;
        }
        bool VertexHasUV2(Vertex vert, Vector2 uv, out int i)
        {
            for (i = 0; i < vert.UV2s.Count; i++)
            {
                if (vert.UV2s[i].Coords == uv)
                {
                    i = vert.UV2s[i].ID;
                    return true;
                }
            }
            return false;
        }

        bool MeshHasVertex(Vector3 vec, out int i)
        {
            for (i = 0; i < Vertecies.Count; i++)
            {
                if (Vertecies[i].Position == vec)
                    return true;
            }
            return false;
        }

        public class Vertex2
        {
            public int ID;
            public Vector2 Coords;
            public Vertex Vertex;

            public Vertex2(int id, Vector2 coords, Vertex vertex)
            {
                ID = id;
                Coords = coords;
                Vertex = vertex;
            }
        }
        public class Vertex3
        {
            public int ID;
            public Vector3 Coords;
            public Vertex Vertex;

            public Vertex3(int id, Vector3 coords, Vertex vertex)
            {
                ID = id;
                Coords = coords;
                Vertex = vertex;
            }
        }

        public class Vertex
        {
            public int ID;
            public Vector3 Position;
            public List<Vertex3> Normals;
            public List<Vertex2> UV1s;
            public List<Vertex2> UV2s;

            public List<EdgeVertex> Edges;
            public List<Edge<Vertex3>> NormalEdges;
            public List<Edge<Vertex2>> UV1Edges;
            public List<Edge<Vertex2>> UV2Edges;

            public Vertex(int id, Vector3 pos)
            {
                ID = id;
                Position = pos;
                Normals = new List<Vertex3>();
                UV1s = new List<Vertex2>();
                UV2s = new List<Vertex2>();

                Edges = new List<EdgeVertex>();
                NormalEdges = new List<Edge<Vertex3>>();
                UV1Edges = new List<Edge<Vertex2>>();
                UV2Edges = new List<Edge<Vertex2>>();
            }

            internal bool HasEdgeWith(Vertex vb, out EdgeVertex newEdge)
            {
                foreach (var edge in Edges)
                {
                    if (edge.A == this && edge.B == vb || edge.B == this && edge.A == vb)
                    {
                        newEdge = edge; return true;
                    }
                }
                newEdge = null;
                return false;
            }
            internal bool HasNormalEdgeWith(Vertex3 va, Vertex3 vb, out Edge<Vertex3> newEdge)
            {
                foreach (var edge in NormalEdges)
                {
                    if (edge.A == va && edge.B == vb || edge.B == va && edge.A == vb)
                    {
                        newEdge = edge; return true;
                    }
                }
                newEdge = null;
                return false;
            }
            internal bool HasUV1EdgeWith(Vertex2 va, Vertex2 vb, out Edge<Vertex2> newEdge)
            {
                foreach (var edge in UV1Edges)
                {
                    if (edge.A == va && edge.B == vb || edge.B == va && edge.A == vb)
                    {
                        newEdge = edge; return true;
                    }
                }
                newEdge = null;
                return false;
            }
            internal bool HasUV2EdgeWith(Vertex2 va, Vertex2 vb, out Edge<Vertex2> newEdge)
            {
                foreach (var edge in UV2Edges)
                {
                    if (edge.A == va && edge.B == vb || edge.B == va && edge.A == vb)
                    {
                        newEdge = edge; return true;
                    }
                }
                newEdge = null;
                return false;
            }
        }

        public class EdgeVertex : Edge<Vertex>
        {
            public List<Edge<Vertex3>> NormalEdges;
            public List<Edge<Vertex2>> UV1Edges;
            public List<Edge<Vertex2>> UV2Edges;

            public EdgeVertex(int id) : base(id)
            {
                NormalEdges = new List<Edge<Vertex3>>();
                UV1Edges = new List<Edge<Vertex2>>();
                UV2Edges = new List<Edge<Vertex2>>();
            }
            public EdgeVertex(int id, Vertex a, Vertex b) : base(id, a, b)
            {
                NormalEdges = new List<Edge<Vertex3>>();
                UV1Edges = new List<Edge<Vertex2>>();
                UV2Edges = new List<Edge<Vertex2>>();
            }
        }

        public class Edge<T>
        {
            public int ID;
            public T A;
            public T B;

            public bool IsBoundary;
            public List<Triangle> Triangles;

            public EdgeVertex BaseEdge;

            public Edge(int id) { ID = id; }
            public Edge(int id, T a, T b)
            {
                Triangles = new List<Triangle>();
                ID = id;
                A = a;
                B = b;
            }
            public Edge(int id, T a, T b, EdgeVertex baseEdge) : this(id, a, b)
            {
                BaseEdge = baseEdge;
            }

            internal Triangle GetOtherTriangle(Triangle triangle)
            {
                if (Triangles.Count > 1)
                    return Triangles[0] == triangle ? Triangles[1] : Triangles[0];
                else
                    return Triangles[0];
            }
        }
        public class Triangle
        {
            public int ID;
            public Vertex[] Vertecies = new Vertex[3];
            public Vertex3[] Normals = new Vertex3[3];
            public Vertex2[] UV1s = new Vertex2[3];
            public Vertex2[] UV2s = new Vertex2[3];
            public List<EdgeVertex> Edges = new List<EdgeVertex>(3);
            public List<Edge<Vertex3>> NormalEdges = new List<Edge<Vertex3>>(3);
            public List<Edge<Vertex2>> UV1Edges = new List<Edge<Vertex2>>(3);
            public List<Edge<Vertex2>> UV2Edges = new List<Edge<Vertex2>>(3);

            public Triangle(int id)
            {
                ID = id;
            }

            public Vertex GetOtherVertex(Vertex a, Vertex b)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Vertecies[i] != a && Vertecies[i] != b)
                        return Vertecies[i];
                }
                //Shall never happen
                return null;
            }

            internal Vertex2 GetOtherVertexUV1(Edge<Vertex2> uvEdge)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (UV1s[i] != uvEdge.A && UV1s[i] != uvEdge.B)
                        return UV1s[i];
                }
                //Shall never happen
                return null;
            }
            internal Vertex2 GetOtherVertexUV2(Edge<Vertex2> uvEdge)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (UV2s[i] != uvEdge.A && UV2s[i] != uvEdge.B)
                        return UV2s[i];
                }
                //Shall never happen
                return null;
            }
        }
        public class Link2
        {
            public int A;
            public int B;

            public Link2() { }
            public Link2(int a, int b)
            {
                A = a;
                B = b;
            }
        }
    }
}