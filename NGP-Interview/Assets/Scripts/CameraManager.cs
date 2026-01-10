using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;
    [Header("Base Offset")]
    [SerializeField] Vector3 baseOffset;
    [Header("Mouse Offset")]
    [SerializeField] float mouseOffsetStrength;
    [SerializeField] float mouseOffsetSmooth;
    [Header("Follow")]
    [SerializeField] float followSmooth;
    Vector3 currentMouseOffset;

    private void LateUpdate()
    {
        if (target == null) return;

        currentMouseOffset = Vector3.Lerp(currentMouseOffset, CalculateMouseOffset(), mouseOffsetSmooth * Time.deltaTime);
        Vector3 desiredPosition = target.position + baseOffset + currentMouseOffset;
        transform.position = Vector3.Lerp( transform.position, desiredPosition, followSmooth * Time.deltaTime);
    }

    private Vector3 CalculateMouseOffset()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        Vector2 normalized = (mousePos - screenCenter) / screenCenter;
        normalized = Vector2.ClampMagnitude(normalized, 1f);

        return new Vector3(normalized.x, 0f, normalized.y) * mouseOffsetStrength;
    }
}