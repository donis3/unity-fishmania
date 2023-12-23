using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rhinotap.StateMachine
{
    public abstract class StateControllerBase : MonoBehaviour
    {
        [Header("Draw Gizmos")]
        [SerializeField]
        private bool drawGizmosEnabled = false;
        [Range(0f, 10f)]
        [SerializeField]
        private float gizmoSize = .5f;

        [Header("Initial State")]
        [SerializeField]
        protected State _currentState;
        public State CurrentState { get { return _currentState; } }


        //AI Toggle
        [SerializeField]
        protected bool _isActive = true;
        public bool isActive { get { return _isActive; } }



        protected float _timeInThisState = 0f;
        protected float _lastStateChangeTime = 0f;

        public float TimeInThisState { get { return _timeInThisState; } }
        public float LastStateChangeTime { get { return _lastStateChangeTime; } }



        // Update is called once per frame
        protected void Update()
        {
            //Execute update function in the current state
            if (isActive && _currentState != null)
            {
                _currentState.OnUpdate(this as StateController);
                _timeInThisState += Time.deltaTime;
            }
        }


        public void Pause()
        {
            _isActive = false;
        }

        public void Resume()
        {
            _isActive = true;
        }

        public void ChangeState(State newState)
        {
            if (_currentState == newState)
                return;
            if (newState == null)
                return;

            //Debug.Log("Changing state from " + _currentState.StateName + " to " + newState.StateName);
            _currentState = newState;
            _lastStateChangeTime = Time.time;
            _timeInThisState = 0f;
        }


        //Memory
        Dictionary<string, object> memory = new Dictionary<string, object>();

        public void SetData<T>(string key, T data)
        {
            if (memory.ContainsKey(key))
                memory[key] = data;
            else
                memory.Add(key, data as object);
        }

        public T GetData<T>(string key)
        {
            if(memory.ContainsKey(key))
            {
                return (T)memory[key];
            }else
            {
                return default(T);
            }

        }

        public void RemoveData(string key)
        {
            if( memory.ContainsKey(key))
            {
                memory.Remove(key);
            }
        }



        private void OnDrawGizmos()
        {
            if( drawGizmosEnabled && CurrentState != null)
            {
                Gizmos.color = CurrentState.StateColor;
                Gizmos.DrawWireSphere(transform.position, gizmoSize);
            }
        }


    }

    
}
