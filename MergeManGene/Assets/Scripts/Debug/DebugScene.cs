using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeManGene
{
	public class DebugScene : MergeManGene.Scene {

		[SerializeField]
		GameObject m_debugMenu;

		public override IEnumerator OnEnter() {
			if (m_debugMenu) {
				m_debugMenu.SetActive(true);
			}
			yield break;
		}

		public override IEnumerator OnUpdate() {
			yield return new WaitWhile(() => !Input.GetMouseButtonUp(0));
		}

		public override IEnumerator OnExit() {
			AppManager.Instance.m_fade.Fill(Color.black);
			SceneManager.LoadScene("Title");
			yield break;

		}
	}
}