#if UNITY_EDITOR
#define IS_EDITOR
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TouchInput
/// </summary>
public class Punicon : MonoBehaviour
{
    [SerializeField][Range(0.01f,0.05f)]
    private float m_moveSpeed;

    /// <summary>ドラッグ終了地点 </summary>
    private Vector3 m_dragEndPoint;

    /// <summary>ドラッグ挙動 </summary>
    private bool m_dragflg;

    private Vector3 m_screenMouse;

    private Rigidbody2D m_rigidbody2D;

    private SpriteRenderer m_spriteRenderer;

    private Animator m_animator;

    public AudioClip m_audioClip;

    private AudioSource m_audioSource;

    /// <summary>
    /// 仮想インプット領域
    /// </summary>

    /// <summary>
    /// プレイヤーの移動状態
    /// </summary>
    public enum MoveState { Default, right, left }

    MoveState movestate = 0;
    /// <summary>
    /// シングルトン
    /// </summary>
    private static Punicon m_instance;
    private static Punicon Instance
    {
        get
        {
            if (m_instance == null)
            {
                var obj = new GameObject(typeof(Punicon).Name);
                m_instance = obj.AddComponent<Punicon>();
            }
            return m_instance;
        }
    }

    /// <summary>
    /// タッチ開始時のイベント
    /// </summary>
    public static event System.Action<TouchInfo> Started
    {
        add
        {
            Instance.started += value;
        }
        remove
        {
            Instance.started -= value;
        }
    }

    /// <summary>
    /// タッチ中のイベント
    /// </summary>
    public static event System.Action<TouchInfo> Moved
    {
        add
        {
            Instance.moved += value;
        }
        remove
        {
            Instance.moved -= value;
        }
    }

    /// <summary>
    /// タッチ終了時のイベント
    /// </summary>
    public static event System.Action<TouchInfo> Ended
    {
        add { Instance.ended += value; }
        remove { Instance.ended -= value; }
    }

    private TouchInfo m_info = new TouchInfo();
    private event System.Action<TouchInfo> started;
    private event System.Action<TouchInfo> moved;
    private event System.Action<TouchInfo> ended;

    /// <summary>
    /// 現在のタッチ状態
    /// </summary>
    private TouchState State
    {
        get
        {
#if IS_EDITOR
            // エディタの場合の処理
            if (Input.GetMouseButtonDown(0))
            {
                return TouchState.Started;
            }
            else if (Input.GetMouseButton(0))
            {
                return TouchState.Moved;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                return TouchState.Ended;
            }
#else
            // エディタ以外の場合
            if (Input.touchCount > 0) {
                switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    return TouchState.Started;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    return TouchState.Moved;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    return TouchState.Ended;
                default:
                    break;
                }
            }
#endif
            return TouchState.None;
        }
    }


    /// <summary>
    /// タッチされている位置
    /// </summary>
    private Vector2 Position
    {
        get
        {
#if IS_EDITOR
            return State == TouchState.None ? Vector2.zero : (Vector2)Input.mousePosition;
#else
            return Input.GetTouch(0).position;
#endif
        }
    }

    private void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();

        m_audioSource = gameObject.GetComponent<AudioSource>();
        m_audioSource.clip = m_audioClip;

    }

    private void Update(){

        if (State == TouchState.Started){
            
            m_info.screenPoint = Position;
            m_info.deltaScreenPoint = Vector2.zero;
            if (started != null){ started(m_info); }

            m_dragflg = true;
            // Debug.Log("Start");
            m_screenMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //移動処理
        else if (State == TouchState.Moved){
            m_info.deltaScreenPoint = Position - m_info.screenPoint;
            m_info.screenPoint = Position;

            //ＳＥが鳴ってなければならす
            if (!m_audioSource.isPlaying){
                PlaySE();
            }

            m_animator.Play("Run");

            if (moved != null){ moved(m_info); }

            //タッチ方向で移動向き変更
            if (m_info.deltaScreenPoint.x > 0) { movestate = MoveState.right; }
            if (m_info.deltaScreenPoint.x < 0) { movestate = MoveState.left; }
        }
        else if (State == TouchState.Ended){

            m_animator.Play("Idle");

            //音ストップ
            StopSE();

            //終了地点を渡す
            m_dragEndPoint = m_info.deltaScreenPoint;

            m_info.deltaScreenPoint = Position - m_info.screenPoint;
            m_info.screenPoint = Position;
            if (ended != null){ended(m_info);}
            m_dragflg = false;
            //Debug.Log("end");
            movestate = MoveState.Default;
        }
        else  {
            m_info.deltaScreenPoint = Vector2.zero;
            m_info.screenPoint = Vector2.zero;
            // Debug.Log("another");
        }

        //プレイヤー移動処理
        PlayerMove();
    }

    void PlayerMove()
    {      
        if (movestate == MoveState.right){
            m_spriteRenderer.flipX = false; 
            transform.position += new Vector3(m_moveSpeed, 0,0);
        }
        if (movestate == MoveState.left) {
            m_spriteRenderer.flipX = true;
            transform.position += new Vector3(-m_moveSpeed, 0, 0);
        }
    }

    //音鳴らす
    void PlaySE(){
        m_audioSource.Play();
    }

    //音を止める
    void StopSE(){
        m_audioSource.Stop();
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "BlackHand"){
            if (col.gameObject.GetComponent<SpriteRenderer>().flipX)
            {
                transform.position += new Vector3(1f, 0, 0);
                //m_rigidbody2D.AddForce(new Vector2(-2f, 2f), ForceMode2D.Impulse);
            }
            else
            {
                transform.position += new Vector3(-1f, 0, 0);
            }
        }
    }
}
    


    /// <summary>
    /// タッチ情報
    /// </summary>
    public class TouchInfo
    {
        /// <summary>
        /// タッチされたスクリーン座標
        /// </summary>
        public Vector2 screenPoint;
        /// <summary>
        /// 1フレーム前にタッチされたスクリーン座標との差分
        /// </summary>
        public Vector2 deltaScreenPoint;
        /// <summary>
        /// タッチされたビューポート座標
        /// </summary>
        public Vector2 ViewportPoint
        {
            get
            {
                viewportPoint.x = screenPoint.x / Screen.width;
                viewportPoint.y = screenPoint.y / Screen.height;
                return viewportPoint;
            }
        }
        /// <summary>
        /// 1フレーム前にタッチされたビューポート座標との差分
        /// </summary>
        public Vector2 DeltaViewportPoint
        {
            get
            {
                deltaViewportPoint.x = deltaScreenPoint.x / Screen.width;
                deltaViewportPoint.y = deltaScreenPoint.y / Screen.height;
                return deltaViewportPoint;
            }
        }

        private Vector2 viewportPoint = Vector2.zero;
        private Vector2 deltaViewportPoint = Vector2.zero;
    }

    /// <summary>
    /// タッチ状態
    /// </summary>
    public enum TouchState
    {
        /// <summary>
        /// タッチなし
        /// </summary>
        None = 0,
        /// <summary>
        /// タッチ開始
        /// </summary>
        Started = 1,
        /// <summary>
        /// タッチ中
        /// </summary>
        Moved = 2,
        /// <summary>
        /// タッチ終了
        /// </summary>
        Ended = 3,
    }

