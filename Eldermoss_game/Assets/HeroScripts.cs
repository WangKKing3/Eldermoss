using UnityEngine;

public class HeroScripts : MonoBehaviour
{
    public Rigidbody2D myRigidbody2D;
    public float flapStrangth = 10f; // Styrken på hoppet
    public float moveSpeed = 5f;     // NY: Hastighet for sidelengs bevegelse

    void Start()
    {
        if (myRigidbody2D == null)
        {
            myRigidbody2D = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        // 1. Hopp/Flakse Logikk (Uendret fra forrige gang)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Nulstiller vertikal hastighet for å sikre et fullt hopp
            myRigidbody2D.linearVelocity = new Vector2(myRigidbody2D.linearVelocity.x, 0f);
            myRigidbody2D.linearVelocity += Vector2.up * flapStrangth;
        }

        // 2. Sidelengs Bevegelses Logikk

        // Få input fra aksen "Horizontal". Dette leser innspill fra A/D og Venstre/Høyre piltaster.
        // Verdien vil være: -1 (Venstre), 0 (Ingen input), eller 1 (Høyre).
        float horizontalInput = Input.GetAxis("Horizontal");

        // Lag en ny hastighetsvektor
        // X: Bruk inputen ganger hastigheten
        // Y: Behold karakterens nåværende vertikale hastighet (så den kan hoppe/falle samtidig)
        Vector2 newVelocity = new Vector2(horizontalInput * moveSpeed, myRigidbody2D.linearVelocity.y);

        // Sett Rigidbody2D's hastighet
        myRigidbody2D.linearVelocity = newVelocity;
    }
}