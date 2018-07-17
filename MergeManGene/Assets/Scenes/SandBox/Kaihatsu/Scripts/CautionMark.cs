using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchScript.Gestures;

public class CautionMark : MonoBehaviour
{

    private GameObject m_rayobj;

    //注意記号
    [SerializeField]
    public GameObject m_tutoSprite;
    private GameObject m_sprite;

    public Vector3 m_tutoSpritePosition;

    Collider2D collition2d;
    Vector2 tapPoint;

    private AudioSource m_audioSource;

    [SerializeField]
    private AudioClip m_audioClip;


    private void Start(){
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_audioClip;
    }
    // 左クリックしたオブジェクトを取得する関数(2D)
    void Ray()
    {
        // 左クリックされた場所のオブジェクトを取得
        if (Input.GetMouseButtonDown(0))
        {
            if (!m_sprite)
            {

                tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                collition2d = Physics2D.OverlapPoint(tapPoint);
                if (collition2d.transform.gameObject.tag == "Exclamation")
                {
                    m_sprite = Instantiate(m_tutoSprite);
                    m_sprite.transform.position = m_tutoSpritePosition;

                    if(!m_audioSource.isPlaying)
                        m_audioSource.Play();
                }
            }

            else
            {
                Destroy(m_sprite);
            }
        }
    }

    void Update()
    {
        Ray();
    }
}
