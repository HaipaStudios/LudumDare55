using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")]
public class CinemachineLockCameraYDeadZoneSmooth : CinemachineExtension
{
    [Tooltip("Lock the camera's Y position to this value")]
    public float m_DefaultYPosition = 0; // Default Y position
    public float m_DeadZoneHeight = 5f; // Dead zone height
    public float m_FollowSpeed = 5f; // Speed of camera follow

    private float m_TargetYOffset = 0f; // Target offset for smooth follow

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            var playerY = Player.INSTANCE.transform.position.y;

            // Calculate target Y offset based on player position and dead zone
            float yOffset = Mathf.Clamp(playerY - m_DefaultYPosition, -m_DeadZoneHeight, m_DeadZoneHeight);
            m_TargetYOffset = Mathf.Lerp(m_TargetYOffset, yOffset, m_FollowSpeed * deltaTime);

            // Apply the offset to the camera's Y position
            pos.y = m_DefaultYPosition + m_TargetYOffset;

            state.RawPosition = pos;
        }
    }
}
