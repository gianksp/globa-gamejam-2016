using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {

	public Button button;

	// Use this for initialization
	void Start () {
		button.onClick.AddListener(() => { StartAgain(); });
	}
	
	// Update is called once per frame
	void StartAgain() {
		Application.LoadLevel ("Main");
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("YESSSS");
		StartAgain();
	}
}
