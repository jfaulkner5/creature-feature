using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace jfaulkner
{
    [System.Serializable]
    public class PathGrid
    {
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
        public Vector3 worldBotLeft;


        public void RunSetup()
        {
            //gridWorldSize = new Vector2(40, 40);
            //Hack: ---------------------vv -- vv ew
            gridWorldSize = new Vector2(100, 100);
            obstacleMask = ~LayerMask.NameToLayer("obstacleMask");
            //nodeRad = 0.25f;
            // hack: eww -v
            nodeRad = 0.5f;


            nodeDiam = nodeRad * 2;
            gridSize = (int)gridWorldSize.x;

            //worldBotLeft = new Vector3(-50, 0, -50);
        }

        //TODO delete later if needed
        //private void NewMethod(Vector3 _start, Vector3 _end)
        //{
        //    myPathFinding.FindPath(_start, _end);
        //}


        public Node[,] GetGrid()
        {
            DebugTool.DLog("Get Grid was called");

            Node[,] newLevelGrid = new Node[gridSize, gridSize];

            //TODO add in override for unknown grid size, where start/end point are. q
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiam + nodeRad) + Vector3.forward * (y * nodeDiam + nodeRad);
                    bool passable = !(Physics.CheckCapsule(worldPoint, worldPoint + new Vector3(0, 1, 0), nodeRad, obstacleMask));


                    //Do raycast here. 
                    int layerMask = 1 << 10;
                    layerMask = ~LayerMask.NameToLayer("terrain");
                    RaycastHit altitudeData;
                   
                    Vector3 rayOrigin = new Vector3(worldPoint.x, worldPoint.y + 200, worldPoint.z);
                    Vector3 rayEnd = new Vector3(0,-1);
                    if(Physics.Raycast(rayOrigin, rayEnd, out altitudeData, Mathf.Infinity, layerMask))
                    {
                        Debug.Log(altitudeData.point);
                    }
                    worldPoint.y = altitudeData.point.y;
                    
                    newLevelGrid[x, y] = new Node(passable, worldPoint, x, y);
                    //Debug.Log(string.Format("Node added to list with pos {0} and isPassable = {1}",worldPoint,passable));
                }
            }
            levelGrid = newLevelGrid;
            return newLevelGrid;
        }

        public void Cleanup()
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    levelGrid[x, y].Cleanup();
                    //Should null out all the data relevant to pathfinding
                    //Debug.Log("gridpos " + levelGrid[x, y].gridPosX + " " + levelGrid[x, y].gridPosY);
                }
            }

        }



    }


}
