using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatListener : MonoBehaviour
{
    [SerializeField] Image m_Image = null;
    [SerializeField] Color m_Collect= Color.yellow;
    [SerializeField] Color m_OffRhythm = Color.black;

    private void Update()
    {
        if (WarriorBeat.Instance.IsInBeat()) m_Image.color = m_Collect;
        else m_Image.color = m_OffRhythm;
    }
}
