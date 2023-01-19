using UnityEngine;

public class Player : MonoBehaviour
{
    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";

    [SerializeField] private OrbitCamera _orbitCamera;
    [SerializeField] private Transform _cameraFollowPoint;
    [SerializeField] private CustomCharacterController _customCharacter;
    [SerializeField] private CharacterRenderer _characterRenderer;

    private bool _isGameFocused;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _orbitCamera.FollowTo(_cameraFollowPoint, _characterRenderer);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _isGameFocused = hasFocus;
    }

    private void Update()
    {
        HandleCharacterInput();
    }

    private void LateUpdate()
    {
        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        Vector2 lookInputVector = GetLookInputVector();
        float zoomInput = GetZoomInput();

        _orbitCamera.UpdateWithInput(lookInputVector, zoomInput);

        if (Input.GetMouseButtonDown(1))
        {
            _orbitCamera.SwitchPointOfView();
        }
    }

    private static float GetZoomInput()
    {
        float zoomInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
            zoomInput = 0f;
#endif
        return zoomInput;
    }

    private Vector2 GetLookInputVector()
    {
        if (_isGameFocused)
        {
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            return new Vector2(mouseLookAxisUp, mouseLookAxisRight);
        }

        return Vector2.zero;
    }

    private void HandleCharacterInput()
    {
        var moveInputVector = new Vector3(Input.GetAxisRaw(HorizontalInput), 0f, Input.GetAxisRaw(VerticalInput));
        moveInputVector = Vector3.ClampMagnitude(moveInputVector, 1f);

        var characterInputs = new PlayerCharacterInputs(
            moveInputVector: moveInputVector,
            cameraRotation: _orbitCamera.Rotation,
            jumpDown: Input.GetKeyDown(KeyCode.Space),
            jumpHeld: Input.GetKey(KeyCode.Space),
            crouchDown: Input.GetKeyDown(KeyCode.C),
            crouchUp: Input.GetKeyUp(KeyCode.C),
            crouchHeld: Input.GetKey(KeyCode.C),
            chargingDown: Input.GetKeyDown(KeyCode.Q),
            noClipDown: Input.GetKeyUp(KeyCode.Z),
            noClipFlyUp: Input.GetKey(KeyCode.Space),
            noClipFlyDown: Input.GetKey(KeyCode.C));

        _customCharacter.SetInputs(in characterInputs);
    }
}