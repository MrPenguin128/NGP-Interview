using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingletonPersistent<InputManager>
{
    [SerializeField] InputActionAsset inputActions;
    public InputActionAsset InputActions => inputActions;
}
