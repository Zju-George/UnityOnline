using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float DestroyTime = 0.5f;
    private float RandomizeIntensity = 0.1f;
    void Start()
    {
        Destroy(gameObject, DestroyTime);
        switch (Random.Range(0, 3))
        {
            case 0:
                GetComponent<TextMesh>().color = new Color(0.6941177f, 0.4078431f, 0.7921569f);
                break;
            case 1:
                GetComponent<TextMesh>().color = new Color(0.6352941f, 0.7921569f, 0.4078431f);
                break;
            case 2:
                GetComponent<TextMesh>().color = new Color(0.4470588f, 0.3254902f, 0.2039216f);
                break;
        }
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity, RandomizeIntensity),
            0,
            Random.Range(-RandomizeIntensity, RandomizeIntensity)
            );
    }

}
