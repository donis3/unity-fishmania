using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.StateMachine;

namespace Rhinotap.StateMachine
{
	[CreateAssetMenu(menuName = "StateMachine/Logic/New SwitchStateAfter", fileName = "SwitchStateAfter")]
	public class SwitchStateAfter : StateLogic
	{
        [Header("True when time in this state exceeds this")]
        [SerializeField]
        private float timeElapsed = 0f;


	    public override bool Decide(StateController Controller)
		{
            if (Controller.TimeInThisState >= timeElapsed)
                return true;
            else
                return false;
		}
	}
}
