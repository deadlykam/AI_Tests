using UnityEngine;
using System.Collections;

public class Camera_LookAt : MonoBehaviour {

    public GameObject target;
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target.transform);
	}
}
