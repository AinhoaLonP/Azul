using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotPlacedRow : MonoBehaviour
{
    public int totalTiles;
    public int AvailableTiles { get; set; }

    private void Start()
    {
        AvailableTiles = totalTiles;
    }
}
