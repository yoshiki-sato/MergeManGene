using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{

	public interface IArmCallBackReceiver {

		/// <summary>
		/// アームを伸ばし始めたときに実行される
		/// </summary>
		void OnStartLengthen(Arm arg_arm);

		/// <summary>
		/// アームが縮み終わったときに実行される
		/// </summary>
		void OnEndShorten(Arm arg_arm);
	}

	public class Arm : MonoBehaviour //,IHandCallBackReceiver
    {

		//---------------------------------------------
			[Header("Unity Component")]
		//---------------------------------------------

		/// <summary>直線描画用コンポーネント</summary>
		[SerializeField]
		private LineRenderer m_lineRenderer;

		[SerializeField]
		private BoxCollider2D m_collider;

		//---------------------------------------------
			[Header("Parts")]
		//---------------------------------------------

		/// <summary>アームの先端となるGameObject</summary>
		[SerializeField]
		private GameObject m_top;

		/// <summary>アームの根本となるGameObject</summary>
		[SerializeField]
		private GameObject m_bottom;
		
		//---------------------------------------------
			[Header("Status")]
		//---------------------------------------------

		/// <summary>伸縮速度</summary>
		[SerializeField]
		private float m_reachSpeed = 0.5f;

		/// <summary>射程距離</summary>
		[SerializeField]
		private float m_range = 10;

		/// <summary>
		/// ジャンプに影響させる腕の長さの割合
		/// 腕の長さ * JUMP_POWER_RATE = 押し出される距離
		/// </summary>
		[SerializeField , Range(0 , 1)]
		private float m_jumpPowerRate = 0.5f;

		/// <summary>この座標まで腕を伸ばす</summary>
		private Vector2 m_targetPosition;

		/// <summary>腕の根元から目標までの単位ベクトル</summary>
		private Vector2 m_targetDirection;

		/// <summary>現在の腕の長さ</summary>
		private float m_currentLength;

		/// <summary>実行する処理</summary>
		private System.Action m_currentTask;

		/// <summary>アイテムを掴んだか</summary>
		private bool m_itemGrasped;

		/// <summary>描画時の重なり度</summary>
		[SerializeField,Space(10),Tooltip("描画時の深度")]
		private float m_depth;
		
		/// <summary>コールバックを受け取る対象</summary>
		private readonly List<IArmCallBackReceiver> m_callBackReceivers = new List<IArmCallBackReceiver>();
		
		/// <summary>
		/// 射程距離
		/// </summary>
		public float Range {
			get {
				return m_range;
			}
		}

		/// <summary>
		/// 現在の長さ
		/// </summary>
		public float CurrentLength {
			get {
				return m_currentLength;
			}
		}
		
		/// <summary>
		/// 先端の座標
		/// </summary>
		public Vector2 TopPosition {
			get {
				return m_top.transform.position;
			}
			set {
				m_top.transform.position = value;
			}
		}

		/// <summary>
		/// 根元の座標
		/// </summary>
		public Vector2 BottomPosition {
			get {
				return m_bottom.transform.position;
			}
			set {
				transform.parent.position = value - (Vector2)transform.localPosition;
				m_bottom.transform.position = value;
			}
		}
		
		/// <summary>
		/// この座標まで腕を伸ばすことになる
		/// </summary>
		private Vector2 TargetPosition {
			get {
				return m_targetPosition;
			}
			set {
				m_targetPosition = value;
				m_targetDirection = (value - (Vector2)transform.position).normalized;
			}
		}

		/// <summary>
		/// OnEnable by Unity
		/// 不具合防止のため
		/// 有効化時に初期化を行う
		/// </summary>
		private void OnEnable() {
			TopPosition = transform.position;
			BottomPosition = transform.position;

			m_currentLength = 0;

			m_currentTask = null;
		}

		/// <summary>
		/// Update処理
		/// </summary>
		private void Update() {

			if (m_currentTask != null) {
				m_currentTask();
			}

			#region 直線描画

			if (m_currentLength <= 0) {
				//長さ０でも描画されてしまうのでコンポーネントを無効化させる必要がある
				m_lineRenderer.enabled = false;
			}
			else {
				//先端から根本にかけて直線描画
				m_lineRenderer.enabled = true;
				m_lineRenderer.SetPosition(0 , new Vector3(BottomPosition.x , BottomPosition.y , -m_depth));
				m_lineRenderer.SetPosition(1 , new Vector3(TopPosition.x , TopPosition.y , -m_depth));
			}

			#endregion

			#region 角度反映

			float x0 = TopPosition.x;
			float y0 = TopPosition.y;
			float x1 = BottomPosition.x;
			float y1 = BottomPosition.y;

			//Atan2でラジアン値を取得
			float theta = Mathf.Atan2(y1 - y0 , x1 - x0);

			//度数値に修正
			float angle = theta * Mathf.Rad2Deg;

			//角度を反映
			m_top.transform.eulerAngles = new Vector3(0 , 0 , angle + 90f);

			#endregion

			#region コライダーの反映
			m_collider.size = new Vector2(
				Mathf.Sqrt(Mathf.Pow((TopPosition.x - BottomPosition.x) , 2) + Mathf.Pow((TopPosition.y - BottomPosition.y) , 2)) , 0.1f);

			m_collider.transform.eulerAngles = new Vector3(0 , 0 ,
				Mathf.Atan2(TopPosition.y - BottomPosition.y , TopPosition.x - BottomPosition.x) * Mathf.Rad2Deg);
			m_collider.gameObject.transform.localPosition = new Vector3(
				(TopPosition.x - BottomPosition.x) / 2 ,
				(TopPosition.y - BottomPosition.y) / 2 , 0);
			#endregion
		}

		/// <summary>
		/// 指定された方向へ射程距離分、腕を伸ばす
		/// </summary>
		/// <param name="arg_direction">腕を伸ばす方向</param>
		public void ReachOut(Vector2 arg_direction) {
			this.ReachOutFor(BottomPosition + arg_direction.normalized * Range);
		}

		/// <summary>
		/// 指定された座標へ腕を伸ばす
		/// この場合、射程距離を無視して指定座標まで手を伸ばし続けます
		/// </summary>
		/// <param name="arg_targetPosition">指定された座標まで手を伸ばす</param>
		public void ReachOutFor(Vector2 arg_targetPosition) {

			//使用中の場合は実行しない
			if (this.IsUsing()) return;

			this.gameObject.SetActive(true);

			TargetPosition = arg_targetPosition;
			m_currentTask = this.LengthenTop;
			foreach (IArmCallBackReceiver receiver in m_callBackReceivers) {
				receiver.OnStartLengthen(this);
			}
		}

		/// <summary>
		/// 腕の先端を伸ばす
		/// </summary>
		private void LengthenTop() {
			
			m_currentLength += m_reachSpeed;
			
			float targetLength = (TargetPosition - BottomPosition).magnitude;

			if (m_currentLength >= targetLength) {
				//伸ばしきった
				TopPosition = m_targetPosition;
				m_currentLength = targetLength;
				m_currentTask = null;
				StartCoroutine(OnLengthenFinished());
			}

			TopPosition = BottomPosition + m_targetDirection * m_currentLength;
		}

		/// <summary>
		/// 腕の根元を伸ばす
		/// </summary>
		private void LengthenBottom() {

			//BottomPositionにより、親の座標も動いてしまうので調整させる
			Vector2 topPos = TopPosition;

			float targetLength = (TargetPosition - TopPosition).magnitude;

			m_currentLength += m_reachSpeed;

			#region めり込み防止
			RaycastHit2D hitInfo = Physics2D.BoxCast(transform.position ,
				Vector2.one * 0.5f , 0 , m_targetDirection , m_reachSpeed ,
				1 << LayerMask.NameToLayer("Ground"));

			if (hitInfo) {
				//衝突した
				targetLength = m_currentLength = (BottomPosition - TopPosition).magnitude;
				TargetPosition = BottomPosition;
			}
			#endregion

			if (m_currentLength >= targetLength) {
				//伸ばしきった
				BottomPosition = TargetPosition;

				#region 縮めるために必要
				TargetPosition = TopPosition = topPos;
				m_currentLength = targetLength;
				m_currentTask = ShortenTop;
				#endregion
				return;
			}

			BottomPosition = TopPosition + m_targetDirection * m_currentLength;
			//BottomPositionにより、親の座標も動いてしまうので調整
			TopPosition = topPos;
		}

		/// <summary>
		/// 腕を伸ばし終わったあとの処理
		/// </summary>
		/// <returns></returns>
		private IEnumerator OnLengthenFinished() {

			//当たり判定のコールバックを受け取るために一度待機させる
			yield return new WaitForEndOfFrame();
			
			//コールバックを受け取らなかったら強制的に腕を縮める
			if(!m_itemGrasped) {
				m_currentTask = this.ShortenTop;
			}

		}

		/// <summary>
		/// 腕の先端を縮める
		/// </summary>
		private void ShortenTop() {

			m_currentLength -= m_reachSpeed;

			if (m_currentLength <= 0) {
				m_currentLength = 0;
				TopPosition = BottomPosition;
				m_currentTask = null;
				foreach (IArmCallBackReceiver receiver in m_callBackReceivers) {
					receiver.OnEndShorten(this);
				}
				this.gameObject.SetActive(false);
				return;
			}
			
			TopPosition = BottomPosition + m_targetDirection * m_currentLength;
			
		}

		/// <summary>
		/// 腕の根元を縮める
		/// </summary>
		private void ShortenBottom() {

			m_currentLength -= m_reachSpeed;

			if (m_currentLength <= 0) {
				m_currentLength = 0;
				BottomPosition = TargetPosition;
				m_currentTask = null;
				foreach (IArmCallBackReceiver receiver in m_callBackReceivers) {
					receiver.OnEndShorten(this);
				}
				this.gameObject.SetActive(false);
				return;
			}
			
			BottomPosition = TargetPosition - m_targetDirection * m_currentLength;
			//BottomPositionにより、親の座標も動いてしまうので調整
			TopPosition = TargetPosition;
		}

		/// <summary>
		/// アームを静止させる
		/// </summary>
		private void Rest() {
			return;
		}

		/// <summary>
		/// アームが使用状態か調べる
		/// </summary>
		/// <returns></returns>
		public bool IsUsing() {
			return m_currentTask != null;
		}

		/// <summary>
		/// コールバックを受け取る対象を追加する
		/// </summary>
		/// <param name="arg_callBackReceiver"></param>
		public void AddCallBackReceiver(IArmCallBackReceiver arg_callBackReceiver) {
			m_callBackReceivers.Add(arg_callBackReceiver);
		}

		///// <summary>
		///// ハンドが壁に衝突したときに実行される
		///// </summary>
		///// <param name="arg_hand"></param>
		//void IHandCallBackReceiver.OnCollided(Hand arg_hand) {

		//	m_currentTask = this.ShortenTop;

		//	//ジャンプ
		//	Vector2 direction = (TopPosition - (Vector2)transform.position).normalized;
		//	if (Vector2.Angle(Vector2.down , direction) <= 45) {
		//		TargetPosition = BottomPosition - m_targetDirection * ((Range - m_currentLength) * m_jumpPowerRate);
		//		this.m_currentTask = LengthenBottom;
		//	}
		//}

		///// <summary>
		///// ハンドがものをつかんだ時に実行される
		///// </summary>
		///// <param name="arg_hand"></param>
		//void IHandCallBackReceiver.OnGrasped(Hand arg_hand) {
		//	m_itemGrasped = true;

		//	TargetPosition = arg_hand.GraspingItem.transform.position;
		//	TopPosition = TargetPosition;

		//	switch (arg_hand.GraspingItem.Reaction) {
		//		case Item.GraspedReaction.PULL_TO_ITEM:		
		//			m_currentTask = this.ShortenBottom;	
		//			break;

		//		case Item.GraspedReaction.REST_ARM:
		//			m_currentTask = this.Rest;
		//			break;

		//		case Item.GraspedReaction.PULL_TO_PLAYER:
		//			m_currentTask = this.ShortenTop;
		//			AudioManager.Instance.PlaySE("extend",true);
		//			break;
		//	}
		//}

		///// <summary>
		///// ハンドがものを放したときに実行される
		///// </summary>
		///// <param name="arg_hand"></param>
		//void IHandCallBackReceiver.OnReleased(Hand arg_hand) {

		//	m_itemGrasped = false;

		//	switch (arg_hand.GraspingItem.Reaction) {
		//		case Item.GraspedReaction.PULL_TO_ITEM:
		//			m_currentTask = null;
		//			break;

		//		case Item.GraspedReaction.REST_ARM:
		//			m_currentTask = this.ShortenTop;
		//			AudioManager.Instance.PlaySE("extend",true);
		//			break;

		//		case Item.GraspedReaction.PULL_TO_PLAYER:
		//			m_currentTask = null;
		//			break;
		//	}
		//}
	}
}