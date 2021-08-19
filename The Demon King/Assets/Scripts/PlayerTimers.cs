using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimers : MonoBehaviour
{
    [SerializeField] private Image stunImg;
    [SerializeField] private Image devouringImg;
    [SerializeField] private Image beingDevouredImg;
    [SerializeField] private Image evolveingImg;



    private float TimeToCompleteAnimation;
    private float ActiveTime = 0f;
    private bool playStun = false;
    private bool playDevouring = false;
    private bool playBeingDevoured = false;
    private bool playEvolve = false;




    #region StunTimer
    public void StartStunTimer(float Duration)
    {
        StartTimer(stunImg, Duration);
        playStun = true;
    }

    public void StopStunTimer()
    {
        StopTimer(stunImg);
        playStun = false;
    }
    #endregion

    #region DevourTimer
    public void StartDevourTimer(float Duration)
    {
        StartTimer(devouringImg, Duration);
        playDevouring = true;
    }

    public void StopDevourTimer()
    {
        StopTimer(devouringImg);
        playDevouring = false;
    }
    #endregion

    #region BeingDevouredTimer
    public void StartBeingDevouredTimer(float Duration)
    {
        StartTimer(beingDevouredImg, Duration);
        playBeingDevoured = true;
    }

    public void StopBeingDevouredTimer()
    {
        StopTimer(beingDevouredImg);
        playBeingDevoured = false;
    }
    #endregion

    #region EvolveTimer
    public void StartEvolveTimer(float Duration)
    {
        StartTimer(evolveingImg, Duration);
        playEvolve = true;
    }

    public void StopEvolveTimer()
    {
        StopTimer(evolveingImg);
        playEvolve = false;
    }
    #endregion

    public void Update()
    {
        if (playStun)
        {
            LerpFillImg(stunImg);
        }
        else if (playDevouring)
        {
            LerpFillImg(devouringImg);
        }
        else if (playBeingDevoured)
        {
            LerpFillImg(beingDevouredImg);
        }
        else if (playEvolve)
        {
            LerpFillImg(evolveingImg);
        }
    }

    public void LerpFillImg(Image ImgToChange)
    {
        ActiveTime += Time.deltaTime;
        float percent = ActiveTime / TimeToCompleteAnimation;
        ImgToChange.fillAmount = Mathf.Lerp(1, 0, percent);
    }

    void StartTimer(Image ImgToUseAsTimer, float Duration)
    {
        ActiveTime = 0;
        TimeToCompleteAnimation = Duration;
        ImgToUseAsTimer.gameObject.SetActive(true);
    }

    void StopTimer(Image ImgToStop)
    {
        ImgToStop.gameObject.SetActive(false);
    }
}
