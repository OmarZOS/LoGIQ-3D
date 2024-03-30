using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    // animation variables
    bool startGrowing,startShrinking;
    public Vector3 maxScale = new Vector3(0.1f,0.1f,0.1f);
    public Vector3 minScale = new Vector3(0.07f,0.07f,0.1f);
    public float animationTime = 0.1f;
    public float creationTime;



    public static GameObject CreateArrow(GameObject arrowPrefab, Transform worldHolder,float rotation, Color arrowColor) {

        GameObject arrow;
        arrow = Instantiate(arrowPrefab, worldHolder.position,  worldHolder.rotation);
        arrow.transform.Rotate(rotation, -90, 0, Space.Self);
        // arrow.transform.Translate(-6, 0, 0, Space.Self);
        // TODO: random color choice could have been done here, but it's alright



        arrow.GetComponent<MeshRenderer>().material.SetFloat("_Mode",3);
        arrow.GetComponent<MeshRenderer>().material.SetColor("_Color", arrowColor);
        // arrow.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        // arrow.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",arrowColor);
        // // arrow.GetComponent<MeshRenderer>().material.SetFloat("_EmissiveIntensity", 0.5f);
        // arrow.GetComponent<MeshRenderer>().material.SetFloat("_EmissiveExposureWeight", 2.0f);
        // arrow.GetComponent<MeshRenderer>().material.EnableKeyword("_EmissiveIntensity");

        DynamicGI.UpdateEnvironment();
        return arrow;
        
    }


    // Start is called before the first frame update
    void Start()
    {
        creationTime = Time.time;
        this.gameObject.transform.localScale = new Vector3(0f,0f,0f);
        StartCoroutine(AnimateArrow(new Vector3(0f,0f,0f)
            ,this.gameObject.GetComponent<ArrowScript>().maxScale
            ,this.gameObject.GetComponent<ArrowScript>().animationTime));

        // this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        if (startGrowing) {
            StartCoroutine(AnimateArrow(this.gameObject.GetComponent<ArrowScript>().minScale
                ,this.gameObject.GetComponent<ArrowScript>().maxScale
                ,this.gameObject.GetComponent<ArrowScript>().animationTime));
            this.gameObject.GetComponent<ArrowScript>().startGrowing = false;
        }
        if (startShrinking)
        {
            StartCoroutine(AnimateArrow(this.gameObject.GetComponent<ArrowScript>().maxScale
                ,this.gameObject.GetComponent<ArrowScript>().minScale
                ,this.gameObject.GetComponent<ArrowScript>().animationTime));
            this.gameObject.GetComponent<ArrowScript>().startShrinking = false;
        }
    }

    private IEnumerator AnimateArrow(Vector3 startScale,Vector3 targetScale, float totalTime)
    {

        Vector3 augmentation = (targetScale-startScale) / totalTime;
        float rate = (targetScale-startScale).magnitude / totalTime;
        //Start Rotate
        for (float i = 0.0f; Mathf.Abs(i) < Mathf.Abs(totalTime*rate); i += Time.deltaTime*rate)
        {
            this.gameObject.transform.localScale +=  augmentation * Time.deltaTime;
            yield return null;
        }


        if (this.gameObject.GetComponent<ArrowScript>().maxScale == targetScale)
            this.gameObject.GetComponent<ArrowScript>().startShrinking=true;
        else
            this.gameObject.GetComponent<ArrowScript>().startGrowing=true;
 

    }

    

}
