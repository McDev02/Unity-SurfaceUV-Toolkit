using UnityEngine;

namespace SurfaceMeshToolkit.Utility
{
	public static class SurfaceMath
	{
		#region Constants

		public const float e = 2.718281828459045235f;
		public const float GoldenRatio = 0.6180339887f;
		public const double Tolerance = 0.00001;
		public const float sDensity = 10;
		public const float OneByPI = (float)(1.0 / 3.1415926535897932384626433832795);

		#endregion Constants

		#region Random

		public static Vector2 RandomVector2()
		{
			return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}

		public static Vector3 RandomVector3()
		{
			return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}

		public static Vector4 RandomVector4()
		{
			return new Vector4(Random.Range(-1f, 1f), Random.Range(-1f, 1), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}

		public static Color RandomColor()
		{
			return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
		}

		#endregion Random

		#region Vector

		public static Vector2 GetCircleDirection(float t)
		{
			return new Vector2(Mathf.Cos(Mathf.PI * 2 * t), Mathf.Sin(Mathf.PI * 2 * t));
		}

		public static Vector2 GetCircleDirection2(float t)
		{
			return new Vector2(Mathf.Sin(Mathf.PI * 2 * t), Mathf.Cos(Mathf.PI * 2 * t));
		}

		public static Quaternion Vector3ToQuaternion(Vector3 Up)
		{
			Vector3 dir = Random.onUnitSphere;
			Vector3 up = Up.normalized;
			Vector3 right = up + dir;
			Vector3 forward = Vector3.Cross(right.normalized, up).normalized;

			Quaternion q = Quaternion.LookRotation(forward, up);

			return q;
		}


		/// <summary>
		/// Convert 2D to 3D point (xy to x yHeight z)
		/// </summary>
		/// <param name="pos">2D point</param>
		/// <param name="yHeight">y height</param>
		/// <returns>3D point</returns>
		public static Vector3 To3D(this Vector2 pos, float yHeight = 0)
		{
			return new Vector3(pos.x, yHeight, pos.y);
		}

		//public static Vector3 To3D ( this Vector2i pos, float yHeight = 0 )
		//{
		//	return new Vector3 ( pos.X, yHeight, pos.Y );
		//}

		/// <summary>
		/// Convert 3D to 2D point (xyz to xz)
		/// </summary>
		/// <param name="pos">3D point</param>
		/// <returns>2D point (xyz to xz)</returns>
		public static Vector2 To2D(this Vector3 pos)
		{
			return new Vector2(pos.x, pos.z);
		}

		/// <summary>
		/// Performs <see cref="Mathf.Floor"/> per component
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector2 FloorVector(Vector2 vec)
		{
			return new Vector2(Mathf.Floor(vec.x), Mathf.Floor(vec.y));
		}

		/// <summary>
		/// Performs <see cref="Mathf.Floor"/> per component
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector3 FloorVector(Vector3 vec)
		{
			return new Vector3(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z));
		}

		/// <summary>
		/// Scales a vector to a length between 0 and 1
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static float Clamp01(float v)
		{
			return v < 0 ? 0 : (v > 1 ? 1 : v);
		}

		/// <summary>
		/// Scales a vector to a length between 0 and 1
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector2 ClampVector(Vector2 vec)
		{
			float vr = vec.magnitude;
			Vector2 v = vec.normalized;
			return v * Mathf.Clamp01(vr);
		}

		/// <summary>
		/// Scales a vector to a length between 0 and value
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector2 ClampVector(Vector2 vec, float r)
		{
			float vr = vec.magnitude;
			Vector2 v = vec.normalized;
			return v * Mathf.Clamp(vr, 0, r);
		}

		/// <summary>
		/// Scales a vector to a length between 0 and 1
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector3 ClampVector(Vector3 vec)
		{
			float vr = vec.magnitude;
			Vector3 v = vec.normalized;
			return v * Mathf.Clamp01(vr);
		}

		/// <summary>
		/// Scales a vector to a length between 0 and value
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector3 ClampVector(Vector3 vec, float r)
		{
			float vr = vec.magnitude;
			Vector3 v = vec.normalized;
			return v * Mathf.Clamp(vr, 0, r);
		}

		/// <summary>
		/// Scales a vector to a length between 0 and 1
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector4 ClampVector(Vector4 vec)
		{
			float vr = vec.magnitude;
			Vector4 v = vec.normalized;
			return v * Mathf.Clamp01(vr);
		}

