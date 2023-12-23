using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.StateMachine;

namespace Rhinotap.StateMachine
{
	[CreateAssetMenu(menuName = "StateMachine/Logic/New WaitForRandom", fileName = "WaitForRandom")]
	public class WaitForRandom : StateLogic
	{
        [Header("Wait for x seconds then return true")]
        [Range(0.05f, 5f)]
        [SerializeField]
        private float minWaitTime;

        [Range(0.05f, 5f)]
        [SerializeField]
        private float maxWaitTime;

	    public override bool Decide(StateController Controller)
		{
            VerifyValues();
            //Find the current random timer for this controller
            float? randomTimer = Controller.GetData<float?>("randomWaitTimer");

            //if its null, set a random timer
            if( randomTimer == null || randomTimer == 0f)
            {
                randomTimer = Random.Range(minWaitTime, maxWaitTime);
                Controller.SetData("randomWaitTimer", randomTimer);
            }

            

            //check if time elapsed
            if( Controller.TimeInThisState >= randomTimer.Value)
            {
                //Random time has been elapsed
                Controller.RemoveData("randomWaitTimer");//reset timer
                return true;
            } else
            {
                return false;
            }
			
		}

        private void VerifyValues() {

            if( minWaitTime <= 0.05f) { minWaitTime = 0.05f; }
            if( maxWaitTime <= 0.05f) { maxWaitTime = 0.05f; }

            if( minWaitTime > maxWaitTime)
            {
                float oldMinWaitTime = minWaitTime;
                minWaitTime = maxWaitTime;
                maxWaitTime = oldMinWaitTime;
                return;
            }
        }
	}
}
