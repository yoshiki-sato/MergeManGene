using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{
	public partial class Player {
		public class IdleState : IPlayerState {

			/// <summary>
			/// プレイヤー自身への参照
			/// </summary>
			private Player m_player;

			public IdleState(Player arg_player) {
				m_player = arg_player;
			}

			void IPlayerState.OnEnter() {
				
			}

			void IPlayerState.OnUpdate() {

			}

			void IPlayerState.OnExit() {

			}
			
			IPlayerState IPlayerState.GetNextState() {
				if (m_player.m_dead) return new DeadState(m_player);
				if (m_player.m_reach) return new ReachState(m_player);
				if (m_player.m_leftRun || m_player.m_rightRun) return new RunState(m_player);
				
				return null;
			}

		}
	}
}