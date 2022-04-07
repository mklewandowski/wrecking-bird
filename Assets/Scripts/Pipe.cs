using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField]
    AudioClip WreckedSound;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GameObject.Find("SceneManager").GetComponent<AudioSource>();
    }

    // check if the played collided with a pipe
    void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.gameObject.GetComponent<Player>();
        if (player != null && Globals.CurrentGameState == Globals.GameState.Playing)
        {
            audioSource.PlayOneShot(WreckedSound, 1f);
            // save the y position so we can show a dead message
            Globals.WreckedYpos = player.gameObject.transform.localPosition.y;
            // hide the player
            player.gameObject.SetActive(false);
            // change game state
            Globals.CurrentGameState = Globals.GameState.Dead;
        }
    }
}
