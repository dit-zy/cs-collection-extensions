#if !NETSTANDARD2_0
using System.Numerics;

namespace DitzyExtensions {
	public static class MathUtils {
		public static Vector2 V2(float x) => new Vector2(x, x);
		public static Vector2 V2(float x, float y) => new Vector2(x, y);

		public static Vector3 V3(float x) => new Vector3(x, x, x);
		public static Vector3 V3(Vector2 xy, float z) => new Vector3(xy.X, xy.Y, z);
		public static Vector3 V3(float x, float y, float z) => new Vector3(x, y, z);

		public static Vector4 V4(float x) => new Vector4(x, x, x, x);
		public static Vector4 V4(Vector2 xy, float z, float w) => new Vector4(xy.X, xy.Y, z, w);
		public static Vector4 V4(Vector3 xyz, float w) => new Vector4(xyz.X, xyz.Y, xyz.Z, w);
		public static Vector4 V4(float x, float y, float z, float w) => new Vector4(x, y, z, w);

		public static Vector2 Transpose(this Vector2 vec) => new Vector2(vec.Y, vec.X);
	}
}
#endif
