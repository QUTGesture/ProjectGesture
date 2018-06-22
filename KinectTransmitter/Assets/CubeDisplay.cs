using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDisplay : MonoBehaviour {

    public GameObject cubeScreen;

    public bool forward = false;
    public bool backward = false;
    private int cubecounter = 0;

	// Use this for initialization
	void Start () {
		 
	}

    // Update is called once per frame
    void Update() {
        if (forward) {
            StartCoroutine(RotateCube(Vector3.up * 90, 1));
            forward = false;
        }
        /*
        if (backward)
        {
            StartCoroutine(RotateCube(Vector3.down * 90, 1));
            backward = false;
        }
        */
    }

    IEnumerator RotateCube(Vector3 byAngle, float inTime)
    {
        var fromAngle = cubeScreen.transform.rotation;
        var toAngle = Quaternion.Euler(cubeScreen.transform.eulerAngles + byAngle);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            cubeScreen.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
    }
}
