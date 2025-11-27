using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject Treasure;
    public float duration = 2f;

    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerBehaviour>() != null && !pickedUp)
        {
            StartCoroutine(ShowMessage());
        }
    }

    IEnumerator ShowMessage()
    {
        pickedUp = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;


        if (Treasure != null)
        {
        Treasure.SetActive(true);
        }

    yield return new WaitForSeconds(duration);
        
        if(Treasure != null)
        {
        Treasure.SetActive(false);
        }
        Destroy(gameObject);
    
    }

}