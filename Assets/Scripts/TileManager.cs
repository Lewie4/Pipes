using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public enum FillLocation
    {
        None,
        Top,
        Left,
        Right,
        Bottom}

    ;

    [System.Serializable]
    public class Level
    {
        public List<Column> m_board = new List<Column>();
        public List<Tile> m_startTiles = new List<Tile>();
        public List<Tile> m_endTiles = new List<Tile>();
        public float m_timeToStart = 30f;
        public float m_timeToFill = 3f;
    }

    [System.Serializable]
    public class Column
    {
        public List<Tile> m_column = new List<Tile>();
    }

    [System.Serializable]
    public class PipesToFill
    {
        public Vector2 m_location = Vector2.zero;
        public FillLocation m_filledFrom = FillLocation.None;

        public PipesToFill(Vector2 loc, FillLocation from)
        {
            m_location = loc;
            m_filledFrom = from;
        }
    }

    [SerializeField] private List<Level> m_levels = new List<Level>();

    [Space(25f)]

    [SerializeField] private GameObject m_startContainer;
    [SerializeField] private List<Tile> m_startTiles = new List<Tile>();
    [SerializeField] private GameObject m_endContainer;
    [SerializeField] private List<Tile> m_endTiles = new List<Tile>();

    [SerializeField] private List<Column> m_gameBoard = new List<Column>();
    private float m_tileSize = 220f;

    private RectTransform RT;
    private float m_scale;
    private float m_halfScale;
    private float m_sizeScale;

    [SerializeField] private float m_timeToStart = 30f;
    [SerializeField] private float m_timeToFill = 3f;
    [SerializeField] private List<PipesToFill> m_pipesToFill = new List<PipesToFill>();
 
    private bool m_isFilling = false;
    private float m_startFillTime = 0;
    private float m_currentFillTime = 0;

    [SerializeField] private bool m_hasWon = false;
    [SerializeField] private List<GameObject> m_gameWonObjects = new List<GameObject>();

    [SerializeField] private bool m_hasLost = false;
    [SerializeField] private List<GameObject> m_gameLostObjects = new List<GameObject>();

    private void Awake()
    {
        LoadLevel(GameManager.Instance.m_currentLevel);

        SetBoardSize();
        PositionTiles();
        PositionStartTiles();
        PositionEndTiles();
    }

    private void LoadLevel(int level)
    {
        if (level < m_levels.Count)
        {
            m_timeToStart = m_levels[level].m_timeToStart;
            m_timeToFill = m_levels[level].m_timeToFill;
            m_gameBoard = m_levels[level].m_board;
            m_startTiles = m_levels[level].m_startTiles;
            m_endTiles = m_levels[level].m_endTiles;

        }
        else
        {
            Debug.LogError("Level out of range!");
        }
    }

    private void SetBoardSize()
    {
        RT = this.GetComponent<RectTransform>();
        RectTransform parentRT = this.transform.parent.GetComponent<RectTransform>();

        Vector2 parentSize = new Vector2(parentRT.rect.width, parentRT.rect.height);

        float diff = 0;
        if (parentSize.x >= parentSize.y)
        {
            float mod = Screen.width / 5;
            RT.sizeDelta = new Vector2(parentSize.y - mod, parentSize.y - mod);
            diff = parentSize.x - parentSize.y;
            RT.position = new Vector3(diff / 2, 0, 0);
        }
        else
        {
            float mod = Screen.width / 5;
            RT.sizeDelta = new Vector2(parentSize.x - mod, parentSize.x - mod);
            diff = parentSize.y - parentSize.x;
            RT.position = new Vector3(mod, diff / 2, 0);
        }
        RT.anchoredPosition3D = new Vector3((parentSize.x - RT.sizeDelta.x) / 2, (parentSize.y - RT.sizeDelta.y) / 2, 0);
    }

    private void PositionTiles()
    {
        m_scale = RT.sizeDelta.x / m_gameBoard.Count;
        m_halfScale = m_scale / 2;

        m_sizeScale = m_scale / m_tileSize;

        for (int i = 0; i < m_levels[GameManager.Instance.m_currentLevel].m_board.Count; i++)
        {
            for (int j = 0; j < m_levels[GameManager.Instance.m_currentLevel].m_board[i].m_column.Count; j++)
            {
                if (m_levels[GameManager.Instance.m_currentLevel].m_board[i].m_column[j] != null)
                {
                    var currentTile = Instantiate(m_levels[GameManager.Instance.m_currentLevel].m_board[i].m_column[j], this.transform);
                    m_gameBoard[i].m_column[j] = currentTile;
                    m_gameBoard[i].m_column[j].transform.localPosition = new Vector3(m_halfScale + (i * m_scale), m_halfScale + (j * m_scale), 0);
                    m_gameBoard[i].m_column[j].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                }
            }
        }
    }

    private void PositionStartTiles()
    {
        RectTransform startRT = m_startContainer.GetComponent<RectTransform>();
        startRT.sizeDelta = new Vector2(m_scale, RT.sizeDelta.x);
        float currentY = -startRT.sizeDelta.y / 2;

        for (int i = 0; i < m_levels[GameManager.Instance.m_currentLevel].m_startTiles.Count; i++)
        {
            if (m_levels[GameManager.Instance.m_currentLevel].m_startTiles[i] != null)
            {
                var currentTile = Instantiate(m_levels[GameManager.Instance.m_currentLevel].m_startTiles[i], m_startContainer.transform);
                m_startTiles[i] = currentTile;

                m_startTiles[i].transform.localPosition = new Vector3(-m_halfScale, currentY + m_halfScale + (i * m_scale), 0);
                m_startTiles[i].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                m_startTiles[i].PipeFill();

                m_pipesToFill.Add(new PipesToFill(new Vector2(0, i), FillLocation.Left));
            }
        }
    }

    private void PositionEndTiles()
    {
        RectTransform endRT = m_endContainer.GetComponent<RectTransform>();
        endRT.sizeDelta = new Vector2(m_scale, RT.sizeDelta.x);
        float currentY = -endRT.sizeDelta.y / 2;

        for (int i = 0; i < m_levels[GameManager.Instance.m_currentLevel].m_endTiles.Count; i++)
        {
            if (m_levels[GameManager.Instance.m_currentLevel].m_endTiles[i] != null)
            {
                var currentTile = Instantiate(m_levels[GameManager.Instance.m_currentLevel].m_endTiles[i], m_endContainer.transform);
                m_endTiles[i] = currentTile;

                m_endTiles[i].transform.localPosition = new Vector3(m_halfScale, currentY + m_halfScale + (i * m_scale), 0);
                m_endTiles[i].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                m_endTiles[i].GetComponent<Button>().enabled = false;
            }
        }
    }

    private void Update()
    {

        RT.position = RT.position;
        if (!m_hasLost)
        {
            if (m_hasWon)
            {
                if (m_gameWonObjects != null)
                {
                    for (int i = 0; i < m_gameWonObjects.Count; i++)
                    {
                        if (!m_gameWonObjects[i].activeInHierarchy)
                        {
                            m_gameWonObjects[i].SetActive(true);
                        }
                    }
                }
            }
            else
            {
                if (m_isFilling)
                {
                    if (m_currentFillTime >= m_timeToFill)
                    {
                        m_currentFillTime = 0;

                        if (m_pipesToFill.Count == 0)
                        {
                            GetStartPipe();
                        }

                        int currentFillCount = m_pipesToFill.Count;

                        for (int i = 0; i < currentFillCount; i++)
                        {      
                            bool endFilled = false;
                            if (m_pipesToFill[i].m_location.x < 0 || m_pipesToFill[i].m_location.y >= m_gameBoard.Count || m_pipesToFill[i].m_location.y < 0)
                            {
                                m_hasLost = true;
                            }
                            else if (m_pipesToFill[i].m_location.x >= m_gameBoard.Count)
                            {
                                if (m_endTiles[(int)m_pipesToFill[i].m_location.y] != null)
                                {
                                    m_endTiles[(int)m_pipesToFill[i].m_location.y].PipeFill();
                                    endFilled = true;

                                    int endTiles = 0;
                                    int filledEndTiles = 0;
                                    for (int j = 0; j < m_endTiles.Count; j++)
                                    {
                                        if (m_endTiles[j] != null)
                                        {
                                            endTiles++;
                                            if (m_endTiles[j].GetIsFull())
                                            {
                                                filledEndTiles++;
                                            }
                                        }
                                    }

                                    if (filledEndTiles >= endTiles)
                                    {
                                        m_hasWon = true;
                                    }
                                }
                                else
                                {
                                    m_hasLost = true;
                                }
                            }
                            if (!m_hasLost && !endFilled)
                            {
                                Tile currentTile = m_gameBoard[(int)m_pipesToFill[i].m_location.x].m_column[(int)m_pipesToFill[i].m_location.y];
                                if (!currentTile.GetIsFull())
                                {
                                    if (CheckFrom(currentTile, m_pipesToFill[i].m_filledFrom))
                                    {                                    
                                        currentTile.PipeFill();

                                        CheckTop(i);
                                        CheckLeft(i);
                                        CheckRight(i);
                                        CheckBottom(i);
                                    }
                                    else
                                    {
                                        m_hasLost = true;
                                    }
                                }
                            }
                        }

                        m_pipesToFill.RemoveRange(0, currentFillCount);
                    }
                    else
                    {
                        m_currentFillTime += Time.deltaTime;
                    }
                }
                else
                {
                    if (m_startFillTime < m_timeToStart)
                    {
                        m_startFillTime += Time.deltaTime;
                    }
                    else
                    {
                        m_isFilling = true;
                        m_currentFillTime = m_timeToFill;
                    }
                }
            }
        }
        else
        {
            if (m_gameLostObjects != null)
            {
                for (int i = 0; i < m_gameLostObjects.Count; i++)
                {
                    if (!m_gameLostObjects[i].activeInHierarchy)
                    {
                        m_gameLostObjects[i].SetActive(true);
                    }
                }
            }
        }
    }

    private void GetStartPipe()
    {
        for (int i = 0; i < m_gameBoard[0].m_column.Count; i++)
        {
            if (m_gameBoard[0].m_column[i].GetTileType() != Tile.TileType.Empty)
            {
                m_pipesToFill.Add(new PipesToFill(new Vector2(0, i), FillLocation.Left));

                break;
            }
        }
    }

    private bool CheckFrom(Tile currentTile, FillLocation filledFrom)
    {
        if (filledFrom == FillLocation.Top && currentTile.GetTop())
        {
            currentTile.SetTop(false);
            return true;
        }
        else if (filledFrom == FillLocation.Left && currentTile.GetLeft())
        {
            currentTile.SetLeft(false);
            return true;
        }
        else if (filledFrom == FillLocation.Right && currentTile.GetRight())
        {
            currentTile.SetRight(false);
            return true;
        }
        else if (filledFrom == FillLocation.Bottom && currentTile.GetBottom())
        {
            currentTile.SetBottom(false);
            return true;
        }
        return false;
    }

    private void CheckTop(int i)
    {
        if (m_gameBoard[(int)m_pipesToFill[i].m_location.x].m_column[(int)m_pipesToFill[i].m_location.y].GetTop())
        {       
            m_pipesToFill.Add(new PipesToFill(new Vector2(m_pipesToFill[i].m_location.x, m_pipesToFill[i].m_location.y + 1), FillLocation.Bottom));
        }
    }

    private void CheckLeft(int i)
    {
        if (m_gameBoard[(int)m_pipesToFill[i].m_location.x].m_column[(int)m_pipesToFill[i].m_location.y].GetLeft())
        {       
            m_pipesToFill.Add(new PipesToFill(new Vector2(m_pipesToFill[i].m_location.x - 1, m_pipesToFill[i].m_location.y), FillLocation.Right));
        }
    }

    private void CheckRight(int i)
    {
        if (m_gameBoard[(int)m_pipesToFill[i].m_location.x].m_column[(int)m_pipesToFill[i].m_location.y].GetRight())
        {       
            m_pipesToFill.Add(new PipesToFill(new Vector2(m_pipesToFill[i].m_location.x + 1, m_pipesToFill[i].m_location.y), FillLocation.Left));
        }
    }

    private void CheckBottom(int i)
    {
        if (m_gameBoard[(int)m_pipesToFill[i].m_location.x].m_column[(int)m_pipesToFill[i].m_location.y].GetBottom())
        {       
            m_pipesToFill.Add(new PipesToFill(new Vector2(m_pipesToFill[i].m_location.x, m_pipesToFill[i].m_location.y - 1), FillLocation.Top));
        }
    }

    public void FastForward()
    {
        m_timeToStart = 0;
        m_timeToFill = 0;
    }
}
