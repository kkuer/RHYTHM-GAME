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

    public AudioSource loseSFX;

    public AudioSource countdownSFX;

    public AudioClip metronomeSpeed1;
    public AudioClip metronomeSpeed2;
    public AudioClip metronomeSpeed3;

    public GameObject[] inputs;
    public GameObject[] enemyPoses;

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

    public GameObject leftPose;
    public GameObject rightPose;
    public GameObject upPose;
    public GameObject downPose;
    public GameObject idlePose;

    public GameObject losePose;

    public GameObject enemyIdle;

    public GameObject countdown3;
    public GameObject countdown2;
    public GameObject countdown1;

    public GameObject countdownGo;

    public GameObject flash;

    // Start is called before the first frame update
    void Start()
    {
        idlePose.SetActive(true);
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

                idlePose.SetActive(false);

                leftPose.SetActive(true);
                rightPose.SetActive(false);
                upPose.SetActive(false);
                downPose.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                inputString = "up(Clone)";

                idlePose.SetActive(false);

                leftPose.SetActive(false);
                rightPose.SetActive(false);
                upPose.SetActive(true);
                downPose.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                inputString = "right(Clone)";

                idlePose.SetActive(false);

                leftPose.SetActive(false);
                rightPose.SetActive(true);
                upPose.SetActive(false);
                downPose.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                inputString = "down(Clone)";

                idlePose.SetActive(false);

                leftPose.SetActive(false);
                rightPose.SetActive(false);
                upPose.SetActive(false);
                downPose.SetActive(true);
            }
        }
        //else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        else if (!playerTurn && gameActive)
        {
            leftPose.SetActive(false);
            rightPose.SetActive(false);
            upPose.SetActive(false);
            downPose.SetActive(false);

            idlePose.SetActive(true);
        }
        
        scoreText.text = score.ToString();
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
            newChoice.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            enemyChoices.Insert(i, newChoice.name);

            enemyIdle.SetActive(false);
            foreach (GameObject pose in enemyPoses)
            {
                if (pose.name + "(Clone)" == newChoice.name)
                {
                    pose.SetActive(true);
                }
                else if (pose.name != newChoice.name)
                {
                    pose.SetActive(false);
                }
            }

            yield return new WaitForSeconds(metronomeSpeed / 4);
        }
        foreach (Transform child in enemySlotsObject.transform)
        {
            if (child.gameObject.tag == "choice")
            {
                Destroy(child.gameObject);
            }
        }
        foreach (GameObject pose in enemyPoses)
        {
            pose.SetActive(false);
        }
        enemyIdle.SetActive(true);
        playerTurn = true;
    }

    public IEnumerator falloffTime(string enemySelection)
    {
        yield return new WaitForSeconds(metronomeSpeed / 16);
        if (inputString != enemySelection)
        {
            Debug.Log(inputString);
            Debug.Log(enemySelection);

            if (!loseSFX.isPlaying)
            {
                loseSFX.Play();
            }

            gameActive = false;

            leftPose.SetActive(false);
            rightPose.SetActive(false);
            upPose.SetActive(false);
            downPose.SetActive(false);
            idlePose.SetActive(false);

            losePose.SetActive(true);

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
            newChoice.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

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
        yield return new WaitForSeconds(0.5f);
        countdownSFX.Play();
        flash.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        flash.SetActive(false);
        countdown3.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown3.SetActive(false);
        flash.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        flash.SetActive(false);
        countdown2.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown2.SetActive(false);
        flash.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        flash.SetActive(false);
        countdown1.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown1.SetActive(false);
        flash.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        flash.SetActive(false);
        countdownGo.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdownGo.SetActive(false);
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
