using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemyRotation : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 10f;

    private Vector3 _rotateDirection = Vector3.forward;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotateDirection * _rotateSpeed * Time.deltaTime);
    }
}
