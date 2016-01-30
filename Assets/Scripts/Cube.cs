using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Cube : NetworkItem {

	private Vector3 _targetPosition;
	private Quaternion _targetRotation;
	private float light;
	private float fog;
//	public Light masterLight;
	public float targetLight;
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
				
				_targetRotation    = JsonUtility.FromJson<Quaternion> (_properties["rotation"].ToString());
				transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime*5f);
				targetLight = float.Parse(_properties["light"].ToString());
				intensity   = light/100f > 1 ? 1 : light/100f;
				light = Mathf.Lerp(light, targetLight, Time.deltaTime*2f);
				RenderSettings.ambientIntensity = intensity;
				RenderSettings.fogDensity       = 0.01f*intensity;
//				masterLight.intensity           = intensity;
			}
		} catch (Exception ex) { }
	}
}
