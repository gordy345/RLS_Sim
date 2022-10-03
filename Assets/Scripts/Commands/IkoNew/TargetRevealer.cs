using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TargetRevealer : MonoBehaviour
{
    [SerializeField] private GameObject TargetPrefabTheirs;
    [SerializeField] private GameObject TargetPrefabOurs;

    public Vector3 Center => IkoController.IkoCenter;
    public bool IsOurs { get; set; }

    private bool _isRevealed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Line") return;
        GameObject instance = null;
        if (!_isRevealed)
        {
            instance = Instantiate(TargetPrefabTheirs);
            _isRevealed = true;
        }
        else
        {
            instance = IsOurs ?
                Instantiate(TargetPrefabOurs) :
                Instantiate(TargetPrefabTheirs);
        }

        instance.transform.position = transform.position;
        instance.transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(
                Vector2.up,
                Center - transform.position
            )
        );
        instance.transform.SetParent(IkoController.Instance.TargetsFolder, true);
        instance.transform.localScale = Vector3.one;

    }
}
