using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Rhinotap Event Handling Class
/// Using action delegates
/// </summary>

namespace Rhinotap.Toolkit
{
    public class EventManager : MonoBehaviour
    {

        #region Initialization (Created: EventManager instance , EventDictionary events )
        private static EventManager _instance; //instance of this object
        public static EventManager instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<EventManager>();//find the instance of EventManager on the scene.
                    //If the instance is not in the scene, throw error
                    if (_instance == null)
                        Debug.LogError("Rhinotap Events: EventManager object is not in the scene.");
                    else
                        _instance.Initialize();//Object has been found. Run init
                }
                return _instance;

            }
        }
        


        private EventDictionary events;
        private Dictionary<string, Action> voidEvents;
        //Initialization
        private void Initialize()
        {
            events = new EventDictionary();
            voidEvents = new Dictionary<string, Action>();
        }
        #endregion

        //===============| GENERIC METHODS FOR PARAMETER SUPPORTED EVENTS |==================//

        //Start listening to an event. The event will be created if it doesnt exist
        public static void StartListening<ActionType>(string eventName, Action<ActionType> listener)
        {
            if (instance == null)
            {
                Debug.LogWarning("Rhinotap Event Handler: StartListening " + eventName + " failed. System is not initialized or missing");
                return;
            }
            //Active dictionary. (Will be created if doesnt exist)
            Dictionary<string, Action<ActionType>> currentDictionary = instance.events.GetDictionary<Action<ActionType>>();
            Type actionEventType = typeof(Action<ActionType>);

            if( currentDictionary.ContainsKey(eventName))
            {
                currentDictionary[eventName] += listener;
            } else
            {
                Action<ActionType> thisEvent = delegate { };
                thisEvent += listener;
                currentDictionary.Add(eventName, thisEvent);
            }
            instance.events.UpdateDictionary<Action<ActionType>>(currentDictionary);
            //Debug.Log("Event Listener Added");
        }

        //To stop listening an event
        public static void StopListening<ActionType>(string eventName, Action<ActionType> listener)
        {
            if (instance == null)
            {
                Debug.LogWarning("Rhinotap Event Handler: StopListening " + eventName + " failed. System is not initialized or missing");
                return;
            }

            if ( instance.events.KeyExists<Action<ActionType>>(eventName))
            {
                Dictionary<string, Action<ActionType>> currentDictionary = instance.events.GetDictionary<Action<ActionType>>();
                if( currentDictionary[eventName] != null)
                {
                    currentDictionary[eventName] -= listener;
                    instance.events.UpdateDictionary<Action<ActionType>>(currentDictionary);
                    //Debug.Log("Event Listener Removed");
                }
            }

        }

        //Trigger. Call to fire all methods that are listening to this event. Param will be passed to those
        public static void Trigger<ActionType>(string eventName, ActionType param )
        {
            if (instance == null)
            {
                Debug.LogWarning("Rhinotap Event Handler: " + eventName + " couldn't be triggered. System is not initialized or missing");
                return;
            }

            Action<ActionType> thisEvent = delegate { };

            if(instance.events.KeyExists<Action<ActionType>>(eventName))
            {
                thisEvent = instance.events.GetValue<Action<ActionType>>(eventName);
                if( thisEvent != null)
                    thisEvent.Invoke(param);
            } else
            {
                //event action was not created because startlistening was not called
                Debug.LogWarning("RhinoTap Events:" + eventName + " failed to trigger. Event doesn't exist");
            }
            
        }


        //===============| NON GENERIC METHODS FOR SIMPLE EVENTS WITHOUT PARAM |==================//
        public static void StartListening(string eventName, Action listener)
        {
            if (instance == null)
            {
                Debug.LogWarning("Rhinotap Event Handler: StartListening " + eventName + " failed. System is not initialized or missing");
                return;
            }
            Action thisEvent;
            if( instance.voidEvents.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                instance.voidEvents[eventName] = thisEvent;
            }else
            {
                thisEvent = delegate { };
                thisEvent += listener;
                instance.voidEvents.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action listener)
        {
            if (instance == null)
            {
                Debug.LogWarning("Rhinotap Event Handler: StopListening " + eventName + " failed. System is not initialized or missing");
                return;
            }

            if (instance.voidEvents.ContainsKey(eventName))
            {
                instance.voidEvents[eventName] -= listener;
            }
        }

        public static void Trigger(string eventName)
        {
            if (instance == null)
            {
                Debug.LogWarning("Rhinotap Event Handler: " + eventName + " couldn't be triggered. System is not initialized or missing");
                return;
            }

            Action thisEvent = null;
            if( instance.voidEvents.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke();
            }else
            {
                Debug.LogWarning("RhinoTap Events:" + eventName + " failed to trigger. Event doesn't exist");
            }
        }
    }//EOC


    #region Custom Event Dictionary
    public class EventDictionary
    {
        //Dictionary of dictionaries. Will hold all dictionary types
        public Dictionary<Type, object> collection = new Dictionary<Type, object>();


        //Get a dictionary of action type from collection
        //If it doesnt exist, create one 
        //then return it
        public Dictionary<string, ActionType> GetDictionary<ActionType>() where ActionType : class
        {
            if (!dictionaryExists<ActionType>())
            {
                AddDictionary<ActionType>();
            }

            Type key = typeof(ActionType);

            return collection[key] as Dictionary<string, ActionType>;

        }


        //After getting a dictionary and doing stuff to it,
        //Pass it back here to update the main collection
        public void UpdateDictionary<ActionType>(object updatedDictionary) where ActionType : class
        {
            Type dictType = typeof(ActionType);
            collection[dictType] = updatedDictionary;
        }

        //Check if this event exists in the specified type of dictionary
        public bool KeyExists<ActionType>(string key) where ActionType : class
        {
            if(!dictionaryExists<ActionType>())
            {
                //Debug.Log("No dictionary for type " + typeof(ActionType).ToString());
                return false;
            }

            Dictionary<string, ActionType> dict = GetDictionary<ActionType>();
            if( dict.ContainsKey(key))
            {
                return true;
            }
            //Debug.Log("No value in the  " + typeof(ActionType).ToString() + " dictionary for key: " + key);
            return false;
        }

        //Get the specific event for this action type and event name
        public ActionType GetValue<ActionType>(string key) where ActionType : class
        {
            if (KeyExists<ActionType>(key))
                return GetDictionary<ActionType>()[key] as ActionType;
            else
                return default;
        }

        //Does the dictionary for this ActionType exists?
        private bool dictionaryExists<ActionType>() where ActionType : class
        {
            Type dictType = typeof(ActionType);
            return collection.ContainsKey(dictType);
        }

        //Add the dictionary for this action type if it doesnt exist
        private void AddDictionary<ActionType>() where ActionType : class
        {
            Dictionary<string, ActionType> newDictionary = new Dictionary<string, ActionType>();
            Type dictionaryType = typeof(ActionType);

            if (!dictionaryExists<ActionType>())
            {
                collection.Add(dictionaryType, newDictionary);
            }
        }

    }
    #endregion

}//EON

