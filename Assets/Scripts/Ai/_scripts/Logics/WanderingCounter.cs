using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.StateMachine;

namespace Rhinotap.StateMachine
{
	[CreateAssetMenu(menuName = "StateMachine/Logic/New WanderingCounter", fileName = "WanderingCounter")]
	public class WanderingCounter : StateLogic
	{
        [Header("How many times WanderingAction must be completed to return true")]
        [SerializeField]
        private int switchAfterXTimes = 3;

	    public override bool Decide(StateController Controller)
		{
            int wandersComplete = Controller.GetData<int>("wanderingActionsCompleted");

            if (wandersComplete >= switchAfterXTimes)
            {
                //Reset the wandering counter before switching states
                Controller.SetData<int>("wanderingActionsCompleted", 0);
                return true;
            }
            else
            {
                return false;
            }

		}
	}
}
