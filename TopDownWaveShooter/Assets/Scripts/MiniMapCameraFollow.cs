using UnityEngine;

public class MiniMapCameraFollow : MonoBehaviour
{

    public Transform playerTarget;
    public float yOffset = 16f;

    // Update is called once per frame
    void Update()
    {
        if(playerTarget != null)
        {
            transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y + yOffset, playerTarget.position.z);
        }
    }
}
