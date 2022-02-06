using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenuButton : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(changeScene);
    }

    public void hover(bool hover)
    {
        Color tempColor = gameObject.GetComponent<Image>().color;
        if (hover)
            tempColor.b = 0.25f;   //set to yellow
        else
            tempColor.b = 1f;

        gameObject.GetComponent<Image>().color = tempColor;
    }

    public void changeScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
