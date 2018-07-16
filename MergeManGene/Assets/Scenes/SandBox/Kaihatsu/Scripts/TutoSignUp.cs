using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoSignUp : MonoBehaviour {

    [SerializeField]
    private Camera m_camera;

    public GameObject m_tutoSprite;
    private GameObject m_sprite;

    private bool m_Displaying;


    public Vector3 m_spritePosition;

    void Start(){
        m_Displaying = true;
    }

    //オブジェクトが触れている間
    void OnTriggerEnter2D()
    {
        Debug.Log("呼ばれた");
        if (m_Displaying){
            m_sprite = Instantiate(m_tutoSprite);
            m_sprite.transform.position = m_spritePosition;
            m_Displaying = false;
        }

    }

    //オブジェクトが触れている間
    void OnTriggerExit2D(){
        if (!m_Displaying){
            Destroy(m_sprite);
            m_Displaying = true;
        }
    }

}
