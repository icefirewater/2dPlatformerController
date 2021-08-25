using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    [SerializeField] private GameObject
        deathBloodParticle,
        deathChunkParticle;
    
    private float currentHealth;
    private GameManager gameManager;

    private void Start()
    {
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void DecreaseHealth(float _amount)
    {
        currentHealth -= _amount;

        if(currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathChunkParticle, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        gameManager.Respawn();
        Destroy(gameObject);
    }
}
