using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackEnemy : MonoBehaviour {

    /// <summary>動く床の状態 </summary
    enum State { Default, right, left };

    /// <summary>動くスピード </summary>
    [SerializeField]
    private float m_moveSpeed;

    /// <summary>動く敵にかける力 </summary>
    private Vector3 m_moveEnemy;

    /// <summary>最初の場所 </summary>
    private Vector3 m_startPos;

    /// <summary>現在の状態 </summary>
    private State m_state;

    private SpriteRenderer m_spriteRendrer;

    private void Start()
    {
        m_startPos = transform.position;
        m_state = State.Default;
        m_spriteRendrer = GetComponent<SpriteRenderer>();
    }

    public void Move()
    {

        m_moveEnemy = new Vector3(m_moveSpeed, 0, 0);

        switch (m_state)
        {
            case State.Default:
                transform.position += m_moveEnemy;
                break;
            case State.left:
                transform.position -= m_moveEnemy;
                break;
            case State.right:
                transform.position += m_moveEnemy;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.x >= m_startPos.x + 5f)
        {
            m_spriteRendrer.flipX = false;
            m_state = State.left;
        }

        if (transform.position.x <= m_startPos.x - 5f)
        {
            m_spriteRendrer.flipX = true;
            m_state = State.right;
        }

        Move();

    }
}
