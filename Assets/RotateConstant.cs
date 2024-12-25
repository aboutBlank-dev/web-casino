using UnityEngine;

public class RotateConstant : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed;

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
