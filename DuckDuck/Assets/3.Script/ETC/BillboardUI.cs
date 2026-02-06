using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform _mainCamTransform;

    void Start()
    {
        if (Camera.main != null)
            _mainCamTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (_mainCamTransform == null) return;

        transform.rotation = _mainCamTransform.rotation;
    }
}