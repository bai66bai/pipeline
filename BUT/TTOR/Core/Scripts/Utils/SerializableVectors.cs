using System;
using UnityEngine;

namespace BUT.TTOR.Core.Utils
{
	[Serializable]
	public struct Vector2S
	{
		public float x;
		public float y;

		public Vector2S(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		public Vector2S(Vector2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}
		public Vector2S(Quaternion q)
		{
			this.x = q.x;
			this.y = q.y;
		}


		public override bool Equals(object obj)
		{
			if (!(obj is Vector2S))
			{
				return false;
			}

			var s = (Vector2S)obj;
			return x == s.x &&
				   y == s.y;
		}

		public override int GetHashCode()
		{
			var hashCode = 373119288;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			return hashCode;
		}

		public Vector2 ToVector2()
		{
			return new Vector2(x, y);
		}


		public static bool operator ==(Vector2S a, Vector2S b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2S a, Vector2S b)
		{
			return a.x != b.x && a.y != b.y;
		}

		public static implicit operator Vector2(Vector2S x)
		{
			return new Vector2(x.x, x.y);
		}

		public static implicit operator Vector2S(Vector2 x)
		{
			return new Vector2S(x.x, x.y);
		}
	}


	[Serializable]
	public struct Vector3S
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public Vector3S(float x, float y, float z, float w = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Vector3S(Vector3 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = 0;
		}
		public Vector3S(Quaternion q)
		{
			this.x = q.x;
			this.y = q.y;
			this.z = q.z;
			this.w = q.w;
		}


		public override bool Equals(object obj)
		{
			if (!(obj is Vector3S))
			{
				return false;
			}

			var s = (Vector3S)obj;
			return x == s.x &&
				   y == s.y &&
				   z == s.z &&
				   w == s.w;
		}

		public override int GetHashCode()
		{
			var hashCode = 373119288;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			hashCode = hashCode * -1521134295 + z.GetHashCode();
			return hashCode;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x, y, z);
		}

		public Quaternion ToQuaternion()
		{
			return new Quaternion(x, y, z, w);
		}

		public static bool operator ==(Vector3S a, Vector3S b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Vector3S a, Vector3S b)
		{
			return a.x != b.x && a.y != b.y && a.z != b.z;
		}

		public static implicit operator Vector3(Vector3S x)
		{
			return new Vector3(x.x, x.y, x.z);
		}

		public static implicit operator Vector3S(Vector3 x)
		{
			return new Vector3S(x.x, x.y, x.z);
		}
	}
}
