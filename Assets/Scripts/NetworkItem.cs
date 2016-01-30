using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class NetworkItem : MonoBehaviour {

	public string identifier;

	public bool _isPlayer;
	protected IDictionary<string,object> _properties = new Dictionary<string,object> ();	//Data dictionary to store in firebase
	protected IFirebase _playerRef;															//This object firebase reference only in case of player (the only one who writes)

	/// <summary>
	/// Initialise properties. If thiXs is the player, mark it and tag it as player
	/// </summary>
	public void Start() {
		_isPlayer = identifier == NetworkManager.identifier;
		if (_isPlayer && identifier != null) {
			_playerRef = NetworkManager.firebase.Child (identifier);
			gameObject.tag = "Player";
		}
	}
		
	/// <summary>
	/// Fixeds the update. Only the player controlling can write to firebase, everyone else must read and interpret
	/// </summary>
	public void Update() {
		if (_isPlayer && _playerRef != null) {
			_playerRef.SetValue (_properties);
		} else if (identifier != null && NetworkManager.items != null){
			_properties = (Dictionary<string,object>)NetworkManager.items [identifier];
		}
	}

}
