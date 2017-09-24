using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openChestController : MonoBehaviour
{

    public GameObject content;
    public Vector3 contentPositionOffset = new Vector3(0, 1, 0);

    public float contentDisplayDelayTime = 1.0f;
    float contentDisplayCounter = 0.0f;

    private Animator chestAnimator;

    enum state { CLOSED, OPEN, EMPTY };
    state currentState = state.CLOSED;

    [Header("Sound FXs")]
    public AudioClip openChest;

    private void Awake()
    {
        chestAnimator = GetComponentInChildren<Animator>();
        currentState = state.CLOSED;
        contentDisplayCounter = 0.0f;
    }


    private void Update()
    {
        if ( currentState == state.OPEN)
        {
            contentDisplayCounter += Time.deltaTime;
            if (contentDisplayCounter >= contentDisplayDelayTime)
            {
                if (content != null)
                    Instantiate(content, transform.position + contentPositionOffset, content.transform.rotation);
                currentState = state.EMPTY;
            }
        }
    }
    // when attacking the chest it gets open
    public void ApplyDamage(int dmg = 0)
    {
        ParticlesManager.SpawnParticle("hitEffect", transform.position, true);
        if (currentState == state.CLOSED)
        {
            if (openChest) AudioManager.instance.PlayDiegeticFx(gameObject, openChest);
            currentState = state.OPEN;
            chestAnimator.SetBool("open", true);
        }

    }
}
