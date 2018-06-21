﻿using System.Collections;
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
        Vector3 worldBotLeft;


        public void RunSetup()
        {
            gridWorldSize = new Vector2(20, 20);
            obstacleMask =~ LayerMask.NameToLayer("obstacleMask");
            nodeRad = 0.25f;

            nodeDiam = nodeRad * 2;
            gridSize = Mathf.RoundToInt(gridWorldSize.x / nodeDiam);

            worldBotLeft = new Vector3(-10,0,-10);
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

                    newLevelGrid[x, y] = new Node(passable, worldPoint, x, y);
                    //Debug.Log(string.Format("Node added to list with pos {0} and isPassable = {1}",worldPoint,passable));
                }
            }
            levelGrid = newLevelGrid;
            return newLevelGrid;
        }

        //TODO | Is it necessary to reverse from node to worldpoint?
        public Node ConvertFromWorldPoint(Vector3 worldPoint)
        {
            float posX = (worldPoint.x - /*transform.position.x*/ + gridWorldSize.x / 2) / gridWorldSize.x;
            float posY = (worldPoint.z - /*transform.position.z*/ + gridWorldSize.y / 2) / gridWorldSize.y;
            posX = Mathf.Clamp01(posX);
            posY = Mathf.Clamp01(posY);

            int x = Mathf.RoundToInt((gridSize - 1) * posX);
            int y = Mathf.RoundToInt((gridSize - 1) * posY);

            return levelGrid[x, y];
        }



    }

    
}
