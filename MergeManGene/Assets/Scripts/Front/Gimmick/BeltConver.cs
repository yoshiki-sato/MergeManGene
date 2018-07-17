using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToyBox
{
    public class BeltConver : MonoBehaviour
    {
        //オブジェクト検出フラグ
        private bool m_isRidden;

        private Rigidbody2D m_rigidbody2d;

        public float m_moveSpeed=0.05f;

        void OnCollisionEnter2D(Collision2D col)
        {
            //触れたオブジェクトのRigitbody取得
            m_rigidbody2d = col.gameObject.GetComponent<Rigidbody2D>();

            //ベルコンに乗った瞬間true
            m_isRidden = true;
        }

        void OnCollisionExit2D(Collision2D arg_col)
        {
            //離れてfalse
            m_isRidden = false;
        }

        void UpdateByFrame()
        {
            //ベルコンに乗ってる間は移動
            if (m_isRidden == true)
            {
                m_rigidbody2d.transform.position += new Vector3(m_moveSpeed, 0, 0);
            }
        }

        void Update()
        {
            UpdateByFrame();
        }

        void OnDisable()
        {
            m_isRidden = false;
        }
    }
}
