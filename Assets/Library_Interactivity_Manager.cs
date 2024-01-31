using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.SceneManagement;

public class Library_Interactivity_Manager : MonoBehaviour
{
    // public float test_float;
    [SerializeField]
    UniversalRendererData Feature;


    [SerializeField]
    private GameObject DirLight;
    private bool toggle = true;

    public GameObject selectedObject;
    public GameObject prevObject;
    Ray ray;
    RaycastHit hitData;

    float timeStop;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePos = Input.mousePosition;
        // {
        //     Debug.Log(mousePos.x);
        //     Debug.Log(mousePos.y);
        // }

        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        ray = Camera.main.ScreenPointToRay(center);
        
        // mouse on object
        if (Physics.Raycast(ray, out hitData, 1000)) {
            prevObject = selectedObject;
            selectedObject = hitData.transform.gameObject;

            // set object layer to NoPost
            if (selectedObject != prevObject && prevObject != null) {
                int LayerNoPost = LayerMask.NameToLayer("NoPost");
                prevObject.layer = LayerNoPost;
            }

            if (selectedObject.tag == "EnterTeaHouseLevel" || selectedObject.tag == "EnterNurseryLevel") {
                if (toggle == false) {
                    // set selectedObject layer to default post
                    int LayerOutline = LayerMask.NameToLayer("Default");
                    selectedObject.layer = LayerOutline;

                    // turn on outlines
                    toggle = true;
                    Feature.rendererFeatures[2].SetActive(toggle);
                }

                // clicked object
                if (Input.GetMouseButtonDown(0) && selectedObject.tag == "EnterTeaHouseLevel") {
                    SceneManager.LoadScene(1);
                } else if (Input.GetMouseButtonDown(0) && selectedObject.tag == "EnterNurseryLevel") {
                    SceneManager.LoadScene(2);
                }
            } else {
                if (toggle == true) {
                    toggle = false;
                    Feature.rendererFeatures[2].SetActive(toggle);
                    
                    timeStop = Time.time;
                }
                float delta = 4;
                float curr = Time.time;
                float t = (curr - timeStop) / delta;
            }
        }      

        
    }
    
}
