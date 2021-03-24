using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUIScript : MonoBehaviour
{
    public Transform target;
    public int speed;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.position, Vector3.forward, speed * Time.deltaTime);
    }
}