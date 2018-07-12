using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToyBox;

namespace MargeManGene
{
    public class CameraPosController : MonoBehaviour
    {

        #region Singleton実装
        private static CameraPosController m_instance;

        public static CameraPosController Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<CameraPosController>();
                }
                return m_instance;
            }
        }

        private CameraPosController() { }
        #endregion

        //ターゲットになるカメラ
        [SerializeField]
        Camera m_obj_camera;
        //カメラが追従するターゲット
        [SerializeField]
        GameObject m_obj_cameraTarget;


        //カメラの各座標への移動に使用するステータ群。
        //配列No.をIDとし、各IDを指定して再生...といった使用をする。
        //なお、ID0番のみ追従モードなる物で使用するので必須だが、
        //pos_targetの値を設定する必要はない。
        [System.Serializable]
        struct CameraStatus
        {
            //カメラ移動に使用するSinカーブ(出発点から到着点までの移動力)
            public AnimationCurve cur_move;
            //何フレーム使用して移動するか
            public int num_complateFrame;
            //実際に向かう座標(GameObjectで指定する事を想定しているので、Transformで指定している)
            public Transform pos_target;
            //こっちで直打ちでもいいよ？
            public Vector2 vec_target;
            //カメラの指定スケール(デフォルトで5)
            public float num_scale;
            //このカメラidを使用中、プレーヤーは操作可能か？
            public bool flg_isPlayerActive;
        }
        [SerializeField]
        CameraStatus[] m_str_cameraStatus = new CameraStatus[1];

        //現在再生するID
        public int num_id = 0;
        //モード。true=追従、false=座標固定
        [SerializeField]
        bool m_flg_homingMode = true;

        //Homing専用。移動が完了したらtrueになる
        bool m_flg_complate = false;

        public bool flg_hoge;
		
        // Use this for initialization
        void Awake()
        {
            
            //カメラが何も入っていなければ、MainCameraを取得する
            if (m_obj_camera == null)
            {
				m_obj_camera = Camera.main;
            }

			if (m_obj_cameraTarget != null) {
				m_str_cameraStatus[0].pos_target = m_obj_cameraTarget.transform;
			}
        }

        // Update is called once per frame
        void Update()
        {
            //isHomingModeとisComplateがtrueなら、カメラをターゲットの位置に固定する
            if (m_flg_homingMode && m_flg_complate)
            {
                m_obj_camera.transform.position = new Vector3(
                    m_obj_cameraTarget.transform.position.x,
                    m_obj_cameraTarget.transform.position.y,
                    -10);
            }

            if (flg_hoge)
            {
                flg_hoge = false;
                StartTargetMove();
            }

        }


        IEnumerator MoveToTarget()
        {
            

            if (!isActiveAndEnabled)
                yield break;

            CameraStatus baf_status;
            float baf_size = m_obj_camera.orthographicSize;

            //0番は追従専用のモードなので、指定されたらHomingModeをOnにする
            if (num_id == 0)
            {
                m_flg_homingMode = true;
            }
            else {
                //その他はfalseにする
                m_flg_homingMode = false;
            }

            //HomingModeによって対象を分岐する
            if (!m_flg_homingMode)
            {
                baf_status = m_str_cameraStatus[num_id];
            }
            else {
                baf_status = m_str_cameraStatus[0];
            }

            Vector3 baf_pos = m_obj_camera.transform.position;

            //対象の座標に向かってAnimationCurveを使用して遷移する
            for (int i = 1; i <= baf_status.num_complateFrame; i++)
            {
                if (baf_status.pos_target != null)
                {
                    m_obj_camera.transform.position = new Vector3(
                        baf_pos.x + (baf_status.pos_target.position.x - baf_pos.x) * baf_status.cur_move.Evaluate((float)i / (float)baf_status.num_complateFrame),
                        baf_pos.y + (baf_status.pos_target.position.y - baf_pos.y) * baf_status.cur_move.Evaluate((float)i / (float)baf_status.num_complateFrame),
                        -10
                    );
                }
                else {
                    m_obj_camera.transform.position = new Vector3(
                        baf_pos.x + (baf_status.vec_target.x - baf_pos.x) * baf_status.cur_move.Evaluate((float)i / (float)baf_status.num_complateFrame),
                        baf_pos.y + (baf_status.vec_target.y - baf_pos.y) * baf_status.cur_move.Evaluate((float)i / (float)baf_status.num_complateFrame),
                        -10
                    );
                }
                m_obj_camera.orthographicSize = baf_size + (baf_status.num_scale - baf_size) * baf_status.cur_move.Evaluate((float)i / (float)baf_status.num_complateFrame);

                yield return null;
            }

            if (m_flg_homingMode)
                m_flg_complate = true;
            

            yield break;

        }


        /// <summary>
        /// 再生したいカメラムーブのIDを設定します。
        /// IDは配列のNo.と同期しています。
        /// 実際に再生する場合は、StartTargetMove関数を使用してください。
        /// </summary>
        /// <param name="id">再生するカメラムーブのID</param>
        public void SetTargetID(int id)
        {
            num_id = id;
            m_flg_complate = false;
        }

        /// <summary>
        /// 引数で設定したIDのカメラムーブに設定して再生します。
        /// IDは配列のNo.と同期しています。
        /// </summary>
        public void SetTargetAndStart(int id)
        {
            num_id = id;
            m_flg_complate = false;
            StartCoroutine(MoveToTarget());
        }

        /// <summary>
        /// 追従モードの際、カメラが追従するオブジェクトを設定します。
        /// この値はInspector内で初期設定を行う事もできます。
        /// </summary>
        /// <param name="obj">設定するGameObject</param>
        public void SetTargetObject(GameObject obj)
        {
            m_obj_cameraTarget = obj;
            m_str_cameraStatus[0].pos_target = obj.transform;
            m_flg_complate = false;
        }

        /// <summary>
        /// SetTargetID関数で設定したIDのカメラムーブを再生します。
        /// </summary>
        public void StartTargetMove()
        {
            StartCoroutine(MoveToTarget());
        }

        /// <summary>
        /// 現在使用中のカメラIDで、プレーヤーが操作可能かどうかを返します。
        /// true :プレーヤーは操作可能です。
        /// false:プレーヤーは操作されるべきではありません。
        /// </summary>
        public bool CheckPlayerActive()
        {
            return m_str_cameraStatus[num_id].flg_isPlayerActive;
        }

    }
}