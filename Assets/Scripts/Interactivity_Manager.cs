using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class Interactivity_Manager : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI timerDisplay;
    
    // public float test_float;
    [SerializeField]
    UniversalRendererData Feature;


    //[SerializeField]
    //private GameObject DirLight;
    private bool toggle = true;

    public GameObject selectedObject;
    public GameObject prevObject;
    Ray ray;
    RaycastHit hitData;

    float timeStop;
    public GameObject playerObj;
    public GameObject thCamera;
    public GameObject nCamera;
    public GameObject lCamera;

    public GameObject dialogueBox;
    public GameObject exitButton;

    // props
    public GameObject sakuraIkebanaPlant;
    public GameObject sakuraIkebana;
    public GameObject doorNoShadow;
    public GameObject door;
    public GameObject pottedPlantsWoPlant;
    public GameObject pottedPlants;
    public GameObject pottedPlantsMoved;
    public GameObject window;
    public GameObject windowNoShadow;
    public GameObject curtainsOpen;
    public GameObject curtainsClosed;
    public GameObject endDoor;

    //inventory
    string item;

    bool potsMoved;

    // timer
    public float timeRemaining;
    bool timerIsRunning;
    float min;
    float sec;

    // Start is called before the first frame update
    void Start()
    {
        // initialize vars
        item = "empty";
        potsMoved = false;

        playerObj = GameObject.FindWithTag("Player");
        thCamera = GameObject.Find("TeaHouseCamera");
        nCamera = GameObject.Find("NurseryCamera");
        lCamera = GameObject.Find("LibraryCamera");

        dialogueBox = GameObject.Find("DialogueBox");
        exitButton = GameObject.Find("Exit_Game");

        sakuraIkebanaPlant = GameObject.Find("sakura_ikebana_leaves");
        sakuraIkebana = GameObject.Find("sakura_ikebana");
        doorNoShadow = GameObject.Find("door_noShadow");
        door = GameObject.Find("door");
        pottedPlantsWoPlant = GameObject.Find("plants_emptyPot");
        pottedPlants = GameObject.Find("plants");
        pottedPlantsMoved = GameObject.Find("plants_moved");
        window = GameObject.Find("window");
        windowNoShadow = GameObject.Find("window_noShadow");
        curtainsOpen = GameObject.Find("curtains_open");
        curtainsClosed = GameObject.Find("curtains_closed");
        endDoor = GameObject.Find("end_door");

        // set inactive prop objects
        dialogueBox.SetActive(false);
        sakuraIkebana.SetActive(false);
        door.SetActive(false);
        pottedPlants.SetActive(false);
        pottedPlantsMoved.SetActive(false);
        windowNoShadow.SetActive(false);
        curtainsClosed.SetActive(false);
        endDoor.SetActive(false);
        
        timeRemaining = 180;
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning) {
            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
            } else {
                timeRemaining = 0;
                timerIsRunning = false;
                SceneManager.LoadScene(3);
            }
        }
        min = Mathf.FloorToInt(timeRemaining / 60);
        sec = Mathf.FloorToInt(timeRemaining % 60);
        timerDisplay.text = min + ":" + sec;

        Vector3 mousePos = Input.mousePosition;
        // {
        //     Debug.Log(mousePos.x);
        //     Debug.Log(mousePos.y);
        // }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // mouse on object
        if (Physics.Raycast(ray, out hitData, 1000)) {
            prevObject = selectedObject;
            selectedObject = hitData.transform.gameObject;

            // set object layer to NoPost
            if (selectedObject != prevObject && prevObject != null) {
                int LayerNoPost = LayerMask.NameToLayer("NoPost");
                prevObject.layer = LayerNoPost;
            }

            if (selectedObject.tag != "Untagged") {
                dialogueBox.SetActive(true);

                if (toggle == false) {
                    // set selectedObject layer to default post
                    int LayerOutline = LayerMask.NameToLayer("Default");
                    selectedObject.layer = LayerOutline;

                    // turn on outlines
                    toggle = true;
                    Feature.rendererFeatures[2].SetActive(toggle);
                }

                // clicked object
                if (selectedObject.tag == "EnterTeaHouseLevel") {
                    if (doorNoShadow.activeInHierarchy) {
                        dialogText.text = "This picture doesn't look quite right.";
                    } else {
                        dialogText.text = "";
                    }
                    dialogText.text = dialogText.text + "\nPress \"F\" to enter the picture.";

                    if (Input.GetKeyDown(KeyCode.F)) {
                        playerObj.transform.position = thCamera.transform.position;
                        playerObj.transform.rotation = thCamera.transform.rotation;
                    }
                } else if (selectedObject.tag == "EnterNurseryLevel") {
                    if (curtainsClosed.activeInHierarchy || window.activeInHierarchy) {
                        dialogText.text = "";
                    } else {                        
                        dialogText.text = "This picture doesn't look quite right.";
                    }
                    dialogText.text = dialogText.text + "\nPress \"F\" to enter the picture.";
                     
                    if (Input.GetKeyDown(KeyCode.F)) {
                        playerObj.transform.position = nCamera.transform.position;
                        playerObj.transform.rotation = nCamera.transform.rotation;
                    }
                    
                } else if (selectedObject.tag == "EnterLibraryLevel") {
                    dialogText.text = "Press \"F\" to return to the original room.";
                    if (Input.GetKeyDown(KeyCode.F)) {
                        playerObj.transform.position = lCamera.transform.position;
                        playerObj.transform.rotation = lCamera.transform.rotation;
                    }
                } else if (selectedObject.tag == "SakuraVaseWPlant") {
                    dialogText.text = "This leafy plant seems a bit out of place... \nPress \"F\" to take plant.";
                    if (Input.GetKeyDown(KeyCode.F) && item == "empty") {
                        sakuraIkebana.SetActive(true);
                        sakuraIkebanaPlant.SetActive(false);
                        item = "plant";
                    }
                } else if (selectedObject.tag == "SakuraVase") {
                    if (item == "plant") {
                        dialogText.text = "Press \"F\" to place plant.";

                        if (Input.GetKeyDown(KeyCode.F)) {
                            sakuraIkebana.SetActive(false);
                            sakuraIkebanaPlant.SetActive(true);
                            item = "empty";
                        }
                    }
                } else if (selectedObject.tag == "DoorNoShadow") {
                    dialogText.text = "This door seems to be missing a shadow.";
                    if (item == "window shadow") {
                        dialogText.text = dialogText.text + "\nPress \"F\" to give the door a shadow.";

                        if (Input.GetKeyDown(KeyCode.F)) {
                            doorNoShadow.SetActive(false);
                            door.SetActive(true);
                            item = "empty";
                        }
                    }                    
                } else if (selectedObject.tag == "Door") {
                    dialogText.text = "Press \"F\" to take the door.";
                    
                    if (Input.GetKeyDown(KeyCode.F) && item == "empty") {
                        door.SetActive(false);
                        item = "door";
                    }
                } else if (selectedObject.tag == "PottedPlantsWoPlant") {
                    dialogText.text = "One of the pots is missing a plant. \nPress \"F\" to add a plant.";

                    if (Input.GetKeyDown(KeyCode.F) && item == "plant") {
                        pottedPlantsWoPlant.SetActive(false);
                        pottedPlants.SetActive(true);
                        item = "empty";
                    }
                } else if (selectedObject.tag == "PottedPlants") {
                    dialogText.text = "Press \"F\" to move potted plants to the side.";
                    if (Input.GetKeyDown(KeyCode.F)) {
                        pottedPlants.SetActive(false);
                        pottedPlantsMoved.SetActive(true);
                        potsMoved = true;
                    }
                } else if (selectedObject.tag == "PottedPlantsMoved") {
                    dialogText.text = "Press \"F\" to move potted plants back in front of the window.";
                    if (Input.GetKeyDown(KeyCode.F)) {
                        pottedPlants.SetActive(true);
                        pottedPlantsMoved.SetActive(false);
                        potsMoved = false;
                    }
                } 
                else if (selectedObject.tag == "CurtainsOpen") {
                    dialogText.text = "Press \"F\" to close curtains.";

                    if (Input.GetKeyDown(KeyCode.F)) {
                        curtainsClosed.SetActive(true);
                        curtainsOpen.SetActive(false);
                    }
                } else if (selectedObject.tag == "CurtainsClosed") {
                    dialogText.text = "Press \"F\" to open curtains.";

                    if (Input.GetKeyDown(KeyCode.F)) {
                        curtainsClosed.SetActive(false);
                        curtainsOpen.SetActive(true);
                    }
                } else if (selectedObject.tag == "Window") {
                    dialogText.text = "This window's shape looks similar to the door's shape in that other painting...";

                    if (potsMoved) {
                        dialogText.text = dialogText.text + "\nPress \"F\" to take window shadow.";

                        if (Input.GetKeyDown(KeyCode.F) && item == "empty") {
                            window.SetActive(false);
                            windowNoShadow.SetActive(true);
                            item = "window shadow";
                        }
                    }                    
                } else if (selectedObject.tag == "WindowNoShadow") {
                    dialogText.text = "This window is missing a shadow.";

                    if (item == "window shadow") {
                        dialogText.text = dialogText.text + "\nPress \"F\" to give the window a shadow.";

                        if (Input.GetKeyDown(KeyCode.F)) {
                            window.SetActive(true);
                            windowNoShadow.SetActive(false);
                            item = "empty";
                        }
                    }
                } else if (selectedObject.tag == "Wall") {
                    if (!endDoor.activeInHierarchy) {
                        dialogText.text = "Where am I? How did I get here? I need to find a way out of this room.";

                        if (item == "door") {
                            dialogText.text = dialogText.text + "\nPress \"F\" to place a door on the wall.";

                            if (Input.GetKeyDown(KeyCode.F) && item == "door") {
                                endDoor.SetActive(true);
                                item = "empty";
                            }
                        }
                    } else {
                        selectedObject.tag = "Untagged";
                    }                    
                } else if (selectedObject.tag == "EndDoor") {
                    dialogText.text = "Press \"F\" to leave the room.";
                    if (Input.GetKeyDown(KeyCode.F)) {
                        SceneManager.LoadScene(2);
                    }
                }
                dialogText.text = dialogText.text + "\n\nInventory: " + item;
            } else {
                if (toggle == true) {
                    toggle = false;
                    Feature.rendererFeatures[2].SetActive(toggle);
                    
                    timeStop = Time.time;
                }
                float delta = 4;
                float curr = Time.time;
                float t = (curr - timeStop) / delta;

                dialogueBox.SetActive(false);
            }
        }      

        
    }
    
}
