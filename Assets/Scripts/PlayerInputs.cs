using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputs : MonoBehaviour
{
	public Vector2 Move { get; private set; }
	public Vector2 Look { get; private set; }
	public bool Jump { get; set; }
	public bool Sprint { get; set; }
	public bool Crouch { get; set; }

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	public void OnMove(InputValue value) => Move = value.Get<Vector2>();

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
		{
			Look = value.Get<Vector2>();
		}
	}

	public void OnJump(InputValue value) => Jump = value.isPressed;
	public void OnSprint(InputValue value) => Sprint = value.isPressed;
	public void OnCrouch(InputValue value) => Crouch = value.isPressed;

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
