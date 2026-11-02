using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float max_CameraRange;
    [SerializeField] private float smoothTime = 0.3f;

    private float velocityX = 0f;

    private void Update()
    {
        float cameraPosX = Mathf.SmoothDamp(
            transform.position.x,
            player.transform.position.x,
            ref velocityX,
            smoothTime  
        );
        float clampXValue = Mathf.Clamp(cameraPosX, 0, max_CameraRange);
        transform.position = new Vector3(clampXValue, transform.position.y, transform.position.z);
    }
}
