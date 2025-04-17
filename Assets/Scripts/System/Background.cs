using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    //[SerializeField] private CinemachineCamera cinemachineCamera;

    private Transform cam;
    private Vector2 camstartposi;
    private float distance;

    //need on filler background behind (that will not move)
    private GameObject[] backgrounds;
    private Material[] mats;
    private float[] backspeed;

    private float farthestback;
    [Range(0.05f, 1f)] public float backgroundspeed;

    private void Awake()
    {
        cam = Camera.main.transform;
        camstartposi = cam.position;
    }

    private void Start()
    {
        int backgroundcount = transform.childCount;
        mats = new Material[backgroundcount];
        backspeed = new float[backgroundcount];
        backgrounds = new GameObject[backgroundcount];

        for (int i = 0; i < backgroundcount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mats[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        backspeedcalculation(backgroundcount);

        transform.position = new Vector3(cam.position.x, cam.position.y, 0);
    }
    public void BackgroundOnStart()
    {
        if(cam == null) cam = Camera.main.transform;
        //transform.position = cinemachineCamera.Target.TrackingTarget.transform.position;
        //transform.position = new Vector3(cam.position.x, cam.position.y, 0);
    }
    private void backspeedcalculation(int backgroundcount)
    {
        for (int i = 0; i < backgroundcount; i++)
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestback)
            {
                farthestback = backgrounds[i].transform.position.z - cam.position.z;
            }
        }
        for (int i = 0; i < backgroundcount; i++)
        {
            backspeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestback;
        }
    }
    //private void Update()
    //{
    //    var v3 = new Vector3(10 * transform.localScale.x, 10 * transform.localScale.y, transform.position.z);
    //    v3 = Camera.main.WorldToScreenPoint(v3);
    //    var v3Zero = Camera.main.WorldToScreenPoint(Vector3.zero);
    //    v3 = v3 - v3Zero;
    //    Debug.Log("Image screen size: " + v3.x + " x " + v3.y);
    //}


    private void LateUpdate()
    {
        distance = cam.position.x - camstartposi.x;

        //transform.position = Vector3.Lerp(transform.position, cinemachineCamera.Target.TrackingTarget.transform.position, 40 * Time.deltaTime);
        //transform.position = cinemachineCamera.Target.TrackingTarget.transform.position;
        //transform.position = Vector3.Lerp(transform.position, new Vector3(cam.position.x, cam.position.y, 0), 40 * Time.deltaTime);
        transform.position = new Vector3(cam.position.x, cam.position.y, 0);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backspeed[i] * backgroundspeed;
            mats[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
}
