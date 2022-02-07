using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(changeScene);
    }
    public void showImage(bool show)
    {
        Color tempColor = gameObject.GetComponent<Image>().color;
        if (show)
            tempColor.a = 1f;
        else
            tempColor.a = 0f;

        gameObject.GetComponent<Image>().color = tempColor;
    }

    public void changeScene()
    {
        CrossSceneSettings.Ai = false;
        SceneManager.LoadScene("GameScene");
    }
}
