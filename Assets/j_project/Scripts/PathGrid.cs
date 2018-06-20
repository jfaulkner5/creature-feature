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

        public void Awake()

        public void Awake()
        {
            nodeDiam = nodeRad * 2;
            gridSize = Mathf.RoundToInt(gridWorldSize.x / nodeDiam);

#if UNITY_EDITOR
            //hacky stuff for test
            testMin = _gameObject.GetComponent<Collider>().bounds.min;
            //testCenter = _gameObject.GetComponent<Collider>().bounds.center;
            worldBotLeft = testMin;
            Debug.Log("Unity Editor");
#endif
            CreateGrid();
            path = GetPath();


        }

        //TODO call from game manager only
        public void CreateGrid()
        {
            levelGrid = new Node[gridSize, gridSize];
            //Vector3 worldBotLeft = new Vector3(0, 0, 0);



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

        //HELP shouldn't this be in the pathfinding system? 
        public List<Node> GetPath()
        {
            return PathFinding.FindPath(levelGrid[0,0],levelGrid[7,5]);
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
}
