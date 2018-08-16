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
            Returning
        }
        AntState antState;

        public PathFinding pathfinderInstance;
        public int travelSpeed;

        public List<Node> desiredPath;
        public Node currentNode;
        public Node endNode;
        float stopDistance;
        int currentNodeIndex;

        bool isInit = false;

        //Food collection
        public bool isReturning = false,
            hasFood = false;
        public Nest.FoodType foodType;

        void Start()
        {
            stopDistance = 0.2f;
            currentNodeIndex = 0;
            travelSpeed = 1;


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
                case AntState.Travel:
                    Travel();

                    break;

                case AntState.Returning:

                    break;
                default:

                    break;
            }

        }

        //public void FirstTravel()
        //{

        //    while (GameManager.Instance.levelGrid == null)
        //    {
        //        Debug.Log("Waiting for level grid to generate");

        //    }

        //    Debug.Log("LevlGrid generated, starting pathfinder", this.gameObject);


        //    Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
        //    Node _endNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];

        //    desiredPath = pathfinderInstance.FindPath(_startNode, _endNode);
        //    currentNodeIndex = 0;

        //    if (desiredPath != null)
        //    {
        //        currentNode = desiredPath[currentNodeIndex];
        //        antState = AntState.Travel;
        //    }
        //    else if (desiredPath == null)
        //    {
        //        Debug.LogError("Desired path is null on first attempt", this.gameObject);

        //        Invoke("SetNewPath", 5);
        //    }

        //    isInit = !isInit;
        //}

        public void Travel()
        {

            if (currentNode != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentNode.worldPos, Time.deltaTime * travelSpeed);
                transform.LookAt(new Vector3(currentNode.worldPos.x, transform.position.y, currentNode.worldPos.z));

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

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == transform.parent && isReturning == true)
            {
                Nest.instance.FoodCollection(foodType);
            }

            if (other.CompareTag("Candy"))
            {
                hasFood = true;
                foodType = Nest.FoodType.SugaryFood;
                Destroy(other.gameObject);
            }
            if (other.CompareTag("Meat"))
            {
                hasFood = true;
                foodType = Nest.FoodType.FattyFood;
                Destroy(other.gameObject);
            }
        }

        /// <summary>
        /// Order from the nest to find new food.
        /// </summary>
        /// <param name="foodVec"></param>
        public void NextFood(Vector3 foodVec)
        {
            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.ConvertFromWorldPoint(foodVec);
            desiredPath = pathfinderInstance.FindPath(_startNode, _endNode);
            if (desiredPath != null)
            {

            }
            else
            {
                Debug.LogError("Order failed. path is null", this);

            }
        }


    }

}
