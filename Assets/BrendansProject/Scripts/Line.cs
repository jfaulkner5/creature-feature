﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// Used for shortcutting between waypoints and also displaying line gizmos.
    /// </summary>
    public struct Line
    {

        const float verticalLineGradient = 1e5f;

        float gradient;
        float y_intercept;
        Vector2 pointOnLine_1;
        Vector2 pointOnLine_2;

        float gradientPerpendicular;

        bool approachSide;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointOnLine"></param>
        /// <param name="pointPerpendicularToLine"></param>
        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            float dx = pointOnLine.x - pointPerpendicularToLine.x;
            float dy = pointOnLine.y - pointPerpendicularToLine.y;

            if (dx == 0)
            {
                gradientPerpendicular = verticalLineGradient;
            }
            else
            {
                gradientPerpendicular = dy / dx;
            }

            if (gradientPerpendicular == 0)
            {
                gradient = verticalLineGradient;
            }
            else
            {
                gradient = -1 / gradientPerpendicular;
            }

            y_intercept = pointOnLine.y - gradient * pointOnLine.x;
            pointOnLine_1 = pointOnLine;
            pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

            approachSide = false;
            approachSide = GetSide(pointPerpendicularToLine);
        }

        /// <summary>
        /// Determine which side of the line the vector2 is on
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool GetSide(Vector2 p)
        {
            return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
        }

        /// <summary>
        /// Check is has crossed a line
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != approachSide;
        }

        /// <summary>
        /// Float that returns the distance from a point to the next intersect
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float DistanceFromPoint(Vector2 p)
        {
            float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
            float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
            float intersectY = gradient * intersectX + y_intercept;
            return Vector2.Distance(p, new Vector2(intersectX, intersectY));
        }

        /// <summary>
        /// Draw line gizmos
        /// </summary>
        /// <param name="length"></param>
        public void DrawWithGizmos(float length)
        {
            Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCentre = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
            Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
        }

    }
}