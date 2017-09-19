using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBossBarTrigger : MonoBehaviour {

    BossGUIManager bossGUIManager;

    private void Awake()
    {
        bossGUIManager = GameObject.Find("BossGUI").GetComponent<BossGUIManager>();
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            if (bossGUIManager.isActiveAndEnabled == false)
            {
                bossGUIManager.gameObject.SetActive(true);
                bossGUIManager.StartAnimationIntoScene();
            }
        }
    }
}
