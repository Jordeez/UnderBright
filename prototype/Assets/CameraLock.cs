using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider2D))]
public class LockedCameraZone : MonoBehaviour
{
    public CinemachineVirtualCamera lockedCam;
    public int lockedPriority = 20;
    public int defaultPriority = 5;
    public float screenPadding = 1f; // Extra space beyond room edges

    private BoxCollider2D box;
    private float originalZoom;
    private int originalPriority;

    void Start()
    {
        box = GetComponent<BoxCollider2D>();

        if (lockedCam != null)
        {
            originalZoom = lockedCam.m_Lens.OrthographicSize;
            originalPriority = lockedCam.Priority;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lockedCam != null)
        {
            // 1. Move camera to center of the room
            Vector3 roomCenter = box.bounds.center;
            lockedCam.transform.position = new Vector3(roomCenter.x, roomCenter.y, lockedCam.transform.position.z);

            // 2. Calculate proper zoom to fit the entire room
            float roomWidth = box.bounds.size.x + screenPadding;
            float roomHeight = box.bounds.size.y + screenPadding;

            float targetSize = roomHeight / 2f;
            float screenAspect = (float)Screen.width / Screen.height;
            float horizontalSize = (roomWidth / screenAspect) / 2f;

            lockedCam.m_Lens.OrthographicSize = Mathf.Max(targetSize, horizontalSize);

            // 3. Activate camera
            lockedCam.Priority = lockedPriority;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lockedCam != null)
        {
            lockedCam.Priority = defaultPriority;
        }
    }
}
