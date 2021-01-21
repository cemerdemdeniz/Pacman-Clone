﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public static int boardWidth = 28;
    public static int boardHeight = 36;

    private bool didStartDeath = false;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;
    public AudioClip backgroundAudioPacmanDeath;

    public int totalPellets = 0;
    public int score = 0;
    public int pacManLives = 3; 
    void Start()
    {


        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject o in objects)
        {

            Vector2 pos = o.transform.position;
            if (o.name != "PacMan" && o.name != "Pellets" && o.name != "Nodes" && o.name != "NonNodes" && o.name != "Maze" && o.tag != "Ghost"&&o.tag != "ghostHome")
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
    }

    public void StartDeath()
    {
        if (!didStartDeath)
        {
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

        StartCoroutine(ProcessRestart(2));

    }

    IEnumerator ProcessRestart (float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        yield return new WaitForSeconds(delay);

        Restart();

    }


    public void Restart()
    {
        pacManLives -= 1;

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
