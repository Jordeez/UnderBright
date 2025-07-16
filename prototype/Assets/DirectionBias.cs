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

    private CinemachineFramingTransposer transposer;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;

    void Start()
    {
        Transform player = virtualCam.Follow;
        if (player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
            playerRb = player.GetComponent<Rigidbody2D>();
            transposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    void Update()
    {
        if (playerSprite == null || transposer == null || playerRb == null) return;


        float facing = playerSprite.flipX ? -1f : 1f;
        float targetXOffset = sideOffsetAmount * facing;


        float verticalVelocity = playerRb.linearVelocity.y;
        bool isFalling = verticalVelocity < -0.1f;

        float targetYOffset = isFalling ? fallYOffset : defaultYOffset;
        float targetYDamping = isFalling ? fallDamping : normalDamping;


        Vector3 currentOffset = transposer.m_TrackedObjectOffset;
        Vector3 targetOffset = new Vector3(targetXOffset, targetYOffset, 0);
        transposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * 5f);

        transposer.m_YDamping = Mathf.Lerp(transposer.m_YDamping, targetYDamping, Time.deltaTime * 5f);
    }
}
