using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagiHanGenerat : MonoBehaviour {


    /// <summary>生成するマジハン </summary>
    [SerializeField]
    private GameObject m_magihan;
    /// <summary>生成座標 </summary>
    private Vector2 m_gPos;

    //生成間隔
    [SerializeField]
    private float m_generatTime;
    /// <summary>
    /// 各生成座標指定
    /// </summary>
    private void Start()
    {
        StartCoroutine("Fire");
        m_gPos = transform.position;
    }

    private IEnumerator Fire(){

        while (true)
        {
            yield return new WaitForSeconds(m_generatTime);
            Instantiate(m_magihan, new Vector3(m_gPos.x,m_gPos.y,0), Quaternion.identity);
        }
    }
}
