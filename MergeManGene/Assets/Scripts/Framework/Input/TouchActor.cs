//概要：EventTriggerからInputを受け取り、イベントとして発行するスクリプトです。
//		継承先でいろいろ動作を定義してください。
//参考：なし
//担当：久野

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MergeManGene
{
	public class TouchActor : MonoBehaviour{

		//自身のEventTrigger取得
		EventTrigger m_evTrigger_;

		//TatchStay用flg
		bool m_flg_touchKeep;


		// Use this for initialization
		protected void Start () {
			//EventTriggerがアタッチされていなければ新規作成
			if (GetComponent<EventTrigger> ()) {
				m_evTrigger_ = GetComponent<EventTrigger> ();
			} else {
				m_evTrigger_ = gameObject.AddComponent<EventTrigger> ();
			}

			//イベント登録：Click時
			EventTrigger.Entry evEntry_press = new EventTrigger.Entry ();
			evEntry_press.eventID = EventTriggerType.PointerDown;
			evEntry_press.callback.AddListener ((data) => {TouchStart((PointerEventData)data);});
			m_evTrigger_.triggers.Add (evEntry_press);

			//イベント登録：Release時
			EventTrigger.Entry evEntry_release = new EventTrigger.Entry ();
			evEntry_release.eventID = EventTriggerType.PointerUp;
			evEntry_release.callback.AddListener ((data) => {
				//Swipeしたかそうでないかで分岐
				if(m_flg_touchKeep){
					TouchEnd ((PointerEventData)data);
				}else{
					SwipeEnd((PointerEventData)data);
				}
			});
			m_evTrigger_.triggers.Add (evEntry_release);

			//イベント登録：Swipe
			EventTrigger.Entry evEntry_swipe = new EventTrigger.Entry ();
			evEntry_swipe.eventID = EventTriggerType.Drag;
			evEntry_swipe.callback.AddListener ((data) => {Swipe((PointerEventData)data);});
			m_evTrigger_.triggers.Add (evEntry_swipe);

		}

		protected void Update(){
			if(m_flg_touchKeep){
				TouchStay ();
			}
		}

        /// <summary>
        /// <para>このオブジェクトがタッチされた時に１度だけ呼ばれる関数です。</para>
        /// <para>継承先でBaseの関数を呼ぶ必要があります。</para>
        /// <para>data:EventSystemからタッチ座標などの情報が格納されます。</para>
        /// </summary>
        protected virtual void TouchStart(PointerEventData data){
			m_flg_touchKeep = true;
		}

        /// <summary>
        /// <para>このオブジェクト上で指が静止している時に継続して呼ばれる関数です。</para>
        /// <para>Swipe後には呼ばれません。</para>
        /// </summary>
        protected virtual void TouchStay(){
		}

        /// <summary>
        /// <para>スワイプ中に継続して呼ばれる関数です。</para>
        /// <para>継承先でBaseの関数を呼ぶ必要があります。    </para>
        /// <para>data:EventSystemからタッチ座標などの情報が格納されます。</para>
        /// </summary>
        protected virtual void Swipe(PointerEventData data){
			m_flg_touchKeep = false;
		}

        /// <summary>
        /// <para>１度も指を動かさずに指を離した際に１度だけ呼ばれる関数です。</para>
        /// /// <para>継承先でBaseの関数を呼ぶ必要があります。</para>
        /// <para>data:EventSystemからタッチ座標などの情報が格納されます。</para>
        /// </summary>
        protected virtual void TouchEnd(PointerEventData data){
            m_flg_touchKeep = false;
        }

        /// <summary>
        /// <para>スワイプ後に指を離した際に１度だけ呼ばれる関数です。</para>
        /// /// <para>継承先でBaseの関数を呼ぶ必要があります。</para>
        /// <para>data:EventSystemからタッチ座標などの情報が格納されます。</para>
        /// </summary>
        protected virtual void SwipeEnd(PointerEventData data){
            m_flg_touchKeep = false;
        }
		
	}

}