#region Simple Example for void events
/*

    Action listener = new Action(() => Debug.Log("Event Fired"));
    EventManager.StartListening("testEvent", listener);

    EventManager.Trigger("testEvent");

 
*/
#endregion

#region USAGE
/*

//Defining an action using lambda
Action<bool> GameStatusEvent = new Action<bool>((param) => { if (param) { Debug.Log("its true"); } else { Debug.Log("its false"); }  } );

//Adding event listener
Rhinotap.Events.EventManager.StartListening<bool>("testingEvents", GameStatusEvent);

WHAT THIS MEANS
when something uses trigger() to trigget "testingEvents",
the lambda expression will run.
A method name can also be used

For example:
we want a function to run when an event is triggered.
We want a parameter to be passed to this function from the triggering object.

    1: create a method to fire 
    void test( string name) {
        Debug.Log("Hello " + name);
    }

    2: Add a listener
    EventManager.StartListening<string>("onTestEvent", test);

    3: Trigger the event whenever you want from wherever you want
    EventManager.Trigger<string>("onTestEvent", "Abidin");

=====================================
Working Example

    //Pausing Mechanism (When called, triggers event)
    bool paused = false;
    public  void PlayPause()
    {
        paused = !paused;
        Rhinotap.Events.EventManager.Trigger<bool>("testingEvents", paused);
    }

    //Action placeholder for event (delagate)
    Action<bool> GameStatusEvent;

    //Bind a method to delagate
    private void Start()
    {
        GameStatusEvent = new Action<bool>(EventTester);
        StartCoroutine(testevents());
    }

    //Testing engine
    IEnumerator testevents()
    {
        yield return null;
        while(true)
        {
            startagain();
            yield return new WaitForSeconds(5f);

            stopagain();
            yield return new WaitForSeconds(5f);
        }
    }

    void startagain()
    {
        //Will add a listener to this event (A new event will be created if not exists)
        Rhinotap.Events.EventManager.StartListening<bool>("testingEvents", GameStatusEvent);
    }

    void stopagain()
    {
        //Will remove listener from event
        Rhinotap.Events.EventManager.StopListening<bool>("testingEvents", GameStatusEvent);
    }

    //The method that will be called when event fires! 
    public void EventTester(bool param)
    {
        if (param == true)
            Debug.Log("Oh its tested true");
        else
            Debug.Log("Oh its quite false");
    }
 
*/
#endregion
