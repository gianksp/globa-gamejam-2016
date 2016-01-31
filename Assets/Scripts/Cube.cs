using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Cube : NetworkItem {
	private Quaternion _previousRotation = Quaternion.identity;
	private Vector3 _targetPosition;
	private Quaternion _targetRotation = Quaternion.identity;
	float _interpolationTime = 0;

	private float light;
	private float fog;
//	public Light masterLight;
	public float _targetLight = 100;
	public float _previousLight = 100;
	float _lightInterpolationTime = 0;
	public float intensity;

	/// <summary>
	/// Fixeds the update. Only the player controlling can write to firebase, everyone else must read and interpret
	/// </summary>
	public void Update() {
		base.Update ();
		//Non playable draw based on properties read
		InterpretValues ();
	}

	/// <summary>
	/// Based on the values stored in "SetValues" use them to update this gameobject instance
	/// </summary>
	public void InterpretValues() {
		try {
			if (_properties != null && _properties.Count > 0) {
				var rot = JsonUtility.FromJson<Quaternion> (_properties["rotation"].ToString());
				if (rot != _targetRotation)
				{
					_targetRotation = rot;
					_previousRotation = transform.rotation;
					_interpolationTime = 0;
				}

				var l = float.Parse(_properties["light"].ToString());
				if (l != _targetLight)
				{
					_previousLight = Mathf.Lerp(_previousLight, _targetLight, _lightInterpolationTime);
					_targetLight = l;
					_lightInterpolationTime = 0;
				}
			}

			_interpolationTime = Mathf.Min(1.0f, _interpolationTime+Time.deltaTime*5f);
			_lightInterpolationTime = Mathf.Min(1.0f, _lightInterpolationTime+Time.deltaTime*5f);


			transform.rotation = Quaternion.Lerp(_previousRotation, _targetRotation, _interpolationTime);
			light = Mathf.Lerp(_previousLight, _targetLight, _lightInterpolationTime);
				
			intensity = (light/100f > 1) ? 1 : light/100f;
			intensity =  Mathf.Max(intensity, 0.5f);
				
				RenderSettings.ambientIntensity = intensity;
				RenderSettings.fogDensity       = 0.01f*intensity;
//				masterLight.intensity           = intensity;

		} catch (Exception ex) { }
	}
}
