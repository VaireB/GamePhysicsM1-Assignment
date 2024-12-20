using UnityEngine;
using TMPro;

public class CannonController : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign the projectile prefab
    public Transform spawnPoint; // Where the projectile spawns
    public TextMeshProUGUI distanceText; // Assign a TextMeshProUGUI element in the inspector

    public float acceleration = 9.8f; // Simulating gravity (m/s^2)
    public float initialVelocity = 10f; // Initial velocity of the projectile (m/s)
    private Vector3 initialPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>(); // Get the Rigidbody from the prefab

        if (rb == null)
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody component!");
            return;
        }

        // Calculate the launch velocity using the formula X = 1/2at^2 + vt * i
        float time = 1.0f; // Example time step for initial velocity computation
        Vector3 launchVelocity = spawnPoint.forward * (0.5f * acceleration * Mathf.Pow(time, 2) + initialVelocity * time);
        rb.velocity = launchVelocity;

        // Track initial position
        initialPosition = spawnPoint.position;

        // Start tracking the distance
        StartCoroutine(LogDistanceTravelled(projectile, rb));
    }

    System.Collections.IEnumerator LogDistanceTravelled(GameObject projectile, Rigidbody rb)
    {
        bool hasHitGround = false;

        while (!hasHitGround && projectile != null && rb != null)
        {
            if (Physics.Raycast(projectile.transform.position, Vector3.down, out RaycastHit hit, 0.1f))
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Ground"))
                {
                    hasHitGround = true;
                }
            }

            yield return null;
        }

        // Log and display the total distance travelled
        if (projectile != null)
        {
            Vector3 finalPosition = projectile.transform.position;
            float distanceTravelled = Vector3.Distance(initialPosition, finalPosition);
            Debug.Log($"Distance Travelled: {distanceTravelled} meters");

            // Display the distance on the UI
            if (distanceText != null)
            {
                distanceText.text = $"Distance Travelled: {distanceTravelled:F2} meters";
            }

            // Destroy the projectile after logging distance
            Destroy(projectile);
        }
    }
}