using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimers : MonoBehaviour
{
    [Header("Stun")]
    [SerializeField] private Image StunParent;
    [SerializeField] private Image stunTimerImg;

    [Header("Devour")]
    [SerializeField] private Image DevourParent;
    [SerializeField] private Image devouringTimerImg;

    [Header("Being Devour")]
    [SerializeField] private Image BeingDevouredParent;
    [SerializeField] private Image beingDevouredTimerImg;

    [Header("Evolve")]
    [SerializeField] private Image EvolveParent;
    [SerializeField] private Image evolveingTimerImg;


    private float TimeToCompleteAnimation;
    private float ActiveTime = 0f;
    private bool playStun = false;
    private bool playDevouring = false;
    private bool playBeingDevoured = false;
    private bool playEvolve = false;




    #region StunTimer
    public void StartStunTimer(float Duration)
    {
        StartTimer(StunParent, Duration);
        playStun = true;
    }

    public void StopStunTimer()
    {
        StopTimer(StunParent);
        playStun = false;
    }
    #endregion

    #region DevourTimer
    public void StartDevourTimer(float Duration)
    {
        StartTimer(DevourParent, Duration);
        playDevouring = true;
    }

    public void StopDevourTimer()
    {
        StopTimer(DevourParent);
        playDevouring = false;
    }
    #endregion

    #region BeingDevouredTimer
    public void StartBeingDevouredTimer(float Duration)
    {
        StartTimer(BeingDevouredParent, Duration);
        playBeingDevoured = true;
    }

    public void StopBeingDevouredTimer()
    {
        StopTimer(BeingDevouredParent);
        playBeingDevoured = false;
    }
    #endregion

    #region EvolveTimer
    public void StartEvolveTimer(float Duration)
    {
        StartTimer(EvolveParent, Duration);
        playEvolve = true;
    }

    public void StopEvolveTimer()
    {
        StopTimer(EvolveParent);
        playEvolve = false;
    }
    #endregion

    public void Update()
    {
        if (playStun)
        {
            LerpFillImg(stunTimerImg);
        }
        else if (playDevouring)
        {
            LerpFillImg(devouringTimerImg);
        }
        else if (playBeingDevoured)
        {
            LerpFillImg(beingDevouredTimerImg);
        }
        else if (playEvolve)
        {
            LerpFillImg(evolveingTimerImg);
        }
    }

    public void LerpFillImg(Image ImgToChange)
    {
        ActiveTime += Time.deltaTime;
        float percent = ActiveTime / TimeToCompleteAnimation;
        ImgToChange.fillAmount = Mathf.Lerp(1, 0, percent);
    }

    void StartTimer(Image ParentImg, float Duration)
    {
        ActiveTime = 0;
        TimeToCompleteAnimation = Duration;
        ParentImg.gameObject.SetActive(true);
    }

    void StopTimer(Image ParentImg)
    {
        ParentImg.gameObject.SetActive(false);
    }
}
