using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [SerializeField] 
    protected TrapConfig _trapConfig;

    [SerializeField] 
    protected TrapManager _trapManager;

    protected virtual void TrapActivated(){}

    protected virtual void TrapDisabled(){}

    protected virtual void TrapRemoved(){}
}
