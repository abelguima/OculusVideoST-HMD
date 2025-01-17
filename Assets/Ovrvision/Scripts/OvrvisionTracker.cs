﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This class provides main interface to the Ovrvision Ex
/// </summary>
public class OvrvisionTracker : MonoBehaviour {

	//var
	public int markerID = 0;
	public bool MovieOVRCameraRig = false;
	//define
	private const int MARKERGET_ARG10 = 10;
	private GameObject OVRCameraRigObj = null;
	
	// ------ Function ------

	// Tracker initialization
	public void Start()
	{
		if (GameObject.Find("OVRCameraRig"))
			OVRCameraRigObj = GameObject.Find("OVRCameraRig");

		if (!MovieOVRCameraRig)
		{
			if (GameObject.Find("LeftEyeAnchor"))
				this.transform.parent = GameObject.Find("LeftEyeAnchor").transform;
		}
	}

	// UpdateTracker
	public void UpdateTransform (float[] markerGet, int elementNo) {
		int i = elementNo * MARKERGET_ARG10;
		if (!MovieOVRCameraRig)
		{
			this.transform.localPosition = new Vector3(markerGet[i + 1], markerGet[i + 2], markerGet[i + 3]);
			this.transform.localRotation = new Quaternion(markerGet[i + 4], markerGet[i + 5], markerGet[i + 6], markerGet[i + 7]);
		}
		else
		{
			if (OVRCameraRigObj != null)
			{
				Vector3 pos = new Vector3(markerGet[i + 1], markerGet[i + 2], markerGet[i + 3]);
				Quaternion qat = new Quaternion(markerGet[i + 4], markerGet[i + 5], markerGet[i + 6], markerGet[i + 7]);
				setCameraTrackerPosition(pos, qat, OVRCameraRigObj);
			}
		}
	}

	public void UpdateTransformNone () {
		if (!MovieOVRCameraRig)
		{
			this.transform.localPosition = new Vector3(-10000.0f, -10000.0f, -10000.0f);
		}
		else
		{
			if (OVRCameraRigObj != null)
			{
				OVRCameraRigObj.transform.localPosition = new Vector3(-10000.0f, -10000.0f, -10000.0f);
			}
		}
	}


	// TrackerPosition to CameraPosition
	private void setCameraTrackerPosition(Vector3 p, Quaternion q, GameObject cameraobj)
	{
		// todo, make a function out of this, otherwhise its the same as metaioTracker.cs
		Matrix4x4 rotationMatrix = new Matrix4x4();
		NormalizeQuaternion(ref q);

		rotationMatrix.SetTRS(Vector3.zero,
							  q,
							  new Vector3(1.0f, 1.0f, 1.0f));

		Matrix4x4 translationMatrix = new Matrix4x4();
		translationMatrix.SetTRS(p,
								 new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
								 new Vector3(1.0f, 1.0f, 1.0f));

		//split up rotation and translation
		Matrix4x4 composed = translationMatrix * rotationMatrix;
		//from world to camera so we have to invert the matrix
		Matrix4x4 invertedMatrix = composed.inverse;

		//center the camera in front of goal - z-axis
		cameraobj.transform.position = invertedMatrix.GetColumn(3);
		cameraobj.transform.rotation = QuaternionFromMatrix(invertedMatrix);
	}

	private void NormalizeQuaternion(ref Quaternion q)
	{
		float sum = 0;
		for (int i = 0; i < 4; ++i)
			sum += q[i] * q[i];
		float magnitudeInverse = 1 / Mathf.Sqrt(sum);
		for (int i = 0; i < 4; ++i)
			q[i] *= magnitudeInverse;
	}

	private Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
		q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
		q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
		q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;

		q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
		q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
		q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));

		return q;

	}
}