		/// <summary>
		/// Scales a vector to a length between 0 and value
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector4 ClampVector(Vector4 vec, float r)
		{
			float vr = vec.magnitude;
			Vector4 v = vec.normalized;
			return v * Mathf.Clamp(vr, 0, r);
		}

		/// <summary>
		/// Returns the per component value modulo(%) by value. E.g. ModVector({1.6,0.8}, 1) -> (0.6,0.8)
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="modulo"></param>
		/// <returns></returns>
		public static Vector2 ModVector(Vector2 vec, float modulo)
		{
			return new Vector2(vec.x % modulo, vec.y % modulo);
		}

		/// <summary>
		/// Returns the per component value modulo(%) by value. E.g. ModVector({1.6,0.8}, 1) -> (0.6,0.8)
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="modulo"></param>
		/// <returns></returns>
		public static Vector3 ModVector(Vector3 vec, float modulo)
		{
			return new Vector3(vec.x % modulo, vec.y % modulo, vec.z % modulo);
		}

		/// <summary>
		/// Returns the per component value modulo(%) by value. E.g. ModVector({1.6,0.8}, 1) -> (0.6,0.8)
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="modulo"></param>
		/// <returns></returns>
		public static Vector4 ModVector(Vector4 vec, float modulo)
		{
			return new Vector4(vec.x % modulo, vec.y % modulo, vec.z % modulo, vec.w % modulo);
		}

