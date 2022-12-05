using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuadScript : MonoBehaviour
{
    Material material;
    MeshRenderer meshRenderer;

    float[] points;
    int hitCount;

    float delay;

    void Start()
    {
        delay = 3;

        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

        points = new float[256 * 3]; // x,y,intensity values
    }

    public void AddHitPoint(float xp, float yp, float instensity)
    {
        points[hitCount * 3] = xp;
        points[hitCount * 3 + 1] = yp;
        points[hitCount * 3 + 2] = instensity;

        hitCount++;

        hitCount %= 256;

        if (hitCount == 255)
        {
            Debug.Log(hitCount);
        }
        material.SetFloatArray("_Hits", points);
        material.SetInt("_HitCount", hitCount);
    }
}