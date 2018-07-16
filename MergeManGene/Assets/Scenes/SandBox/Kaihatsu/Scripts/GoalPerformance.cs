using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class GoalPerformance : MonoBehaviour {

    [SerializeField]
    private GameObject m_goalFrame;
    private AudioSource m_audioSource;

    [SerializeField]
    private AudioClip m_goalSE;

	// Use this for initialization
	void Start () {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_goalSE;
	}



    void OnTriggerEnter2D(Collider2D arg_col){

        if (arg_col.tag == "Player"){
            if (!m_audioSource.isPlaying) { m_audioSource.Play(); }
            Instantiate(m_goalFrame, new Vector3(72f, 0.32f, 0), Quaternion.identity);
            StartCoroutine("Change");

        }
    }

    private IEnumerator Change(){
        yield return new WaitForSeconds(3f);
        FadeManager.Instance.LoadScene("GameEnd", 1.5f);

    }
}
