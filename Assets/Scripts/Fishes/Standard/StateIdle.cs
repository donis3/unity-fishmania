using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.States;
using System;

namespace Fishes.Standard
{

    public class StateIdle : State
    {
        bool startPosRecorded = false;
        //Fish Controller Component on obj
        FishController fish;
        //initial position of fish
        Vector2 startPos = Vector2.zero;
        //Time between new destinations
        float destinationChangeTime = 4f;
        //Tracker
        float timeSinceLastDestinationChange = 0f;

        public override void EnterState()
        {
            if (!startPosRecorded)
            {
                //Debug.Log("Entered state for " + obj.name);
                startPos = obj.transform.position;
                startPosRecorded = true;
            }
            if (fish == null)
                fish = obj.GetComponent<FishController>();

            if (fish != null)
            {
                destinationChangeTime = fish.TimeBetweenMovement;
            }
        }

        public override void ExitState()
        {

        }

        public override Type UpdateState()
        {

            if (fish.isMoving == false)
            {
                if (timeSinceLastDestinationChange >= destinationChangeTime)
                {
                    fish.SetDestination(FindRandomDestination());
                }
                else
                {
                    //Debug.Log("Waiting for change time, intervals :" + fish.Interval.ToString("n2") + " currentWait: " + timeSinceLastDestinationChange.ToString("n2"));
                    timeSinceLastDestinationChange += fish.Interval;
                }
            }
            return typeof(StateIdle);
        }

        //Custom Methods

        private Vector2 FindRandomDestination()
        {
            Vector2 dest = startPos;
            dest.x = dest.x + UnityEngine.Random.Range(-(fish.FishMovementLimit.x / 2f), (fish.FishMovementLimit.x / 2f));
            dest.y = dest.y + UnityEngine.Random.Range(-(fish.FishMovementLimit.y / 2f), (fish.FishMovementLimit.y / 2f));
            timeSinceLastDestinationChange = 0f;

            //Debug.Log("New Destination: " + dest);
            return dest;
        }


    }

}
