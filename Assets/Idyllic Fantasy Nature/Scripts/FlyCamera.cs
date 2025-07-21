using UnityEngine;

public class FlyCamira : MonoBehaviour
{
    public float panSpeed = 20f;                 // سرعة التحرك في المحاور X وZ
    public float zoomSpeed = 2f;                 // سرعة الزوم
    public float rotateSpeed = 5f;               // سرعة التدوير بالزر الأيمن
    public float minY = 10f, maxY = 100f;        // حدود الزوم العمودي

    private Vector3 lastMousePosition;

    void Update()
    {
        HandlePan();
        HandleZoom();
        HandleRotation();
    }

    void HandlePan()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button (Pan)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * panSpeed * Time.deltaTime;
            transform.Translate(move, Space.Self);
        }

        lastMousePosition = Input.mousePosition;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 dir = transform.forward;
        transform.position += dir * scroll * zoomSpeed * 100f * Time.deltaTime;

        // Clamp height
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button (Rotate)
        {
            float rotX = Input.GetAxis("Mouse X") * rotateSpeed;
            float rotY = -Input.GetAxis("Mouse Y") * rotateSpeed;

            transform.eulerAngles += new Vector3(rotY, rotX, 0);
        }
    }
}
