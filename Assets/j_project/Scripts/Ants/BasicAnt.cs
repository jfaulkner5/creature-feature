using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    [System.Serializable]
    public class BasicAnt : MonoBehaviour
    {

        enum AntState
        {
            Search,
            Scout,
            StrengthenPath,
            Travel,
            Hunt,
        }
        AntState antState;

        public PathFinding pathfinderInstance;


        public List<Node> desiredPath;
        public Node currentNode;
        float stopDistance;
        int currentNodeIndex;


        void Start()
        {
            stopDistance = 0.2f;
            currentNodeIndex = 0;

            FirstTravel();

        }

        public void OnDrawGizmosSelected()
        {
            if (desiredPath != null)
            {
                foreach (Node node in desiredPath)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(node.worldPos, (GameManager.Instance.myPathGrid.nodeRad));
                }
            }
            else
            {
                //Debug.Log("levelGrid is null");
            }
        }

        // Update is called once per frame
        void Update()
        {
            BehaviourDecide();

        }

        public void BehaviourDecide()
        {
            switch (antState)
            {
                case AntState.Search:

                    break;
                case AntState.Scout:

                    break;
                case AntState.StrengthenPath:

                    break;
                case AntState.Travel:
                    Travel();

                    break;
                case AntState.Hunt:

                    break;
                default:

                    break;
            }

        }

        public void FirstTravel()
        {

            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];

            desiredPath = pathfinderInstance.FindPath(_startNode, _endNode);
            currentNodeIndex = 0;

            if (desiredPath != null)
            {
                currentNode = desiredPath[currentNodeIndex];
                antState = AntState.Travel;
            }
            else if (desiredPath == null)
            {
                Debug.LogError("Desired path is null on first attempt", this.gameObject);

                Invoke("SetNewPath", 5);
            }
        }

        public void Travel()
        {

            //if (/*currentNode != null && */desiredPath != null && IsStillTravelling())
            //    return;

            if (currentNode == null)
            {
                Debug.Log("currentNode == null. attempting to trigger new path", this.gameObject);
                SetNewPath();
            }

            transform.position = Vector3.MoveTowards(transform.position, currentNode.worldPos, Time.deltaTime);
            if (Vector3.Distance(transform.position, currentNode.worldPos) <= stopDistance)
            {
                currentNodeIndex++;
                if (currentNodeIndex >= desiredPath.Count)
                {
                    currentNode = null;
                    //URGENT doesn't seem to fire properly
                    //return;
                    //SetNewPath();
                }
                else
                {
                    currentNode = desiredPath[currentNodeIndex];
                }
            }

        }

        /// <summary>
        /// Attempt to reset path when it ends for "hive behaviour" until other features implemented
        /// </summary>
        public void SetNewPath()
        {
            //URGENT _startnode THROWS NULL ERROR | Repeatable
            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];

            desiredPath = pathfinderInstance.FindPath(_startNode, _endNode);
            currentNodeIndex = 0;
            if (desiredPath != null)
            {
                //URGENT currentnode THROWS NULL ERROR | Repeatable
                currentNode = desiredPath[currentNodeIndex];
            }
            else
            {
                Debug.LogError("DesiredPath wasn't returned", this.gameObject);
            }
        }

        public bool IsStillTravelling()
        {
            if (currentNode == null)
            {
                Debug.Log("IsStillTravel check. currentNode == null", this.gameObject);
                return false;
            }

            if (Vector3.Distance(transform.position, currentNode.worldPos) <= stopDistance)
            {
                return true;
            }
            else
                return false;
        }

    }

}
