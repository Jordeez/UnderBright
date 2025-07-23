using UnityEngine;
using Cinemachine;

public class CameraFaceOffset : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public float sideOffsetAmount = 3f;
    public float fallYOffset = -2f;
    public float defaultYOffset = 0f;
    public float fallDamping = 0.3f;
    public float normalDamping = 2f;
    public float dashDamping = 10f; // NEW: heavy damping during dash

    private CinemachineFramingTransposer transposer;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;

    private PlayerMovement playerMovement; // Get dash state

    void Start()
    {
        Transform player = virtualCam.Follow;
        if (player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
            playerRb = player.GetComponent<Rigidbody2D>();
            playerMovement = player.GetComponent<PlayerMovement>(); // <-- Make sure you have a reference to your movement script
            transposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    void Update()
    {
        if (playerSprite == null || transposer == null || playerRb == null || playerMovement == null) return;

        float facing = playerSprite.flipX ? 1f : -1f;
        float targetXOffset = sideOffsetAmount * facing;

        float verticalVelocity = playerRb.linearVelocity.y;
        bool isFalling = verticalVelocity < -0.1f;
        bool isDashing = playerMovement.IsDashing(); // You must expose this

        float targetYOffset = isFalling ? fallYOffset : defaultYOffset;
        float targetYDamping = isFalling ? fallDamping : normalDamping;
        float targetXDamping = isDashing ? dashDamping : normalDamping;

        Vector3 currentOffset = transposer.m_TrackedObjectOffset;
        Vector3 targetOffset = new Vector3(targetXOffset, targetYOffset, 0);
        transposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * 5f);

        transposer.m_YDamping = Mathf.Lerp(transposer.m_YDamping, isDashing ? dashDamping : targetYDamping, Time.deltaTime * 5f);
        transposer.m_XDamping = Mathf.Lerp(transposer.m_XDamping, targetXDamping, Time.deltaTime * 5f); // <--- affects horizontal smoothing
    }
}
