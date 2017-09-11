using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlayerInput : MonoBehaviour {

    public enum inputSwitch
    {
        Player2AI,
        AI2Player
    };

    public inputSwitch change = inputSwitch.Player2AI;
    public PlayerInputAI.AIProgram program = PlayerInputAI.AIProgram.None;

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            bool useAI = change == inputSwitch.Player2AI ? true : false;
            PlayerController controller = collision.GetComponent<PlayerController>();
            if (useAI)
            {
                controller.GetComponent<PlayerInputAI>().SetAIProgram(program);
            }
            controller.switchAIInput(useAI);
        }
    }
}
