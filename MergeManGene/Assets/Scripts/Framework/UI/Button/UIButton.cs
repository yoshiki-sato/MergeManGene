using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MergeManGene
{

	/// <summary>コールバックを実行するタイミング</summary>
	public enum ButtonEventTrigger {
		OnRelease,
		OnPress,
		OnLongPress,
        OnSwipe
	}

	/// <summary>
	/// ボタンのコールバック（通知）機能
	/// </summary>
	public class ButtonAction {
			
		/// <summary>押されたとみなすタイミング</summary>
		public ButtonEventTrigger m_trigger { get; private set; }

		/// <summary>コールバック</summary>
		public System.Action m_action { get; private set; }

		/// <summary>引数ありコールバック</summary>
		public System.Action<object> m_objectAction { get; private set; }

		/// <summary>コールバック時の引数</summary>
		public object m_value;

		/// <summary>
		/// コンストラクタ
		/// コールバックを設定する
		/// </summary>
		/// <param name="arg_trigger">タイミング</param>
		/// <param name="arg_action">コールバック</param>
		public ButtonAction(ButtonEventTrigger arg_trigger , Action arg_action) {
			m_trigger = arg_trigger;
			m_action = arg_action;
		}

		/// <summary>
		/// コンストラクタ
		/// 引数ありコールバックを設定する
		/// </summary>
		/// <param name="arg_trigger">タイミング</param>
		/// <param name="arg_objectAction">コールバック</param>
		/// <param name="arg_value">引数</param>
		public ButtonAction(ButtonEventTrigger arg_trigger , System.Action<object> arg_objectAction , object arg_value) {
			m_trigger = arg_trigger;
			m_objectAction = arg_objectAction;
			m_value = arg_value;
		}
	}

	/// <summary>
	/// ボタンの設定
	/// ボタンが処理をおこなうときの設定がまとまっている
	/// </summary>
	[System.Serializable]
	public class ButtonOption {

		/// <summary>
		/// ボタンが押されたときに自動でスケーリング処理をするか
		/// ここをfalseにすることで独自のアニメーションを実装可能
		/// </summary>
		[SerializeField]
		private bool m_isAutoScale = true;

		[SerializeField,Range(0.0f,1.0f)]
		private float m_scalePower = 0.05f;
		
		/// <summary>
		/// ボタンが押されたときにColorを変えるか
		/// </summary>
		[SerializeField]
		private bool m_isAutoColor = false;

		/// <summary>参照するImageコンポーネント</summary>
		[SerializeField]
		private Image m_image;

		/// <summary>
		/// 自動でスケーリング処理をおこなう
		/// </summary>
		public bool IsAutoScale {
			get { return m_isAutoScale; }
		}

		/// <summary>
		/// AutoScaleモードでスケーリングする量
		/// </summary>
		public float ScalePower {
			get{ return 1.0f - m_scalePower; }
		}

		/// <summary>
		/// 自動でSpriteのカラーリングを行う
		/// </summary>
		public bool IsAutoColor {
			get { return m_isAutoColor; }
		}
	
		/// <summary>
		/// Imageコンポーネント
		/// </summary>
		public Image ImageComponent {
			get { return m_image; }
		}

	}

	/// <summary>
	/// UIに使用されるボタンの機能
	/// </summary>
	[AddComponentMenu("ToyBox/UI/Button")]
	public class UIButton : TouchActor {
		
		/// <summary>コールバック</summary>
		protected readonly List<ButtonAction> m_btnActions = new List<ButtonAction>();

		/// <summary>ボタンの設定</summary>
		[SerializeField]
		protected ButtonOption m_btnOption;

		/// <summary>このボタンが現在押されている状態である</summary>
		protected bool m_isUsing;

		/// <summary>
		/// ボタンの設定
		/// ※読み取り専用
		/// </summary>
		public ButtonOption BtnOption {
			get { return m_btnOption; }
		}

		/// <summary>
		/// このボタンが現在押されている状態であるか
		/// </summary>
		public bool IsUsing {
			get {
				return m_isUsing;
			}
		}
		
		/// <summary>
		/// 初期化
		/// ここでコールバックを設定する
		/// </summary>
		/// <param name="arg_action">コールバック</param>
		public void Initialize(ButtonAction arg_action) {
			if (arg_action == null) {
				Debug.LogWarning("[ToyBox]コールバックがNULL");
			}
			else {
				m_btnActions.Add(arg_action);
			}
		}

		/// <summary>
		/// 初期化
		/// ここでコールバックを設定する
		/// </summary>
		/// <param name="arg_action1">コールバック</param>
		/// <param name="arg_action2">コールバック</param>
		public void Initialize(ButtonAction arg_action1,ButtonAction arg_action2) {
			if (arg_action1 == null) {
				Debug.LogWarning("[ToyBox]コールバックがNULL");
			}
			else {
				m_btnActions.Add(arg_action1);
			}
			
			if (arg_action2 == null) {
				Debug.LogWarning("[ToyBox]コールバックがNULL");
			}
			else {
				m_btnActions.Add(arg_action2);
			}
		}

		/// <summary>
		/// 初期化
		/// ここでコールバックを設定する
		/// </summary>
		/// <param name="arg_action1">コールバック</param>
		/// <param name="arg_action2">コールバック</param>
		/// <param name="arg_action3">コールバック</param>
		public void Initialize(ButtonAction arg_action1 , ButtonAction arg_action2,ButtonAction arg_action3) {
			if (arg_action1 == null) {
				Debug.LogWarning("[ToyBox]コールバックがNULL");
			}
			else {
				m_btnActions.Add(arg_action1);
			}

			if (arg_action2 == null) {
				Debug.LogWarning("[ToyBox]コールバックがNULL");
			}
			else {
				m_btnActions.Add(arg_action2);
			}

			if (arg_action3 == null) {
				Debug.LogWarning("[ToyBox]コールバックがNULL");
			}
			else {
				m_btnActions.Add(arg_action3);
			}
		}

		#region TouchActorからの継承

		/// <summary>
		/// カーソルが押されたときの処理
		/// </summary>
		/// <param name="pos"></param>
		protected override void TouchStart(PointerEventData data) {
			base.TouchStart(data);
			m_isUsing = true;

			this.OnPressed();

			foreach (var action in m_btnActions.FindAll(_ => _.m_trigger == ButtonEventTrigger.OnPress)) {
				ExecCallBack(action);
			}
		}


		/// <summary>
		/// カーソルが押されている間の処理
		/// </summary>
		protected override void TouchStay() {
			base.TouchStay();
			foreach (var action in m_btnActions.FindAll(_ => _.m_trigger == ButtonEventTrigger.OnLongPress)) {
				ExecCallBack(action);
			}
		}

		/// <summary>
		/// カーソルが離された時の処理
		/// </summary>
		protected override void TouchEnd(PointerEventData data) {
			base.TouchEnd(data);
			m_isUsing = false;
			OnReleased();
			foreach (var action in m_btnActions.FindAll(_ => _.m_trigger == ButtonEventTrigger.OnRelease)) {
				ExecCallBack(action);
			}
		}

		/// <summary>
		/// カーソルが離された時の処理
		/// </summary>
		/// <param name="pos"></param>
		protected override void SwipeEnd(PointerEventData data) {
			base.SwipeEnd(data);
			m_isUsing = false;
			OnReleased();
			foreach (var action in m_btnActions.FindAll(_ => _.m_trigger == ButtonEventTrigger.OnRelease)) {
				ExecCallBack(action);
			}
		}

		#endregion

		/// <summary>
		/// カーソルが押されたときの処理
		/// </summary>
		protected virtual void OnPressed() {
			if (m_btnOption.IsAutoScale) {
				iTween.ScaleTo(this.gameObject , 
					new Vector3(m_btnOption.ScalePower , m_btnOption.ScalePower , 1) ,
					0.5f);
			}
			if (m_btnOption.IsAutoColor) {
				m_btnOption.ImageComponent.color = Color.gray;
			}
		}

		/// <summary>
		/// カーソルが押されている間の処理
		/// </summary>
		protected virtual void OnLongPressed() { }

		/// <summary>
		/// カーソルが離された時の処理
		/// </summary>
		protected virtual void OnReleased() {
			if (m_btnOption.IsAutoScale) {
				iTween.ScaleTo(this.gameObject , Vector3.one , 0.5f);
			}
			if (m_btnOption.IsAutoColor) {
				m_btnOption.ImageComponent.color = Color.white;
			}
		}
		
		/// <summary>
		/// ボタンが押されたときの処理
		/// コールバックを実行する
		/// </summary>
		protected void ExecCallBack(ButtonAction arg_action) {

			if (arg_action == null) return;

			System.Action action = arg_action.m_action;

			if (action != null) {
				action();
				return;
			}

			System.Action<object> objectAction = arg_action.m_objectAction;
			object value = arg_action.m_value;

			if (arg_action.m_objectAction != null) {
				arg_action.m_objectAction(arg_action.m_value);
				return;
			}
		}
	}
}