using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

    public TouchZone m_touchzone;
    
    // Update is called once per frame
    void Update(){

        if (m_touchzone.pressing){
            FadeManager.Instance.LoadScene("Stage001",0.5f);
        }
    }
}
