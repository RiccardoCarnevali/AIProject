using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSpot : MonoBehaviour
{

    private GameFlow gameFlow;
    private int tileNum;
    // Start is called before the first frame update
    void Start()
    {
        gameFlow = GameObject.Find("GameController").GetComponent<GameFlow>();
    }
    void OnMouseUp() {
        if(!CrossSceneSettings.Ai) {
            if(gameFlow.isFlowerSelected()) {
                gameFlow.moveFlowerToTile(tileNum);
            }
        }
    }

    public void setTileNum(int tileNum) {
        this.tileNum = tileNum;
    }
}
