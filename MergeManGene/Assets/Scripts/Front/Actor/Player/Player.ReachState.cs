using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{
	public partial class Player {
		public class ReachState : IPlayerState {

			/// <summary>
			/// プレイヤー自身への参照
			/// </summary>
			private Player m_player;

			public ReachState(Player arg_player) {
				m_player = arg_player;
			}

			void IPlayerState.OnEnter() {
				m_player.m_rigidbody.isKinematic = true;
			}

			void IPlayerState.OnUpdate() {
				m_player.m_rigidbody.velocity = Vector2.zero;
			}

			void IPlayerState.OnExit() {
				m_player.m_rigidbody.isKinematic = false;

				if (m_player.m_jump) {
					m_player.Jump(m_player.m_jumpDirection);
					m_player.m_jump = false;
				}

			}
			
			IPlayerState IPlayerState.GetNextState() {
				if (m_player.m_dead) return new DeadState(m_player);
				if (!m_player.m_reach) return new IdleState(m_player);
				return null;
			}

		}

		/// <summary>加速度のバッファ</summary>
		private Vector2 m_velocityBuf;

		/// <summary>
		/// 自身を射出状態に遷移させアームを起動させる
		/// プレイヤーのアニメーションを再生させるためのラッパー関数
		/// </summary>
		public void ReachOut(Vector2 arg_targetDirection) {
			//不整合防止のためすでに射出状態であれば受け付けない
			if (m_reach) return;

			m_reach = true;

			StartCoroutine(AwakeArm(arg_targetDirection));
		}

		/// <summary>
		/// 自身を射出状態に切り替える
		/// アニメーションによる処理の待機をおこなう
		/// </summary>
		/// <param name="arg_targetDirection"></param>
		/// <returns></returns>
		private IEnumerator AwakeArm(Vector2 arg_targetDirection) {
			m_velocityBuf = this.m_rigidbody.velocity;
			m_rigidbody.velocity = Vector2.zero;
			AppManager.Instance.m_timeManager.Pause();

			m_animator.Play("Reach.Open");
			m_animator.SetBool("Reach" , true);
			m_animator.Update(0);

			yield return new Tsubakit.WaitForAnimation(m_animator , 0);
			m_arm.ReachOut(arg_targetDirection);
		}

		/// <summary>
		/// 自身の射出状態を終了する
		/// アニメーションによる処理の待機をおこなう
		/// </summary>
		/// <returns></returns>
		private IEnumerator AsleepArm() {
            
			m_animator.Play("Reach.Close");
			m_animator.SetBool("Reach" , false);

			m_animator.Update(0);

			yield return new Tsubakit.WaitForAnimation(m_animator , 0);

			this.m_rigidbody.velocity = m_velocityBuf;

			//不整合防止のためバッファを空にする
			m_velocityBuf = Vector2.zero;

			m_reach = false;
			AppManager.Instance.m_timeManager.Resume();
		}
	}
}
