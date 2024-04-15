using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")]
public class CinemachineLockCameraYDeadZone : CinemachineExtension
{
    [Tooltip("Lock the camera's Y position to this value")]
    public float m_DefaultYPosition = 0; // Default Y position
    public float m_DeadZoneHeight = 5f; // Dead zone height
    private bool m_FollowingY = false; // Flag to indicate if following Y

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            var playerY = Player.INSTANCE.transform.position.y;

            // Check if the player is within the dead zone
            if (Mathf.Abs(playerY - m_DefaultYPosition) > m_DeadZoneHeight)
            {
                m_FollowingY = true; // Start following Y
            }
            else
            {
                m_FollowingY = false; // Stop following Y
            }

            // Adjust Y position based on follow state
            if (m_FollowingY)
            {
                pos.y = playerY;
            }
            else
            {
                pos.y = m_DefaultYPosition;
            }

            state.RawPosition = pos;
        }
    }
}
