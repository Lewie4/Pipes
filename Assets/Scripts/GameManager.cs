using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [Header("Levels")]
    [SerializeField] private int m_currentLevel = 0;

    [Header("Lives")]
    [SerializeField] private int m_currentLives;
    [SerializeField] private int m_maxLives = 5;
    [SerializeField] private int m_timeToLivesRefil = 600;
    private int m_timeLivesSpent;
    private double m_timeOffset = 0;
    private bool m_hasUnlimitedLives = false;

    [SerializeField] private int m_currentCoins = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            #if !UNITY_EDITOR
            m_currentLevel = PlayerPrefs.GetInt("ProgressLevel", 0);
            #endif

            m_currentLives = PlayerPrefs.GetInt("CurrentLives", m_maxLives);
            m_timeLivesSpent = PlayerPrefs.GetInt("TimeLivesSpent", 0);

            var localTime = LocalTime();
            var networkTime = GetNetworkTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            m_timeOffset = localTime - networkTime;

            if (PlayerPrefs.GetInt("HasUnlimitedLives", 0) != 0)
            {
                m_hasUnlimitedLives = true;
            }

            CheckLives();

            m_currentCoins = PlayerPrefs.GetInt("CurrentCoins", 0);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }          
    }

    private void Update()
    {
        if (m_currentLives < m_maxLives)
        {
            CheckLives();
        }

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D) && DebugController.Instance != null)
        {
            DebugController.Instance.gameObject.SetActive(!DebugController.Instance.gameObject.activeSelf);
        }
        #endif
    }

    public int GetCurrentLevel()
    {
        return m_currentLevel;
    }

    public void SetCurrentLevel(int currentLevel)
    {
        m_currentLevel = currentLevel;
    }

    public void UnlockNextLevel()
    {
        m_currentLevel++;

        if (m_currentLevel > PlayerPrefs.GetInt("ProgressLevel", 0))
        {
            PlayerPrefs.SetInt("ProgressLevel", m_currentLevel);
            SendLevelUnlockedAnalytic();
            if (PlayFabManager.Instance != null)
            {
                PlayFabManager.Instance.SetCurrentLevel();
            }
        }
    }

    public void SendLevelUnlockedAnalytic()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add("Level", GameManager.Instance.GetCurrentLevel());

        Analytics.CustomEvent("LevelUnlocked", eventData);
    }

    public static DateTime GetNetworkTime()
    {
        try
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;     

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }
        catch (Exception exception)
        {
            Debug.Log("Could not get network time");
            Debug.Log(exception);
            return DateTime.Now;
        }
    }

    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
        ((x & 0x0000ff00) << 8) +
        ((x & 0x00ff0000) >> 8) +
        ((x & 0xff000000) >> 24));
    }

    public static double LocalTime()
    {
        return DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public void CheckLives()
    {            
        if (m_hasUnlimitedLives)
        {
            PlayerPrefs.SetInt("CurrentLives", m_maxLives);
            PlayerPrefs.SetInt("TimeLivesSpent", 0);
        }
        else
        {
            double diff = (LocalTime() - m_timeOffset) - m_timeLivesSpent;
            if (diff > m_timeToLivesRefil)
            {
                m_currentLives += (int)(diff / m_timeToLivesRefil);
                m_timeLivesSpent = (int)((LocalTime() - m_timeOffset) - (diff % m_timeToLivesRefil));

                if (m_currentLives >= m_maxLives)
                {
                    m_currentLives = m_maxLives;
                    m_timeLivesSpent = 0;
                }

                PlayerPrefs.SetInt("CurrentLives", m_currentLives);
                PlayerPrefs.SetInt("TimeLivesSpent", m_timeLivesSpent);
            }
        }
    }

    public int GetLivesTimer()
    {
        if (m_currentLives != m_maxLives)
        {
            int timer = (m_timeLivesSpent + m_timeToLivesRefil) - (int)(LocalTime() - m_timeOffset);

            if (timer > 0)
            {
                return timer;
            }
        }
        return 0;
    }

    public void SpendLives(int num)
    {
        if (!m_hasUnlimitedLives)
        {
            m_currentLives -= num;

            if (m_timeLivesSpent == 0)
            {
                m_timeLivesSpent = (int)(LocalTime() - m_timeOffset);
            }

            if (m_currentLives < 0)
            {
                m_currentLives = 0;
            }

            PlayerPrefs.SetInt("CurrentLives", m_currentLives);

            PlayerPrefs.SetInt("TimeLivesSpent", m_timeLivesSpent);
        }
    }

    public void GainLives(int num)
    {
        m_currentLives += num;

        if (m_currentLives >= m_maxLives)
        {
            m_currentLives = m_maxLives;
            m_timeLivesSpent = 0;
        }

        PlayerPrefs.SetInt("CurrentLives", m_currentLives);
    }

    public void SetTimeLivesSpentToCurrent()
    {
        m_timeLivesSpent = (int)(LocalTime() - m_timeOffset);
        PlayerPrefs.SetInt("TimeLivesSpent", m_timeLivesSpent);
    }

    public int GetCurrentLives()
    {
        if (m_hasUnlimitedLives)
        {
            return m_maxLives;
        }
        else
        {
            return m_currentLives;
        }
    }

    public int GetMaxLives()
    {
        return m_maxLives;
    }

    public void SetUnlimitedLives()
    {
        PlayerPrefs.SetInt("HasUnlimitedLives", 1);
        m_hasUnlimitedLives = true;
        m_currentLives = m_maxLives;
    }

    public bool GetUnlimitedLives()
    {
        return m_hasUnlimitedLives;
    }

    public void AddCoins(int amount)
    {
        m_currentCoins += amount;
        PlayerPrefs.SetInt("CurrentCoins", m_currentCoins);
        if (PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.SetCurrentCoins();
        }
    }

    public void SetCoins(int amount)
    {
        m_currentCoins = amount;
        PlayerPrefs.SetInt("CurrentCoins", m_currentCoins);
    }

    public int GetCurrentCoins()
    {
        return m_currentCoins;
    }
}
