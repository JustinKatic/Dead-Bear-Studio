using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuffTimer : MonoBehaviour
{
    [SerializeField] private Image uiFill;

    private float TimeToCompleteAnimation;
    private float ActiveTime = 0f;
    private bool playAnim = false;

    public void StartTimer(float Duration)
    {
        ActiveTime = 0;
        TimeToCompleteAnimation = Duration;
        uiFill.gameObject.SetActive(true);
        playAnim = true;
    }

    public void StopTimer()
    {
        uiFill.gameObject.SetActive(false);
        playAnim = false;
    }


    public void Update()
    {
        if (playAnim)
        {
            ActiveTime += Time.deltaTime;
            var percent = ActiveTime / TimeToCompleteAnimation;
            uiFill.fillAmount = Mathf.Lerp(1, 0, percent);
        }
    }
}
