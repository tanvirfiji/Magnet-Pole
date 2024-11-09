using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticPole : MonoBehaviour
{
    [Header("Magnetic Field Properties")]
    public bool isNorthPoleActive; // True = North Pole, False = South Pole
    public float magneticForceStrength = 10f;

    private Collider fieldCollider;

    private void Start()
    {
        fieldCollider = GetComponent<Collider>();

        if (fieldCollider == null)
        {
            Debug.LogError("No collider found! Ensure the object has a Box, Capsule, or Mesh Collider.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        MagneticPlayerController player = other.GetComponent<MagneticPlayerController>();
        if (player != null && fieldCollider != null)
        {
            Vector3 closestPoint = fieldCollider.ClosestPoint(other.transform.position);

            bool playerPole = player.IsNorthPoleActive();
            Vector3 direction;

            if (playerPole == isNorthPoleActive)
            {
                direction = (other.transform.position - closestPoint).normalized;
            }
            else
            {
                direction = (closestPoint - other.transform.position).normalized;
            }

            player.ApplyMagneticForce(direction * magneticForceStrength);
        }
    }
}
