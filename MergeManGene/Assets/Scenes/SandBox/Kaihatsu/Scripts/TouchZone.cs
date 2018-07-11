using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchScript.Gestures;

public class TouchZone : MonoBehaviour {

    /// <summary>タッチ開始座標 </summary>
    [HideInInspector]
    public Vector3 m_pressStartPosition;
    /// <summary>タッチ開始座標 </summary>
    [HideInInspector]
    public Vector3 m_touchStartPositon;

    /// <summary>タッチ終了座標 </summary>
    [HideInInspector]
    public Vector3 m_touchEndPositon;

    /// <summary>入力動作の各フラグ </summary>
    [HideInInspector]
    public bool pressing, flicking, longPressing, releasing;

    //イベント登録
    void OnEnable()
    {
        // 各delegateに登録
        GetComponent<FlickGesture>().Flicked += FlickedHandle;
        GetComponent<TapGesture>().Tapped += tappedHandle;
        GetComponent<LongPressGesture>().LongPressed += LongPressHandle;
        GetComponent<ReleaseGesture>().Released += ReleaseHandle;
        GetComponent<PressGesture>().Pressed += PressedHandle;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        // 登録を解除
        GetComponent<PressGesture>().Pressed += PressedHandle;
        GetComponent<FlickGesture>().Flicked += FlickedHandle;
        GetComponent<TapGesture>().Tapped += tappedHandle;
        GetComponent<ReleaseGesture>().Released += ReleaseHandle;
        GetComponent<LongPressGesture>().LongPressed += LongPressHandle;
    }

    //タップ処理
    void PressedHandle(object sender, System.EventArgs e)
    {
        //処理したい内容
        var send = sender as PressGesture;
        m_pressStartPosition = send.ScreenPosition;
        pressing = true;
    }

    //タップ処理
    void tappedHandle(object sender, System.EventArgs e)
    {
        //処理したい内容
        var send = sender as TapGesture;
    }

    //フリック処理
    void FlickedHandle(object sender, System.EventArgs e)
    {
        var gesture = sender as FlickGesture;
        // ジェスチャが適切かチェック
        if (gesture.State != FlickGesture.GestureState.Recognized)
            return;
        // 処理したい内容
        // gesture.ScreenFlickVectorにフリック方向が入るので
        // if (gesture.ScreenFlickVector.y < 0)としたら下方向へのフリックを検知できる
        //フリックの強さ
        // m_touchEndPositon = gesture.ScreenFlickVector;

        //フリックした座標
        m_touchEndPositon = gesture.ScreenPosition;
        flicking = true;
        Debug.Log("フリック" + gesture.ScreenFlickVector);
    }

    //長押し処理
    void LongPressHandle(object sender, System.EventArgs e)
    {
        //処理したい内容
        var send = sender as LongPressGesture;

        Debug.Log("押してる");

        m_touchStartPositon = send.ScreenPosition;
        longPressing = true;
    }

    //指を離した際の処理
    void ReleaseHandle(object sender, System.EventArgs e)
    {
        var send = sender as ReleaseGesture;
        releasing = true;
        Debug.Log("離した");
    }
}

