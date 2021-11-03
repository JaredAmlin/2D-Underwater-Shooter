using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform _cameraTransform;

    [SerializeField] private float _reductionFactor = 1f;

    private Vector2 _originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = GetComponent<Transform>();

        if (_cameraTransform == null)
        {
            Debug.LogError("The Transform on the Main Camera is NULL");
        }

        _originalPosition = _cameraTransform.localPosition;
    }

    public IEnumerator ShakeTheCamera(float shakeDuration, float shakeMagnitude)
    { 
        while (shakeDuration > 0f)
        {  
            _cameraTransform.localPosition = _originalPosition + Random.insideUnitCircle * shakeMagnitude;

            shakeDuration -= Time.deltaTime * _reductionFactor;
            shakeMagnitude -= Time.deltaTime * _reductionFactor;
            
            yield return null;
        }

        _cameraTransform.localPosition = _originalPosition;
    }
}
