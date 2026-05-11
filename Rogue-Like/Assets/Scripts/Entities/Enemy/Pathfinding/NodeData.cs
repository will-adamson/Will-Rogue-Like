using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData
{
    public Vector2Int position;
    public float gCost;
    public float hCost;
    public float FCost => gCost + hCost;
    public Vector2Int parent;
}