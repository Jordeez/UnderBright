using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class CutScene : MonoBehaviour
    {
        [Header("Cutscene Settings")]
        [SerializeField] private float cutsceneDuration = 5f;
        [SerializeField] private GameObject cutsceneCamera;
        [SerializeField] private GameObject playerCharacter;

        private void Start()
        {
            StartCoroutine(PlayCutScene());
        }

        private IEnumerator PlayCutScene()
        {
            // Disable player controls
            if (playerCharacter != null)
            {
                playerCharacter.SetActive(false);
            }

            // Activate cutscene camera
            if (cutsceneCamera != null)
            {
                cutsceneCamera.SetActive(true);
            }

            // Wait for the duration of the cutscene
            yield return new WaitForSeconds(cutsceneDuration);

            // Deactivate cutscene camera and re-enable player controls
            if (cutsceneCamera != null)
            {
                cutsceneCamera.SetActive(false);
            }
            if (playerCharacter != null)
            {
                playerCharacter.SetActive(true);
            }
        }
    }
}