
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileColor Color { get; set; }
}

public enum TileColor
{
    Blue = 1,
    Yellow = 2,
    Red = 3,
    Black = 4,
    Green = 5
}
