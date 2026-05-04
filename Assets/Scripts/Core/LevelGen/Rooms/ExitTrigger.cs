using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HurtboxGroup>() && other.GetComponentInParent<PlayerController>())
        {
            var player = other.GetComponentInParent<PlayerController>().gameObject;
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.SavePlayerState(player);
                LevelManager.Instance.AdvanceLevel();
            }

            if (LevelManager.Instance != null && LevelManager.Instance.HasMoreLevels)
            {
                GameSceneLoader.Instance.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }
            else
            {
                GameSceneLoader.Instance.LoadSceneAsync("MainMenu");
            }
        }
    }
}
