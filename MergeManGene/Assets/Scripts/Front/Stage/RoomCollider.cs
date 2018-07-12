using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MargeManGene {
	public class RoomCollider : MonoBehaviour {

		[HideInInspector]
		public uint m_roomId;

		[HideInInspector]
		public uint m_prevRoomId;

		//コールバック
		private System.Action<uint> CallBack;

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="arg_action">通知用関数インスタンス</param>
		public void Initialize(System.Action<uint> arg_callBack) {
			CallBack = arg_callBack;
		}

		/// <summary>
		/// Stageへ通知を送る
		/// </summary>
		/// <param name="arg_col"></param>
		void OnTriggerEnter2D(Collider2D arg_col) {
			if (arg_col.gameObject.layer == LayerMask.NameToLayer("Player")) {
				CallBack(m_prevRoomId);
			}
		}

	}

}