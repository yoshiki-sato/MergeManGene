
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendHook : MonoBehaviour
{

    enum HogeState { def, extend, shrink }
    HogeState m_hogeState;

    LineRenderer m_linerendrer;
    private Vector3 m_extendPositon;

    public GameObject target;

    //当たったゲームオブジェクト用
    private GameObject colGameObject;

    /// <summary>タッチ開始位置 </summary>
    private Vector3 touchStartPos;
    /// <summary>タッチ終了位置 </summary>
    private Vector3 touchEndPos;

    /// <summary>当たった場所 </summary>
    private Vector3 hitPosition;

    private float AngleRotation;

    /// <summary>向いている方向ベクトル </summary>
    private Vector3 angleVec;

    public TouchZone m_touchZone;

    [SerializeField]
    private float m_extendSpeed;



    void Start()
    {
        m_linerendrer = GetComponent<LineRenderer>();
        m_hogeState = HogeState.def;
    }

    void Update()
    {
        m_extendPositon = target.transform.position;

        m_linerendrer.SetPosition(0, transform.position);
        m_linerendrer.SetPosition(1, m_extendPositon);

        if (m_hogeState == HogeState.def)
            Def();
        if (m_hogeState == HogeState.extend)
            Extend();
        if (m_hogeState == HogeState.shrink)
            Shrink();

        Debug.Log("フリック処理" + m_touchZone.flicking);
    }


    //インプット処理初期化
    void Init()
    {
        m_touchZone.flicking = false;
        m_touchZone.longPressing = false;
        m_touchZone.releasing = false;
    }

    void Def()
    {
        transform.position = target.transform.position;

        touchStartPos = Camera.main.ScreenToWorldPoint(m_touchZone.m_touchStartPositon);


        //長押しとフリック検知で射出
        if (m_touchZone.releasing)
            if (m_touchZone.longPressing && m_touchZone.flicking)
            {
                m_hogeState = HogeState.extend;
            }
            else { Init(); }
    }

    //フック伸縮
    void Extend()
    {

        touchEndPos = Camera.main.ScreenToWorldPoint(m_touchZone.m_touchEndPositon);

        //角度指定
        float zRotation = Mathf.Atan2(touchEndPos.y - touchStartPos.y,
        touchEndPos.x - touchStartPos.x) * Mathf.Rad2Deg;

        //値格納
        AngleRotation = zRotation;

        //取得した角度反映
        transform.rotation = Quaternion.Euler(0f, 0f, AngleRotation);

        //移動量
        angleVec = transform.rotation * new Vector3(m_extendSpeed, 0f, 0);

        //向いている方向に向かって移動
        transform.position += angleVec * Time.deltaTime;
    }

    //フック収縮
    void Shrink()
    {

        switch (colGameObject.tag)
        {

            //壁にあたったらそのまま移動
            case "Wall":
                target.transform.position = Vector3.MoveTowards(target.transform.position, hitPosition, 0.5f);
                break;

            //地形は移動しない
            case "Ground":
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.5f);
                break;
            default:
                break;
        }

        //射出先に到達したら初期化
        if (transform.position == target.transform.position)
        {
            Init();
            m_hogeState = HogeState.def;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        hitPosition = transform.position;

        if (col.gameObject.tag == "Wall")
        {
            colGameObject = col.gameObject;
            m_hogeState = HogeState.shrink;
        }

        if (col.gameObject.tag == "Ground")
        {
            colGameObject = col.gameObject;
            m_hogeState = HogeState.shrink;
        }
    }

}
