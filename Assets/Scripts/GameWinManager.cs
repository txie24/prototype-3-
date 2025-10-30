using UnityEngine;

public class GameWinManager : MonoBehaviour
{
    public static GameWinManager Instance { get; private set; }

    [Tooltip("Optional: assign a Canvas panel that says 'You Win'")]
    public GameObject winPanel;

    bool hasWon;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (winPanel) winPanel.SetActive(false);
    }

    public void Win()
    {
        if (hasWon) return; 
        hasWon = true;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (winPanel) winPanel.SetActive(true);

        Debug.Log("[GameWinManager] WIN triggered.");
    }
}
