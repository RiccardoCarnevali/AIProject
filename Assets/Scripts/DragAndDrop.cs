using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    private GameFlow gameFlow;

    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        gameFlow = GameObject.Find("GameController").GetComponent<GameFlow>();
    }

    // Update is called once per frame
    void Update()
    {
        if(selected) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x + 0.4f, mousePos.y + 0.4f, -2);
        }
    }

    void OnMouseUp()
    {
        if(!CrossSceneSettings.Ai) {
            if(!gameFlow.isFlowerSelected() && !gameFlow.isGameOver()) {
                selected = true;
                gameFlow.selectFlower(gameObject);
            }
        }
    }

    public void deselect() {
        selected = false;
    }
}
