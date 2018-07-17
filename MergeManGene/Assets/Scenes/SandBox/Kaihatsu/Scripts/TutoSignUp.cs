using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoSignUp : MonoBehaviour {

    [SerializeField]
    private Camera m_camera;

    //注意記号
    public GameObject m_cautionSprite;
    private GameObject m_sprite;

    private bool m_Displaying;


    public Vector3 m_spritePosition;

    void Start(){
        m_Displaying = true;
    }

    //オブジェクトが触れている間
    void OnTriggerEnter2D(Collider2D arg_col)
    {
        if (arg_col.tag == "Player")
        {
            Debug.Log("呼ばれた");
            if (m_Displaying)
            {
                m_sprite = Instantiate(m_cautionSprite);
                m_sprite.transform.position = m_spritePosition;
                m_Displaying = false;
            }
        }

    }

    //オブジェクトが離れている間
    void OnTriggerExit2D(){
        if (!m_Displaying){
            Destroy(m_sprite);
            m_Displaying = true;
        }
    }

}
