using UnityEngine;

public class Player : Character
{
    // The initial position of the player.
    public Vector3 spawnPosition;

    // Reference to the death screen interface.
    public GameObject deathScreen;

    // Reference to the character controller.
    private CharacterController m_characterController;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        if (m_characterController == null)
        {
            Debug.LogWarning("Couldn't find CharacterController on Player.");
        }

        transform.position = spawnPosition;
    }

    private void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 mousePoint = hit.point;
                Vector3 direction = mousePoint - transform.position;

                // Flatten and normalize the direction.
                direction.y = 0f;
                direction = Vector3.Normalize(direction);

                gun.Fire(direction);
            }
        }
    }

    private void HandleMovement()
    {
        // Player inputs.
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        // Get direction of movement.
        Vector3 direction = new Vector3(h, 0, v);

        m_characterController.Move(speed * direction * Time.deltaTime);
    }

    public override void Die()
    {
        gameObject.SetActive(false);
        deathScreen.SetActive(true);
    }
}
