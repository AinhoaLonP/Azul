using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject blueTile;
    public GameObject yellowTile;
    public GameObject redTile;
    public GameObject blackTile;
    public GameObject greenTile;

    public GameObject plate1Panel;
    public GameObject plate2Panel;
    public GameObject plate3Panel;
    public GameObject plate4Panel;
    public GameObject plate5Panel;

    public GameObject player1Canvas;
    public GameObject inTableTilesCanvas;

    private static int numberOfPlayers = 2;
    private List<Tile> tiles = new List<Tile>();
    private Plate[] plates = new Plate[numberOfPlayers * 2 + 1];

    private List<GameObject> chosenTiles = new List<GameObject>();
    private List<GameObject> notChosenTiles = new List<GameObject>();

    void Start()
    {
        // Initialize and assign a panel GameObject to each plate
        InitializePlates();

        // Fill tiles list with 20 of each color
        for (int c = 1; c <= Enum.GetNames(typeof(TileColor)).Length; c++)
        {
            for (int i = 0; i < 20; i++)
            {
                Tile tile = new Tile();
                tile.Color = (TileColor)c;
                tiles.Add(tile);
            }
        }

        // Fill the plates
        FillPlates();
    }

    /// <summary>
    /// Initialize and assign a panel GameObject to each plate
    /// </summary>
    private void InitializePlates()
    {
        for (int i = 0; i < plates.Length; i++)
        {
            plates[i] = new Plate();
        }
        plates[0].Panel = plate1Panel;
        plates[1].Panel = plate2Panel;
        plates[2].Panel = plate3Panel;
        plates[3].Panel = plate4Panel;
        plates[4].Panel = plate5Panel;
    }

    /// <summary>
    /// Fills each plate with 4 random tiles
    /// </summary>
    private void FillPlates()
    {
        foreach (Plate plate in plates)
        {
            for (int i = 0; i < Plate.tilesInPlate; i++)
            {
                Tile tile = ExtractRandomTile();
                plate.tiles[i] = tile;

                // Returns the proper tile GameObject based on its color
                GameObject tileGO = GetProperTileGO(tile);

                // Instantiate the tile GameObject
                GameObject instantiatedTileGO = plate.Panel.GetComponent<TileGeneratorLogic>().GenerateTile(tileGO);
                instantiatedTileGO.GetComponent<Tile>().Color = tile.Color;

                // We put a listener to the tile so that the method is called when we click on it.
                // It must be done with a listener because prefabs don't admit onClick(), and with "delegate" to be able to pass the parameter
                instantiatedTileGO.GetComponent<Button>().onClick.AddListener(delegate { SelectAndPlaceTiles(plate, instantiatedTileGO); });
            }
        }
    }

    /// <summary>
    /// Returns the proper tile GameObject based on its color
    /// </summary>
    /// <param name="tile"></param>
    /// <returns>The tile GameObject</returns>
    private GameObject GetProperTileGO(Tile tile)
    {
        //TODO get rid of the ifs
        if (tile.Color == TileColor.Black)
            return blackTile;
        else if (tile.Color == TileColor.Blue)
            return blueTile;
        else if (tile.Color == TileColor.Yellow)
            return yellowTile;
        else if (tile.Color == TileColor.Red)
            return redTile;
        else
            return greenTile;
    }

    /// <summary>
    /// Extract a random tile from the tiles list
    /// </summary>
    /// <returns>The extracted tile</returns>
    private Tile ExtractRandomTile()
    {
        Tile selectedTile = null;
        System.Random random = new System.Random();
        if (tiles.Count > 0)
        {
            int index = random.Next(tiles.Count);
            selectedTile = tiles[index];
            tiles.RemoveAt(index);
        }
        return selectedTile;
    }

    /// <summary>
    /// Selects and places the choosen tiles
    /// </summary>
    /// <param name="plate"></param>
    /// <param name="selectedTile"></param>
    private void SelectAndPlaceTiles(Plate plate, GameObject selectedTile)
    {
        // Select all the tiles with the same color in a plate
        chosenTiles = SelectTile(plate, selectedTile);

        // Gets the available rows to place the selected tiles
        Transform notPlacedTiles = player1Canvas.transform.Find("NotPlacedTiles");
        GameObject row;
        List<GameObject> elegibleRows = new List<GameObject>();
        for (int i = 0; i < notPlacedTiles.childCount; i++)
        {
            row = notPlacedTiles.GetChild(i).gameObject;
            if (IsElegible(row))
            {
                row.GetComponent<Button>().interactable = true;
                //TODO change this
                row.GetComponent<Image>().color = Color.red;
                elegibleRows.Add(row);
            }
        }

        // Add a listener to the elegible rows
        foreach (GameObject r in elegibleRows)
        {
            r.GetComponent<Button>().onClick.AddListener(delegate { PlaceTiles(r); });
        }
    }

    /// <summary>
    /// Selects a tile and all the other tiles with the same color
    /// </summary>
    /// <param name="plate"></param>
    /// <param name="selectedTile"></param>
    private List<GameObject> SelectTile(Plate plate, GameObject selectedTile)
    {
        List<GameObject> sameColorTiles = new List<GameObject>();

        // Get tile color
        TileColor selectedTileColor = selectedTile.GetComponent<Tile>().Color;

        // For each tile inside the panel, check if it has the same color. If so, add it to the list.
        foreach (Transform child in plate.Panel.transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
            {
                if (child.gameObject.GetComponent<Tile>().Color == selectedTileColor)
                    sameColorTiles.Add(child.gameObject);
                else
                    notChosenTiles.Add(child.gameObject);
            }
        }

        return sameColorTiles;
    }

    /// <summary>
    /// Determines if a row is elegible to place the chosen tiles
    /// </summary>
    /// <param name="row"></param>
    /// <param name="chosenTiles"></param>
    /// <returns></returns>
    private bool IsElegible(GameObject row)
    {
        //TODO
        return true;
    }

    /// <summary>
    /// Places the chosen tiles in the chosen row
    /// </summary>
    /// <param name="row"></param>
    private void PlaceTiles(GameObject row)
    {
        List<GameObject> excessTiles = new List<GameObject>();
        NotPlacedRow rowScript = row.GetComponent<NotPlacedRow>();
        // If there is enough space available, place all the tiles. Otherwise, send some of them to the excess panel.
        if (rowScript.AvailableTiles >= chosenTiles.Count)
            rowScript.AvailableTiles -= chosenTiles.Count;
        else
        {
            rowScript.AvailableTiles = 0;
            for (int i = 0; i < chosenTiles.Count - rowScript.AvailableTiles; i++)
            {
                excessTiles.Add(chosenTiles[0]);
                chosenTiles.RemoveAt(0);
            }
        }
        // Disable the button
        if (rowScript.AvailableTiles == 0)
        {
            row.GetComponent<Button>().interactable = false;
            //TODO change this
            row.GetComponent<Image>().color = Color.blue;
        }
        // Remove blank tiles and add the chosen tiles
        for (int i = 0; i < chosenTiles.Count; i++)
        {
            // Delete a blank tile
            row.transform.GetChild(0).parent = null;
        }
        for (int i = 0; i < chosenTiles.Count; i++)
        {
            // Place the chosen tile
            GameObject newTile = Instantiate(chosenTiles[0]);
            newTile.transform.SetParent(row.transform);
            newTile.transform.localScale = new Vector3(1, 1, 1);
            chosenTiles[i].transform.parent = null;
        }

        DiscardNotChosenTiles();
        PlaceExcessTiles(excessTiles);

        chosenTiles = new List<GameObject>();
    }

    /// <summary>
    /// Put the not chosen tiles on the table
    /// </summary>
    void DiscardNotChosenTiles()
    {
        /*
        for (int i = 0; i < notChosenTiles.Count; i++)
        {
            inTableTilesCanvas.transform.GetChild(0).parent = null;
        }
        */

        for (int i = 0; i < notChosenTiles.Count; i++)
        {
            GameObject newTile = Instantiate(notChosenTiles[i]);
            newTile.transform.SetParent(inTableTilesCanvas.transform);
            newTile.transform.localScale = new Vector3(1, 1, 1);
            chosenTiles[i].transform.parent = null;
        }

        notChosenTiles = new List<GameObject>();
    }

    /// <summary>
    /// Places the excess tiles
    /// </summary>
    /// <param name="excessTiles"></param>
    void PlaceExcessTiles(List<GameObject> excessTiles)
    {
        //TODO
    }

}
