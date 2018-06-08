using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeManGene
{
    public class StartUpScene : MergeManGene.Scene
    {
        public override IEnumerator OnEnter()
        {
            yield break;
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        public override IEnumerator OnUpdate()
        {
            AppManager.Instance.m_fade.StartFade(new FadeOut(), Color.black, 1.0f);
            yield return new WaitWhile(AppManager.Instance.m_fade.IsFading);
            UnityEngine.SceneManagement.SceneManager.LoadScene("YOSHIKI");
            
            yield return null;
        }

    }
}