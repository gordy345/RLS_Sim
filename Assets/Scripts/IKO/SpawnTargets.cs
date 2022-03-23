using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTargets : MonoBehaviour
{
    public Transform startPosition;
    [SerializeField] GameObject target;
    [SerializeField] GameObject center;

    // Start is called before the first frame update
    void Start()
    {
        target.GetComponent<Renderer>().enabled = false; //Скрываем цель при запуске программы
    }

    public void Spawn() //Рандомим место спавна и включаем цель
    {
        target.transform.position = startPosition.position + new Vector3(0, -Random.Range(0, 0.2f), 0);
        center.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        target.GetComponent<Renderer>().enabled = true;
    }

    

    public void Despawn()
	{
        target.GetComponent<Renderer>().enabled = false;

        target.transform.position = startPosition.position; 

        Color color = target.GetComponent<Renderer>().material.color;
        color.a = 1;
        target.GetComponent<Renderer>().material.color = color;

        center.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
