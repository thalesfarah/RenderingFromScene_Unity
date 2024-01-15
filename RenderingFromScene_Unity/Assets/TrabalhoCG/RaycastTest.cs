using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class RaycastTest : MonoBehaviour
{
    Ray ray;
    Texture2D texture;
    public float cameraSize;
    public Transform lightTest;
    public float specular;
    public float ambient;
    void Start()
    {
        ray = new Ray(transform.position, Vector3.forward);
        Renderer rend = GetComponent<Renderer>();
        texture = new Texture2D(128, 128);
        texture.filterMode = FilterMode.Bilinear;
        rend.material.mainTexture = texture;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            StartCoroutine(RenderScene());
        }
        Debug.DrawRay(ray.origin, ray.direction * 7f, Color.red);
    }
    IEnumerator RenderScene() 
    {
        for (int y = 0; y < texture.height; y++) 
        {
            for (int x = 0; x < texture.width; x++) 
            {
                float px = ((float)x / texture.width) * cameraSize - cameraSize * 0.5f;
                float py = ((float)y / texture.height) * cameraSize - cameraSize * 0.5f;
                ray.origin = new Vector3(px, py, 0f);
                if (Physics.Raycast(ray, out RaycastHit hit)) 
                {
                    Color c = BlinnPhong(hit);
                    texture.SetPixel(x, y, c);
                }
                else 
                {
                    texture.SetPixel(x, y, Color.yellow);
                }
                texture.Apply();
                yield return new WaitForSeconds(0.0001f);

            }
        }
    }
    Color Phong(RaycastHit hit) 
    {
        Color hitColor = hit.transform.GetComponent<MeshRenderer>().material.color;
        Vector3 luz = (lightTest.position - hit.point).normalized;
        Vector3 norm = hit.normal;
        Vector3 v = (transform.position - hit.point).normalized;
        Vector3 r = Vector3.Reflect(-luz, norm);

        Color amb = hitColor * 0.2f;
        float diff = Mathf.Max(0, Vector3.Dot(norm, luz));
        float spec = Mathf.Max(0, Mathf.Pow(Vector3.Dot(v, r), specular * 2 + 1));
        return hitColor * diff + spec * Color.white;
    }
    Color BlinnPhong(RaycastHit hit) 
    {
        Color hitColor = hit.transform.GetComponent<MeshRenderer>().material.color;
        Vector3 luz = (lightTest.position - hit.point).normalized;
        Vector3 norm = hit.normal;
        Vector3 v = (transform.position - hit.point).normalized;
        Vector3 h = (luz + v).normalized;
        float ndoth = Mathf.Max(0, Vector3.Dot(norm, h));
        float diff = Mathf.Max(0, Vector3.Dot(norm, luz));
        float spec = Mathf.Pow(ndoth, (specular * 2 + 1));
        return ambient * hitColor + diff * hitColor + spec * Color.white;
    }
}
