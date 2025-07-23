using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyPrefabReference;

    private void Start()
    {
        // Prevent registering null or non-runtime instances
        if (enemyPrefabReference != null)
        {
            EnemyManager.Instance.RegisterEnemy(enemyPrefabReference, transform.position);
        }
    }
}
