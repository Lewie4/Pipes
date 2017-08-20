using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    [SerializeField] private Toggle m_audioToggle;

    private void Start()
    {
        m_audioToggle.isOn = !SoundManager.Instance.GetMuted();
    }

    public void SendEmail()
    {
        string email = "Lewies@live.co.uk";
        string subject = MyEscapeURL("Pipes!");
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void ToogleAudio(bool active)
    {
        SoundManager.Instance.MuteAudio(active);
    }
}
