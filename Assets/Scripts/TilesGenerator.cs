using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGenerator : MonoBehaviour
{
    public GameObject tile;
    public GameObject floor;
    public GameObject[,] initTiles()
    {
        GameObject[,] tiles = new GameObject[9,9];

        float floorSizeX = floor.transform.localScale.x;
        float floorSizeY = floor.transform.localScale.y;

        float tileSpaceX = floorSizeX / 9;
        float tileSpaceY = floorSizeY / 9;

        float startPositionX = floor.transform.position.x - floor.transform.localScale.x / 2 + tileSpaceX / 2;
        float startPositionY = floor.transform.position.y + floor.transform.localScale.y / 2 - tileSpaceY / 2;

        float currentPositionX = startPositionX;
        float currentPositionY = startPositionY;

        for(int i = 0; i < 9; ++i) {
            currentPositionX = startPositionX;
            for(int j = 0; j < 9; ++j) {
                tiles[i,j] = Instantiate(tile, new Vector3(currentPositionX, currentPositionY, 0), Quaternion.identity);
                tiles[i,j].GetComponent<SelectSpot>().setTileNum(i * 9 + j);
                currentPositionX += tileSpaceX;
            }
            currentPositionY -= tileSpaceY;
        }

        return tiles;
    }
}
