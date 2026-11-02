using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private bool locked;
    [SerializeField] private string nextSceneName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!locked)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    public void UnlockDoor()
    {
        locked = false;
    }
}
