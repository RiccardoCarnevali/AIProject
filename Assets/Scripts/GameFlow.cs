using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public GameObject aimove;
    public GameObject flowersPlaced;
    private GameObject selectedFlower = null;
    private FlowerHandler flowerGenerator;
    private TilesGenerator tilesGenerator;
    private bool nextStep = false;
    private int score = 0;
    private bool gameOver = false;
    private bool blockEverything = false;
    public GameObject gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        flowerGenerator = gameObject.GetComponent<FlowerHandler>();
        tilesGenerator = gameObject.GetComponent<TilesGenerator>();

        flowerGenerator.init(tilesGenerator.initTiles());
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver && !blockEverything) {
            if(nextStep) {
                flowerGenerator.manualUpdate();
                if(gameOver)
                    return;
                nextStep = false;
            }
            MakeMove nextMove;
            if(CrossSceneSettings.Ai) {
                if(AI.getInstance(this).isMoveMade()) {
                    nextMove = AI.getInstance(this).getNextMove();
                    moveFromTileToTile(nextMove.getX1() * 9 + nextMove.getY1(), nextMove.getX2() * 9 + nextMove.getY2());
                    AI.getInstance(this).makeMove();
                    //Debug.Log("moved from " + nextMove.getX1() + "," + nextMove.getY1() + " " + nextMove.getX2() + "," + nextMove.getY2());
                    flowersPlaced.SetActive(false);
                    aimove.SetActive(true);
                    StartCoroutine("block");
                }
                else if(!AI.getInstance(this).isMoveBeingMade()) {
                    GameObject[,] flowers = flowerGenerator.getFlowers();
                    GameObject[] nextThreeFlowers = flowerGenerator.getNextThreeFlowers();

                    Tile[] tiles = new Tile[81];

                    for(int i = 0; i < 81; ++i) {
                        string flowerColor = flowers[i / 9, i % 9] == null ? "none" : flowers[i / 9, i % 9].name.Substring(0, flowers[i / 9, i % 9].name.IndexOf("F"));
                        tiles[i] = new Tile(flowerColor, i / 9, i % 9);
                    }

                    Dictionary<string, int> flowerCount = new Dictionary<string, int>();
                    
                    foreach(GameObject flower in nextThreeFlowers) {
                        string flowerColor = flower.name.Substring(0, flower.name.IndexOf("F"));
                        if(!flowerCount.ContainsKey(flowerColor)) {
                            flowerCount.Add(flowerColor, 1);
                        }
                        else {
                            flowerCount[flowerColor] = flowerCount[flowerColor] + 1;
                        }
                    }

                    List<Next> nexts = new List<Next>();

                    foreach(KeyValuePair<string, int> entry in flowerCount) {
                        nexts.Add(new Next(entry.Key, entry.Value));
                    }
                    AI.getInstance(this).startNextMove(tiles, nexts.ToArray());
                }
            }
        }
    }

    public bool isFlowerSelected() {
        return selectedFlower != null;
    }

    public void selectFlower(GameObject flower) {
        selectedFlower = flower;
    }

    public void moveFlowerToTile(int toTile) {
        int result = flowerGenerator.moveFlowerToTile(selectedFlower, toTile);

        if(result == FlowerHandler.SAME_SPOT || result == FlowerHandler.DESTROYED_FLOWERS) {
            selectedFlower = null;
        }
        else if(result == FlowerHandler.SUCCESS) {
            selectedFlower = null;
            nextStep = true;
        }
    }

    private void moveFromTileToTile(int fromTile, int toTile) {
        int result = flowerGenerator.moveFromTileToTile(fromTile, toTile);

        if(result == FlowerHandler.SAME_SPOT || result == FlowerHandler.DESTROYED_FLOWERS) {
            selectedFlower = null;
        }
        else if(result == FlowerHandler.SUCCESS) {
            selectedFlower = null;
            nextStep = true;
        }
    }

    public void addScore(int num) {
        if(score + num > 999)
            score = 999;
        else
            score += num;
        GameObject.Find("ScoreNum").GetComponent<Text>().text = score.ToString();
    }

    public void endGame() {
        if(!gameOver) {
            gameOver = true;
            gameOverScreen.SetActive(true);
        }
    }

    public bool isGameOver() {
        return gameOver;
    }

    private IEnumerator block() {
        blockEverything = true;
        yield return new WaitForSecondsRealtime(1);
        blockEverything = false;
    }
}
