using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    [SerializeField] GameObject IKOCenter;


    // Update is called once per frame
    void FixedUpdate()
    {
        IKOCenter.transform.Rotate(0, 0, -40 * Time.deltaTime);
    }

}
