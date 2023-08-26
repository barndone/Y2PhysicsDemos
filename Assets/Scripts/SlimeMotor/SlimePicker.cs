using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SlimePicker : MonoBehaviour
{
    [SerializeField] LayerMask slimeLayer;
    
    Camera cam;
    
    Vector3 mousePos = new Vector3(0,0,0);

    [SerializeField] SlimeMotor activeSlime = null;

    [SerializeField] float explosionRadius = 2.0f;
    [SerializeField] float explosionForce = 4.0f;
    [SerializeField] float explosionVerticalImpulse = 6.0f;

    Collider[] slimesInExplosionRange = new Collider[32];

    bool selectWish = false;
    bool explosionWish = false;

    void Awake()
    {
        cam = GetComponent<Camera>();    
    }

    private void Update()
    {
        mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            selectWish = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (activeSlime != null)
            {
                activeSlime = null;
                Debug.Log("Slime selection cleared.");
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            explosionWish = true;
        }
    }

    void FixedUpdate()
    {
        if (selectWish)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (activeSlime != null)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Debug.Log("Destination changed from: " + activeSlime.destination.position);
                    activeSlime.destination.position = hit.point;
                    Debug.Log("Destination changed to: " + activeSlime.destination.position);
                }
            }

            else
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, slimeLayer))
                {
                    Debug.Log("Hit " + hit.transform.gameObject.name, hit.transform.gameObject);

                    activeSlime = hit.transform.gameObject.GetComponent<SlimeMotor>();
                }
            }

            selectWish = false;
        }

        if (explosionWish)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            int overlaps = 0;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                overlaps = Physics.OverlapSphereNonAlloc(hit.point, explosionRadius, slimesInExplosionRange, slimeLayer);

                for (int i = 0; i < overlaps; i++)
                {
                    var slimeRB = slimesInExplosionRange[i].gameObject.GetComponent<SlimeMotor>().rb;

                    var dir = slimeRB.position - hit.point;

                    Ray losCheck = new(hit.point, dir);
                    RaycastHit slimeHitCheck;

                    if (Physics.Raycast(losCheck, out slimeHitCheck, Mathf.Infinity))
                    {

                        if (1 << slimeHitCheck.collider.gameObject.layer == slimeLayer)
                        {
                            slimeRB.AddExplosionForce(explosionForce, hit.point, explosionRadius, explosionVerticalImpulse, ForceMode.Impulse);

                            Debug.Log("Explosion applied to: " + slimeRB.gameObject.name, slimeRB.gameObject);
                        }
                        else
                        {
                            Debug.Log("Explosion could not reach " + slimeRB.gameObject.name, slimeRB.gameObject);
                        }
                    }
                }
            }

            Debug.Log(overlaps);



            explosionWish = false;
        }
    }
}
