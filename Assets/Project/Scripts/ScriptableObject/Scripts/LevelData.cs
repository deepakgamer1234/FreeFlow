using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "SO/Level")]
public class LevelData : ScriptableObject
{
    public string LevelName;
    public int levelNumber;
    public List<Edge> Edges;
}

[System.Serializable]
public struct Edge
{
   // public List<Vector2Int> Points;
    public Vector2Int StartPoint;
    public Vector2Int EndPoint;
}

