using UnityEngine;

public class CharFloat : MonoBehaviour
{
    [SerializeField]
    private float _floatStrength = 1; // ¿ØÖÆÕñ·ù

    private float _originalY;

    private void Start()
    {
        _originalY = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x,
            _originalY + Mathf.Sin(2 * Time.time) * _floatStrength,
            transform.position.z);
    }
}
