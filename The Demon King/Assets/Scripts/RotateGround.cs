using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGround : MonoBehaviour
{
    public float RotateSpeed;

    //Rotate ground by speed (needs to be in fixxed update to allow character to attach to this)
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * RotateSpeed);
    }

}
