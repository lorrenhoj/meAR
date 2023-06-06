using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public TapeManager tapeManager;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(Reset);
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
