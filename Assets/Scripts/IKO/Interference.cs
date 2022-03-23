using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interference : MonoBehaviour
{


    [SerializeField] GameObject Int;
    [SerializeField] GameObject[] Ints;
    [SerializeField] GameObject[] IntsPrefabs;
    [SerializeField] GameObject[] IntButtons;
    int correctCount;
    const int InterN = 2; // < 6 иначе бесконечный цикл

    [SerializeField] GameObject resultText;
    [SerializeField] GameObject answerText;
    [SerializeField] GameObject countText;

    void Start()
    {
        SetButtonInteractable("InterferenceButton", false);
    }

    public void Create()
    {
        int interferenceNumber;
        for (int i = 0; i < InterN; ++i)
        {
            while (Ints[(interferenceNumber = Random.Range(0, 6))].activeSelf) ;
            //Debug.Log(interferenceNumber);
            //Debug.Log(Ints.Length);
            Ints[interferenceNumber].SetActive(true);
        }

        resultText.SetActive(false);
        answerText.SetActive(true);
        countText.GetComponent<Text>().text = "0 / " + InterN;
        correctCount = 0;
        countText.SetActive(true);
        SetButtonInteractable("InterferenceButton", true);
        SetButtonInteractable("CreateInterference", false);
    }

    void checkAnswer(int intButtonNumber)
    {
        if (Ints[intButtonNumber].activeSelf)
		{
            IntsPrefabs[intButtonNumber].GetComponent<SpriteRenderer>().color = new Color(90 / 255f, 1f, 90 / 255f);

            resultText.GetComponent<Text>().text = "Верно";
            resultText.SetActive(true);
            correctCount++;

            countText.GetComponent<Text>().text = correctCount + " / " + InterN;
            IntButtons[intButtonNumber].GetComponent<Button>().interactable = false;
        }
        else
        {
            resultText.GetComponent<Text>().text = "Неверно";
            resultText.SetActive(true);
        }

        //Ints[intButtonNumber].SetActive(false);
        if (correctCount == InterN)
        {
            SetButtonInteractable("InterferenceButton", false);
            for (int i = 0; i < Ints.Length; ++i)
            {
                if (Ints[i].activeSelf)
                {
                    SpriteRenderer[] inters = Ints[i].GetComponentsInChildren<SpriteRenderer>();
                    for (int j = 0; j < inters.Length; ++j) inters[j].color = new Color(1f, 1f, 1f, 0f);
                    Ints[i].SetActive(false);
                    IntsPrefabs[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                }
            }
        }
        SetButtonInteractable("CreateInterference", true);
    }

    public void onClickInterference1()
	{
        checkAnswer(0);
	}
    public void onClickInterference2()
    {
        checkAnswer(1);
    }
    public void onClickInterference3()
    {
        checkAnswer(2);
    }
    public void onClickInterference4()
    {
        checkAnswer(3);
    }
    public void onClickInterference5()
    {
        checkAnswer(4);
    }
    public void onClickInterference6()
    {
        checkAnswer(5);
    }

    void SetButtonInteractable(string tag, bool value)
    {
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < Buttons.Length; ++i) Buttons[i].GetComponent<Button>().interactable = value;
    }
}
