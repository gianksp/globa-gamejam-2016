using UnityEngine;
using System.Collections;
using System;

public class Player : NetworkItem {

	private Vector3 _targetPosition;
	private Quaternion _targetRotation;
	public GameObject head;

	/// <summary>
	/// Init player / others
	/// </summary>
	public void Start() {
		base.Start ();
		if (_isPlayer == true) {
			Input.gyro.enabled = true;
			head.SetActive (true);
		} else {
			gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		}
	}

	/// <summary>
	/// Fixeds the update. Only the player controlling can write to firebase, everyone else must read and interpret
	/// </summary>
	public void FixedUpdate() {

		//Playable
		if (_isPlayer) {

			//Control motion
			MoveByGamepad ();

			//Auto move for now
//			transform.Translate(head.transform.forward*0.1f);

			//Set properties to propagate
			SetValues ();

		} else {
			//Non playable draw based on properties read
			InterpretValues ();
		}

	}
	public float speed = 10.0F;
	void MoveByGamepad()
	{
		var v = Input.GetAxis ("Vertical")*speed;
		var h = Input.GetAxis ("Horizontal")*speed;
		transform.Translate (head.transform.forward * v * Time.deltaTime,Space.World);
		transform.Translate (head.transform.right * h * Time.deltaTime);
	}
	/// <summary>
	/// Based on the values stored in "SetValues" use them to update this gameobject instance
	/// </summary>
	public void InterpretValues() {

		try {
			//Update position
			_targetPosition    = JsonUtility.FromJson<Vector3> (_properties["position"].ToString());
			if (Vector3.Distance(transform.position,_targetPosition) > 1) {
				transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime*5f);
			}

			_targetRotation    = JsonUtility.FromJson<Quaternion> (_properties["rotation"].ToString());
			transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime*5f);

		} catch (Exception ex) { }
	}

	/// <summary>
	/// Store in the properties dictionary all values you wish to track
	/// </summary>
	public void SetValues() {
		_properties ["position"] = JsonUtility.ToJson (transform.position);
		_properties ["rotation"] = JsonUtility.ToJson (transform.rotation);
	}
		
}
