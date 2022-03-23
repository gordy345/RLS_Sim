using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float speedY;
    public float fading;
    private float speedX;
    public float prefabsAlpha;

    public bool isActive = false;
    public bool isGroup = false;
    public bool isUnion = false;

    public GameObject spawnPrefab;
    public List<GameObject> prefabs = new List<GameObject>();

    public GameObject singleTarget;
    public GameObject groupTarget;

    // Start is called before the first frame update
    void Start()
    {
        speedY = 1f / 100;
        fading = 1f / 1000;
        prefabsAlpha = 100f / 100;
        speedX = Random.Range(-1.2f, 1.2f);

        if (Random.Range(0, 2) > 0) isUnion = true;
        else isUnion = false;

        if (Random.Range(0, 2) > 0)
        {
            spawnPrefab = groupTarget;
            isGroup = true;
        }
        else
        {
            spawnPrefab = singleTarget;
            isGroup = false;
        }
    }

    void OnTriggerEnter2D(Collider2D line)
    {
        if (line.gameObject.tag == "Line" && GetComponent<Renderer>().enabled)
        {
            GameObject targetEcho = Instantiate(spawnPrefab, transform.position, transform.rotation);
            targetEcho.transform.parent = transform.parent.transform;
            targetEcho.GetComponent<TargetClone>().parent = transform.gameObject;
            if (isActive) targetEcho.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0);
            prefabs.Add(targetEcho);

            targetEcho.transform.localScale = new Vector3(1, 0.5f, 1);

            Color color = targetEcho.GetComponent<Renderer>().material.color;
            color.a = prefabsAlpha;
            targetEcho.GetComponent<Renderer>().material.color = color;
        }
    }

    void FixedUpdate()
    {
        transform.Translate(speedX * speedY * Time.deltaTime, -speedY * Time.deltaTime, 0);

        for (int i = 0; i < prefabs.Count; ++i)
        {
            Color color = prefabs[i].GetComponent<Renderer>().material.color;
            color.a = color.a - fading;
            if (color.a < 0)
            {
                Destroy(prefabs[i]);
                prefabs.RemoveAt(i);
            }
            else
            {
                prefabs[i].GetComponent<Renderer>().material.color = color;
            }
        }
    }

}
