//担当：森田　勝
//概要：一時停止などの影響を受けるオブジェクト
//　　　GameObjectにAddComponentすることでも機能する
//参考：http://wordpress.notargs.com/blog/blog/2015/01/31/unity%E6%9C%80%E3%82%82%E3%82%B7%E3%83%B3%E3%83%97%E3%83%AB%E3%81%AA%E3%83%9D%E3%83%BC%E3%82%BA%E5%87%A6%E7%90%86/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MergeManGene
{
	public class Pausable : MonoBehaviour {

		/// <summary>
		/// Rigidbodyを保存しておくクラス
		/// </summary>
		public class RigidbodyBuffer {
			public Vector2 m_velocity;
			public RigidbodyBuffer(Rigidbody2D arg_rigidbody) {
				m_velocity = arg_rigidbody.velocity;
			}
		}

		/// <summary>
		/// 一時停止中か
		/// </summary>
		public bool m_isPausing;

		/// <summary>
		/// 無視するGameObject
		/// </summary>
		public GameObject[] m_ignoreGameObjects;

		/// <summary>
		/// Rigidbodyのポーズ前の速度の配列
		/// </summary>
		RigidbodyBuffer[] m_rigidbodyVelocities;

		/// <summary>
		/// ポーズ中のRigidbodyの配列
		/// </summary>
		Rigidbody2D[] m_pausingRigidbodies;

		/// <summary>
		/// ポーズ中のMonoBehaviourの配列
		/// </summary>
		MonoBehaviour[] m_pausingMonoBehaviours;

		/// <summary>
		/// 中断
		/// </summary>
		public void Pause() {
			if (m_isPausing) { return; }
			// Rigidbodyの停止
			// 子要素から、スリープ中でなく、IgnoreGameObjectsに含まれていないRigidbodyを抽出
			Predicate<Rigidbody2D> rigidbodyPredicate =
				obj => !obj.IsSleeping() &&
					   Array.FindIndex(m_ignoreGameObjects , gameObject => gameObject == obj.gameObject) < 0;
			
			//フィルタリング
			m_pausingRigidbodies = Array.FindAll(transform.GetComponentsInChildren<Rigidbody2D>() , rigidbodyPredicate);
			m_rigidbodyVelocities = new RigidbodyBuffer[m_pausingRigidbodies.Length];

			for (int i = 0; i < m_pausingRigidbodies.Length; i++) {
				// 速度、角速度も保存しておく
				m_rigidbodyVelocities[i] = new RigidbodyBuffer(m_pausingRigidbodies[i]);
				m_pausingRigidbodies[i].Sleep();
			}

			// MonoBehaviourの停止
			// 子要素から、有効かつこのインスタンスでないもの、IgnoreGameObjectsに含まれていないMonoBehaviourを抽出
			Predicate<MonoBehaviour> monoBehaviourPredicate =
				obj => obj.enabled &&
					   obj != this &&
					   Array.FindIndex(m_ignoreGameObjects , gameObject => gameObject == obj.gameObject) < 0;
			m_pausingMonoBehaviours = Array.FindAll(transform.GetComponentsInChildren<MonoBehaviour>() , monoBehaviourPredicate);
			foreach (var monoBehaviour in m_pausingMonoBehaviours) {
				monoBehaviour.enabled = false;
			}

			m_isPausing = true;
		}

		/// <summary>
		/// 再開
		/// </summary>
		public void Resume() {
			if (!m_isPausing) { return; }
			// Rigidbodyの再開
			for (int i = 0; i < m_pausingRigidbodies.Length; i++) {
				m_pausingRigidbodies[i].WakeUp();
				m_pausingRigidbodies[i].velocity = m_rigidbodyVelocities[i].m_velocity;
			}

			// MonoBehaviourの再開
			foreach (var monoBehaviour in m_pausingMonoBehaviours) {
				monoBehaviour.enabled = true;
			}

			m_isPausing = false;
		}
	}
}