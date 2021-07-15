using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{

    #region Variables
    public Arm[] arms;

    
    #endregion

    #region States
    public virtual IEnumerator Start()
    {
        yield break;
    }
    public virtual IEnumerator Setup()
    {
        yield break;
    }
    public virtual IEnumerator Working()
    {
        yield break;
    }
    public virtual IEnumerator Editing()
    {
        yield break;
    }
    public virtual IEnumerator ExitEditMode()
    {
        yield break;
    }
    public virtual IEnumerator Stopped()
    {
        yield break;
    }
    #endregion
}