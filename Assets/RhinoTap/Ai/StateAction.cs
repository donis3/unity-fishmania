using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhinotap.StateMachine
{
    //[CreateAssetMenu(menuName = "StateMachine/Actions/ActionName", fileName = "ActionName")]
    public abstract class StateAction : ScriptableObject
    {
        public abstract void OnUpdate(StateController Controller);
    }
}
