using UnityEngine;

public class EnableDobbleJump : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            player.unlockDoubleJump = true;
            Destroy(gameObject);
        }
    }
}
