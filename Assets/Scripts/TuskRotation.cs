using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuskRotation : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 50.0f;

    private Vector3 _rotateDirection = Vector3.back;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotateDirection * _rotateSpeed * Time.deltaTime);
    }
}
