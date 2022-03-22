using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pomeha : MonoBehaviour
{



    [SerializeField] GameObject prefabsAlphaSlider;
    [SerializeField] GameObject targetFadingSlider;

    [SerializeField] GameObject pomehaPrefabs;
    [SerializeField] GameObject parent;
    List<GameObject> prefabs = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
    }

    void FixedUpdate()
    {
        if (GetComponent<Renderer>().enabled)
        {
            for (int i = 0; i < prefabs.Count; ++i)
            {
                Color color = prefabs[i].GetComponent<Renderer>().material.color;
                color.a = color.a - 0.01f; //«¿Ã≈Õ»“‹ Õ¿ «Õ¿◊≈Õ»≈ —À¿…ƒ≈–¿ fading
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

    void OnTriggerEnter2D(Collider2D line)
    {
        if (line.gameObject.tag == "Line" && enabled)
        {
            GameObject pomehaEcho = Instantiate(pomehaPrefabs, transform.position, transform.rotation);
            pomehaEcho.transform.parent = parent.transform;
            prefabs.Add(pomehaEcho);
            //Debug.Log("Pomeha");

            Color color = GetComponent<Renderer>().material.color;
            color.a = 1f; //«¿Ã≈Õ»“‹ Õ¿ «Õ¿◊≈Õ»≈ —À¿…ƒ≈–¿ prefabsAlpha
            GetComponent<Renderer>().material.color = color;
        }
    }
}
