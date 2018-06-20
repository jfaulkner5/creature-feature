using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace jfaulkner
{
    [System.Serializable]
    public class PathGrid : MonoBehaviour
    {
        //HACK node generator instance
        public static PathGrid instance;

        //Filters the player level to make it easier to find ojects that are considered obsticles
        public LayerMask obstacleMask;

        public Vector2 gridWorldSize;
        public float nodeRad;
        public float nodeDiam;
        public int gridSize;
        public Node[,] levelGrid;
        public List<Node> path;

        //HACK erase afterwards
        public GameObject _gameObject;
        Vector3 testMin;
        //Vector3 testCenter;
        Vector3 worldBotLeft;

        private void OnDrawGizmos()
        {
            if (levelGrid != null)
            {

                foreach (Node node in levelGrid)
                {
                    Gizmos.color = (node.isPassable) ? Color.grey : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(node))
                        {
                            Gizmos.color = Color.green;
                        }
                    }
                    Gizmos.DrawSphere(node.worldPos, (nodeDiam / 2));
                }
            }

        }


        public void Start()
        {
            nodeDiam = nodeRad * 2;
            gridSize = Mathf.RoundToInt(gridWorldSize.x / nodeDiam);

            //HACK stuff for test
            testMin = _gameObject.GetComponent<Collider>().bounds.min;
            //testCenter = _gameObject.GetComponent<Collider>().bounds.center;
            worldBotLeft = testMin;
            Debug.Log("Unity Editor");

            CreateGrid();

            Vector3 _start = new Vector3(-10, 0, -10);
            Vector3 _end = new Vector3(10, 0, 10);
            NewMethod(_start, _end);
        }

        private static void NewMethod(Vector3 _start, Vector3 _end)
        {
            PathFinding.instance.FindPath(_start, _end);
        }


        //TODO call from game manager only
        public void CreateGrid()
        {
            levelGrid = new Node[gridSize, gridSize];
            //Vector3 worldBotLeft = new Vector3(0, 0, 0);

            //TODO add in override for unknown grid size, where start/end point are. q
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiam + nodeRad) + Vector3.forward * (y * nodeDiam + nodeRad);
                    bool passable = !(Physics.CheckCapsule(worldPoint, worldPoint + new Vector3(0, 1, 0), nodeRad, obstacleMask));
                    //List<Node> neighbours = GetNeighbourNodes(levelGrid[x,y]);

                    levelGrid[x, y] = new Node(passable, worldPoint, x, y);
                }
            }
        }

        //TODO | Is it necessary to reverse from node to worldpoint?
        public Node ConvertFromWorldPoint(Vector3 worldPoint)
        {
            float posX = (worldPoint.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float posY = (worldPoint.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
            posX = Mathf.Clamp01(posX);
            posY = Mathf.Clamp01(posY);

            int x = Mathf.RoundToInt((gridSize - 1) * posX);
            int y = Mathf.RoundToInt((gridSize - 1) * posY);

            return levelGrid[x, y];
        }



    }

    public class PathFinding
    {
        //HACK fix singleton
        public static PathFinding instance;

        public List<Node> neighbourNodes;

        public List<Node> FindPath(Vector3 startPos, Vector3 finishPos)
        {
            Node startNode = PathGrid.instance.ConvertFromWorldPoint(startPos);
            Node endNode = PathGrid.instance.ConvertFromWorldPoint(finishPos);

            return FindPath(startNode, endNode);
        }

        public List<Node> FindPath(Node startNode, Node endNode)
        {

            List<Node> openNodeList = new List<Node>();
            List<Node> closedNodeList = new List<Node>();
            List<Node> cameFrom = new List<Node>();

            //NOTE not neccesary unless there is value per path

            openNodeList.Add(startNode);

            Node currentNode = openNodeList[0];

            while (openNodeList != null && openNodeList.Count != 0)
            {


                //HELP Unsure if tests prove difference or needs to be kept outside the while loop
                //Node currentNode = openSet[0];
                for (int index = 1; index < openNodeList.Count; index++)
                {
                    Debug.Log(index);

                    if (openNodeList[index].F < currentNode.F || openNodeList[index].F == currentNode.F && openNodeList[index].H < currentNode.H)
                    {
                        currentNode = openNodeList[index];


                    }

                }

                openNodeList.Remove(currentNode);
                closedNodeList.Add(currentNode);

                if (currentNode == endNode)
                {

                    currentNode = endNode;

                    while (currentNode != startNode)
                    {
                        cameFrom.Add(currentNode);
                        currentNode = currentNode.parent;
                    }
                    cameFrom.Reverse();
                    return cameFrom;
                }

                foreach (Node neighbourNode in GetNeighbourNodes(currentNode))
                {

                    if (!neighbourNode.isPassable || closedNodeList.Contains(neighbourNode))
                    {
                        continue;
                    }

                    float tempCost = currentNode.G + GetDistance(currentNode, neighbourNode);

                    if (tempCost < neighbourNode.G || !openNodeList.Contains(neighbourNode))
                    {
                        //URGENT fix code to be more fluid
                        neighbourNode.G = tempCost;
                        neighbourNode.H = GetDistance(neighbourNode, endNode);
                        neighbourNode.parent = currentNode;

                        if (!openNodeList.Contains(neighbourNode))
                        {
                            openNodeList.Add(neighbourNode);
                        }
                    }


                }


            }

            return null;
        }

        //private static int GetDistance(Node currentNode, Node neighbour)
        //{
        //    throw new NotImplementedException();
        //}

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
            int dstY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * dstX - dstY;
            }
            else
            {
                return 14 * dstX + 10 * dstY - dstX;
            }
        }

        List<Node> GetNeighbourNodes(Node node)
        {
            neighbourNodes = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int nCheckX = node.gridPosX + x;
                    int nCheckY = node.gridPosY + y;

                    if (nCheckX >= 0 && nCheckX < PathGrid.instance.gridSize && nCheckY >= 0 && nCheckY < PathGrid.instance.gridSize)
                    {
                        neighbourNodes.Add(PathGrid.instance.levelGrid[nCheckX, nCheckY]);
                    }
                }
            }
            return neighbourNodes;
        }

    }
}
