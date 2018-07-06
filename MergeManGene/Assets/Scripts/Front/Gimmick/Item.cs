using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MargeManGene {
	public abstract class Item : MonoBehaviour {

		public enum GraspedReaction {
			PULL_TO_ITEM,
			REST_ARM,
			PULL_TO_PLAYER
		}

		public abstract GraspedReaction Reaction { get; }

		public enum State {
			GRABBED,
			CARRIED
		}

		public State m_currentState;

		private bool m_flg_ableReleace = false;
		private bool m_flg_ableGrasp = true;

		/// <summary>
		/// 掴まれたときの処理
		/// 最初の一度だけ呼ばせること
		/// </summary>
		/// <param name="arg_player"></param>
		public virtual void OnGraspedEnter(Player arg_player) {
			
		}

		/// <summary>
		/// 掴まれているときの処理
		/// ここでは掴まれている間常に呼ばれる
		/// </summary>
		/// <param name="arg_player"></param>
		public virtual void OnGraspedStay(Player arg_player) {
		
		}

		/// <summary>
		/// 掴まれていた状態から離された時の処理
		/// ここでは一度だけ呼ばせること
		/// </summary>
		/// <param name="arg_player"></param>
		public virtual void OnGraspedExit(Player arg_player) {
			
		}

		/// <summary>
		/// このオブジェクトからプレーヤーは手を放してよい状態かを返します。
		/// </summary>
		public bool IsAbleRelease() {
			return m_flg_ableReleace;
		}

		public bool IsAbleGrasp() {
			return m_flg_ableGrasp;
		}

		/// <summary>
		/// このオブジェクトにプレーヤーは干渉可能かどうかを返します。
		/// </summary>
		public void SetAbleRelease(bool release){
			m_flg_ableReleace = release;
		}

		public void SetAbleGrasp(bool grasp){
			m_flg_ableGrasp = grasp;
		}

		
		
	}
}