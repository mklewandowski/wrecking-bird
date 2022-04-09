using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    AudioClip FlapSound;
    [SerializeField]
    AudioClip WreckedSound;

    Vector2 movementVector = new Vector2(0, 0);
    float gravity = -20f;
    float flapVelocity = 6f;
    float minVelocity = -6f;
    float minY = -2.9f;
    float maxY = 5.4f;
    float screenMaxY = 5.4f;

    AudioSource audioSource;

    bool flap = false;

    void Awake()
    {
        audioSource = GameObject.Find("SceneManager").GetComponent<AudioSource>();
    }

    void Start()
    {
        Vector3 screenCorner = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        float screenMax = screenCorner.y * -1f;
        float characterOffset = .9f;
        screenMaxY = Mathf.Min(maxY, screenMax - characterOffset);
    }

    // Update is called once per frame
    void Update()
    {
        flap = Input.GetKeyDown ("space") | Input.GetButtonDown ("Fire1") | Input.GetButtonDown ("Fire2");
        if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            // apply some gravity, make Miley fall
            movementVector.y += gravity * Time.deltaTime;

            // user "flaps", give Miley some upward movement
            if (flap)
            {
                movementVector.y = flapVelocity;
                audioSource.PlayOneShot(FlapSound, 1f);
            }
            movementVector.y = Mathf.Max(movementVector.y, minVelocity);
            GetComponent<Rigidbody2D>().velocity = movementVector;

            // user dies if Miley hits the ground
            if (transform.localPosition.y < minY)
            {
                audioSource.PlayOneShot(WreckedSound, 1f);
                // save the y position so we can show a dead message
                Globals.WreckedYpos = gameObject.transform.localPosition.y;
                // hide the player
                gameObject.SetActive(false);
                // change game state
                Globals.CurrentGameState = Globals.GameState.Dead;
            }
            // limit Miley Y pos to stay onscreen
            if (transform.localPosition.y > screenMaxY)
            {
                Vector2 maxTopPos = new Vector2 (transform.localPosition.x, screenMaxY - .1f);
                transform.localPosition = maxTopPos;
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
