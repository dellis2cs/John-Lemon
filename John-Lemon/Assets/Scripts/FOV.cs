

using UnityEngine;

public class GhostTriggerEffect : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Distance from the enemy at which effects should trigger.")]
    public float triggerDistance = 2f;
    [Tooltip("Minimum dot product threshold to verify if the enemy is facing the player.")]
    public float dotThreshold = 0.5f;

    [Header("References")]
    [Tooltip("Reference to the player's transform.")]
    public Transform player; // Ensure to assign the player in the Inspector
    [Tooltip("Particle system that flashes from blue to red.")]
    public ParticleSystem ghostParticle;
    [Tooltip("AudioSource that plays the sound effect.")]
    public AudioSource ghostAudio;

    // Private variable to store the particle system's main module for color manipulation
    private ParticleSystem.MainModule particleMain;

    void Start()
    {
        // Get the main module of the particle system for modifying its properties
        if (ghostParticle != null)
        {
            particleMain = ghostParticle.main;
        }
        else
        {
            Debug.LogWarning("Ghost Particle System not assigned.");
        }
        
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // Check if the player reference is set
        if (player == null) return;

        // Calculate the distance between this enemy and the player
        float distance = Vector3.Distance(transform.position, player.position);

        // If the player is within trigger distance
        if (distance <= triggerDistance)
        {
            // Calculate normalized direction vector from enemy to player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            // Calculate dot product between enemy's forward vector and direction to the player
            float dot = Vector3.Dot(transform.forward, directionToPlayer);

            // Use dot product to determine if enemy is roughly facing the player
            if (dot >= dotThreshold)
            {
                Debug.Log("Enemy is facing the player and within range.");
                // PLAY PARTICLE EFFECT: if not already playing, start the particle system
                if (!ghostParticle.isPlaying)
                {
                    ghostParticle.Play();
                }

                // PLAY SOUND EFFECT: if not already playing, play the audio clip once
                if (!ghostAudio.isPlaying)
                {
                    ghostAudio.Play();
                }

                // CHANGE PARTICLE COLOR: use linear interpolation for color
                // Mathf.PingPong(Time.time, 1.0f) creates a repeating pattern between 0 and 1
                float t = Mathf.PingPong(Time.time, 1f);
                Color currentColor = Color.Lerp(Color.blue, Color.red, t);
                particleMain.startColor = currentColor;
            }
            else
            {
                // Optional: Stop effects if the enemy isn’t facing the player
                if (ghostParticle.isPlaying )
                {
                    ghostParticle.Stop();
                    ghostParticle.Clear();
                } if(ghostAudio.isPlaying)
                {
                    ghostAudio.Stop();
                }
            }
        }
        
    }
}
