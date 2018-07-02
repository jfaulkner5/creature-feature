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

        public PathGrid myPathGrid = new PathGrid();
        public PathFinding myPathFinding = new PathFinding();

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
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Keypad1))
            {
                Node _startNode = levelGrid[0, 0];
                Node _endNode = levelGrid[Random.Range(10, 39), Random.Range(10, 39)];
                path = FindPath(_startNode, _endNode);

                myPathGrid.Cleanup();
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {

            }

        }

        /// <summary>
        /// Drawing the level grid for Debug purposes
        /// </summary>
        private void OnDrawGizmosSelected()
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

        /// <summary>
        /// returns a Node that corresponds to a world posistion
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public Node ConvertFromWorldPoint(Vector3 worldPoint)
        {
            Vector3 _nodepoint = (worldPoint - myPathGrid.worldBotLeft);// / (myPathGrid.nodeDiam + myPathGrid.nodeRad);
            int x = Mathf.RoundToInt((_nodepoint.x - myPathGrid.nodeRad) / myPathGrid.nodeDiam);
            int y = Mathf.RoundToInt((_nodepoint.z - myPathGrid.nodeRad) / myPathGrid.nodeDiam);

            return levelGrid[x, y];

        }

    }
}
