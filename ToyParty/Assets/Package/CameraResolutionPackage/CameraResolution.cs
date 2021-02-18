using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        camera.orthographicSize = Screen.height / 2.0f;

        float x = Screen.width / 2.0f;
        float y = Screen.height / 2.0f;
        float z = camera.transform.position.z;
        camera.transform.position = new Vector3(x, y, z);
    }
}
