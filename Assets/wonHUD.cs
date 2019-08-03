using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class wonHUD : MonoBehaviour
{
    [SerializeField] won won = null;
    [SerializeField] TextMeshProUGUI m_WonText = null;
    [SerializeField] TextMeshProUGUI m_StreakText = null;

    void LateUpdate()
    {
        m_WonText.text = won.Won.ToString();
        m_StreakText.text = $"Streak: {won.Streak}";
    }
}
