using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] int targetFramerate = -1;
    // Start is called before the first frame update
    void Start()
    {
        if (targetFramerate >= 1)
        {
            Application.targetFrameRate = targetFramerate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update: " + Time.deltaTime);
    }

    void FixedUpdate()
    {
        Debug.Log("Fixed: " + Time.deltaTime);
    }
}
