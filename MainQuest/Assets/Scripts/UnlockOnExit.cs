using UnityEngine;

public class UnlockOnExit : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAnimatorController controller = animator.GetComponent<PlayerAnimatorController>();
        if (controller != null)
        {
            controller.EndAction(); // Unlocks controls
        }
    }
}
