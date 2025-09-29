using UnityEngine;
using Vuforia;

public class EnableGameObjectOnGround : MonoBehaviour
{
    public GameObject objectToEnable;   // The GameObject you want to enable
    public GameObject groundPlaneStage; // Reference to Ground Plane Stage

    private ContentPositioningBehaviour contentPositioningBehaviour;

    void Start()
    {
        contentPositioningBehaviour = GetComponent<ContentPositioningBehaviour>();

        if (contentPositioningBehaviour != null)
        {
            contentPositioningBehaviour.OnContentPlaced.AddListener(OnGroundPlaced);
        }

        // Make sure it's disabled at start
        if (objectToEnable != null)
            objectToEnable.SetActive(false);
    }

    void OnGroundPlaced(GameObject placedObject)
    {
        if (objectToEnable != null && !objectToEnable.activeSelf)
        {
            objectToEnable.SetActive(true);
            Debug.Log("Ground detected → object enabled!");
        }
    }
}
