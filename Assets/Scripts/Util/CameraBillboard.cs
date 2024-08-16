using UnityEngine;

namespace GMTK_Jam.Util
{
	public class CameraBillboard : MonoBehaviour
	{
		public GameObject TargetOverride;
		public bool DisableAfterRotation;
		public Vector3 ValidAxis = new Vector3(0, 1, 1);
		private bool _disabled;
		private Transform _objectToTrack { get { return TargetOverride == null ? Camera.main.transform : TargetOverride.transform; } }

		public void FacePlayer()
		{
			Quaternion rotationAsQuat = Quaternion.LookRotation(transform.position - _objectToTrack.position);
			Vector3 rotationAsVector = rotationAsQuat.eulerAngles;
			rotationAsVector = new Vector3(rotationAsVector.x * ValidAxis.x, rotationAsVector.y * ValidAxis.y, rotationAsVector.z * ValidAxis.z);
			transform.rotation = Quaternion.Euler(rotationAsVector);

			//transform.rotation = Quaternion.LookRotation(transform.position - _objectToTrack.position);
		}

		public void Update()
		{
			if (_disabled)
				return;

			Quaternion rot = Quaternion.LookRotation(transform.position - _objectToTrack.position);
			transform.rotation = new Quaternion(rot.x * ValidAxis.x, rot.y * ValidAxis.y, rot.z * ValidAxis.z, rot.w);

			if (DisableAfterRotation)
				_disabled = true;
		}
	}
}