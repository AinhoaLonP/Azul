using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneratorLogic : MonoBehaviour
{
    /// <summary>
    /// Instantiates the GameObject tile
    /// </summary>
    /// <param name="tile"></param>
    public GameObject GenerateTile(GameObject tile)
    {
        GameObject newTile = Instantiate(tile);
        newTile.transform.SetParent(transform);
        newTile.transform.localScale = new Vector3(1, 1, 1);

        return newTile;
    }
}
