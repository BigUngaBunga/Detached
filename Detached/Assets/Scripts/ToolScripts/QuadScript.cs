using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScript : MonoBehaviour
{
    [SerializeField]GameObject Projectile;

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

        points = new float[64 * 3]; // x,y,intensity values
    }

    void Update()
    {
        //delay -= Time.deltaTime;
        //if (delay <= 0 )
        //{
        //    GameObject go = Instantiate(Projectile);
        //    go.transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, -0.5f));
        //    delay = 0.2f;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        //foreach (ContactPoint cp in collision.contacts)
        //{
        //    Debug.Log("Contact with object " + cp.otherCollider.gameObject.name);

        //    Vector3 StartOfRay = cp.point - cp.normal;
        //    Vector3 RayDir = cp.normal;

        //    Ray ray = new Ray(StartOfRay, RayDir);
        //    RaycastHit hit;

        //    bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMap"));

        //    if (hitit)
        //    {
        //        Debug.Log("Hit object " + hit.collider.gameObject.name);
        //        Debug.Log("Hit Texture coordinates" + hit.textureCoord.x + "," + hit.textureCoord.y);
        //        AddHitPoint(hit.textureCoord.x*4-2, hit.textureCoord.y*4-2);
        //    }

        //    Destroy(cp.otherCollider.gameObject);
        //}
    }

    public void AddHitPoint(float xp, float yp)
    {
        points[hitCount * 3] = xp;
        points[hitCount * 3 + 1] = yp;
        points[hitCount * 3 + 2] = 1f;

        hitCount++;

        hitCount %= 64;

        material.SetFloatArray("_Hits", points);
        material.SetInt("_HitCount", hitCount);
    }
}
