using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

namespace MergeManGene
{
    public class AudioManager : MonoBehaviour
    {

        /// <summary>
        /// 管理しているAudio全体の設定
        /// </summary>
        [System.Serializable]
        private class AudioOption
        {
            [Range(0.0f, 1.0f), Tooltip("SEの音量")]
            public float seVolume = 1.0f;

            [Range(0.0f, 1.0f), Tooltip("BGMの音量")]
            public float bgmVolume = 0.7f;

            [HideInInspector]
            public Coroutine m_fadeIE;
        }

        #region Singleton
        private static AudioManager m_instance;

        public static AudioManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<AudioManager>();
                }
                return m_instance;
            }
        }

        private AudioManager() { }
        #endregion

        /// <summary>Resourcesフォルダに格納されているSEファイル</summary>
        private readonly Dictionary<string, AudioClip> m_seBuffer = new Dictionary<string, AudioClip>();

        /// <summary>Resourcesフォルダに格納されているBGMファイル</summary>
        private readonly Dictionary<string, AudioClip> m_bgmBuffer = new Dictionary<string, AudioClip>();

        /// <summary>現在再生されているAudioSource</summary>
        private readonly Dictionary<string, AudioSource> m_activeAudioSources = new Dictionary<string, AudioSource>();

        /// <summary>オブジェクトプール</summary>
        private readonly Queue<AudioSource> m_sePool = new Queue<AudioSource>();

        /// <summary>BGM用のAudioSource</summary>
        private AudioSource m_bgm;

        //再生できるSEの量
        [SerializeField, Tooltip("再生できるSEの量")]
        private uint m_seSize = 20;

        //再生するAudioSourceの設定
        [SerializeField]
        private AudioOption m_option;

        /// <summary>
        /// UnityEvent
        /// 起動時に処理される
        /// </summary>
        private void Awake()
        {

            if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            #region オーディオファイルをキャッシュする
            //SEフォルダの中身を全て取得
            //AudioClip[] seList = Resources.LoadAll<AudioClip>(ResourcePath.SE);
            //foreach (AudioClip clip in seList)
            //{
            //    m_seBuffer[clip.name] = clip;
            //}

            //BGMフォルダの中身を全て取得
            //AudioClip[] bgmList = Resources.LoadAll<AudioClip>(ResourcePath.BGM);
            //foreach (AudioClip clip in bgmList)
            //{
            //    m_bgmBuffer[clip.name] = clip;
            //}
            #endregion

            #region BGMの初期設定
            m_bgm = this.gameObject.AddComponent<AudioSource>();
            m_bgm.loop = true;
            #endregion

            #region SEの初期設定
            //最初にデフォルトのAudioSourceを用意する
            CreateAndEnqueueAudioSource(m_seSize);
            #endregion

            //シーン遷移で削除されないようにする
            DontDestroyOnLoad(this.gameObject);

        }

        /// <summary>
        /// SE用のAudioSourceを新しく用意する
        /// 用意されたAudioSourceはオブジェクトプールに追加される
        /// </summary>
        /// <param name="arg_size">追加する量</param>
        private void CreateAndEnqueueAudioSource(uint arg_size)
        {
            for (int i = 0; i < arg_size; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.enabled = false;
                m_sePool.Enqueue(audioSource);
            }
        }

        /// <summary>
        /// 使用可能なAudioSourceをプールから取得する
        /// プールから取得できなければ新しく追加する
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAudioSourceFromPool()
        {
            if (m_sePool.Count <= 0)
            {
                Debug.LogWarning("[ToyBox]Audioの許容量を超えたためAudioSourceを追加しました");
                CreateAndEnqueueAudioSource(1);
            }
            return m_sePool.Dequeue();
        }

        /// <summary>
        /// 再生するSEを新しく登録する
        /// ※Tag名が既に存在している場合は登録できない
        /// ※指定されたファイル名が見つからない場合も登録できない
        /// </summary>
        /// <param name="arg_tag">登録名</param>
        /// <param name="arg_clipName">再生するSEファイル名</param>
        public void RegisterSE(string arg_tag, string arg_clipName)
        {
            if (!m_seBuffer.ContainsKey(arg_clipName))
            {
                Debug.LogError("[ToyBox]指定されたファイルが見つかりません:" + "<color=red>" + arg_clipName + "</color>");
                return;
            }
            if (m_activeAudioSources.ContainsKey(arg_tag))
            {
                Debug.LogWarning("[ToyBox]Tag名が重複したため上書きしました:" + "<color=red>" + arg_tag + "</color>");
                ReleaseSE(arg_tag);
            }

            AudioSource audioSource = this.GetAudioSourceFromPool();
            audioSource.enabled = true;
            audioSource.clip = m_seBuffer[arg_clipName];
            m_activeAudioSources.Add(arg_tag, audioSource);
        }

        /// <summary>
        /// 登録されたSEを再生する
        /// </summary>
        /// <param name="arg_tag">登録名</param>
        /// <param name="arg_loop">ループするか</param>
        public void PlaySE(string arg_tag, bool arg_loop = false)
        {
            if (!m_activeAudioSources.ContainsKey(arg_tag))
            {
                Debug.LogWarning("[ToyBox]指定されたTagで登録されたSEが見つかりません:" + "<color=red>" + arg_tag + "</color>");
                return;
            }

            AudioSource audioSource = m_activeAudioSources[arg_tag];
            audioSource.volume = m_option.seVolume;
            audioSource.loop = arg_loop;
            audioSource.Play();
        }

        /// <summary>
        /// SEの再生を停止させる
        /// </summary>
        /// <param name="arg_tag">登録名</param>
        public void StopSE(string arg_tag)
        {
            if (!m_activeAudioSources.ContainsKey(arg_tag))
            {
                Debug.LogWarning("[ToyBox]指定されたTagで登録されたSEが見つかりません:" + "<color=red>" + arg_tag + "</color>");
                return;
            }

            AudioSource audioSource = m_activeAudioSources[arg_tag];
            audioSource.Stop();
        }

        /// <summary>
        /// SEを終了させ、登録を破棄する
        /// </summary>
        /// <param name="arg_tag">登録名</param>
        public void ReleaseSE(string arg_tag)
        {
            if (!m_activeAudioSources.ContainsKey(arg_tag))
            {
                Debug.LogWarning("[ToyBox]指定されたTagで登録されたSEが見つかりません:" + "<color=red>" + arg_tag + "</color>");
                return;
            }

            AudioSource audioSource = m_activeAudioSources[arg_tag];
            audioSource.Stop();
            audioSource.enabled = false;
            m_activeAudioSources.Remove(arg_tag);
            m_sePool.Enqueue(audioSource);
        }

        /// <summary>
        /// SEを登録せずに直接再生させる
        /// ※登録せずに再生させる場合は管理を行わないので注意
        /// </summary>
        /// <param name="arg_clipName">再生するSEファイル名</param>
        public void QuickPlaySE(string arg_clipName)
        {
            if (!m_seBuffer.ContainsKey(arg_clipName))
            {
                Debug.LogError("[ToyBox]指定されたファイルが見つかりません:" + "<color=red>" + arg_clipName + "</color>");
                return;
            }

            //ユニーク（唯一）な名前にするために現在時刻 + ランダムな数字で登録する
            string seName = arg_clipName + System.DateTime.Now.Hour +
                System.DateTime.Now.Minute + System.DateTime.Now.Second + System.DateTime.Now.Millisecond +
                UnityEngine.Random.Range(0, 255);

            RegisterSE(seName, arg_clipName);
            StartCoroutine(PlayAndRelease(seName));
        }

        /// <summary>
        /// SEを再生させる
        /// SEの再生が終了したらプールへ戻す
        /// </summary>
        /// <param name="arg_tag">登録名</param>
        /// <returns></returns>
        private IEnumerator PlayAndRelease(string arg_tag)
        {

            AudioSource quickAudio = m_activeAudioSources[arg_tag];

            PlaySE(arg_tag);

            //再生終了後、自動で解放をおこなう
            yield return new WaitWhile(() => quickAudio.isPlaying);
            ReleaseSE(arg_tag);
        }

        /// <summary>
        /// BGMを登録する
        /// </summary>
        /// <param name="arg_clipName">登録するBGMファイル名</param>
        public void RegisterBGM(string arg_clipName)
        {
            if (!m_bgmBuffer.ContainsKey(arg_clipName))
            {
                Debug.LogError("[ToyBox]指定されたファイルが見つかりません:" + "<color=red>" + arg_clipName + "</color>");
                return;
            }
            m_bgm.clip = m_bgmBuffer[arg_clipName];
        }

        /// <summary>
        /// BGMを再生させる
        /// </summary>
        public void PlayBGM()
        {
            if (m_bgm.isPlaying) return;
            m_bgm.volume = m_option.bgmVolume;
            m_bgm.Play();
        }

        /// <summary>
        /// BGMをフェードインで再生させる
        /// </summary>
        /// <param name="arg_fadeDuration">フェードにかける時間</param>
        public void PlayBGM(float arg_fadeDuration)
        {
            if (m_bgm.isPlaying) return;

            if (m_option.m_fadeIE == null)
            {
                m_option.m_fadeIE = StartCoroutine(FadeInBGM(arg_fadeDuration));
                m_bgm.Play();
            }
        }

        /// <summary>
        /// BGMの再生を終了させる
        /// </summary>
        public void StopBGM()
        {
            if (!m_bgm.isPlaying) return;
            m_bgm.Stop();
        }

        /// <summary>
        /// BGMの再生をフェードアウトで終了させる
        /// </summary>
        /// <param name="arg_fadeDuration">フェードにかける時間</param>
        public void StopBGM(float arg_fadeDuration)
        {
            if (!m_bgm.isPlaying) return;

            if (m_option.m_fadeIE == null)
            {
                m_option.m_fadeIE = StartCoroutine(FadeOutBGM(arg_fadeDuration));
            }
        }

        /// <summary>
        /// BGMの再生を一時停止させる
        /// </summary>
        public void PauseBGM()
        {
            m_bgm.Pause();
        }

        /// <summary>
        /// BGMの再生を再開する
        /// </summary>
        public void ResumeBGM()
        {
            m_bgm.UnPause();
        }

        /// <summary>
        /// BGMをフェードインさせる
        /// </summary>
        /// <param name="arg_duration">フェードにかける時間</param>
        /// <returns></returns>
        private IEnumerator FadeInBGM(float arg_duration)
        {
            float targetVolume = m_option.bgmVolume;
            m_bgm.volume = 0;
            while (m_bgm.volume < targetVolume)
            {
                yield return null;
                targetVolume = m_option.bgmVolume;
                float increase = (targetVolume * Time.deltaTime) / arg_duration;
                m_bgm.volume += increase;
            }
            m_bgm.volume = targetVolume;
            m_option.m_fadeIE = null;
        }

        /// <summary>
        /// BGMをフェードアウトさせる
        /// </summary>
        /// <param name="arg_duration">フェードにかける時間</param>
        /// <returns></returns>
        private IEnumerator FadeOutBGM(float arg_duration)
        {

            float targetVolume = m_option.bgmVolume;

            while (m_bgm.volume > 0)
            {
                yield return null;
                float decrease = (targetVolume * Time.deltaTime) / arg_duration;
                m_bgm.volume -= decrease;
            }
            m_bgm.volume = 0;
            m_bgm.Stop();
            m_option.m_fadeIE = null;
        }

        /// <summary>
        /// SEの音量を設定する
        /// </summary>
        /// <param name="arg_volume">音量</param>
        public void SetSEVolume(float arg_volume)
        {
            m_option.seVolume = arg_volume;
            //設定を反映させる
            foreach (AudioSource audioSource in m_activeAudioSources.Values)
            {
                audioSource.volume = m_option.seVolume;
            }
        }

        /// <summary>
        /// BGMの音量を設定する
        /// </summary>
        /// <param name="arg_volume">音量</param>
        /// <param name="arg_isStopFade">フェードを中断するか</param>
        public void SetBGMVolume(float arg_volume, bool arg_isStopFade = false)
        {
            m_option.bgmVolume = arg_volume;

            if (m_option.m_fadeIE != null)
            {
                if (arg_isStopFade)
                {
                    StopCoroutine(m_option.m_fadeIE);
                    m_option.m_fadeIE = null;
                    m_bgm.volume = m_option.bgmVolume;
                }
            }
            else
            {
                m_bgm.volume = m_option.bgmVolume;
            }
        }

        /// <summary>
        /// SEの音量設定
        /// </summary>
        public float SEVolume
        {
            get
            {
                return m_option.seVolume;
            }
            set
            {
                SetSEVolume(value);
            }
        }

        /// <summary>
        /// BGMの音量設定
        /// </summary>
        public float BGMVolume
        {
            get
            {
                return m_option.bgmVolume;
            }
            set
            {
                //設定を反映させる
                SetBGMVolume(value);
            }
        }

        #region Inspectorで音量調節を可能にするための処理
#if UNITY_EDITOR

        private float m_seVolumeBuf = 0;
        private float m_bgmVolumeBuf = 0;

        void Update()
        {
            if (m_seVolumeBuf != m_option.seVolume)
            {
                m_seVolumeBuf = m_option.seVolume;
                //設定を反映させる
                foreach (AudioSource audioSource in m_activeAudioSources.Values)
                {
                    audioSource.volume = m_option.seVolume;
                }
            }
            if (m_bgmVolumeBuf != m_option.bgmVolume)
            {
                m_bgmVolumeBuf = m_option.bgmVolume;
                //設定を反映させる
                if (m_option.m_fadeIE == null)
                {
                    m_bgm.volume = m_option.bgmVolume;
                }
            }
        }
#endif
        #endregion
    }
}