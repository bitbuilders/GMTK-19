using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Action
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

[System.Serializable]
public class DirectionalAction
{
    public Action Action;
    public Sprite Sprite;
}

public class DirectionList : Singleton<DirectionList>
{
    [SerializeField] Sprite m_FlairSprite = null;
    [SerializeField] Sprite m_PickupSprite = null;
    [SerializeField] Sprite m_SwapSprite = null;
    [SerializeField] Sprite m_DropSprite = null;
    [SerializeField] Sprite m_MoveSprite = null;
    [SerializeField] Sprite m_PunchSprite = null;
    [SerializeField] Sprite m_ThrowSprite = null;

    public DirectionalAction UpAction { get; private set; } = new DirectionalAction();
    public DirectionalAction SideAction { get; private set; } = new DirectionalAction();
    public DirectionalAction DownAction { get; private set; } = new DirectionalAction();

    public void SetFlairSprite()
    {
        UpAction.Sprite = m_FlairSprite;
    }

    public void SetPickupSprite()
    {
        UpAction.Sprite = m_PickupSprite;
    }

    public void SetSwapSprite()
    {
        UpAction.Sprite = m_SwapSprite;
    }

    public void SetDropSprite()
    {
        UpAction.Sprite = m_DropSprite;
    }

    public void SetMoveSprite()
    {
        SideAction.Sprite = m_MoveSprite;
    }

    public void SetPunchSprite()
    {
        DownAction.Sprite = m_PunchSprite;
    }

    public void SetThrowSprite()
    {
        DownAction.Sprite = m_ThrowSprite;
    }
}
