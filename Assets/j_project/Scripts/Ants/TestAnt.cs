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

        // Use this for initialization
        void Start()
        {
            stopDistance = 0.2f;
        }

        // Update is called once per frame
        void Update()
        {
            TestTravel();

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

            Node _startNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];
            Node _endNode = GameManager.Instance.levelGrid[Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1)), Random.Range(0, (GameManager.Instance.myPathGrid.gridSize - 1))];

            desiredPath = GameManager.Instance.FindPath(_startNode, _endNode);
            antState = AntState.Travel;
        }

        public void Travel()
        {
            currentNode = desiredPath[0];
            int currentNodeIndex = 0;
            while (currentNode != desiredPath[desiredPath.Count - 1])
            {
                Vector3.MoveTowards(transform.position, currentNode.worldPos, Time.deltaTime);
                if (Vector3.Distance(transform.position, currentNode.worldPos) <= stopDistance)
                {
                    currentNodeIndex++;
                    currentNode = desiredPath[currentNodeIndex];
                }

            }
            throw new System.ArgumentException("Reached desired path");
        }

    }

}