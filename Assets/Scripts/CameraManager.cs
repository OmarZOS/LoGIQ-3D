using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target ;
    public float rotateTime = 3.0f;
    public float rotateDegrees = 90.0f;
    private bool rotating = false;
    // private int rotationCount = 0;

    // Start is called before the first frame update
    void Start()
    {
    }


    public Vector3 pivot =new Vector3 (0,0,0);
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("right")&&!rotating)
        {
            RotateRight();
        }
        if (Input.GetKeyDown("left")&&!rotating)
        {
            RotateLeft();
        }
        // if (!rotating)
        //     StartCoroutine(Rotate(GameObject.FindGameObjectsWithTag("floating-text")
        //     ,transform, pivot
        //         , Vector3.up, -rotateDegrees, rotateTime));
            
        
    }

    public void AutoRotate(int number, float rotateDegs = 90.0f) {
        rotateDegs = Mathf.Round((rotateDegs*number  / 360)%number);
        if(Mathf.Abs(rotateDegs)>number/2)
            if (rotateDegs>0)
                RotateLeft((number-rotateDegs)*360/number);
            else
                RotateRight((number+rotateDegs)*360/number);
        else
            RotateRight( rotateDegs * 360/number);


    }

    public void RotateRight(float rotateDegs = 90.0f) {
        if(!rotating)
            StartCoroutine(Rotate(GameObject.FindGameObjectsWithTag("floating-text"),transform, pivot, Vector3.up, -rotateDegs, rotateTime));
    }

    public void RotateLeft(float rotateDegs = 90.0f) {
        if(!rotating)
            StartCoroutine(Rotate(GameObject.FindGameObjectsWithTag("floating-text"),transform, pivot, Vector3.up, rotateDegs, rotateTime));
    }

    private IEnumerator Rotate(GameObject[] facingObjects,Transform camTransform, Vector3 position, Vector3 rotateAxis, float degrees, float totalTime)
    {
        if (rotating)
            yield return null;
        rotating = true;
 
        Quaternion startRotation = camTransform.rotation;
        Vector3 startPosition = camTransform.position;
        // Get end position;
        transform.RotateAround(position, rotateAxis, degrees);
        Quaternion endRotation = camTransform.rotation;
        Vector3 endPosition = camTransform.position;
        camTransform.rotation = startRotation;
        camTransform.position = startPosition;
 
        float rate = degrees / totalTime;
        //Start Rotate
        for (float i = 0.0f; Mathf.Abs(i) < Mathf.Abs(degrees); i += Time.deltaTime * rate)
        {
            camTransform.RotateAround(position, rotateAxis, Time.deltaTime * rate);
            foreach (GameObject facingObject in facingObjects)
            {
                if (facingObject!=null)
                    facingObject.transform.rotation = Quaternion.LookRotation(facingObject.transform.position - camTransform.position  , Vector3.up);
            }
            yield return null;
        }
 
        camTransform.rotation = endRotation;
        camTransform.position = endPosition;
        rotating = false;
    }
    
}

