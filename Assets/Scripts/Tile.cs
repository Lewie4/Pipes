using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Empty,
        Corner,
        Line,
        Cross,
        Threeway,
        DeadEnd}

    ;

    [System.Serializable]
    public class TileProperties
    {
        public TileType m_tileType;
        public bool m_top;
        public bool m_left;
        public bool m_right;
        public bool m_bottom;
        public bool m_isFull;
    }

    [SerializeField] private TileProperties m_tileProperties;

    [SerializeField] private Image m_pipeImage;
    [SerializeField] private Sprite m_emptyImage;
    [SerializeField] private Sprite m_fullImage;

    public Tile(Tile tile)
    {
        m_tileProperties = tile.m_tileProperties;
        m_pipeImage = tile.m_pipeImage;
        m_emptyImage = tile.m_emptyImage;
        m_fullImage = tile.m_fullImage;
    }

    public void PipeFill()
    {
        m_pipeImage.sprite = m_fullImage;
        m_tileProperties.m_isFull = true;
    }

    public void PipeEmpty()
    {
        m_pipeImage.sprite = m_emptyImage;
        m_tileProperties.m_isFull = false;
    }

    public void PipeRotateCW()
    {
        if (!m_tileProperties.m_isFull)
        {
            bool temp = m_tileProperties.m_right;
            m_tileProperties.m_right = m_tileProperties.m_top;
            m_tileProperties.m_top = m_tileProperties.m_left;
            m_tileProperties.m_left = m_tileProperties.m_bottom;
            m_tileProperties.m_bottom = temp;

            Quaternion newRot = Quaternion.Euler(0, 0, this.transform.localRotation.eulerAngles.z - 90);

            this.transform.localRotation = newRot;
        }
    }

    public TileType GetTileType()
    {
        return m_tileProperties.m_tileType;
    }

    public bool GetTop()
    {
        return m_tileProperties.m_top;
    }

    public void SetTop(bool active)
    {
        m_tileProperties.m_top = active;
    }

    public bool GetLeft()
    {
        return m_tileProperties.m_left;
    }

    public void SetLeft(bool active)
    {
        m_tileProperties.m_left = active;
    }

    public bool GetRight()
    {
        return m_tileProperties.m_right;
    }

    public void SetRight(bool active)
    {
        m_tileProperties.m_right = active;
    }

    public bool GetBottom()
    {
        return m_tileProperties.m_bottom;
    }

    public void SetBottom(bool active)
    {
        m_tileProperties.m_bottom = active;
    }

    public bool GetIsFull()
    {
        return m_tileProperties.m_isFull;
    }
}
