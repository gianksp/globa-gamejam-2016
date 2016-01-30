using UnityEngine;
using System.Collections;

public class LevelScript : MonoBehaviour {

	public GameObject CubeA;

	// Use this for initialization
	void Start () {
		CreateBox(CubeA, new Vector3(-40,0,0));
		CreateBox (CubeA, new Vector3(40,0,0));
		CreateBox (CubeA, new Vector3(0,-40,0));
		CreateBox (CubeA, new Vector3(0,40,0));
	}

	void CreateBox(GameObject prefab, Vector3 pos)
	{
		var c = (GameObject)Instantiate (prefab, pos, Quaternion.identity);
		c.transform.SetParent (gameObject.transform);
		c.transform.localPosition = pos;
		c.transform.localRotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
