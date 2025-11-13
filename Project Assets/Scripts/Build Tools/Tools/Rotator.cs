using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 100f;

    void Update()
    {
        transform.localRotation *= Quaternion.Euler(new Vector3(0f, Time.deltaTime * _rotateSpeed, 0f));
    }
}
