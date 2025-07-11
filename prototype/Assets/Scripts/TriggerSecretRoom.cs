using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerSecretRoom : MonoBehaviour
{
    [Header("Secret Room Settings")]
    [SerializeField] private GameObject secretRoom;
    [SerializeField] private float activationDelay = 1f;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color fadeColor = Color.black;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ActivateSecretRoom());
        }
    }

    private IEnumerator ActivateSecretRoom()
    {
        // Fade in effect
        if (fadeImage != null)
        {
            fadeImage.color = fadeColor;
            fadeImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(activationDelay);
            fadeImage.gameObject.SetActive(false);
        }

        // Activate the secret room
        if (secretRoom != null)
        {
            secretRoom.SetActive(true);
        }
    }
}