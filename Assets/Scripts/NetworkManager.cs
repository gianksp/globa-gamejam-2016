using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkManager : MonoBehaviour {

	public static IFirebase firebase;			 //Firebase instance
	public static string identifier;			 //This network item id
	public GameObject prefab;					 //Main player prefab

	public readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>(); 	//Actions to be performed in main thread
	public static IDictionary <string,object> items;

	/// <summary>
	/// Set firebase
	/// </summary>
	void Awake() {
		firebase = Firebase.CreateNew ("https://godcube.firebaseio.com/");
	}

	/// <summary>
	/// Initialise this player
	/// </summary>
	void Start () {
		
		//Initialise this object in map
		Vector3 initialPosition = Vector3.zero;
		firebase.UnAuth ();
		firebase.AuthAnonymously ((AuthData auth) => {
			InitFirebasePlayer(auth.Uid,initialPosition);
		}, (FirebaseError e) => {
			Debug.Log ("auth failure!! "+e);
		});
			
		//Listeners
		firebase.ChildAdded   += (object sender, ChangedEventArgs e) => { ExecuteOnMainThread.Enqueue(() => { StartCoroutine(Add(e.DataSnapshot));    }); };
		firebase.ChildRemoved += (object sender, ChangedEventArgs e) => { ExecuteOnMainThread.Enqueue(() => { StartCoroutine(Remove(e.DataSnapshot)); }); };
		firebase.ValueUpdated += (object sender, ChangedEventArgs e) => { ExecuteOnMainThread.Enqueue(() => { StartCoroutine(Modify(e.DataSnapshot)); }); };
	}
	
	/// <summary>
	/// Execute items in main thread
	/// </summary>
	public void Update() {
		// dispatch stuff on main thread
		while (ExecuteOnMainThread.Count > 0) {
			ExecuteOnMainThread.Dequeue().Invoke();
		}
	}

	/// <summary>
	/// Update this firebase instance
	/// </summary>
	/// <param name="snapshot">Snapshot.</param>
	IEnumerator Modify (IDataSnapshot snapshot) {
		try {
			items = snapshot.DictionaryValue;
		} catch (ArgumentNullException) {         }
		yield return null;
	}

	/// <summary>
	/// This instance was created, init it
	/// </summary>
	/// <param name="snapshot">Snapshot.</param>
	IEnumerator Add (IDataSnapshot snapshot) {
		try {
			if (items == null) {
				items = new Dictionary<string,object>();
			}
			//Move players involved
			items [snapshot.Key] = snapshot.DictionaryValue;
			if (snapshot.Key.StartsWith("Player_")) {
				Vector3 initPos = JsonUtility.FromJson<Vector3>(snapshot.DictionaryValue["position"].ToString());
				GameObject player = (GameObject)Instantiate(prefab,initPos,Quaternion.identity);
				player.transform.parent = transform;
				player.GetComponent<NetworkItem>().identifier = snapshot.Key;
			}
		} catch (ArgumentNullException) {         }
		yield return null;
	}


	/// <summary>
	/// This instance died, explode it locally
	/// </summary>
	/// <param name="snapshot">Snapshot.</param>
	IEnumerator Remove (IDataSnapshot snapshot) {
		try {
			items.Remove (snapshot.Key);
		} catch (ArgumentNullException) {         }
		yield return null;
	}

	/// <summary>
	/// Create a Firebase object based on this current player logged in
	/// </summary>
	void InitFirebasePlayer(string id, Vector3 pos) {
		identifier = "Player_"+id;
		IDictionary<string, object> data = new Dictionary<string, object>();
		data.Add ("position", JsonUtility.ToJson(pos));
		data.Add ("rotation", JsonUtility.ToJson(Quaternion.identity));
		firebase.Child(identifier).SetValue(data);
	}
}