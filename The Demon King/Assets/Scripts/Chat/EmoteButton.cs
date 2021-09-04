using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmoteButton : Button
{
    private EmoteWheel emoteWheel;
    private Animation buttonHoverAnimation;
    private Emote emote;
    // Start is called before the first frame update
    void Start()
    {
        emoteWheel = GetComponentInParent<EmoteWheel>();
        emote = GetComponent<Emote>();
        buttonHoverAnimation = GetComponent<Animation>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        buttonHoverAnimation.Play("ButtonHoverAnimation");

        emoteWheel.emote = emote;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        buttonHoverAnimation.Play("ButtonHoverExitAnimation");
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        buttonHoverAnimation.Play("ButtonHoverAnimation");
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        buttonHoverAnimation.Play("ButtonHoverExitAnimation");
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        emoteWheel.ActivateEmote(emote);
        emoteWheel.emote = emote;
    }
}