		/// <summary>
		/// Performs a per component multiplication
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector2 MulVector(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		/// <summary>
		/// Performs a per component multiplication
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 MulVector(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		/// <summary>
		/// Performs a per component multiplication
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 MulVector(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		/// <summary>
		/// Performs a per component value powered by two.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector2 MulVector(Vector2 a)
		{
			return new Vector2(a.x * a.x, a.y * a.y);
		}

		/// <summary>
		/// Performs a per component value powered by two.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 MulVector(Vector3 a)
		{
			return new Vector3(a.x * a.x, a.y * a.y, a.z * a.z);
		}

		/// <summary>
		/// Performs a per component value powered by two.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 MulVector(Vector4 a)
		{
			return new Vector4(a.x * a.x, a.y * a.y, a.z * a.z, a.w * a.w);
		}

		#endregion Vector		

		#region Rect

		/// <summary>
		/// Clamps the Rect on X and Y to a Quader from 0 to size
		/// </summary>
		/// <param name="size"></param>
		public static void RectClamp(ref Rect rect, float size)
		{
			rect.xMin = Mathf.Clamp(rect.xMin, 0, size);
			rect.yMin = Mathf.Clamp(rect.yMin, 0, size);
			rect.xMax = Mathf.Clamp(rect.xMax, 0, size);
			rect.yMax = Mathf.Clamp(rect.yMax, 0, size);
		}

		public static void RectLimit(ref Rect rect, float width, float height)
		{
			if (rect.xMin <= 0) rect.xMin = 0;
			else if (rect.xMax > width) rect.x -= (rect.xMax - width);
			if (rect.yMin <= 0) rect.yMin = 0;
			else if (rect.yMax > height) rect.y -= (rect.yMax - height);
		}

		public static Vector2 RectLimit(Rect rect, float width, float height)
		{
			Vector2 rectOffset = new Vector2();
			if (rect.xMin <= 0) rectOffset.x = rect.xMin;
			else if (rect.xMax > width) rectOffset.x = (rect.xMax - width);
			if (rect.yMin <= 0) rectOffset.y = rect.yMin;
			else if (rect.yMax > height) rectOffset.y = (rect.yMax - height);
			return -rectOffset;
		}

		public static void RectExtend(ref Rect rect, int size)
		{
			rect = new Rect(rect.xMin - size, rect.yMin - size, rect.width + size, rect.height + size);
		}

		public static void RectExtend(ref Rect rect, float top, float right, float bottom, float left)
		{
			top += rect.yMax;
			right += rect.xMax;
			bottom += rect.yMin;
			left += rect.xMin;
			rect = new Rect(left, top, right - left, top - bottom);
		}

		public static void RectTransform(ref Rect rect, float factor)
		{
			rect = new Rect(rect.xMin * factor, rect.yMin * factor, rect.width * factor, rect.height * factor);
		}

		#endregion

		#region Geometry

		//public static Vector2 LinearRegression(Vector2[] Points)
		//{
		//	float avrX = 0, avrY = 0;
		//	foreach (Vector2 CV in Points)
		//	{
		//		avrX += CV.x;
		//		avrY += CV.y;
		//	}
		//	avrX /= Points.Length; avrY /= Points.Length;
		//
		//	float SSxy = 0, SSxx = 0;
		//	for (int i = 0; i < Points.Length; i++)
		//	{
		//		SSxy += (Points[i].x - avrX) * (Points[i].y - avrY);
		//		SSxx += Mathf.Pow((Points[i].x - avrX), 2);
		//	}
		//
		//	if (SSxx != 0)
		//		bLin = SSxy / SSxx;
		//	else
		//		Debug.LogError("Division by Zero at LinearRegression by SSxx");
		//
		//	aLin = avrY - bLin * avrX;
		//}


		// https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
		/// <summary>
		/// Compute barycentric coordinates (u, v, w) for point p with respect to triangle (a, b, c)
		/// </summary>
		public static Vector3 GetBarycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
			float d00 = Vector3.Dot(v0, v0);
			float d01 = Vector3.Dot(v0, v1);
			float d11 = Vector3.Dot(v1, v1);
			float d20 = Vector3.Dot(v2, v0);
			float d21 = Vector3.Dot(v2, v1);
			float denom = d00 * d11 - d01 * d01;
			float v = (d11 * d20 - d01 * d21) / denom;
			float w = (d00 * d21 - d01 * d20) / denom;
			float u = 1.0f - v - w;

			return new Vector3(u, v, w);
		}
		public static float AngleFull(Vector2 v)
		{
			return 360 * AngleFullFactor(v);
		}

		public static float AngleFullFactor(Vector2 v)
		{
			float angle = 0.5f * OneByPI * Mathf.Acos(Vector2.Dot(Vector2.up, v.normalized));
			return v.x < 0 ? 1 - angle : angle;
		}

		public static float fSmoothSqrt(float t)
		{
			return Mathf.Pow(t, 1 - t);
		}

		/// <summary>
		/// Performs quadratic interpolation.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float fQuadratic(float t)
		{
			return Mathf.Pow(t, 2) * (-2 * t + 3);
		}

		//public static float fQubic(float t)
		//{//Seems to be wrong!
		//	return 0;// Mathf.Pow ( t, 3 ) * (6 * Mathf.Pow ( t, 2 ) * 15 * t + 10);
		//	Debug.Log ("Do not use CivMath.fQubic()!");
		//}
		public static float LinePointDistance(Vector2 A, Vector2 aDir, Vector2 Point)
		{
			Vector2 nDir = new Vector2(aDir.y, aDir.x);
			Vector2 sD;
			LineIntersection(A, aDir, Point, nDir, out sD);
			return (sD - Point).magnitude;
		}

		public static float LineSegmentPointDistance(Vector2 A, Vector2 B, Vector2 Point)
		{
			Vector2 nDir = new Vector2(-(B - A).y, (B - A).x);
			Vector2 sD = Vector2.zero;

			if (LineSegmentIntersection(A, B, Point - nDir * 999, Point + nDir * 999, out sD))
			{
				return (sD - Point).magnitude;
			}
			else
			{
				float dist1 = (A - Point).magnitude;
				float dist2 = (B - Point).magnitude;
				if (dist1 <= dist2)
					sD = A;
				else sD = B;
				return dist1;
			}
		}

		public static float LineSegmentPointDistance(Vector2 A, Vector2 B, Vector2 Point, out Vector2 sD)
		{
			Vector2 nDir = new Vector2(-(B - A).y, (B - A).x);
			sD = Vector2.zero;

			if (LineSegmentIntersection(A, B, Point - nDir * 999, Point + nDir * 999, out sD))
			{
				return (sD - Point).magnitude;
			}
			else
			{
				float dist1 = (A - Point).magnitude;
				float dist2 = (B - Point).magnitude;
				if (dist1 <= dist2)
					sD = A;
				else sD = B;
				return dist1;
			}
		}
		/// <summary>
		/// May be broken! Returns the distance from a point to a rect and outputs closest point. Caution, this method is also valid for points inside the rect and would then calculate the distance to the outline.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="Point"></param>
		/// <param name="sD">Closest point on the edge of the box</param>
		/// <returns></returns>
		public static float BoxSegmentPointDistance(Rect rect, Vector2 Point, out Vector2 sD)
		{
			Debug.LogError("This method might be broken. Validate before using!");
			float dist = 9999999;
			float minDist = dist + 1;

			float sign = 1;

			Vector2 A = new Vector2(rect.xMin, rect.yMin);
			Vector2 B = new Vector2(rect.xMax, rect.yMin);
			Vector2 C = new Vector2(rect.xMax, rect.yMax);
			Vector2 D = new Vector2(rect.xMin, rect.yMax);

			Vector2 hit;
			sD = new Vector2();

			//Line A
			dist = LineSegmentPointDistance(A, B, Point, out hit);
			if (dist < minDist)
			{
				sD = hit;
				minDist = dist;
				sign = GetSign(A, B, Point);
			}

			//Line B
			dist = LineSegmentPointDistance(B, C, Point, out hit);
			if (dist < minDist)
			{
				sD = hit;
				minDist = dist;
				sign = GetSign(B, C, Point);
			}

			//Line C
			dist = LineSegmentPointDistance(C, D, Point, out hit);
			if (dist < minDist)
			{
				sD = hit;
				minDist = dist;
				sign = GetSign(C, D, Point);
			}

			//Line D
			dist = LineSegmentPointDistance(D, A, Point, out hit);
			if (dist < minDist)
			{
				sD = hit;
				minDist = dist;
				sign = GetSign(D, A, Point);
			}

			return sign * minDist;
		}

		public static float GetSign(Vector2 A, Vector2 B, Vector2 Point)
		{
			Vector2 tan = (B - A).normalized;
			Vector2 norm = new Vector2(tan.y, -tan.x);
			return (Vector2.Dot(norm, (Point - Vector2.Lerp(A, B, 0.5f)).normalized)) < 0 ? -1 : 1;
		}

		public static bool LineIntersection(Vector2 A, Vector2 aDir, Vector2 B, Vector2 bDir, out Vector2 S)
		{
			//Todo: Hacked to work. Replace with more efficient formular
			S = Vector2.zero;
			Vector2 A1 = A - aDir;
			Vector2 B1 = B - bDir;
			Vector2 A2 = A + aDir;
			Vector2 B2 = B + bDir;

			float s1_x, s1_y, s2_x, s2_y;
			s1_x = A2.x - A1.x;
			s1_y = A2.y - A1.y;
			s2_x = B2.x - B1.x;
			s2_y = B2.y - B1.y;

			float det = (-s2_x * s1_y + s1_x * s2_y);
			if (Mathf.Abs(det) < 0.1f)
				return false;

			//float s = (-s1_y*(A1.x - B1.x) + s1_x*(A1.y - B1.y))/det;
			float t = (s2_x * (A1.y - B1.y) - s2_y * (A1.x - B1.x)) / det;

			// Collision detected

			S = new Vector2(A1.x + (t * s1_x), A1.y + (t * s1_y));
			return true;
		}

		public static bool LineSegmentIntersection(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out Vector2 S)
		{
			S = Vector2.zero;

			float s1_x, s1_y, s2_x, s2_y;
			s1_x = A2.x - A1.x;
			s1_y = A2.y - A1.y;
			s2_x = B2.x - B1.x;
			s2_y = B2.y - B1.y;

			float det = (-s2_x * s1_y + s1_x * s2_y);
			if (Mathf.Abs(det) < 0.1f)
				return false;

			float s, t;
			s = (-s1_y * (A1.x - B1.x) + s1_x * (A1.y - B1.y)) / det;
			t = (s2_x * (A1.y - B1.y) - s2_y * (A1.x - B1.x)) / det;

			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
			{
				// Collision detected
				S = new Vector2(A1.x + (t * s1_x), A1.y + (t * s1_y));
				return true;
			}

			return false; // No collision
		}

		public static float TriangleArea(Vector2 A, Vector2 B, Vector2 C)
		{
			float a = (B - A).magnitude;
			float b = (C - B).magnitude;
			float c = (A - C).magnitude;

			float s = (a + b + c) / 2f;

			float area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
			return area;
		}

		public static float TriangleArea(Vector3 A, Vector3 B, Vector3 C)
		{
			float a = (B - A).magnitude;
			float b = (C - B).magnitude;
			float c = (A - C).magnitude;

			float s = (a + b + c) / 2f;

			return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
		}

		public static float TriangleArea(Vector4 A, Vector4 B, Vector4 C)
		{
			float a = (B - A).magnitude;
			float b = (C - B).magnitude;
			float c = (A - C).magnitude;

			float s = (a + b + c) / 2f;

			return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
		}

		#endregion Geometry
	}
}