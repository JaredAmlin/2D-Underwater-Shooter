using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //variable for the transform component on the main camera
    private Transform _cameraTransform;

    //variable to decrease the shake Duration and Magnitude
    [SerializeField] private float _reductionFactor = 1f;

    //variable to store the original position of the camera
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

    /*public coroutine to be called to shake the camera. 
    Object shaking the camera can insert parameters for length and intensity of the shake*/
    public IEnumerator ShakeTheCamera(float shakeDuration, float shakeMagnitude)
    {
        //while loop to run the shake for as long as there is a duration of time left. 
        while (shakeDuration > 0f)
        {
            //take camera position and assign original position plus random range on X and Y. Multiply by magnitude.  
            _cameraTransform.localPosition = _originalPosition + Random.insideUnitCircle * shakeMagnitude;

            //decrease the duration and magnitude by subtracting a value in real time.
            shakeDuration -= Time.deltaTime * _reductionFactor;
            shakeMagnitude -= Time.deltaTime * _reductionFactor;
            
            //wait until next frame is drawn to continue the loop
            yield return null;
        }

        //resert camera position to original after duration reaches zero
        _cameraTransform.localPosition = _originalPosition;
    }
}
