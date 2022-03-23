using UnityEngine;
using UnityEngine.UI;

public class IKOTarget : MonoBehaviour
{


    void Start()
    {
        transform.position = transform.position + new Vector3(0, -Random.Range(0, 0.2f), 0);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
