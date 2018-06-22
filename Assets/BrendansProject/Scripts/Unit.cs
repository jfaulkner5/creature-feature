﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BrendansProject
{
    /// <summary>
    /// Class that controls the units movment along a path.
    /// </summary>
    public class Unit : MonoBehaviour
    {

        const float minPathUpdateTime = .2f;
        const float pathUpdateMoveThreshold = .5f;

        public Transform target;
        public float speed = 20;
        public float turnSpeed = 3;
        public float turnDst = 5;
        public float stoppingDst = 10;

        Path path;

        private void Start()
        {
            target = GetClosestEnemy(ProcGenerator.instance.targets); // Find the closest target
            print(target);
            StartCoroutine(UpdatePath());
        }

        /// <summary>
        /// Find the closest tranform to the unit from the provided list.
        /// </summary>
        /// <param name="enemies"></param>
        /// <returns></returns>
        Transform GetClosestEnemy(List<Transform> enemies)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Transform potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }


        /// <summary>
        /// If path has been found and sucessfully reaches the target follow the path.
        /// </summary>
        /// <param name="waypoints"></param>
        /// <param name="pathSuccessful"></param>
        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDst, stoppingDst);

                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        /// <summary>
        /// Ticks through path requests so it is not being called every frame.
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdatePath()
        {

            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }
            PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                //print(((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold); //Used to debug location
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    targetPosOld = target.position;
                }
            }
        }

        /// <summary>
        /// Moves the unit along the path using lines for smoother pathing.
        /// </summary>
        /// <returns></returns>
        IEnumerator FollowPath()
        {

            bool followingPath = true; // Tracks if the unit is following the path
            int pathIndex = 0;
            transform.LookAt(path.lookPoints[0]); // Face the first lookpoint

            float speedPercent = 1;

            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }

                if (followingPath)
                {

                    if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                        if (speedPercent < 0.01f)
                        {
                            followingPath = false;
                        }
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); // Rotate unit
                    transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self); // Move unit forward
                }

                yield return null;

            }
        }

        /// <summary>
        /// Draw the path waypoints in path
        /// </summary>
        public void OnDrawGizmos()
        {
            if (path != null)
            {
                path.DrawWithGizmos();
            }
        }
    }
}