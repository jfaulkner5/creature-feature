using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public enum SimpleAIState { Idle, JobWork, MoveTo }
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
                case SimpleAIState.JobWork:
                    JobFSM();
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

        void JobFSM()
        {
            switch (jobState)
            {
                case JobState.GotoJob:
                    GoToJob();
                    break;
                case JobState.RetreveJob:
                    RetreveJob();
                    break;
                case JobState.OnJob:
                    OnJob();
                    break;
                case JobState.JobCompleated:
                    JobCompleated();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region jobStates

        public enum JobState { GotoJob,RetreveJob,OnJob,JobCompleated}
        public JobState jobState;

        public AiJob jobCurrant;

        void GoToJob()
        {
            if (navState != NavState.MoveingTo)
            {
                navigator.PathFromJob(jobCurrant);
            }            
        }

        public void AtJobLocation()
        {
            switch (jobCurrant.Job)
            {
                case AiJob.JobType.Explore:
                    GoToJobBoard();
                    break;
                case AiJob.JobType.Collect:
                    GoToJobBoard();
                    break;
                case AiJob.JobType.GetJob:
                    RetreveJob();
                    break;
                default:
                    break;
            }
        }

        void GoToJobBoard()
        {
            List<Node> path = jobCurrant.Path;
            path.Reverse();
            jobCurrant = new AiJob(AiJob.JobType.GetJob, director.homeNode.worldPosition, null, path);
            navigator.PathFromJob(jobCurrant);
        }

        void RetreveJob()
        {
            if (director.JobBoard.Count > 0)
            {
                jobCurrant = director.JobBoard[director.JobBoard.Count - 1];
                director.JobBoard.Remove(jobCurrant);
                navigator.PathFromJob(jobCurrant);
            }
        }

        void OnJob()
        {

        }

        void JobCompleated()
        {

        }

        #endregion

        #region Sensing
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("WorldTile"))
            {
                Node node = PathGrid.NodeFromWorldPoint(other.gameObject.transform.position);
                

            if (node != null && node.walkable)
                {
                    director.KnownLocations.AddSafe(node, false);
                }
            }
            else if (other.CompareTag("Resource"))
            {
                Resource resource = other.gameObject.GetComponent<Resource>();

                if (resource != null)
                {
                    director.KnownResource.AddSafe(resource,false);
                }
            }
        }
        #endregion
    }
}
