using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimers : MonoBehaviour
{
    [Header("Stun")]
    [SerializeField] private Image StunParent;
    [SerializeField] private Image stunTimerImg;
    private bool playStun = false;

    [Header("Devour")]
    [SerializeField] private Image DevourParent;
    [SerializeField] private Image devouringTimerImg;
    private bool playDevouring = false;

    [Header("Being Devour")]
    [SerializeField] private Image BeingDevouredParent;
    [SerializeField] private Image beingDevouredTimerImg;
    private bool playBeingDevoured = false;

    [Header("Evolve")]
    [SerializeField] private Image EvolveParent;
    [SerializeField] private Image evolveingTimerImg;
    private bool playEvolve = false;

    [Header("Respawn")]
    [SerializeField] private Image RespawnParent;
    [SerializeField] private Image respawnTimerImg;
    private bool playRespawn = false;

    [Header("KingAbility")]
    [SerializeField] private Image KingAbilityParent;
    [SerializeField] private Image KingAbilityTimerImg;
    private bool playKingAbility = false;

    [Header("RayAbility")]
    [SerializeField] private Image RayAbilityParent;
    [SerializeField] private Image RayAbilityTimerImg;
    private bool playRayAbility = false;
    private bool playRayAbilityBackwards = false;



    private float TimeToCompleteAnimation;
    private float ActiveTime = 0f;

    private float ActiveDemonKingTime = 0f;
    private float TimeToCompleteDemonKingAbilityAnim;

    private float ActiveRayAbilityTime = 0f;
    private float TimeToCompleteRayAbilityAnim;




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

    #region RespawnTimer
    public void StartRespawnTimer(float Duration)
    {
        StartTimer(RespawnParent, Duration);
        playRespawn = true;
    }

    public void StopRespawnTimer()
    {
        StopTimer(RespawnParent);
        playRespawn = false;
    }
    #endregion

    #region KingAbilityTimer
    public void StartKingAbilityTimer(float Duration)
    {
        StartKingAbilityTimer(KingAbilityParent, Duration);
        playKingAbility = true;
    }

    public void StopKingAbilityTimer()
    {
        StopTimer(KingAbilityParent);
        playKingAbility = false;
    }
    #endregion

    #region RayAbilityTimer
    public void StartRayAbilityTimer(float Duration)
    {
        StartRayAbilityTimer(RayAbilityParent, Duration);
        playRayAbility = true;
    }
    public void StartRayAbilityBackwardsTimer(float Duration)
    {
        playRayAbilityBackwards = true;
    }

    public void StopRayAbilityTimer()
    {
        StopTimer(RayAbilityParent);
        playRayAbility = false;
        playRayAbilityBackwards = false;
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
        else if (playRespawn)
        {
            LerpFillImg(respawnTimerImg);
        }

        if (playKingAbility)
        {
            LerpKingAbilityFillImg(KingAbilityTimerImg);
        }

        if (playRayAbility)
        {
            LerpRayAbilityFillImg(RayAbilityTimerImg);
            if (playRayAbilityBackwards)
                playRayAbility = false;
        }
        else if (playRayAbilityBackwards)
        {
            LerpRayAbilityBackwardsFillImg(RayAbilityTimerImg);
        }
    }

    public void LerpFillImg(Image ImgToChange)
    {
        ActiveTime += Time.deltaTime;
        float percent = ActiveTime / TimeToCompleteAnimation;
        ImgToChange.fillAmount = Mathf.Lerp(0, 1, percent);
    }

    void StartTimer(Image ParentImg, float Duration)
    {
        ActiveTime = 0;
        TimeToCompleteAnimation = Duration;
        ParentImg.gameObject.SetActive(true);
    }

    void LerpKingAbilityFillImg(Image ImgToChange)
    {
        ActiveDemonKingTime += Time.deltaTime;
        float percent = ActiveDemonKingTime / TimeToCompleteDemonKingAbilityAnim;
        ImgToChange.fillAmount = Mathf.Lerp(0, 1, percent);
    }


    void StartKingAbilityTimer(Image ParentImg, float Duration)
    {
        ActiveDemonKingTime = 0;
        TimeToCompleteDemonKingAbilityAnim = Duration;
        ParentImg.gameObject.SetActive(true);
    }


    void LerpRayAbilityFillImg(Image ImgToChange)
    {
        ActiveRayAbilityTime += Time.deltaTime;
        float percent = ActiveRayAbilityTime / TimeToCompleteRayAbilityAnim;
        ImgToChange.fillAmount = Mathf.Lerp(0, 1, percent);
    }
    void LerpRayAbilityBackwardsFillImg(Image ImgToChange)
    {
        ActiveRayAbilityTime -= Time.deltaTime;
        float percent = ActiveRayAbilityTime / TimeToCompleteRayAbilityAnim;
        ImgToChange.fillAmount = Mathf.Lerp(0, 1, percent);
    }

    void StartRayAbilityTimer(Image ParentImg, float Duration)
    {
        ActiveRayAbilityTime = 0;
        TimeToCompleteRayAbilityAnim = Duration;
        ParentImg.gameObject.SetActive(true);
    }


    void StopTimer(Image ParentImg)
    {
        ParentImg.gameObject.SetActive(false);
    }
}
