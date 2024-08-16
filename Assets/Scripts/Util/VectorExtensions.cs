using System;
using UnityEngine;

namespace GMTK_Jam.Util
{
	public static class VectorExtensions
	{
		public static bool LargerThan(this Vector3 thisVec, Vector3 otherVec, bool x = true, bool y = true, bool z = true)
		{
			return ((!x || (thisVec.x > otherVec.x)) &&
					(!y || (thisVec.y > otherVec.y)) &&
					(!z || (thisVec.z > otherVec.z)));
		}

		public static float ProjectScalar(this Vector3 thisVec, Vector3 vecToProjectOnto)
		{
			return Vector3.Dot(thisVec, vecToProjectOnto) / vecToProjectOnto.magnitude;
			//return thisVec.magnitude*Mathf.Cos(Vector3.Angle(thisVec, vecToProjectOnto));
		}

		public static Vector3 ProjectVector(this Vector3 thisVec, Vector3 vecToProjectOnto)
		{
			return vecToProjectOnto.normalized * thisVec.ProjectScalar(vecToProjectOnto);
		}

		public static Vector3 Clamp(this Vector3 theVector3, Vector3 min, Vector3 max)
		{
			float x = Mathf.Clamp(theVector3.x, min.x, max.x);
			float y = Mathf.Clamp(theVector3.y, min.y, max.y);
			float z = Mathf.Clamp(theVector3.z, min.z, max.z);
			return new Vector3(x, y, z);
		}

		public static bool CloseEnoughToVector(this Vector3 v1, Vector3 v2, float tolerance)
		{
			return (Vector3.Distance(v1, v2) < tolerance);
		}

		public static bool CloseEnoughToVector(this Vector2 v1, Vector2 v2, float tolerance)
		{
			bool closeEnough = true;
			if (Math.Abs(v1.x - v2.x) > tolerance)
			{
				closeEnough = false;
				return closeEnough;
			}
			if (Math.Abs(v1.y - v2.y) > tolerance)
			{
				closeEnough = false;
				return closeEnough;
			}
			return closeEnough;
		}
	}

}