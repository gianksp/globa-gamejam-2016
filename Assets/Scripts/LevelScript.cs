using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelScript : MonoBehaviour {


	public GameObject CubeA;
	public GameObject CubeB;

	// Use this for initialization
	void Start () {
	}

	void CreateBox(GameObject prefab, Vector3 pos)
	{
		var c = (GameObject)Instantiate (prefab, pos, Quaternion.identity);
		c.transform.SetParent (gameObject.transform);
		c.transform.localPosition = pos;
		c.transform.localRotation = Quaternion.identity;
	}
	public void PopulateLayers(IDictionary<string,object> lines)
	{
		try{
		foreach (var level in lines) {
			
			var xy = level.Key.Split (new []{ 'x' }, 2);
			if (xy.Length != 2)
				continue;
			int x, y;
			var line = level.Value.ToString ();
			if (int.TryParse (xy[0], out x) && int.TryParse (xy[1], out y)) {
				Debug.Log("x="+x + " and y = "+y);
				int z = 0;
				for (;z<line.Length; ++z)
				{
					var c = line[z];
					if (c != ' ')
					{
							GameObject prefab = null;
							if (c == 'A')
								prefab = CubeA;
							else if (c == 'B')
								prefab = CubeB;
							if (prefab == null)
								prefab = CubeA;
							if (prefab != null)
								CreateBox (prefab, new Vector3(-45+x*10,-45+y*10,-45+z*10));
					}
				}
			}
		}

		} catch (Exception ex) {
			Debug.Log (ex);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
