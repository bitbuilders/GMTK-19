﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class wonHUD : MonoBehaviour
{
    [SerializeField] won won = null;
    [SerializeField] TextMeshProUGUI m_WonText = null;
    [SerializeField] TextMeshProUGUI m_StreakText = null;
    [SerializeField] Image m_UpControl = null;
    [SerializeField] Image m_SideControl = null;
    [SerializeField] Image m_DownControl = null;
    [SerializeField, Range(0.5f, 5.0f)] float m_ControlsScale = 3.0f;

    void LateUpdate()
    {
        m_WonText.text = won.Won.ToString();
        m_StreakText.text = $"Streak: {won.Streak}";

        m_UpControl.sprite = DirectionList.Instance.UpAction.Sprite;
        SetImageDims(m_UpControl, DirectionList.Instance.UpAction.Sprite);

        m_SideControl.sprite = DirectionList.Instance.SideAction.Sprite;
        SetImageDims(m_SideControl, DirectionList.Instance.SideAction.Sprite);

        m_DownControl.sprite = DirectionList.Instance.DownAction.Sprite;
        SetImageDims(m_DownControl, DirectionList.Instance.DownAction.Sprite);
    }

    void SetImageDims(Image image, Sprite sprite)
    {
        if (!sprite) return;

        Vector2 size = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
        image.rectTransform.sizeDelta = size * 16.0f * m_ControlsScale;
    }
}
