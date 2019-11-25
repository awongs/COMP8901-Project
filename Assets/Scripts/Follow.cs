using UnityEngine;

public class Follow : MonoBehaviour
{
    // Position and rotation offsets for the camera.
    public Vector3 positionOffset;
    public Vector3 rotation;
    private void Update()
    {
        Camera.main.transform.position = transform.position + positionOffset;
        Camera.main.transform.rotation = Quaternion.Euler(rotation);
    }
}
