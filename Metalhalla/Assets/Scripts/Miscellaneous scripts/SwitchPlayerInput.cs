using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlayerInput : MonoBehaviour {

    private const string ENDING_TRIGGER_NAME = "SwitchPlayerInputTrigger - Ending";

    public enum inputSwitch
    {
        PlayerToAI,
        AIToPlayer
    };

    public inputSwitch change = inputSwitch.PlayerToAI;
    public PlayerInputAI.AIProgram program = PlayerInputAI.AIProgram.None;

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            bool useAI = change == inputSwitch.PlayerToAI ? true : false;
            PlayerController controller = collision.GetComponent<PlayerController>();
            if (useAI)
            {
                controller.GetComponent<PlayerInputAI>().SetAIProgram(program);
                if (gameObject.name.Equals(ENDING_TRIGGER_NAME))
                {
                    AudioManager.instance.currentState = AudioManager.State.ENDING;
                    collision.GetComponent<PlayerStatus>().PlayFx("what");
                }
            }
            controller.switchAIInput(useAI);
        }
    }
}
