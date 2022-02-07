using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FlowerHandler : MonoBehaviour
{
    private List<int> unusedTiles = new List<int>();
    private GameObject[,] tiles;
    public GameObject[] flowersPrefabs = new GameObject[7];
    private GameObject[,] flowers = new GameObject[9, 9];
    private GameObject[] nextThreeFlowers = new GameObject[3];

    public static readonly int SAME_SPOT = 0;
    public static readonly int OCCUPIED = 1;
    public static readonly int NO_PATH = 2;
    public static readonly int SUCCESS = 3;
    public static readonly int DESTROYED_FLOWERS = 4;

    private GameFlow gameFlow;
    public void init(GameObject[,] tiles)
    {
        gameFlow = GetComponent<GameFlow>();
        for (int i = 0; i < 81; ++i)
            unusedTiles.Add(i);

        this.tiles = tiles;

        chooseNextThreeFlowers();
        placeNextThreeFlowers();
        chooseNextThreeFlowers();
    }
    public void manualUpdate()
    {
        placeNextThreeFlowers();
        chooseNextThreeFlowers();
    }

    private void chooseNextThreeFlowers()
    {
        for (int i = 0; i < 3; ++i)
        {
            nextThreeFlowers[i] = flowersPrefabs[Random.Range(0, flowersPrefabs.Length)];
            GameObject.Find("NextFlower" + (i + 1) + "Image").GetComponent<Image>().sprite = nextThreeFlowers[i].GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void placeNextThreeFlowers()
    {

        for (int i = 0; i < 3; ++i)
        {
            int randomPosition = unusedTiles[Random.Range(0, unusedTiles.Count)];
            unusedTiles.Remove(randomPosition);
            int randomPositionX = randomPosition / 9;
            int randomPositionY = randomPosition % 9;

            Vector3 actualPosition = new Vector3(tiles[randomPositionX, randomPositionY].transform.position.x, tiles[randomPositionX, randomPositionY].transform.position.y, -1);

            flowers[randomPositionX, randomPositionY] = Instantiate(nextThreeFlowers[i], actualPosition, Quaternion.identity);

            destroyLineContaining(flowers[randomPositionX, randomPositionY], randomPositionX * 9 + randomPositionY);

            if (unusedTiles.Count == 0)
            {
                gameFlow.endGame();
                return;
            }
        }
    }

    public int moveFlowerToTile(GameObject flower, int toTile)
    {
        int fromTile = getFlowerPosition(flower);
        return moveFromTileToTile(fromTile, toTile);
    }

    public int moveFromTileToTile(int fromTile, int toTile) {
        int result = canMoveFromTo(fromTile, toTile);

        GameObject flower = flowers[fromTile / 9, fromTile % 9];

        if (result == OCCUPIED || result == NO_PATH)
        {
            return result;
        }
        else
        {
            flower.transform.position = new Vector3(tiles[toTile / 9, toTile % 9].transform.position.x, tiles[toTile / 9, toTile % 9].transform.position.y, -1);
            flower.GetComponent<DragAndDrop>().deselect();

            if (result == SAME_SPOT)
                return result;

            flowers[fromTile / 9, fromTile % 9] = null;
            flowers[toTile / 9, toTile % 9] = flower;
            unusedTiles.Add(fromTile);
            unusedTiles.Remove(toTile);

            if (destroyLineContaining(flower, toTile))
                return DESTROYED_FLOWERS;
            return result;
        }
    }

    private int getFlowerPosition(GameObject flower)
    {

        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (flowers[i, j] == flower)
                    return i * 9 + j;
            }
        }
        return -1;
    }

    private int canMoveFromTo(int fromTile, int toTile)
    {
        if (fromTile == toTile)
            return SAME_SPOT;
        if (flowers[toTile / 9, toTile % 9] != null)
            return OCCUPIED;
        if (!pathExists(fromTile, toTile))
            return NO_PATH;
        return SUCCESS;
    }

    private bool pathExists(int fromTile, int toTile)
    {
        Queue<int> queue = new Queue<int>();

        if ((fromTile + 1) % 9 != 0)
            queue.Enqueue(fromTile + 1);
        if ((fromTile - 1) % 9 != 8)
            queue.Enqueue(fromTile - 1);
        if ((fromTile + 9) < 81)
            queue.Enqueue(fromTile + 9);
        if ((fromTile - 9) >= 0)
            queue.Enqueue(fromTile - 9);

        HashSet<int> visited = new HashSet<int>();

        while (queue.Count != 0)
        {
            int currentElem = queue.Dequeue();

            if (!visited.Contains(currentElem))
            {
                visited.Add(currentElem);

                if (currentElem >= 0 && currentElem < 81)
                {
                    if (currentElem == toTile)
                        return true;

                    if (flowers[currentElem / 9, currentElem % 9] == null)
                    {
                        if ((currentElem + 1) % 9 != 0)
                            queue.Enqueue(currentElem + 1);
                        if ((currentElem - 1) % 9 != 8)
                            queue.Enqueue(currentElem - 1);
                        if ((currentElem + 9) < 81)
                            queue.Enqueue(currentElem + 9);
                        if ((currentElem - 9) >= 0)
                            queue.Enqueue(currentElem - 9);
                    }
                }
            }
        }
        return false;
    }

    private bool destroyLineContaining(GameObject flower, int tile)
    {
        int destroyed = 0;

        string flowerName = flower.name;

        int sameTypeLeft = 0;
        int sameTypeRight = 0;
        int sameTypeUp = 0;
        int sameTypeDown = 0;

        int currentTile = tile - 1;
        while (currentTile % 9 != 8 && currentTile != -1)
        {
            if (flowers[currentTile / 9, currentTile % 9] != null && flowers[currentTile / 9, currentTile % 9].name.Equals(flowerName))
                sameTypeLeft++;
            else
                break;
            currentTile--;
        }

        currentTile = tile + 1;
        while (currentTile % 9 != 0)
        {
            if (flowers[currentTile / 9, currentTile % 9] != null && flowers[currentTile / 9, currentTile % 9].name.Equals(flowerName))
                sameTypeRight++;
            else
                break;
            currentTile++;
        }

        currentTile = tile + 9;
        while (currentTile < 81)
        {
            if (flowers[currentTile / 9, currentTile % 9] != null && flowers[currentTile / 9, currentTile % 9].name.Equals(flowerName))
                sameTypeDown++;
            else
                break;
            currentTile += 9;
        }

        currentTile = tile - 9;
        while (currentTile >= 0)
        {
            if (flowers[currentTile / 9, currentTile % 9] != null && flowers[currentTile / 9, currentTile % 9].name.Equals(flowerName))
                sameTypeUp++;
            else
                break;
            currentTile -= 9;
        }

        if (sameTypeLeft + sameTypeRight >= 4)
        {
            for (int i = tile - sameTypeLeft; i <= tile + sameTypeRight; ++i)
            {
                if (i != tile)
                {
                    destroyed++;
                    GameObject.Destroy(flowers[i / 9, i % 9]);
                    flowers[i / 9, i % 9] = null;
                    unusedTiles.Add(i);
                }
            }
        }

        if (sameTypeDown + sameTypeUp >= 4)
        {
            for (int i = tile - (9 * sameTypeUp); i <= tile + (9 * sameTypeDown); i += 9)
            {
                if (i != tile)
                {
                    destroyed++;
                    GameObject.Destroy(flowers[i / 9, i % 9]);
                    flowers[i / 9, i % 9] = null;
                    unusedTiles.Add(i);
                }
            }
        }

        if (destroyed > 0)
        {
            destroyed++;
            GameObject.Destroy(flowers[tile / 9, tile % 9]);
            flowers[tile / 9, tile % 9] = null;
            unusedTiles.Add(tile);
            gameFlow.addScore(destroyed);
            return true;
        }

        return false;
    }

    public GameObject[,] getFlowers() {
        return flowers;
    }

    public GameObject[] getNextThreeFlowers() {
        return nextThreeFlowers;
    }
}
