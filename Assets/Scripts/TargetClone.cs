using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetClone : MonoBehaviour
{
    public GameObject parent;
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        GameObject.Find("Targets").GetComponent<Targets>().ChangeActiveTarget(parent);
        parent.GetComponent<Target>().isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
