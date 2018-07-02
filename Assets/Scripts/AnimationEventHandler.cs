using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Captures the animation event and notifies the tic tac script. */
public class AnimationEventHandler : MonoBehaviour {

    private TicTac ticTacScript;

    private void Awake() { ticTacScript = transform.parent.GetComponent<TicTac>(); }

    private void AnimationEventFired() { ticTacScript.Attack(); }
}
