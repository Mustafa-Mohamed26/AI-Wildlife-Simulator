using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 10.0f;
    public float lookSpeed = 2.0f;
    public float boostMultiplier = 2.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        // Lock the cursor to the game window and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Mouse look
        rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Movement
        float speed = movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= boostMultiplier; // Boost speed when holding Shift

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(move * speed * Time.deltaTime);

        // Up and Down movement
        if (Input.GetKey(KeyCode.E))
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q))
            transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Unlock cursor with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
