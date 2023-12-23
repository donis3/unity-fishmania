using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.StateMachine;

namespace Rhinotap.StateMachine
{
	[CreateAssetMenu(menuName = "StateMachine/Actions/FishWanderAction", fileName = "FishWanderAction")]
	public class FishWanderAction : StateAction
	{
        [Header("Wandering Curve Settings")]
        [SerializeField]
        private AnimationCurve curve;

        [Range(0f, 1f)]
        [SerializeField]
        private float curveIntensity = 0.05f;

        [SerializeField]
        private AnimationCurve easing =  AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Movement Radius Settings")]
        [Space(20)]
        [SerializeField]
        private float MovementRadiusX = 1f;
        [SerializeField]
        private float MovementRadiusY = 1f;


        public override void OnUpdate(StateController Controller)
		{
            Initialize(Controller as StateController);

            

            if( DistanceToDestination(Controller) <= 0.5f)
            {
                SetDestination(Controller);
            }

            MoveTowards(Controller);

		}



        /// <summary>
        /// Set required data for this controller (Will run once)
        /// </summary>
        /// <param name="Controller"></param>
        private void Initialize(StateController Controller)
        {
            if (Controller.GetData<bool>("isInitialized") == true) return;

            //Set required data
            Controller.SetData<Vector2?>("CurrentDestination", null);

            Controller.SetData<float>("MovementTimeElapsed", 0f);

            Controller.SetData<bool>("isInitialized", true);

            Controller.SetData<int>("wanderingActionsCompleted", 0);

            //Debug.Log("Initialization Complete");
        }


        /// <summary>
        /// Get a random Vector2 in given radius
        /// </summary>
        /// <returns></returns>
        private Vector2 RandomDestination()
        {
            Vector2 result = Vector2.zero;

            result.x = Random.Range(-MovementRadiusX, MovementRadiusX);
            result.y = Random.Range(-MovementRadiusY, MovementRadiusY);

            return result;
        }

        /// <summary>
        /// Distance to current Destination (Local Space)
        /// </summary>
        /// <param name="Controller"></param>
        /// <returns></returns>
        private float DistanceToDestination(StateController Controller)
        {
            Vector2? destination = Controller.GetData<Vector2?>("CurrentDestination");
            if (destination == null) return 0f;

            int DestinationReachedCount = Controller.GetData<int>("wanderingActionsCompleted");

            float distance =  Vector2.Distance((Vector2)Controller.transform.localPosition, destination.Value);

            if( distance <= 0.5f)
            {
                Controller.SetData<int>("wanderingActionsCompleted", DestinationReachedCount+1);
                RemoveDestination(Controller);
            }
            return distance;
        }


        /// <summary>
        /// Set the current destination (Local Space)
        /// </summary>
        /// <param name="Controller"></param>
        private void SetDestination(StateController Controller)
        {
            Vector2 newDestination = RandomDestination();
            //Set a  random local destination
            Controller.SetData<Vector2?>("CurrentDestination", newDestination);

            //Reset movement timer
            Controller.SetData<float>("MovementTimeElapsed", 0f);

            //Flip fish
            Controller.FishController.FlipTowardsDestination(newDestination);
        }

        /// <summary>
        /// Null the destination value
        /// </summary>
        /// <param name="Controller"></param>
        private void RemoveDestination(StateController Controller)
        {
            Controller.SetData<Vector2?>("CurrentDestination", null);
            Controller.SetData<float>("MovementTimeElapsed", 0f);
        }



        private void MoveTowards(StateController Controller)
        {
            if( Controller.GetData<Vector2?>("CurrentDestination") == null ) { return; }
            Vector2 destination = Controller.GetData<Vector2?>("CurrentDestination").Value;
            float timeElapsed = Controller.GetData<float>("MovementTimeElapsed");

            float percentage = easing.Evaluate(timeElapsed) * Controller.FishController.Speed;
            Vector2 CurrentLocalPosition = Vector2.Lerp(Controller.transform.localPosition, destination, percentage);



            //Add curve to y
            float yCurve = curve.Evaluate(timeElapsed) * curveIntensity;

            //y value - or +
            if( destination.y >= Controller.transform.localPosition.y)
                CurrentLocalPosition.y -= yCurve;
            else
                CurrentLocalPosition.y += yCurve;


            //Debug.Log("Percentage: " + percentage.ToString() + " Current curve: " + yCurve.ToString());

            //adjust position
            Controller.transform.localPosition = CurrentLocalPosition;

            //Save time elapsed
            Controller.SetData<float>("MovementTimeElapsed", timeElapsed + Time.deltaTime);

        }



	}
}

