//担当：森田　勝
//概要：ToyBox用に値を追加したUnity既存のTimeクラスのラッバークラス
//参考：なし

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MergeManGene
{
	public class TimeManager : MonoBehaviour {

		//固定FPS
		public const int m_fps = 60;

		//アプリを初めて起動したときからの総プレイ時間
		private float m_totalPlayingTime;

		//アプリを起動したときの毎プレイ時間
		private float m_localPlayingTime;

		//ゲーム内の時間環境
		public enum State {
			//プレイ中
			Playing,
			//停止中
			Pause
		}

		private State m_currentState;

		//時間停止の影響を受けるオブジェクト
		private List<Pausable> m_pauseObjects;

		public TimeManager() {
			
		}
		~TimeManager() {
			//TODO:最終プレイ時間を保存
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize() {
			Application.targetFrameRate = m_fps;
			m_currentState = State.Playing;
			StartCoroutine(this.UpdateClock());
		}
		
		/// <summary>
		/// 総プレイ時間
		/// </summary>
		public float TotalPlayingTime {
			get { return m_totalPlayingTime; }
		}

		/// <summary>
		/// アプリ起動時間
		/// </summary>
		public float LocalPlayingtime {
			get { return m_localPlayingTime; }
		}

		/// <summary>
		/// 現在の時間の状態
		/// </summary>
		public State CurrentState {
			get { return m_currentState; }
		}

		/// <summary>
		/// 一時停止を解除する
		/// </summary>
		public void Resume() {
			m_currentState = State.Playing;
			m_pauseObjects = GameObject.FindObjectsOfType<Pausable>().ToList();
			foreach (Pausable pauseObject in m_pauseObjects) {
				pauseObject.Resume();
			}
		}

		/// <summary>
		/// 一時停止をおこなう
		/// </summary>
		public void Pause() {
			m_currentState = State.Pause;
			m_pauseObjects = GameObject.FindObjectsOfType<Pausable>().ToList();
			foreach(Pausable pauseObject in m_pauseObjects) {
				pauseObject.Pause();
			}
		}

		public IEnumerator UpdateClock() {
			while (true) {
				m_localPlayingTime += 1.0f;
				yield return new WaitForSeconds(1.0f);
			}
			yield return null;
		}
	}
}