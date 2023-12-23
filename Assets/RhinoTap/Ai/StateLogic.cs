using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.StateMachine;

namespace Rhinotap.StateMachine
{
    //[CreateAssetMenu(menuName = "StateMachine/Logic/DecisionName", fileName = "DecisionName")]
    public abstract class StateLogic : ScriptableObject
    {
        public abstract bool Decide(StateController Controller);
    }
}
