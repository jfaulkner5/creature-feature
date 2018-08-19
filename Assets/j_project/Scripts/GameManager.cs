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

        #region PathData
        public Node[,] levelGrid;
        public List<Node> path;
        #endregion

        public Terrain terrain;


        [Header("Food Source Spawn details")]
        public int maxCandySpawn;
        public int maxMeatSpawn;



        [Header("Food Source Prefabs")]
        public GameObject candyPrefab;
        public GameObject meatPrefab;


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

        public Node NodeRandom()
        {
            int maxRand = myPathGrid.gridSize;
            Node TempNode = myPathGrid.levelGrid[Random.Range(1, maxRand), Random.Range(1, maxRand)];
            while (TempNode.isPassable == false)
            {
                TempNode = myPathGrid.levelGrid[Random.Range(1, maxRand), Random.Range(1, maxRand)];
            }
            return TempNode;
        }

        public bool IsOccupied(Vector3 tempPos)
        {
            foreach (GameObject pos in Nest.instance.candyList)
            {
                if (pos.transform.position == tempPos)
                {
                    return true;
                }
            }
            foreach (GameObject pos in Nest.instance.meatList)
            {
                if (pos.transform.position == tempPos)
                {
                    return true;
                }
            }

            return false;
        }

        public void SpawnCandy()
        {
            while (Nest.instance.candyList.Count <= maxCandySpawn)
            {
                Vector3 tempNodePos = NodeRandom().worldPos;
                //while (IsOccupied(tempNodePos))
                //{
                //    NodeRandom();
                //}

                GameObject tempFood = Instantiate(candyPrefab, tempNodePos, Quaternion.identity);
                Nest.instance.candyList.Add(tempFood);
            }

            while (Nest.instance.meatList.Count <= maxMeatSpawn)
            {
                Vector3 tempNodePos = NodeRandom().worldPos;

                GameObject tempFood = Instantiate(meatPrefab, tempNodePos, Quaternion.identity);
                Nest.instance.meatList.Add(tempFood);
            }
        }
    }
}
