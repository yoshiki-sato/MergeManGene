using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace MergeManGene
{
    public class MainScene : Scene
    {

        /// <summary>
        /// シーン開始時
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnEnter()
        {
            AppManager.Instance.m_fade.StartFade(new FadeIn(), Color.black, 1.0f);
            yield return new WaitWhile(AppManager.Instance.m_fade.IsFading);
        }

        /// <summary>
        /// アップデート処理
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnUpdate()
        {
            yield return null;
        }

        /// <summary>
        /// シーン終了
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnExit()
        {
            yield return new WaitWhile(AppManager.Instance.m_fade.IsFading);
        }

    }

}