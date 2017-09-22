using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBossBarTrigger : MonoBehaviour {

    BossGUIManager bossGUIManager;
    GameObject theBoss;

    private void Awake()
    {
        bossGUIManager = GameObject.Find("BossGUI").GetComponent<BossGUIManager>();
        GetComponent<Renderer>().enabled = false;
        theBoss = GameObject.Find("BossManager3D");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            if (bossGUIManager.isActiveAndEnabled == false && theBoss.activeInHierarchy == true)
            {
                bossGUIManager.gameObject.SetActive(true);
                bossGUIManager.StartAnimationIntoScene();
            }
        }
    }
}
