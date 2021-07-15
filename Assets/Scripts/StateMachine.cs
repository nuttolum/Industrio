using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected BaseState State;
    public void SetState(BaseState state) {
        State = state;
        StartCoroutine(State.Start());
    }
}
