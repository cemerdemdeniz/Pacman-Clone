using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    public static int boardWidth = 28;
    public static int boardHeight = 36;

    public Text playerText;
    public Text readyText;
    public Text highScoreText;
    public Text playerOneUp;
    public Text playerOneScoreText;
    public Image playerLives2;
    public Image playerLives3;
    public Text consumedGhostScoreText;

    private bool didStartDeath = false;
    private bool didStartConsumed = false;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;
    public AudioClip backgroundAudioPacmanDeath;
    public AudioClip consumedGhostAudioClip;

    public int totalPellets = 0;
    public int score = 0;
    public int playerOneScore = 0;
    public int pacManLives = 3;

    public bool isPlayerOneUp = true;

    void Start()
    {

        if (isPlayerOneUp)
        {
            StartCoroutine(StartBlinking(playerOneUp));
        }



        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject o in objects)
        {

            Vector2 pos = o.transform.position;
            if (o.name != "PacMan" && o.name != "Pellets" && o.name != "Nodes" && o.name != "NonNodes" && o.name != "Maze" && o.tag != "Ghost" && o.tag != "ghostHome" && o.name != "Canvas" && o.name != "Player" && o.name != "Ready" && o.tag!="UIElements") 
            {
                if (o.GetComponent<Tile>() != null)
                {
                    if (o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                    {
                        totalPellets++;

                    }
                }
                board[(int)pos.x, (int)pos.y] = o;
            }
            else
            {
                 //Debug.Log("Found PacMan at : " + pos);
            }
        }
        StartGame();
    }

    IEnumerator StartBlinking(Text blinkText)
    {
        yield return new WaitForSeconds(0.25f);
        blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;
        StartCoroutine(StartBlinking(blinkText));
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        playerOneScoreText.text = playerOneScore.ToString();

        if (pacManLives == 3)
        {
            playerLives3.enabled = true;
            playerLives2.enabled = true;
        }else if (pacManLives == 2)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = true;
        }else if (pacManLives == 1)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = false;
        }
    }



    public void StartGame()
    {
       
        //Hide All Ghosts
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;

        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = false;

        StartCoroutine(ShowObjectsAfter(2.25f));
    }
    public void StartConsumed(Ghost consumedGhost)
    {
        if (!didStartConsumed)
        {
            didStartConsumed = true;

            //Pause all ghost
            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            //Hide PacMan
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
            //Hide the consumed ghost
            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

            //Stop Background Music
            transform.GetComponent<AudioSource>().Stop();

            Vector2 pos = consumedGhost.transform.position;

            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            //Play the consumed sound
            transform.GetComponent<AudioSource>().PlayOneShot(consumedGhostAudioClip);
            //Wait for the audio clip to finish
            StartCoroutine(ProcessConsumedAfter(0.75f, consumedGhost));


        }
    }

    IEnumerator ProcessConsumedAfter(float delay,Ghost consumedGhost)
    {
        yield return new WaitForSeconds(delay);

        //Hide The Score

        consumedGhostScoreText.GetComponent<Text>().enabled = false;

        //Show Pac-Man
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        //Show Consumed Ghost
        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true;
        //Resume All Ghost
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        //Resume Pac-Man
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        //Start Background Music
        transform.GetComponent<AudioSource>().Play();

        didStartConsumed = false;
    }

    IEnumerator ShowObjectsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;         
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        playerText.transform.GetComponent<Text>().enabled = false;

        StartCoroutine(StartGameAfter(2));
       
    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        readyText.transform.GetComponent<Text>().enabled = false;

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
    }




    public void StartDeath()
    {
        if (!didStartDeath)
        {
            StopAllCoroutines();

            if (GameMenu.isStartGame)
            {
                playerOneUp.GetComponent<Text>().enabled = true;

            }
            else
            {
                playerOneUp.GetComponent<Text>().enabled = true;
            }

            didStartDeath = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove=false;
            }
            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            pacMan.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeathAfter(2));
        }
    }

    IEnumerator ProcessDeathAfter (float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }
        StartCoroutine(ProcessDeathAnimation(1.9f));
    } 

    IEnumerator ProcessDeathAnimation (float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.localScale = new Vector3(1, 1, 1);
        pacMan.transform.localRotation = Quaternion.Euler(0, 0, 0);

        pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().deathAnimation;
        pacMan.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacmanDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));

    }

    IEnumerator ProcessRestart (float delay)
    {
        pacManLives -= 1;

        if (pacManLives == 0)
        {
            playerText.transform.GetComponent<Text>().enabled = true;
            readyText.transform.GetComponent<Text>().text = "GAME OVER";
            readyText.transform.GetComponent<Text>().color = Color.red;
            //if you wanna create rgb colour you can create like that
            //readyText.transform.GetComponent<Text>().color = new Color(230f / 255f, 230f / 255f, 230f / 255f);

            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessGameOver(2));

        }
        else
        {
            playerText.transform.GetComponent<Text>().enabled = true;
            readyText.transform.GetComponent<Text>().enabled = true;
            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(1));
        }
       

       

    }
    IEnumerator ProcessGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator ProcessRestartShowObjects(float delay)
    {
        playerText.transform.GetComponent<Text>().enabled = false;

        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<Animator>().enabled = true;
       // pacMan.transform.GetComponent<SpriteRenderer>().enabled = ;
        pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();


        yield return new WaitForSeconds(delay);

        Restart();
    }




    public void Restart()
    {
        readyText.transform.GetComponent<Text>().enabled = false;

       

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().Restart();
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart();
        }
        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
        didStartDeath = false;

    }

    
}
