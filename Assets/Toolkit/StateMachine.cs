using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Rhinotap.States
{
    
    //State Machine Engine
    public class StateMachine : MonoBehaviour
    {
        protected Dictionary<Type, State> availableStates = new Dictionary<Type, State>();
        protected State currentState;
        private bool machineOn = false;

        [Header("StateMachine Settings")]
        [SerializeField]
        private float checkStateInterval = 1f;

        
        public float Interval { get { return checkStateInterval; } }
        

        protected void AddState(State stateObj, bool isDefault = false)
        {
            if (stateObj == null)
            {
                return;
            }
            availableStates.Add(stateObj.GetType(), stateObj);

            if (isDefault)
                ChangeState(stateObj);
        }

        protected void StartMachine(Type initialStateType, float interval = 0f)
        {
            if( interval > 0f)
            {
                checkStateInterval = interval;
            }
            ChangeState(initialStateType);



            StopCoroutine(Tick());
            StartCoroutine(Tick());
            machineOn = true;
        }

        protected void StartMachine(float interval = 0f)
        {
            if( availableStates.Count <= 0)
            {
                Debug.LogError("Rhinotap Statemachine: No state available to run machine");
            }

            if (interval > 0f)
            {
                checkStateInterval = interval;
            }
            if( currentState == null)
            {
                Debug.LogWarning("Rhinotap StateMachine: Initial state is not defined");
                ChangeState(availableStates.Values.First<State>());
                Debug.Log("Using first available state to initialize ( " + currentState.GetType().ToString() + " )");
            }


            StopCoroutine("Tick");
            machineOn = true;
            StartCoroutine("Tick");

        }


        protected void PauseMachine()
        {
            machineOn = !machineOn;

            if (machineOn)
                StartCoroutine(Tick());
            else
                StopCoroutine(Tick());

        }
        
        IEnumerator Tick()
        {
            yield return null;

            //Debug.Log("Started Ticking");

            while (true)
            {
                
                
                if( currentState != null)
                {
                    Type stateType = currentState.UpdateState();
                    if( stateType == currentState.GetType())
                    {
                        //No state change
                        //Debug.Log("Tick! interval: " + checkStateInterval.ToString("n2"));
                    }else
                    {
                        //There is a state change
                        ChangeState(stateType);
                    }
                }

                
                if( !machineOn)
                {
                    yield break;
                }

                
                yield return new WaitForSeconds(checkStateInterval);
                //Add to delta time
                continue;
            }

            

            
        }


        protected void ChangeState(Type stateType)
        {
            //Check state availability
            if( !availableStates.ContainsKey(stateType))
            {
                Debug.Log("RhinoTap StateMachine: Requested State of " + stateType + " is not available for [Object: " + gameObject.name + "]");
                return;
            }

            //Check if states are same
            if( currentState != null)
            {
                if( currentState.GetType() == stateType)
                {
                    //Already in this state
                    return;
                }else
                {
                    //Exit current state. We have new state
                    currentState.ExitState();
                }

            }

            //Change state to new state

            currentState = availableStates[stateType];
            if (currentState != null)
            {
                //Switched states
                currentState.Initialize(gameObject);
                currentState.EnterState();
            }
        }

        protected void ChangeState(State stateObj)
        {
            ChangeState(stateObj.GetType());
        }

        protected void StopMachine()
        {
            StopCoroutine("Tick");
            machineOn = false;
        }

    }


    //Abstract for each state. 
    public abstract class State
    {
        protected  GameObject obj;
        
        public virtual void Initialize(GameObject _obj)
        {
            obj = _obj;
        }
        public abstract Type UpdateState();
        public abstract void EnterState();
        public abstract void ExitState();
    }



}

/**
 * Example Child Script for FSM enabled object
 * 
namespace Enemy.Standard
{
    public class EnemyAi : StateMachine
    {
        void Start()
        {
            //StateMachine Setup
            AddState(new StateChase());
            AddState(new StatePatrol(), true);
            StartMachine(0.5f);
        }
    }
}
 * 
 */

/* Example State 
 * 
 * 
public class StateIdle : State
{
    public override void EnterState()
    {
        Debug.Log("Entered state for " + obj.name);
    }

    public override void ExitState()
    {
        
    }

    public override Type UpdateState()
    {
        

        return typeof(StateIdle);
    }
}
*/