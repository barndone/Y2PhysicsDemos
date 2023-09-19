using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class SlimePicker : MonoBehaviour
{
    [SerializeField] LayerMask slimeLayer;
    [SerializeField] LayerMask groundLayer;

    Camera cam;

    Vector3 mousePos = new Vector3(0, 0, 0);

    public List<SlimeMotor> slimeList = new();

    public int SelectedSlimes { get { return slimeList.Count; } }

    
    //[SerializeField] int selectedSlimes = 0;

    [SerializeField] float explosionRadius = 2.0f;
    [SerializeField] float explosionForce = 4.0f;
    [SerializeField] float explosionVerticalImpulse = 6.0f;

    Collider[] slimesInExplosionRange = new Collider[32];

    [SerializeField]bool selectWish = false;
    [SerializeField]bool explosionWish = false;
    [SerializeField]bool additiveSelectWish = false;
    [SerializeField]bool commandWish = false;
    [SerializeField]bool heldLastFrame = false;
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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                additiveSelectWish = true;
            }
            else
            {
                additiveSelectWish = false;
            }

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
            multiSelectionVisualizer.SetActive(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            commandWish = true;
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
            if (!multiSelectionVisualizer.activeSelf)
            {
                multiSelectionVisualizer.SetActive(true);
            }
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
                transform.y += multiSelectionVisualizer.transform.localScale.y / 2f;
                multiSelectionVisualizer.transform.position = transform;
            }
        }

        else if (selectWish)
        {
            if (!additiveSelectWish)
            {
                slimeList.Clear();
                Debug.Log("Shift not held while selecting, list cleared");
            }

            heldLastFrame = true;
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, slimeLayer))
            {
                Debug.Log("Hit " + hit.transform.gameObject.name, hit.transform.gameObject);
            
                var hitSlime = hit.transform.gameObject.GetComponent<SlimeMotor>();

                AddOverlappingSlimeMotor(hitSlime);

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

                            if (slimeRB.gameObject.TryGetComponent<Slime>(out var hitSlime))
                            {
                                hitSlime.TakeDamage(1);
                            }

                            Debug.Log("Explosion applied to: " + slimeRB.gameObject.name, slimeRB.gameObject);
                        }
                        else
                        {
                            Debug.Log("Explosion could not reach " + slimeRB.gameObject.name, slimeRB.gameObject);
                        }
                    }
                }
            }

            //Debug.Log(overlaps);
            explosionWish = false;
        }

        if (commandWish)
        {
            if (slimeList.Count != 0)
            {
                Ray ray = cam.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {

                    if (CheckForRigidbodyFromHit(hit))
                    {
                        foreach (var slime in slimeList)
                        {
                            slime.SetDestination(hit.collider.gameObject.transform);
                        }
                    }

                    else
                    {
                        foreach (var slime in slimeList)
                        {
                            slime.SetDestination(hit.point);
                        }
                    }
                }
            }

            commandWish = false;
        }
    }

    public void AddOverlappingSlimeMotor(SlimeMotor slime)
    {
        //  checks if this slime is NOT in the list currently
        //  due to the amount of slimes that will exist in the game, the list should never reach sizes where this becomes wildly inefficient
        if (!slimeList.Contains(slime) && slime.alive)
        {
            //  if this slime isn't we add it!
            slimeList.Add(slime);
        }
    }

    public bool CheckForRigidbodyFromHit(RaycastHit hit)
    {
        var hitObj = hit.collider.gameObject;

        return hitObj.TryGetComponent<Rigidbody>(out var rb);
    }
}
