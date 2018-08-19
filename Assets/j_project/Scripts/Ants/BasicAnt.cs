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
            StartUp,
            Travel,
            Returning,

            PickUpFood,
            DropOfFood //dropping off food at the nest
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
        bool hasOrders = false;
        //Food collection
        public bool isReturning = false,
            hasFood = false;
        public Nest.FoodType foodType;
        private GameObject foodObj;


        public float distanceTravelled;
        public Vector3 distCheckStartVec,
            distCheckEndVec;

        public GameObject targetFood;
        void Start()
        {
            stopDistance = 0.2f;
            currentNodeIndex = 0;
            travelSpeed = 1;

        }

        public void OnDrawGizmos()
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
                    ReturnToNest();
                    break;
                case AntState.PickUpFood:
                    StartCoroutine(PickingUpFood());
                    break;
                case AntState.DropOfFood:
                    WaitingState();
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
                antState = AntState.DropOfFood;
            }

            if (other.gameObject == targetFood)
            {
                if (other.CompareTag("Candy"))
                {
                    hasFood = true;
                    foodType = Nest.FoodType.SugaryFood;
                    foodObj = other.gameObject;
                    distCheckEndVec = transform.position;
                    CalcDistanceTravelled();
                    antState = AntState.PickUpFood;

                }
                if (other.CompareTag("Meat"))
                {
                    hasFood = true;
                    foodType = Nest.FoodType.FattyFood;
                    foodObj = other.gameObject;
                    distCheckEndVec = transform.position;
                    CalcDistanceTravelled();

                    antState = AntState.PickUpFood;
                }
            }
        }

        /// <summary>
        /// Order from the nest to find new food.
        /// </summary>
        /// <param name="foodTarget"></param>
        public void NextFood(GameObject foodTarget)
        {
            targetFood = foodTarget;
            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.ConvertFromWorldPoint(foodTarget.transform.position);
            desiredPath = pathfinderInstance.FindPath(_startNode, _endNode);
            if (desiredPath != null)
            {

            }
            else
            {
                while (desiredPath == null)
                {
                    Debug.LogError("Order failed. path is null", this);
                    desiredPath = RetryPath(_startNode, _endNode);

                }
            }
            hasOrders = true;
            antState = AntState.Travel;
            distCheckStartVec = transform.position;
            currentNodeIndex = 0;
            currentNode = desiredPath[currentNodeIndex];
        }

        public void WaitingState()
        {
            while (!hasOrders)
            {
                Debug.Log("waiting for a new state");
            }
        }

        public IEnumerator PickingUpFood()
        {
            yield return new WaitForSecondsRealtime(5f);
            Destroy(foodObj);

            //travel to nest code
            antState = AntState.Returning;
        }

        /// <summary>
        /// Called upon need to return to nest
        /// </summary>
        public void ReturnToNest()
        {
            distCheckStartVec = transform.position;

            Node _startNode = GameManager.Instance.ConvertFromWorldPoint(transform.position);
            Node _endNode = GameManager.Instance.ConvertFromWorldPoint(Nest.instance.gameObject.transform.position);
            desiredPath = pathfinderInstance.FindPath(_startNode, _endNode);
            if (desiredPath != null)
            {

            }
            else
            {
                Debug.LogError("Order failed. path is null", this);
                desiredPath = RetryPath(_startNode, _endNode);
            }

            currentNodeIndex = 0;
            currentNode = desiredPath[currentNodeIndex];

            hasOrders = !hasOrders;
            antState = AntState.Travel;

        }

        /// <summary>
        /// Auto pathfinder retry check
        /// </summary>
        /// <param name="_startNode"> starting pos of entity.</param>
        /// <param name="_endNode"> Desired target of the path.</param>
        /// <returns></returns>
        public List<Node> RetryPath(Node _startNode, Node _endNode)
        {
            List<Node> returnPath = pathfinderInstance.FindPath(_startNode, _endNode);
            if (returnPath == null)
            {
                returnPath = RetryPath(_startNode, _endNode);
            }

            return returnPath;
        }

        public void CalcDistanceTravelled()
        {
            distanceTravelled += Vector3.Distance(distCheckStartVec, distCheckEndVec);
        }

        public float ArrivedAtNest()
        {
            distCheckEndVec = transform.position;
            CalcDistanceTravelled();

            return distanceTravelled;
        }
    }

}
