using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{
	public partial class Player {
		public class DeadState : IPlayerState {

			/// <summary>
			/// プレイヤー自身への参照
			/// </summary>
			private Player m_player;

			public DeadState(Player arg_player) {
				m_player = arg_player;
			}

			void IPlayerState.OnEnter() {
				m_player.AnimatorComponent.Play("Dead.Die");
				m_player.AnimatorComponent.SetBool("Dead" , true);
			}

			void IPlayerState.OnUpdate() {

			}

			void IPlayerState.OnExit() {
				m_player.AnimatorComponent.SetBool("Dead" , false);
				m_player.m_animator.SetBool("OnGround" , true);

			}
			
			IPlayerState IPlayerState.GetNextState() {
				if (!m_player.m_dead) return new IdleState(m_player);
				return null;
			}

		}

		///// <summary>
		///// プレイヤーを死亡させる
		///// </summary>
		//public void Dead() {
		//	AudioManager.Instance.QuickPlaySE("SE_Player_Dead_02");
		//	m_dead = true;
		//	AppManager.Instance.user.m_temp.m_isTouchUI = false;
		//	AppManager.Instance.user.m_temp.m_cnt_death += 1;
		//}

		/// <summary>
		/// プレイヤーの死亡状態を解除する
		/// </summary>
		public void Revive() {
			m_dead = false;
			//AppManager.Instance.user.m_temp.m_isTouchUI = true;
		}
	}
}