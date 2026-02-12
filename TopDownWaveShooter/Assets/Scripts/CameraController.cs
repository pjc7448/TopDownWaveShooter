using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 10f, -5f);
    [SerializeField] float followSpeed = 5f;

    private void LateUpdate()
    {

        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

    }

    void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
