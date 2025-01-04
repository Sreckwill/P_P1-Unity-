using UnityEngine;

public class GunIK : MonoBehaviour
{
    public Animator animator;         // Reference to the Animator
    public Transform rightHand;       // Right hand transform (bone of the character)
    public Transform gunSocket;       // Empty GameObject (socket) for the gun position
    public GameObject gun;            // The gun object to be held by the character

    private void OnAnimatorIK(int layerIndex)
    {
        // Ensure we are controlling IK only if the animator is present
        if (animator)
        {
            // Make sure the gun and socket are valid
            if (gun && rightHand != null && gunSocket != null)
            {
                // Set the weight for the IK (full control over the right hand)
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

                // Adjust the hand's position and rotation based on the gun socket
                animator.SetIKPosition(AvatarIKGoal.RightHand, gunSocket.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, gunSocket.rotation);
            }
        }
    }

    void Start()
    {
        // Ensure the gun is active in the scene
        if (gun != null)
        {
            gun.SetActive(true);
        }

        // Attach the gun to the socket at the start
        AttachGunToSocket();
    }

    void AttachGunToSocket()
    {
        if (gunSocket != null && gun != null)
        {
            // Set the gun's parent to the socket, ensuring the gun follows the hand
            gun.transform.SetParent(gunSocket);
            gun.transform.localPosition = Vector3.zero; // Reset position relative to socket
            gun.transform.localRotation = Quaternion.identity; // Reset rotation relative to socket

            // If necessary, adjust the gun's initial position/rotation relative to the socket
            // For example, you can offset the gun to fit better into the hand:
            Vector3 gunOffset = new Vector3(0, 0, 0.1f); // Adjust the Z-axis for proper fit
            gun.transform.localPosition += gunOffset;
        }
    }
}
