using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviourPun
{
    public PlayerControllerRuntimeSet player;
    public TextMeshProUGUI popupText;
    public SOMenuData roomdata;
    PlayerController ourPlayer;

    [Header("First Task")]
    public GameObject[] LookAroundObjects;
    public string FirstTaskDisplayMessage;


    [Header("Second Task")]
    public string SecondTaskDisplayMessage;


    [Header("Third Task")]
    public string ThirdTaskDisplayMessage;
    public MinionHealthManager[] StunnedAI;

    [Header("Fourth Task")]
    public string FourthTaskDisplayMessage;
    public GameObject LearnToShootAI;
    EvolutionManager evolutionManager;

    [Header("Fourth Task")]
    public string FifthTaskDisplayMessage;
    public GameObject JumpPad1;

    [Header("Sixth Task")]
    public string SixthTaskDisplayMessage;
    public GameObject CrownModel;
    public GameObject CrownVFX;

    [Header("Seventh Task")]
    public string SeventhTaskDisplayMessage;
    public MinionHealthManager[] KingAbilityAI;

    [Header("Eigth Task")]
    public string EigthTaskDisplayMessage;
    public GameObject JumpPad2;
    public GameObject ScoreAI;






    private void Start()
    {
        roomdata.InTutorial = true;
        StartCoroutine(CheckIfPlayerHadLoaded());
        StartCoroutine(CheckIfAllObjectesHaveBeenLookedAt());
    }

    IEnumerator CheckIfPlayerHadLoaded()
    {
        bool PlayerLoaded = false;
        while (!PlayerLoaded)
        {
            if (player.Length() >= 1)
            {
                PlayerLoaded = true;
            }
            yield return null;
        }
        ourPlayer = player.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber);

        evolutionManager = ourPlayer.GetComponent<EvolutionManager>();
        //START FIRST TASK LOOK AT OBJECTS
        popupText.gameObject.SetActive(true);
        popupText.text = FirstTaskDisplayMessage;
    }

    IEnumerator CheckIfAllObjectesHaveBeenLookedAt()
    {
        bool allObjectsChecked = false;
        while (!allObjectsChecked)
        {
            int objectsChecked = 0;
            for (int i = 0; i < LookAroundObjects.Length; i++)
            {
                if (!LookAroundObjects[i].activeInHierarchy)
                    objectsChecked++;
            }
            if (objectsChecked >= 3)
                allObjectsChecked = true;
            yield return null;
        }
        //START SECOND TASK MOVE AND JUMP TO NEXT ISLAND
        popupText.text = SecondTaskDisplayMessage;
        ourPlayer.EnableMovement();
    }



    //START THIRD TASK CONSUME 3 Minions
    public void StartConsumeTutorial()
    {
        popupText.text = ThirdTaskDisplayMessage;

        foreach (var stunnedAI in StunnedAI)
        {
            stunnedAI.gameObject.SetActive(true);
            stunnedAI.TakeDamage(20, 1);
            StartCoroutine(ConsumedTutorialCheckForCompletion());
        }
    }

    IEnumerator ConsumedTutorialCheckForCompletion()
    {
        bool AllConsumed = false;
        while (!AllConsumed)
        {
            int consumed = 0;
            for (int i = 0; i < StunnedAI.Length; i++)
            {
                if (StunnedAI[i].reviving)
                    consumed++;
            }
            if (consumed >= 3)
                AllConsumed = true;
            yield return null;
        }
        //START FOURTH TASK
        ShootingTutorial();
    }

    //FOURTH TASK SHOOTING/EVOLVING
    public void ShootingTutorial()
    {
        popupText.text = FourthTaskDisplayMessage;
        LearnToShootAI.SetActive(true);
        StartCoroutine(EvolvedTutorialCheck());
    }

    IEnumerator EvolvedTutorialCheck()
    {
        bool hasEvolved = false;
        while (!hasEvolved)
        {
            if (evolutionManager.evolving)
                hasEvolved = true;
            yield return null;
        }
        //START Fifth TASK
        JumpPadTask();
    }

    //FIFTH TASK JUMP PAD
    void JumpPadTask()
    {
        popupText.text = FifthTaskDisplayMessage;
        JumpPad1.SetActive(true);
    }

    //SXITH TASK CONSUME THE CROWN
    public void ConsumeCrownTask()
    {
        popupText.text = SixthTaskDisplayMessage;
        CrownModel.SetActive(true);
        CrownVFX.SetActive(true);
        StartCoroutine(ConsumedCrownCheck());
    }

    IEnumerator ConsumedCrownCheck()
    {
        bool crownComsumed = false;
        while (!crownComsumed)
        {
            if (!CrownModel.activeInHierarchy)
                crownComsumed = true;
            yield return null;
        }
        //START SEVENTH TASK
        Debug.Log("Crown Consumed");
        KingAbility[] kingAbility = ourPlayer.GetComponentsInChildren<KingAbility>(true);
        foreach (var ability in kingAbility)
        {
            ability.shootCooldown = 1;
        }
        KingAbilityTask();
    }

    public void KingAbilityTask()
    {
        foreach (var ai in KingAbilityAI)
        {
            ai.gameObject.SetActive(true);
        }
        popupText.text = SeventhTaskDisplayMessage;
        StartCoroutine(KingAbilityTaskCheck());
    }


    IEnumerator KingAbilityTaskCheck()
    {
        bool hasStunnedKingTaskAI = false;
        while (!hasStunnedKingTaskAI)
        {
            int count = 0;
            foreach (var ai in KingAbilityAI)
            {
                if (ai.isStunned)
                    count++;
            }
            if (count >= 3)
                hasStunnedKingTaskAI = true;
            yield return null;
        }
        ScoreTask();
    }
    //START EIGHTH TASK
    public void ScoreTask()
    {
        popupText.text = EigthTaskDisplayMessage;
        JumpPad2.SetActive(true);
        ScoreAI.SetActive(true);
    }
}
