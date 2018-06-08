using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{
	public partial class Player : MonoBehaviour , IArmCallBackReceiver , IHandCallBackReceiver {

		/// <summary>
		/// 向き
		/// </summary>
		public enum Direction {
			LEFT = -1,
			RIGHT = 1
		}
		
		/// <summary>
		/// プレイヤーの状態
		/// ※FSMによる実装
		/// </summary>
		public interface IPlayerState {

			void OnEnter();

			void OnUpdate();

			void OnExit();

			IPlayerState GetNextState();
		}

		//-----------------------------------------------
			[Header("Unity Component")]
		//-----------------------------------------------

		/// <summary>Rigidbody</summary>
		[SerializeField]
		private Rigidbody2D m_rigidbody;

		/// <summary>SpriteRenderer</summary>
		[SerializeField]
		private SpriteRenderer m_spriteRenderer;

		/// <summary>Animator</summary>
		[SerializeField]
		private Animator m_animator;

		//-----------------------------------------------
			[Header("Class")]
		//-----------------------------------------------
		
		/// <summary>アーム</summary>
		[SerializeField]
		private Arm m_arm;

		/// <summary>ハンド</summary>
		[SerializeField]
		private Hand m_hand;

		//----------------------------------------------
			[Header("Status")]
		//----------------------------------------------

		/// <summary>現在の向き</summary>
		[SerializeField]
		private Direction m_currentDirection;

		/// <summary>移動速度</summary>
		[SerializeField]
		private float m_runSpeed;

		/// <summary>ジャンプ力</summary>
		[SerializeField]
		private float m_jumpPower;

		/// <summary>地面に着地している状態である</summary>
		private bool m_isGrounded;

		/// <summary>現在のプレイヤーの状態</summary>
		private IPlayerState m_currentState;

		/// <summary>左移動フラグ</summary>
		private bool m_leftRun;

		/// <summary>右移動フラグ</summary>
		private bool m_rightRun;

		/// <summary>アーム稼働フラグ</summary>
		private bool m_reach;
		
		/// <summary>死亡フラグ</summary>
		private bool m_dead;

		/// <summary>ジャンプフラグ</summary>
		private bool m_jump;

		/// <summary>ジャンプする方向</summary>
		private Vector2 m_jumpDirection;

		/// <summary>アイテム処理用コルーチン</summary>
		private Coroutine m_itemCoroutine;

		/// <summary>
		/// Animatorコンポーネント
		/// ※読み取り専用
		/// </summary>
		public Animator AnimatorComponent {
			get {
				return m_animator;
			}
		}

		/// <summary>
		/// Rigidbody2Dコンポーネント
		/// ※読み取り専用
		/// </summary>
		public Rigidbody2D RigidbodyComponent {
			get {
				return m_rigidbody;
			}
		}
		
		/// <summary>
		/// アーム
		/// ※読み取り専用
		/// </summary>
		public Arm PlayableArm {
			get {
				return m_arm;
			}
		}

		/// <summary>
		/// ハンド
		/// ※読み取り専用
		/// </summary>
		public Hand PlayableHand {
			get {
				return m_hand;
			}
		}
		
		/// <summary>
		/// Start by Unity
		/// </summary>
		private void Start() {
			m_arm.AddCallBackReceiver(this);
			m_hand.AddCallBackReceiver(this);
			//m_hand.AddCallBackReceiver(m_arm);

			m_currentState = new IdleState(this);
            
		}

		/// <summary>
		/// OnEnable by Unity
		/// 不具合防止のため
		/// 有効化時に初期化を行う
		/// </summary>
		private void OnEnable() {

			//各行動用のフラグ
			m_leftRun = m_rightRun = m_reach = m_jump = false;

			m_arm.gameObject.SetActive(false);
			m_hand.gameObject.SetActive(false);
		}

		/// <summary>
		/// Update by Unity
		/// </summary>
		void Update() {
			IPlayerState nextState = m_currentState.GetNextState();
			if (nextState != null) {
				StateTransition(nextState);
			}
			m_currentState.OnUpdate();

			//向きをスプライトに反映
			m_spriteRenderer.flipX = m_currentDirection == Direction.RIGHT;
		}
		
		/// <summary>
		/// 現在のステート（状態）を取得する
		/// </summary>
		/// <returns>Player.○○の形式</returns>
		public System.Type GetCurrentState() {
			return m_currentState.GetType();
		}

		/// <summary>
		/// 現在の向きを取得する
		/// </summary>
		/// <returns></returns>
		public Direction GetCurrentDirection() {
			return m_currentDirection;
		}

		/// <summary>
		/// 自身のジャンプ力でジャンプする
		/// </summary>
		public void Jump(Vector2 arg_direction) {
			this.Jump(arg_direction , m_jumpPower);
		}

		/// <summary>
		/// 指定されたジャンプ力でジャンプする
		/// </summary>
		/// <param name="arg_jumpPower">ジャンプ力</param>
		public void Jump(Vector2 arg_direction , float arg_jumpPower) {

			arg_direction.Normalize();

			m_animator.SetTrigger("Jump");
			//気持ちよくジャンプさせるため重力加速度をリセットする
			m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x , 0);
			m_rigidbody.AddForce(arg_direction * arg_jumpPower);
			
		}

		/// <summary>
		/// ステートの遷移を行う
		/// </summary>
		/// <param name="arg_nextState">次のステート</param>
		private void StateTransition(IPlayerState arg_nextState) {
			if (m_currentState != null) {
				m_currentState.OnExit();
				m_currentState = null;
			}
			if (arg_nextState != null) {
				m_currentState = arg_nextState;
				m_currentState.OnEnter();
			}
		}

		///// <summary>
		///// 掴んでいるアイテムの処理を実行する
		///// </summary>
		///// <param name="arg_item"></param>
		///// <returns></returns>
		//private IEnumerator OnGraspStay(Item arg_item) {
		//	while (true) {
		//		arg_item.OnGraspedStay(this);
		//		yield return null;
		//	}
		//}

		//---------------------------------------------------
		//　以下、外部からのコールバック
		//　※今後、コールバックなどを追加するときは以下に追加すること
		//---------------------------------------------------

		/// <summary>
		/// 着地時の処理
		/// 地面と接触したときにコールバックとして実行される
		/// </summary>
		private void OnGroundEnter() {

			//ジャンプ中など上方向へ移動していたら着地として認識しない
			if(m_rigidbody.velocity.y > 0)  return;

			m_animator.SetBool("OnGround" , true);
			m_isGrounded = true;
		}

		/// <summary>
		/// 離陸時の処理
		/// 地面と接触がなくなったときにコールバックとして実行される
		/// </summary>
		private void OnGroundExit() {
			m_animator.SetBool("OnGround" , false);
			m_isGrounded = false;
		}

		/// <summary>
		/// アームからのコールバック
		/// アームが腕を伸ばし始めときに実行される
		/// </summary>
		/// <param name="arg_arm"></param>
		void IArmCallBackReceiver.OnStartLengthen(Arm arg_arm) {
			m_hand.gameObject.SetActive(true);
		}

		/// <summary>
		/// アームからのコールバック
		/// アームが腕を縮み終えたときに実行される
		/// </summary>
		/// <param name="arg_arm"></param>
		void IArmCallBackReceiver.OnEndShorten(Arm arg_arm) {
			
			m_hand.gameObject.SetActive(false);
			//if (m_hand.GraspingItem) {
			//	if (m_hand.GraspingItem.Reaction == Item.GraspedReaction.PULL_TO_ITEM) {
			//		AppManager.Instance.m_timeManager.Resume();
			//		return;
			//	}
			//}
			StartCoroutine(AsleepArm());
		}

		/// <summary>
		/// ハンドからのコールバック
		/// ハンドが壁に衝突したときに実行される
		/// </summary>
		/// <param name="arg_hand"></param>
		void IHandCallBackReceiver.OnCollided(Hand arg_hand) {

		}

		/// <summary>
		/// ハンドからのコールバック
		/// ハンドがものをつかんだ時に実行される
		/// </summary>
		/// <param name="arg_hand"></param>
		void IHandCallBackReceiver.OnGrasped(Hand arg_hand) {

			m_velocityBuf = Vector2.zero;

			//arg_hand.GraspingItem.OnGraspedEnter(this);
			//m_itemCoroutine = StartCoroutine(this.OnGraspStay(arg_hand.GraspingItem));

			//switch (arg_hand.GraspingItem.Reaction) {
			//	case Item.GraspedReaction.PULL_TO_ITEM: m_animator.SetBool("Fix" , true);break;
			//	case Item.GraspedReaction.REST_ARM:	break;
			//	case Item.GraspedReaction.PULL_TO_PLAYER: m_animator.SetBool("Carry" , true); break;
			//}
			
		}

		/// <summary>
		/// ハンドからのコールバック
		/// ハンドがものをはなしたときに実行される
		/// </summary>
		/// <param name="arg_hand"></param>
		void IHandCallBackReceiver.OnReleased(Hand arg_hand) {
			//arg_hand.GraspingItem.OnGraspedExit(this);
			//StopCoroutine(m_itemCoroutine);

			//switch (arg_hand.GraspingItem.Reaction) {
			//	case Item.GraspedReaction.PULL_TO_ITEM:
			//		m_animator.SetBool("Fix" , false);
			//		StartCoroutine(AsleepArm());
			//		break;
			//	case Item.GraspedReaction.REST_ARM: break;
			//	case Item.GraspedReaction.PULL_TO_PLAYER: m_animator.SetBool("Carry" , false); break;
			//}

		}
	}
}