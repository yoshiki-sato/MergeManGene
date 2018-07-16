using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class JumpStand : MonoBehaviour
    {
        [Range(12,24)]
        public float m_jumpPower;

        private GameObject m_colObject;   //衝突したゲームオブジェクト格納用

        //プレイヤがジャンプ台に乗ったとき
        void OnCollisionEnter2D(Collision2D col)
        {

            m_colObject = col.gameObject;

            ContactPoint2D contact = col.contacts[0];

            //プレイヤが上から台に乗ったとき大ジャンプ
            if (contact.normal.y <= -1)
            {
                HighJump();
            }
        }

        void HighJump()
        {
            //velocityでジャンプ量を制御
            m_colObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, m_jumpPower);
        }
    }
