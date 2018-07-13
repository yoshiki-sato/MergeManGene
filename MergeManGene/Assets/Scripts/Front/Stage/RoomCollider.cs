using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MargeManGene {
	public class RoomCollider : MonoBehaviour {

		[SerializeField]
		private int m_roomId;

        private CameraPosController CPController; 

		private void Start()
        {
            CPController = FindObjectOfType<CameraPosController>();
        }
        
		private void OnTriggerEnter2D(Collider2D arg_col)
        {
			if(arg_col.gameObject.tag == "Player")
            {
                CPController.flg_hoge = true;
                CPController.num_id = m_roomId;
            }

        }

    }

}