using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviousLevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public string playerTag = "Player"; // optional: filter by tag

    // Called when something enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) // only trigger for player
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
