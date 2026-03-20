using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HurtboxGroup>() && other.GetComponentInParent<PlayerController>())
        {
            GameSceneLoader.Instance.LoadSceneAsync("MainMenu");
        }
    }
}
