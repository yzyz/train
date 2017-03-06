using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickLevel : MonoBehaviour
{

    public string sceneName = "NULL";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other) {
        // TODO: put in check that it's a controller that triggered it
        if (!other.tag.Equals("GameController"))
        {
            return;
        }

        SteamVR_TrackedObject trackedObj = other.GetComponentInParent<SteamVR_TrackedObject>();
        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            SceneManager.LoadScene(sceneName);
        }
    }
}