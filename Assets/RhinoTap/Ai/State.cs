using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhinotap.StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/New State", fileName = "NewState", order = 0)]
    public class State : ScriptableObject
    {
        //Private Tracking
        private bool isStateInitialized = false;

        

        [Header("Unique State Identifier")]
        [SerializeField]
        protected string stateName;
        public Color StateColor = Color.gray;

        [Header("Actions To Execute")]
        [Space(10)]
        [SerializeField]
        protected StateAction[] actions;

        [Header("Decisions to make")]
        [Space(10)]
        [SerializeField]
        protected Transition[] transitions;


        [Space(30)]
        [TextArea(3, 10)]
        [SerializeField]
        private string Notes;

        //Public APi
        public string StateName { get { return stateName; } }


        

        public void OnUpdate(StateController Controller )
        {
            OnStart(Controller);//Will run once

            if (!Controller.isActive) return;

            //Check transition logic
            ExecuteTransitions(Controller);

            if( Controller.CurrentState == this)
            {
                //Run all actions each update
                ExecuteActions(Controller);
            }
            
            
        }


        private void ExecuteTransitions(StateController Controller)
        {
            if (transitions.Length == 0) return;
            for(int i = 0; i < transitions.Length; i++)
            {
                //Run the logic object in the transition
                if( transitions[i].DecisionLogic.Decide(Controller) == true )
                {
                    //If logic returns true, change state to this
                    if( transitions[i].isTrue != null && transitions[i].isTrue != this)
                    {
                        Controller.ChangeState(transitions[i].isTrue);
                        break;
                    }
                } else
                {
                    //If logic returns true, change state to this
                    if (transitions[i].isFalse != null && transitions[i].isFalse != this)
                    {
                        Controller.ChangeState(transitions[i].isFalse);
                        break;
                    }

                }
            }
        }


        /// <summary>
        /// Run each action defined for this state
        /// </summary>
        /// <param name="Controller">StateController MonoBehaviour</param>
        private void ExecuteActions(StateController Controller)
        {
            if (actions.Length == 0) return;

            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].OnUpdate(Controller);
            }
        }

        /// <summary>
        /// Will run only once the state starts
        /// </summary>
        /// <param name="Controller"></param>
        private void OnStart(StateController Controller)
        {
            //Run once logic
            if (isStateInitialized) return;
            isStateInitialized = true;

        }


    }


    [System.Serializable]
    public class Transition
    {
        [Header("Logic Object")]
        [SerializeField]
        public StateLogic DecisionLogic;
        [Header("Transition To")]
        [SerializeField]
        public State isTrue;
        public State isFalse;
    }
}
