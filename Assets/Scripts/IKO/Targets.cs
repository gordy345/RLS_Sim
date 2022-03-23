using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targets : MonoBehaviour
{
    [SerializeField] GameObject IKOTargetPrefab;
    public GameObject activeTarget = null;

    [SerializeField] GameObject prefabsAlphaSlider;
    [SerializeField] GameObject targetSpeedSlider;
    [SerializeField] GameObject targetFadingSlider;

    List<GameObject> targets = new List<GameObject>();

    public GameObject singleUnion;
    public GameObject groupUnion;

    [SerializeField] GameObject resultText;

    void Start() { }

    public void Spawn()
    {
        GameObject newTarget = Instantiate(IKOTargetPrefab, IKOTargetPrefab.transform.position, IKOTargetPrefab.transform.rotation);
        newTarget.transform.parent = transform;
        targets.Add(newTarget);

        //IKOTarget newTargetScript = newTarget.GetComponent<IKOTarget>();

        resultText.SetActive(false);
        SetButtonInteractable("AnswerButton", false);
        SetButtonInteractable("CheckButton", false);
    }

    public void ChangeActiveTarget(GameObject newActiveTarget)
    {
        if (activeTarget != null) {
            foreach (GameObject gameObject in activeTarget.GetComponent<Target>().prefabs)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            }
            activeTarget.GetComponent<Target>().isActive = false;
        }
        foreach (GameObject gameObject in newActiveTarget.GetComponent<Target>().prefabs)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0);
        }
        activeTarget = newActiveTarget;

        resultText.SetActive(false);
        SetButtonInteractable("AnswerButton", true);
        SetButtonInteractable("CheckButton", true);
    }
    public void ChangeTargetSpeed(Slider elem)
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject gameObject in go) gameObject.GetComponent<Target>().speedY = elem.value / 100;
    }

    public void ChangePrefabsAlpha(Slider elem)
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject gameObject in go) gameObject.GetComponent<Target>().prefabsAlpha = elem.value / 100;
    }

    public void ChangeTargetFading(Slider elem)
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject gameObject in go) gameObject.GetComponent<Target>().fading = elem.value / 1000;
    }
    public void checkUnion()
	{
        Target script = activeTarget.GetComponent<Target>();
        if (script.isUnion)
		{
            if (script.isGroup) script.spawnPrefab = groupUnion;
            else script.spawnPrefab = singleUnion;
        }
        resultText.SetActive(false);
    }

    void checkAnswer(bool group, bool union)
	{
        Target script = activeTarget.GetComponent<Target>();
        if (script.isGroup.Equals(group) && script.isUnion.Equals(union))
        {
            resultText.GetComponent<Text>().text = "Верно";
            resultText.SetActive(true);

            for (int i = 0; i < script.prefabs.Count; ++i) Destroy(script.prefabs[i]);
            Destroy(activeTarget);
            activeTarget = null;

            SetButtonInteractable("AnswerButton", false);
            SetButtonInteractable("CheckButton", false);
        }
        else
        {
            resultText.GetComponent<Text>().text = "Неверно";
            resultText.SetActive(true);
        }
    }

    public void onClickSingleEnemy()
	{
        checkAnswer(false, false);
    }

    public void onClickGroupEnemy()
    {
        checkAnswer(true, false);
    }

    public void onClickSingleUnion()
    {
        checkAnswer(false, true);
    }

    public void onClickGroupUnion()
    {
        checkAnswer(true, true);
    }
    /*
    public void Despawn()
    {
        GetComponent<Renderer>().enabled = false;
        transform.position = startPosition.position;
        IKOTarget.transform.rotation = Quaternion.Euler(0, 0, 0);
        for (int i = 0; i < targets.Count; ++i) Destroy(targets[i]);
        targets.Clear();
    }

    public void prefabsAlphaChanger()
    {
        prefabsAlpha = prefabsAlphaSlider.GetComponent<Slider>().value / 100;
    }

    public void targetSpeedChanger()
    {
        speed = targetSpeedSlider.GetComponent<Slider>().value / 100;
    }

    public void targetFadingChanger()
    {
        fading = 1 / targetFadingSlider.GetComponent<Slider>().value / 1000;
    }
    */

    void SetButtonInteractable(string tag, bool value)
	{
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < Buttons.Length; ++i) Buttons[i].GetComponent<Button>().interactable = value;
    }
}
