using UnityEngine;
using System.Collections;
using System;

public class Player : NetworkItem {

	private Vector3 _targetPosition;
	private Quaternion _targetRotation;
	public GameObject head;
	public Color[] colors = new Color[10];
	public Renderer renderer;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	public void Awake() {
		colors [0] = Color.black;
		colors [1] = Color.white;
		colors [2] = Color.red;
		colors [3] = Color.green;
		colors [4] = Color.blue;
		colors [5] = Color.cyan;
		colors [6] = Color.magenta;
		colors [7] = Color.yellow;
		colors [8] = Color.gray;
		colors [9] = Color.grey;
	}

	/// <summary>
	/// Init player / others
	/// </summary>
	public void Start() {
		base.Start ();
		renderer = gameObject.GetComponent<Renderer> ();
		if (_isPlayer == true) {
			Input.gyro.enabled = true;
			head.SetActive (true);
			renderer.material.color = colors[UnityEngine.Random.Range(0,colors.Length-1)];
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
		transform.Translate (head.transform.right * h * Time.deltaTime,Space.Self);
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

			object color;
			if (_properties.TryGetValue("color",out color)) {
				renderer.material.color = JsonUtility.FromJson<Color> (color.ToString());
			} 

		} catch (Exception ex) { }
	}

	/// <summary>
	/// Store in the properties dictionary all values you wish to track
	/// </summary>
	public void SetValues() {
		_properties ["position"] = JsonUtility.ToJson (transform.position);
		_properties ["rotation"] = JsonUtility.ToJson (transform.rotation);
		_properties ["color"] = JsonUtility.ToJson (renderer.material.color);
	}
		
}
