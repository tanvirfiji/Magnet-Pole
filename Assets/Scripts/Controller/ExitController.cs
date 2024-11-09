using UnityEngine;
using way2tushar.Utility;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            StartCoroutine(SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name));
    }
}
