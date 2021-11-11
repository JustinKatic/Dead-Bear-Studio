using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PerformanceDisplay : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI PingText;
    [SerializeField] TextMeshProUGUI FPSText;

    float frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

    void Update()
    {
        if (photonView.IsMine)
        {
            if (PingText.gameObject.activeSelf)
            {
                PingText.text = "Ping: " + PhotonNetwork.GetPing().ToString();
            }
            if (FPSText.gameObject.activeSelf)
            {
                frameCount++;
                dt += Time.deltaTime;
                if (dt > 1.0 / updateRate)
                {
                    fps = frameCount / dt;
                    frameCount = 0;
                    dt -= 1.0f / updateRate;
                    FPSText.text = "FPS: " + Mathf.Round(fps);
                }
            }
        }
    }





}
