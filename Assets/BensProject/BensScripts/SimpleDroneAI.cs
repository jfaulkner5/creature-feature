using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public enum SimpleAIState { Idle, Wander, MoveTo }
    public enum NavState { Idle, NoPath, NewPath, MoveingTo, AtDestination }

    public class SimpleDroneAI : MonoBehaviour {

        public DirectorAI director;

        public GameObjectList droneList;

        public SimpleAIState state = SimpleAIState.Idle;
        SimpleAIState stateLast = SimpleAIState.Idle;
        public Navigator navigator;
        public NavState navState = NavState.Idle;

        // Use this for initialization
        void Start() {            
            navigator = GetComponent<Navigator>();
            navigator.owner = this;
        }

        // Update is called once per frame
        void Update() {
            switch (state)
            {
                case SimpleAIState.Idle:
                    Idle();
                    break;
                case SimpleAIState.Wander:
                    Wandering();
                    break;
                case SimpleAIState.MoveTo:
                    break;
                default:
                    break;
            }
        }

        #region States

        void Idle()
        {

        }

        void Wandering()
        {
            navigator.SetDestination(PathGrid.GetNewLocation());
        }

        #endregion
    }
}
