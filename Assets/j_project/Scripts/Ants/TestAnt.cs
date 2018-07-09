using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class TestAnt : MonoBehaviour
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

        List<Node> desiredPath;
        Node currentNode;
        float stopDistance;
        int currentNodeIndex;


        void Start()
        {
            stopDistance = 0.2f;
            currentNodeIndex = 0;

            TestTravel();

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

        public void TestTravel()
        {

            //Node _startNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];
            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];

            desiredPath = GameManager.Instance.FindPath(_startNode, _endNode);
            currentNodeIndex = 0;

            if (desiredPath != null)
            {
                currentNode = desiredPath[currentNodeIndex];
                antState = AntState.Travel;
            }
        }

        public void Travel()
        {

            if (currentNode != null && desiredPath != null && IsStillTravelling())
                return;

                if(currentNode == null)
                  SetNewPath();
            //while (currentNode != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentNode.worldPos, Time.deltaTime);
                if (Vector3.Distance(transform.position, currentNode.worldPos) <= stopDistance)
                {
                    currentNodeIndex++;
                    if (currentNodeIndex >= desiredPath.Count)
                    {
                        currentNode = null;
                        //URGENT doesn't seem to fire properly
                        //SetNewPath();
                    }
                    else
                    {
                        currentNode = desiredPath[currentNodeIndex];
                    }
                }

            }

        }

        /// <summary>
        /// Attempt to reset path when it ends for "hive behaviour" until other features implemented
        /// </summary>
        public void SetNewPath()
        {
            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];

            desiredPath = GameManager.Instance.FindPath(_startNode, _endNode);
            currentNodeIndex = 0;
            currentNode = desiredPath[currentNodeIndex];
        }

        public bool IsStillTravelling()
        {

            if (Vector3.Distance(transform.position, currentNode.worldPos) <= stopDistance)
            {
                return true;
            }
            else
                return false;
        }

    }

}
