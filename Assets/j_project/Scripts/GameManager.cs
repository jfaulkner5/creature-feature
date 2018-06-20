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

        }

        // Update is called once per frame
        void Update()
        {
            StartUpStuff();

        }

        public void StartUpStuff()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {

                Debug.Log("Create grid", this.gameObject);

            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                Debug.Log("find path triggered", this.gameObject);


            }
            if (Input.GetKey(KeyCode.Keypad3))
            {

            }
            if (Input.GetKey(KeyCode.Keypad4))
            {

            }

        }
    }
}