using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("へい");
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
