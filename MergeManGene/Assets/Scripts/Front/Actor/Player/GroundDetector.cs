using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{
	public class GroundDetector : MonoBehaviour {

		private enum GroundState {
			GROUND,
			AIR
		}

		private GroundState m_currentGroundState = GroundState.AIR;

		[SerializeField]
		private GameObject m_noticeTarget;

		private BoxCollider2D m_collider;

		private void Awake() {
			m_collider = GetComponent<BoxCollider2D>();
		}

		//private void OnTriggerEnter2D(Collider2D arg_collider) {
		//	if(arg_collider.gameObject.layer == LayerMask.NameToLayer("Ground")
		//		|| arg_collider.gameObject.layer == LayerMask.NameToLayer("Landable")) {
		//		m_noticeTarget.SendMessage("OnGroundEnter");
		//	}
		//}

		//private void OnTriggerExit2D(Collider2D arg_collider) {

		//	Debug.Log(arg_collider.gameObject.name);

		//	if(!Physics2D.IsTouchingLayers(m_collider , 1 << LayerMask.NameToLayer("Ground"))) {
		//		if (arg_collider.gameObject.layer == LayerMask.NameToLayer("Ground")
		//			|| arg_collider.gameObject.layer == LayerMask.NameToLayer("Landable")) {
		//			m_noticeTarget.SendMessage("OnGroundExit");
		//		}
		//	}
		//}


		private void FixedUpdate() {

			RaycastHit2D hitInfo = Physics2D.BoxCast(
				transform.position , m_collider.bounds.size ,
				0 , Vector2.zero , 0 , 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Landable"));

			GroundState prevState;

			prevState = (hitInfo) ? GroundState.GROUND : GroundState.AIR;	

			if(m_currentGroundState != prevState) {
				m_currentGroundState = prevState;
				switch (m_currentGroundState) {
					case GroundState.GROUND: m_noticeTarget.SendMessage("OnGroundEnter"); break;
					case GroundState.AIR: m_noticeTarget.SendMessage("OnGroundExit"); break;
				}
			}
			

		}
	}
}