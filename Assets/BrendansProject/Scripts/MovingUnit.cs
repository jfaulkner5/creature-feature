using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BrendansProject
{
    /// <summary>
    /// Class that controls the units movment along a path.
    /// </summary>
    public class MovingUnit : Unit
    {

        const float minPathUpdateTime = .2f;
        const float pathUpdateMoveThreshold = .5f;
               

        public Transform target;
        public float speed = 3;
        public float turnSpeed = 3;
        public float turnDst = 1;
        public float stoppingDst = 0.25f;

        [HideInInspector] public bool followingPath = false;
        public bool finalLocation = false;
        [HideInInspector] public bool updatingPath = false;

        Path path;
             

        //private void Update()
        //{

        //    if (!finalLocation) //Leave update if not doing final rotation
        //        return;


            
        //    //if (Vector3.Angle(transform.forward, target.transform.position - transform.position) > 0.1f) // Check if angle is greater than 1 degree
        //    //{

        //    //    Vector3 relativeTargetPosition = new Vector3(target.position.x, transform.position.y, target.position.z); // Prevent from looking up or down

        //    //    Quaternion endRotation = Quaternion.LookRotation(relativeTargetPosition - transform.position);

        //    //    transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * turnSpeed); // Rotate unit towards target
        //    //    //transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, turnSpeed);

        //    //    //print("rotating");
        //    //}

        //}


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

                // Stop previous follow path and follow path with new path
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
        

        private Vector3 TargetPos
        {
            get
            {
                if (target.gameObject.CompareTag("Building"))
                {
                    return target.gameObject.GetComponent<Unit>().targetPos;
                }
                else
                {
                    return target.position;
                }
            }
        }

        /// <summary>
        /// Ticks through path requests so it is not being called every frame.
        /// </summary>
        /// <returns></returns>
        public IEnumerator UpdatePath()
        {


            updatingPath = true;
            //Debug.Log("Updating");


            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }


            // Check if target is a building then goto targetPos instead of target.position
            //Vector3 _targetPos;

            //if (target.gameObject.CompareTag("Building"))
            //{
            //    _targetPos = target.gameObject.GetComponent<Unit>().targetPos;
            //}
            //else
            //{
            //    _targetPos = target.position;
            //}

            PathRequestManager.RequestPath(new PathRequest(transform.position, TargetPos, OnPathFound));
            //PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = TargetPos;
            //Vector3 targetPosOld = target.position;


            //TODO check for a building change targetpos instead of target.position
            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);

                //print(((TargetPos - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold) ; //Used to debug location


                if ((TargetPos - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                //if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    //PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    PathRequestManager.RequestPath(new PathRequest(transform.position, TargetPos, OnPathFound));
                    targetPosOld = TargetPos;
                    //targetPosOld = target.position;
                }
            }
        }

        /// <summary>
        /// Moves the unit along the path using lines for smoother pathing.
        /// </summary>
        /// <returns></returns>
        IEnumerator FollowPath()
        {

            followingPath = true; // Tracks if the unit is following the path
            //Debug.Log("Moving location");
            finalLocation = false;


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

                    Quaternion targetRotation;

                    //TODO test not going into buildings
                    //// if path is NEAR finished rotate towards the target
                    if (speedPercent < 1f)
                    {
                        Vector3 relativeTargetPosition = new Vector3(target.position.x, transform.position.y, target.position.z); // Prevent from looking up or down
                        targetRotation = Quaternion.LookRotation(relativeTargetPosition - transform.position);
                    }
                    else
                    {
                        targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                    }
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); // Rotate unit
                    transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self); // Move unit forward
                    
                }



                yield return null;
            }

            // When arrave at last node do final checks

            //Debug.Log("Arrived at final location");
            finalLocation = true;
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

        private void OnDisable()
        {
            StopAllCoroutines();
        }

    }
}