using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Player Health")]
    public int playerHealth = 10;
    [Header("Combat Stats")]
    public int attackPower = 5;
    public int playerDefense = 5;
    [Header("Magic Stats")]
    public int spellsNumber = 0;

     void update()
    {
        doubleJump();
    }

    void doubleJump()
    {
        // Implement double jump logic here
        // This is a placeholder for the actual double jump functionality
        Debug.Log("Double jump logic would be implemented here.");
        Vector2 jumpForce = new Vector2(0, 5f); // Example jump force
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
            Debug.Log("Player jumped with force: " + jumpForce);
        }
        else
        {
            Debug.LogWarning("Rigidbody2D component not found or Jump button not pressed.");
        }
    }

}
