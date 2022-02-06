using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    private GameObject selectedFlower = null;
    private FlowerHandler flowerGenerator;
    private TilesGenerator tilesGenerator;
    private bool nextStep = false;
    private int score = 0;
    private bool gameOver = false;
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
        if(!gameOver) {
            if(nextStep) {
                flowerGenerator.manualUpdate();
                nextStep = false;
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
}
