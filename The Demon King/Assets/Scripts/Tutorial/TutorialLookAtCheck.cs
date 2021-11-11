using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLookAtCheck : MonoBehaviour
{
    Camera cam;
    int count = 0;

    public LayerMask LayersToIgnore;


    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Tutorial")
            Destroy(this);
        cam = Camera.main;
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayersToIgnore))
        {
            if (hit.collider.gameObject.tag == "LookAtObj")
            {
                count++;
                hit.collider.gameObject.SetActive(false);
            }
        }

        if (count >= 3)
        {
            Destroy(this);
        }
    }

}
