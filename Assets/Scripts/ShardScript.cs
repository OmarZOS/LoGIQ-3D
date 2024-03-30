using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardScript : MonoBehaviour {

    private static float fragFactor = 4;

    public Material fadingShader;

    private bool staying,fading;
    public static void Fragment(GameObject target,Material fadingShader) {
        Vector3 position = target.transform.position;

        // This used to work well with uniform objects..
        // target.transform.localScale = target.transform.localScale/fragFactor;
        target.transform.localScale = (new Vector3(1f,1f,1f))/fragFactor;

        for (int i = 0; i < fragFactor; i++)
        {
            for (int j = 0; j < fragFactor; j++)
            {
                for (int k = 0; k < fragFactor; k++)
                {
                    GameObject shard = Instantiate(target,target.transform.position,Quaternion.identity);
                    shard.transform.position += new Vector3(i*(1/fragFactor/fragFactor),0.5f+j*(1/fragFactor/fragFactor),k*(1/fragFactor/fragFactor));
                    shard.AddComponent<ShardScript>();
                    shard.GetComponent<ShardScript>().fadingShader = fadingShader;
                }
            }
        }
    }


void Start()
{
    StartCoroutine(Fade(0.7f,1f));
}
     
private IEnumerator Fade(float progress, float totalTime)
{
    float rate = progress / totalTime;
    MeshRenderer rend = this.gameObject.GetComponent<MeshRenderer>();
    this.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Mode",3);
    Color color = rend.material.color;

    // this.gameObject.GetComponent<MeshRenderer>().material.renderQueue = 4800;

    for (float i = 0.0f; Mathf.Abs(i) < Mathf.Abs(progress); i += Time.deltaTime * rate)
    {
        color.a = 1 - i;
        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color",color);
        yield return null;
    }

    // yield return new WaitForSeconds(0.5f); // wait two seconds    
    rend.material = fadingShader;


    // slooowly animate the dissolution
    rate/=3;
    progress = 1 - progress;
    rend.material.SetFloat("_Progress",progress);

    for (float i = 0.0f; Mathf.Abs(i) < Mathf.Abs(progress); i += Time.deltaTime * rate)
    {
        rend.material.SetFloat("_Progress",progress-i);
        yield return null;
    }
    Destroy(this.gameObject);
}


}