#if !NETSTANDARD2_0
using System.Numerics;
using static DitzyExtensions.MathUtils;

namespace DitzyExtensions {
	public static class MathExtensions {
		public static Vector2 XY(this Vector3 vec) => V2(vec.X, vec.Y);
		public static Vector2 XY(this Vector4 vec) => V2(vec.X, vec.Y);
		public static Vector3 XYZ(this Vector4 vec) => V3(vec.X, vec.Y, vec.Z);
	}
}
#endif
