using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance = null;

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
        public List<Tile> m_topTiles = new List<Tile>();
        public List<Tile> m_leftTiles = new List<Tile>();
        public List<Tile> m_rightTiles = new List<Tile>();
        public List<Tile> m_bottomTiles = new List<Tile>();
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

    [Header("THE ACTUAL LEVELS")]

    public List<Level> m_levels = new List<Level>();

    [Header("THE CURRENT GAME BOARD")]

    [SerializeField] private GameObject m_topContainer;
    [SerializeField] private List<Tile> m_topTiles = new List<Tile>();
    [SerializeField] private GameObject m_leftContainer;
    [SerializeField] private List<Tile> m_leftTiles = new List<Tile>();
    [SerializeField] private GameObject m_rightContainer;
    [SerializeField] private List<Tile> m_rightTiles = new List<Tile>();
    [SerializeField] private GameObject m_bottomContainer;
    [SerializeField] private List<Tile> m_bottomTiles = new List<Tile>();

    [SerializeField] private GameObject m_boardContainter;
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

    private int m_numTotalEndPipes = 0;
    private int m_numFullEndPipes = 0;

    [SerializeField] private bool m_hasWon = false;
    [SerializeField] private List<GameObject> m_gameWonObjects = new List<GameObject>();

    [SerializeField] private bool m_hasLost = false;
    [SerializeField] private List<GameObject> m_gameLostObjects = new List<GameObject>();
    private bool m_lostPopupDisplayed = false;

    private int m_currentLevel = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        
        LoadLevel(GameManager.Instance.GetCurrentLevel());
    }

    public void LoadLevel(int level)
    {
        if (level < m_levels.Count)
        {
            m_currentLevel = level;

            m_timeToStart = m_levels[level].m_timeToStart;
            m_timeToFill = m_levels[level].m_timeToFill;
            m_gameBoard = m_levels[level].m_board;
            m_topTiles = m_levels[level].m_topTiles;
            m_leftTiles = m_levels[level].m_leftTiles;
            m_rightTiles = m_levels[level].m_rightTiles;
            m_bottomTiles = m_levels[level].m_bottomTiles;

            SetBoardSize();
            PositionTiles();
            PositionTopTiles();
            PositionLeftTiles();
            PositionRightTiles();
            PositionBottomTiles();
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

        m_boardContainter.GetComponent<RectTransform>().sizeDelta = RT.sizeDelta;
    }

    private void PositionTiles()
    {
        m_scale = RT.sizeDelta.x / m_gameBoard.Count;
        m_halfScale = m_scale / 2;

        m_sizeScale = m_scale / m_tileSize;

        for (int i = 0; i < m_levels[m_currentLevel].m_board.Count; i++)
        {
            for (int j = 0; j < m_levels[m_currentLevel].m_board[i].m_column.Count; j++)
            {
                if (m_levels[m_currentLevel].m_board[i].m_column[j] != null)
                {
                    var currentTile = Instantiate(m_levels[m_currentLevel].m_board[i].m_column[j], m_boardContainter.transform);
                    m_gameBoard[i].m_column[j] = currentTile;
                    m_gameBoard[i].m_column[j].transform.localPosition = new Vector3(m_halfScale + (i * m_scale), m_halfScale + (j * m_scale), 0);
                    m_gameBoard[i].m_column[j].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                }
            }
        }
    }

    private void PositionTopTiles()
    {
        RectTransform topRT = m_topContainer.GetComponent<RectTransform>();
        topRT.sizeDelta = new Vector2(RT.sizeDelta.x, m_scale * 2);
        float currentX = -topRT.sizeDelta.x / 2;

        for (int i = 0; i < m_levels[m_currentLevel].m_topTiles.Count; i++)
        {
            if (m_levels[m_currentLevel].m_topTiles[i] != null)
            {
                var currentTile = Instantiate(m_levels[m_currentLevel].m_topTiles[i], m_topContainer.transform);
                m_topTiles[i] = currentTile;

                m_topTiles[i].transform.localPosition = new Vector3(currentX + m_halfScale + (i * m_scale), m_scale, 0);
                m_topTiles[i].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                m_topTiles[i].GetComponent<Button>().enabled = false;

                if (m_topTiles[i].GetIsFull())
                {
                    m_pipesToFill.Add(new PipesToFill(new Vector2(i, m_gameBoard.Count - 1), FillLocation.Top));
                }
                else
                {
                    m_numTotalEndPipes++;
                }
            }
        }
    }

    private void PositionLeftTiles()
    {
        RectTransform leftRT = m_leftContainer.GetComponent<RectTransform>();
        leftRT.sizeDelta = new Vector2(m_scale, RT.sizeDelta.y);
        float currentY = -leftRT.sizeDelta.y / 2;

        for (int i = 0; i < m_levels[m_currentLevel].m_leftTiles.Count; i++)
        {
            if (m_levels[m_currentLevel].m_leftTiles[i] != null)
            {
                var currentTile = Instantiate(m_levels[m_currentLevel].m_leftTiles[i], m_leftContainer.transform);
                m_leftTiles[i] = currentTile;

                m_leftTiles[i].transform.localPosition = new Vector3(-m_halfScale, currentY + m_halfScale + (i * m_scale), 0);
                m_leftTiles[i].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                m_leftTiles[i].GetComponent<Button>().enabled = false;

                if (m_leftTiles[i].GetIsFull())
                {
                    m_pipesToFill.Add(new PipesToFill(new Vector2(0, i), FillLocation.Left));
                }
                else
                {
                    m_numTotalEndPipes++;
                }
            }
        }
    }

    private void PositionRightTiles()
    {
        RectTransform rightRT = m_rightContainer.GetComponent<RectTransform>();
        rightRT.sizeDelta = new Vector2(m_scale, RT.sizeDelta.y);
        float currentY = -rightRT.sizeDelta.y / 2;

        for (int i = 0; i < m_levels[m_currentLevel].m_rightTiles.Count; i++)
        {
            if (m_levels[m_currentLevel].m_rightTiles[i] != null)
            {
                var currentTile = Instantiate(m_levels[m_currentLevel].m_rightTiles[i], m_rightContainer.transform);
                m_rightTiles[i] = currentTile;

                m_rightTiles[i].transform.localPosition = new Vector3(m_halfScale, currentY + m_halfScale + (i * m_scale), 0);
                m_rightTiles[i].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                m_rightTiles[i].GetComponent<Button>().enabled = false;

                if (m_rightTiles[i].GetIsFull())
                {
                    m_pipesToFill.Add(new PipesToFill(new Vector2(m_gameBoard.Count - 1, i), FillLocation.Right));
                }
                else
                {
                    m_numTotalEndPipes++;
                }
            }
        }
    }

    private void PositionBottomTiles()
    {
        RectTransform bottomRT = m_bottomContainer.GetComponent<RectTransform>();
        bottomRT.sizeDelta = new Vector2(RT.sizeDelta.x, m_scale * 2);
        float currentX = -bottomRT.sizeDelta.x / 2;

        for (int i = 0; i < m_levels[m_currentLevel].m_bottomTiles.Count; i++)
        {
            if (m_levels[m_currentLevel].m_bottomTiles[i] != null)
            {
                var currentTile = Instantiate(m_levels[m_currentLevel].m_bottomTiles[i], m_bottomContainer.transform);
                m_bottomTiles[i] = currentTile;

                m_bottomTiles[i].transform.localPosition = new Vector3(currentX + m_halfScale + (i * m_scale), -m_scale, 0);
                m_bottomTiles[i].transform.localScale = new Vector3(m_sizeScale, m_sizeScale, 0);
                m_bottomTiles[i].GetComponent<Button>().enabled = false;

                if (m_bottomTiles[i].GetIsFull())
                {
                    m_pipesToFill.Add(new PipesToFill(new Vector2(i, 0), FillLocation.Bottom));
                }
                else
                {
                    m_numTotalEndPipes++;
                }
            }
        }
    }

    private void Update()
    {
        if (!m_hasLost)
        {
            if (m_hasWon && m_pipesToFill.Count == 0)
            {
                if (m_gameWonObjects != null)
                {
                    for (int i = 0; i < m_gameWonObjects.Count; i++)
                    {
                        if (!m_gameWonObjects[i].activeSelf)
                        {
                            m_gameWonObjects[i].SetActive(true);

                            SendLevelCompletedAnalytic();

                            GameManager.Instance.GainLives(1);
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

                        int currentFillCount = m_pipesToFill.Count;

                        if (currentFillCount > 0)
                        {
                            for (int i = 0; i < currentFillCount; i++)
                            {      
                                bool endFilled = false;

                                if (m_pipesToFill[i].m_location.y < 0)
                                {
                                    int current = (int)m_pipesToFill[i].m_location.x;
                                    if (m_bottomTiles[current] != null)
                                    {
                                        m_bottomTiles[current].PipeFill();
                                        endFilled = true;
                                        m_numFullEndPipes++;
                                    }
                                    else
                                    {
                                        m_hasLost = true;
                                    }
                                }
                                else if (m_pipesToFill[i].m_location.x < 0)
                                {
                                    int current = (int)m_pipesToFill[i].m_location.y;
                                    if (m_leftTiles[current] != null)
                                    {
                                        m_leftTiles[current].PipeFill();
                                        endFilled = true;
                                        m_numFullEndPipes++;
                                    }
                                    else
                                    {
                                        m_hasLost = true;
                                    }
                                }
                                else if (m_pipesToFill[i].m_location.x >= m_gameBoard.Count)
                                {
                                    int current = (int)m_pipesToFill[i].m_location.y;
                                    if (m_rightTiles[current] != null)
                                    {
                                        m_rightTiles[current].PipeFill();
                                        endFilled = true;
                                        m_numFullEndPipes++;
                                    }
                                    else
                                    {
                                        m_hasLost = true;
                                    }
                                }
                                else if (m_pipesToFill[i].m_location.y >= m_gameBoard.Count)
                                {
                                    int current = (int)m_pipesToFill[i].m_location.x;
                                    if (m_topTiles[current] != null)
                                    {
                                        m_topTiles[current].PipeFill();
                                        endFilled = true;
                                        m_numFullEndPipes++;
                                    }
                                    else
                                    {
                                        m_hasLost = true;
                                    }
                                }

                                if (m_numFullEndPipes == m_numTotalEndPipes)
                                {
                                    m_hasWon = true;
                                }

                                if (!m_hasLost && !endFilled)
                                {
                                    Tile currentTile = m_gameBoard[(int)m_pipesToFill[i].m_location.x].m_column[(int)m_pipesToFill[i].m_location.y];
                                    if (currentTile != null)
                                    {
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
                                        else
                                        {
                                            if (!CheckFrom(currentTile, m_pipesToFill[i].m_filledFrom))
                                            {
                                                m_hasLost = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        m_hasLost = true;
                                    }
                                }
                            }

                            m_pipesToFill.RemoveRange(0, currentFillCount);
                        }
                        else
                        {
                            m_hasLost = true;
                        }
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
            if (m_gameLostObjects != null && !m_lostPopupDisplayed)
            {
                for (int i = 0; i < m_gameLostObjects.Count; i++)
                {
                    if (!m_gameLostObjects[i].activeSelf)
                    {
                        m_gameLostObjects[i].SetActive(true);
                        m_lostPopupDisplayed = true;
                    }
                }
                SendLevelFailedAnalytic();
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

    public float GetTimeToStart()
    {
        return m_timeToStart;
    }

    public float GetStartFillTime()
    {
        return m_startFillTime;
    }

    public void SendLevelCompletedAnalytic()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add("Level", GameManager.Instance.GetCurrentLevel());

        Analytics.CustomEvent("LevelCompleted", eventData);
    }

    public void SendLevelFailedAnalytic()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add("Level", GameManager.Instance.GetCurrentLevel());

        Analytics.CustomEvent("LevelFailed", eventData);
    }
}
