using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class Node
    {
        public Node parent;
        public bool isPassable;
        public Vector3 worldPos;

        public float Xpos { get { return worldPos.x; } }
        public float Ypos { get { return worldPos.y; } }
        public int gridPosX;
        public int gridPosY;

        public double G { get; set; }               // Distance from start to this node
        public double H { get; set; }               // Heuristic (guess) distance from this node to goal
        public double F { get; set; }               // Distance from start + heuristic distance


        public List<Node> neighbors = new List<Node>();         // All succesors to node

        public Node(bool _isPassable, Vector3 _worldPos, int _gridPosX, int _grixPosY)
        {
            isPassable = _isPassable;
            worldPos = _worldPos;
            gridPosX = _gridPosX;
            gridPosY = _grixPosY;
        }

        public void Calculate_G_Cost(Node other)
        {
            G = other.GetG() + Math.Sqrt((Xpos - other.Xpos) * (Xpos - other.Xpos) + (Ypos - other.Ypos) * (Ypos - other.Ypos));
        }

        public double GetG()
        {
            return G;
        }

        public double GetH()
        {
            return H;
        }


    }
}