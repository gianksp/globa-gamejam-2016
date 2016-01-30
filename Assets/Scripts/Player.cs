using UnityEngine;
using System.Collections;
using System;

public class Player : NetworkItem {

	private Vector3 _targetPosition;
	private Quaternion _targetRotation;
	public GameObject head;

	public float hAccel = 0;
	public float vAccel = 0;

	public void Start() {
		base.Start ();
		if (_isPlayer == true) {
			Input.gyro.enabled = true;
			head.SetActive (true);
		}
	}

	/// <summary>
	/// Fixeds the update. Only the player controlling can write to firebase, everyone else must read and interpret
	/// </summary>
	public void FixedUpdate() {

		//Playable
		if (_isPlayer) {
			MoveByGamepad ();
//			Move ();
//			Jump ();

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
		transform.Translate (transform.forward * v * Time.deltaTime);
		transform.Translate (transform.right * h * Time.deltaTime);
	}
	/// <summary>
	/// Based on the values stored in "SetValues" use them to update this gameobject instance
	/// </summary>
	public void InterpretValues() {

		try {
			//Update position
			_targetPosition    = JsonUtility.FromJson<Vector3> (_properties["position"].ToString());
			transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime*5f);

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

	/// <summary>
	/// move the player forward, backward, left and right
	/// </summary>
	protected void Move()
	{

		// NOTE: you could also use 'GetAxis', but that would add smoothing
		// to the input from both Ultimate FPS and from Unity, and might
		// require some tweaking in order not to feel laggy
		float deltaY = Input.gyro.userAcceleration.y;
		if (deltaY >= 0.2 || deltaY <= -0.2) {
			hAccel = 1f;
		} else if (hAccel > 0){
			hAccel = hAccel - 0.05f;
		}

		transform.Translate (transform.forward * hAccel*0.1f * Time.deltaTime);
	}

	/// <summary>
	/// ask controller to jump when button is pressed (the current
	/// controller preset determines jump force).
	/// NOTE: if its 'MotorJumpForceHold' is non-zero, this
	/// also makes the controller accumulate jump force until
	/// button release.
	/// </summary>
	protected void Jump()
	{

		// TIP: to find out what determines if 'Jump.TryStart'
		// succeeds and where it is hooked up, search the project
		// for 'CanStart_Jump
		float deltaY = Input.gyro.userAcceleration.y;
		if (deltaY <= -1.0f) {
			gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * 5f);
		}

	}

	public GUIStyle style;

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 20), hAccel.ToString(),style);
	}

		
}
