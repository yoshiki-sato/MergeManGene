using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHand : MonoBehaviour {


    /// <summary>開始地点 </summary>
    Vector3 m_startPosition;

    /// <summary>トゲの速度 </summary>
    [SerializeField]
    private float m_fallSpeed;

    /// <summary>消失地点 </summary>
    [SerializeField]
    private float m_lostPoint;

    //回転スピード
    [SerializeField]
    private float m_rotatePower;

    // Use this for initialization
    void Start()
    {
        m_startPosition = transform.position;
    }

    void NeedleMove()
    {
        transform.position += new Vector3(0, -m_fallSpeed, 0);
        transform.Rotate(0, 0, m_rotatePower);
        if (transform.position.y < m_startPosition.y - m_lostPoint)
            Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        NeedleMove();
    }
}
