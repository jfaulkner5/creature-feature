using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }



        PathGrid myPathGrid = new PathGrid();
        PathFinding myPathFinding = new PathFinding();
        
        #region PathData

        public Node[,] levelGrid;
        public List<Node> path;
        #endregion

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this);
                Debug.LogError("MULTIPLE GAMEMANAGERS IN SCENE");
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        // Use this for initialization
        void Start()
        {
            levelGrid = GenerateNodeGrid();
            path = FindPath();
        }

        // Update is called once per frame
        void Update()
        {


        }

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
                    Gizmos.DrawSphere(node.worldPos, (myPathGrid.nodeDiam / 2));
                }
            }
            else
            {
                Debug.Log("levelGrid is null");
            }

        }

        public Node[,] GenerateNodeGrid()
        {
            myPathGrid.RunSetup();
            return myPathGrid.GetGrid();
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// DEBUG Find a path, using a start and end point in the world.
        /// </summary>
        private List<Node> FindPath()
        {
            Vector3 _start = new Vector3(-10, 0, -10);
            Vector3 _end = new Vector3(10, 0, 10);
           return myPathFinding.FindPath(_start, _end);
        }
        
        /// <summary>
        ///  Find a path, using a start and end point in the world.
        /// </summary>
        /// <param name="_startPos"></param>
        /// <param name="_endPos"></param>
        public List<Node> FindPath(Vector3 _startPos, Vector3 _endPos)
        {
            return myPathFinding.FindPath(_startPos, _endPos);
        }

        /// <summary>
        /// Find a path, using a start and end <see cref="Node"/>
        /// </summary>
        /// <param name="_startPos"></param>
        /// <param name="_endPos"></param>
        public List<Node> FindPath(Node _startPos, Node _endPos)
        {
            return myPathFinding.FindPath(_startPos, _endPos);
        }

        public List<Node> GetNeighbourNodes(Node node)
        {
           List<Node> neighbourNodes = new List<Node>();
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

                    //URGENT unspaghetti
                    if (nCheckX >= 0 && nCheckX < myPathGrid.gridSize && nCheckY >= 0 && nCheckY < myPathGrid.gridSize)
                    {
                        neighbourNodes.Add(levelGrid[nCheckX, nCheckY]);
                    }
                }
            }
            return neighbourNodes;
        }

    }


}