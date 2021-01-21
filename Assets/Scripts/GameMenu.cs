using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static bool isStartGame = true;



    public Text startText1;
    public Text quitText;
    public Text selector;


   
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (!isStartGame)
            {
                isStartGame = true;
                selector.transform.localPosition = new Vector3(selector.transform.localPosition.x, startText1.transform.localPosition.y, selector.transform.localPosition.z);

            }
        }else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isStartGame)
            {
                isStartGame = false;
                selector.transform.localPosition = new Vector3(selector.transform.localPosition.x, quitText.transform.localPosition.y, selector.transform.localPosition.y);

            }
        }else if (Input.GetKeyUp(KeyCode.Return))
        {
            SceneManager.LoadScene("Level1");
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.Quit();
        }
    }
}
