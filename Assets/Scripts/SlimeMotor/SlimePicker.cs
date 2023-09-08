using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SlimePicker : MonoBehaviour
{
    [SerializeField] LayerMask slimeLayer;
    [SerializeField] LayerMask groundLayer;
    
    Camera cam;
    
    Vector3 mousePos = new Vector3(0,0,0);

    [SerializeField] SlimeMotor[] activeSlimes = new SlimeMotor[10];
    
    [SerializeField] int selectedSlimes = 0;

    [SerializeField] float explosionRadius = 2.0f;
    [SerializeField] float explosionForce = 4.0f;
    [SerializeField] float explosionVerticalImpulse = 6.0f;

    Collider[] slimesInExplosionRange = new Collider[32];

    bool selectWish = false;
    bool explosionWish = false;

    bool heldLastFrame = false;
    public bool multiSelectWish = false;

    [SerializeField] GameObject multiSelectionVisualizer;
    [SerializeField] float      multiSelectGrowthRate = 1.25f;

    Vector3 defaultVisualizerScaler;
    float visualizerYOffset;

    [SerializeField] float maxVisualizerRadius = 10.0f;

    public static SlimePicker instance;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        cam = GetComponent<Camera>();
        defaultVisualizerScaler = multiSelectionVisualizer.transform.localScale;
        visualizerYOffset = multiSelectionVisualizer.transform.position.y;
    }

    private void Update()
    {
        mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            selectWish = true;
        }

        if (heldLastFrame && Input.GetMouseButton(0))
        {
            multiSelectWish = true;
            heldLastFrame = false;
        }

        if (Input.GetMouseButtonUp(0) && multiSelectWish)
        {
            multiSelectWish = false;
            multiSelectionVisualizer.transform.localScale = defaultVisualizerScaler;
        }

        if (Input.GetMouseButtonDown(1))
        {
            selectedSlimes = 0;
            Debug.Log("Slime selection cleared.");
        }

        if (Input.GetMouseButtonDown(2))
        {
            explosionWish = true;
        }
    }

    void FixedUpdate()
    {
        if (multiSelectWish)
        {

            if (multiSelectionVisualizer.transform.lossyScale.x <= maxVisualizerRadius)
            {
                var scale = multiSelectionVisualizer.transform.lossyScale;
                scale.x = scale.z += (multiSelectGrowthRate * Time.deltaTime);

                multiSelectionVisualizer.transform.localScale = scale;
            }

            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                var transform = hit.point;
                transform.y = visualizerYOffset;
                multiSelectionVisualizer.transform.position = transform;
            }
        }

        else if (selectWish)
        {
            heldLastFrame = true;
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (selectedSlimes != 0)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    for (int i = 0; i < selectedSlimes; i++)
                    {
                        activeSlimes[i].destination.position = hit.point;
                    }
                }
            }

            else
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, slimeLayer))
                {
                    Debug.Log("Hit " + hit.transform.gameObject.name, hit.transform.gameObject);

                    var hitSlime = hit.transform.gameObject.GetComponent<SlimeMotor>();

                    activeSlimes[selectedSlimes] = hitSlime;
                    selectedSlimes++;
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

    public void AddOverlappingSlimeMotor(SlimeMotor slime)
    {
        activeSlimes[selectedSlimes] = slime;
        selectedSlimes++;
    }
}
