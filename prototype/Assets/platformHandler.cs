using System.Collections;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;

    [SerializeField] private BoxCollider2D playerCollider;

    private bool dropping = false;

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && currentOneWayPlatform != null && !dropping)
        {
            StartCoroutine(DisableCollision());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == currentOneWayPlatform)
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        dropping = true;

        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
        if (platformCollider != null)
        {
            // Disable collision
            Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

            // Give time for the player to fall through
            yield return new WaitForSeconds(0.25f);

            // Re-enable collision
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        }

        dropping = false;
    }
}
