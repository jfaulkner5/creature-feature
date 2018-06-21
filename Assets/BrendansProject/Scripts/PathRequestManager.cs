using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace BrendansProject
{
    /// <summary>
    /// Manages pathfinding requests to optimise multiple paths being calculated.
    /// </summary>
    public class PathRequestManager : MonoBehaviour
    {

        Queue<PathResult> results = new Queue<PathResult>(); // Create a Queue of path requests

        static PathRequestManager instance; // Create an static instance for this class
        PathfindingManager pathfinding;

        void Awake()
        {
            instance = this;
            pathfinding = GetComponent<PathfindingManager>();
        }

        void Update()
        {

            // Loop through Queue
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }
        }

        /// <summary>
        /// Request a path using threading.
        /// </summary>
        /// <param name="request"></param>
        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }


        /// <summary>
        /// Called by the pathfinding class once it is finished finding the path.
        /// </summary>
        /// <param name="result"></param>
        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }



    }

    /// <summary>
    /// Struct to hold the path result information.
    /// </summary>
    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
    }

    /// <summary>
    /// Struct to hold path request information used in RequestPath().
    /// </summary>
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}