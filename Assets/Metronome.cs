using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class Metronome : MonoBehaviour
{
    public AudioSource metronome;

    public AudioClip metronomeSpeed1;
    public AudioClip metronomeSpeed2;
    public AudioClip metronomeSpeed3;

    public GameObject[] inputs;

    public List<string> enemyChoices;

    public float metronomeSpeed = 1;

    public float timer = 0f;
    public float playerTimer = 0f;

    public string inputString;

    public bool playerTurn = false;

    public GameObject left;
    public GameObject right;
    public GameObject up;
    public GameObject down;
    public GameObject empty;

    public GameObject[] enemySlots;

    public GameObject[] playerSlots;

    public GameObject enemySlotsObject;
    public GameObject playerSlotsObject;

    public bool gameActive;

    public GameObject restartButton;

    public TMP_Text scoreText;

    public float score = 0;

    // Start is called before the first frame update
    void Start()
    {
        metronome = GetComponent<AudioSource>();
        metronomeSpeed = metronomeSpeed1.length;
        StartCoroutine(gameLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputString = "left(Clone)";
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                inputString = "up(Clone)";
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                inputString = "right(Clone)";
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                inputString = "down(Clone)";
            }
        }
        scoreText.text = "Score: " + score;
    }

    public IEnumerator makeEnemyChoices() 
    {
        for (int i = 0; i < 4; i++)
        {
            int slotSelection = i;
            GameObject slotObject = enemySlots[slotSelection];
            GameObject newSelection = inputs[Random.Range(0, inputs.Length)];
            GameObject newChoice = Instantiate(newSelection, enemySlotsObject.transform);
            newChoice.transform.position = slotObject.transform.position;
            enemyChoices.Insert(i, newChoice.name);

            yield return new WaitForSeconds(metronomeSpeed / 4);
        }
        foreach (Transform child in enemySlotsObject.transform)
        {
            if (child.gameObject.tag == "choice")
            {
                Destroy(child.gameObject);
            }
        }
        playerTurn = true;
    }

    public IEnumerator falloffTime(string enemySelection)
    {
        yield return new WaitForSeconds(metronomeSpeed / 8);
        if (inputString != enemySelection)
        {
            Debug.Log(inputString);
            Debug.Log(enemySelection);
            gameActive = false;
            metronome.Stop();
            timer = 4;
            restartButton.SetActive(true);
        }
    }

    public IEnumerator playerChoices()
    {
        for (int i = 0; i < 4; i++)
        {
            int slotSelection = i;
            GameObject slotObject = playerSlots[slotSelection];
            string enemySelection = enemyChoices[i];
            GameObject enemyInput = null;
            foreach (GameObject input in inputs)
            {
                if (input.name + "(Clone)" == enemySelection)
                {
                    enemyInput = input;
                }
            }
            GameObject newChoice = Instantiate(enemyInput, playerSlotsObject.transform);
            newChoice.transform.position = slotObject.transform.position;

            StartCoroutine(falloffTime(enemySelection));

            if (!gameActive)
            {
                break;
            }

            yield return new WaitForSeconds(metronomeSpeed / 4);
        }
        
        foreach (Transform child in playerSlotsObject.transform)
        {
            if (child.gameObject.tag == "choice")
            {
                Destroy(child.gameObject);
            }
        }
        playerTurn = false;
        inputString = "empty";
    }

    public IEnumerator gameLoop()
    {
        yield return new WaitForSeconds(3);
        gameActive = true;
        metronome.Play();
        while (gameActive)
        {
            playerTurn = false;
            StartCoroutine(makeEnemyChoices());

            yield return new WaitForSeconds(metronomeSpeed);

            StartCoroutine(playerChoices());

            yield return new WaitForSeconds(metronomeSpeed);
            enemyChoices.Clear();

            score++;


            if (timer >= 0 && timer < 1)
            {
                metronomeSpeed = metronomeSpeed1.length;
                metronome.clip = metronomeSpeed1;
                metronome.Play();
                timer += 1f;
            }
            else if (timer >= 1 && timer < 2)
            {
                metronomeSpeed = metronomeSpeed2.length;
                metronome.clip = metronomeSpeed2;
                metronome.Play();
                timer += 0.2f;
            }
            else if (timer >= 2 && timer < 3)
            {
                metronomeSpeed = metronomeSpeed3.length;
                metronome.clip = metronomeSpeed3;
                metronome.Play();
                timer += 0.1f;
            }
        }
    }
}
