
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterEditor : EditorWindow
{
    string[] toolbarStrings = { "Ray Demon", "Lion Demon", "Dragon Demon" };

    List<Evolutions> dragonType = new List<Evolutions>();
    List<Evolutions> lionType = new List<Evolutions>();
    List<Evolutions> rayType = new List<Evolutions>();

    List<DragonAbility> dragonAbility = new List<DragonAbility>();
    List<AoeExplosionAbility> lionAbility = new List<AoeExplosionAbility>();
    List<LaserAbility> rayAbility = new List<LaserAbility>();

    Evolutions[] evolutions;

    int toolbarSel = 0;
    float maxHealth = 2;

    GUILayoutOption[] propertyFields = { GUILayout.Width(150) };
    GUILayoutOption[] textFields = { GUILayout.Width(150) };
    GUIStyle headings = new GUIStyle();

    //Evolution 0 Object
    SerializedObject evolution0;
    SerializedObject abilityEvolution0;
    //Evolution 0 Health Variables
    SerializedProperty maxHealthEvo0;
    SerializedProperty expWorthEvo0;
    SerializedProperty healAfterStunnedEvo0;
    SerializedProperty healWhenEvolvingEvo0;
    //Evolution 0 Projectile Variables
    SerializedProperty projectileSpeedEvo0;
    SerializedProperty damageEvo0;
    SerializedProperty cooldownEvo0;

    //Evolution 1 Object
    SerializedObject evolution1;
    SerializedObject abilityEvolution1;
    //Evolution 1 Health Variables
    SerializedProperty maxHealthEvo1;
    SerializedProperty expWorthEvo1;
    SerializedProperty healAfterStunnedEvo1;
    SerializedProperty healWhenEvolvingEvo1;
    //Evolution 1 Projectile Variables
    SerializedProperty projectileSpeedEvo1;
    SerializedProperty damageEvo1;
    SerializedProperty cooldownEvo1;

    //Evolution 2 Object
    SerializedObject evolution2;
    SerializedObject abilityEvolution2;

    //Evolution 2 Health Variables
    SerializedProperty maxHealthEvo2;
    SerializedProperty expWorthEvo2;
    SerializedProperty healAfterStunnedEvo2;
    SerializedProperty healWhenEvolvingEvo2;
    //Evolution 2 Projectile Variables
    SerializedProperty projectileSpeedEvo2;
    SerializedProperty damageEvo2;
    SerializedProperty cooldownEvo2;

    //Evolution 3 Object
    SerializedObject evolution3;
    SerializedObject abilityEvolution3;

    //Evolution 3 Health Variables
    SerializedProperty maxHealthEvo3;
    SerializedProperty expWorthEvo3;
    SerializedProperty healAfterStunnedEvo3;
    SerializedProperty healWhenEvolvingEvo3;
    //Evolution 3 Projectile Variables
    SerializedProperty projectileSpeedEvo3;
    SerializedProperty damageEvo3;
    SerializedProperty cooldownEvo3;

    //Non-Generic Ability Variables

    //AOE explosive Ability
    SerializedProperty aoeRadius1;
    SerializedProperty aoeDamage1;
    SerializedProperty aoeRadius2;
    SerializedProperty aoeDamage2;
    SerializedProperty aoeRadius3;
    SerializedProperty aoeDamage3;

    //Laser Ability
    SerializedProperty chargeUpTime1;
    SerializedProperty laserDuration1;
    SerializedProperty laserFrequency1;
    SerializedProperty autoShootTimer1;

    SerializedProperty chargeUpTime2;
    SerializedProperty laserDuration2;
    SerializedProperty laserFrequency2;
    SerializedProperty autoShootTimer2;

    SerializedProperty chargeUpTime3;
    SerializedProperty laserDuration3;
    SerializedProperty laserFrequency3;
    SerializedProperty autoShootTimer3;

    //Poison Ability
    SerializedProperty projectileHit1;
    SerializedProperty damageFrequency1;
    SerializedProperty frequencyToReapplyGas1;
    SerializedProperty gasDuration1;
    SerializedProperty gasSize1;
    SerializedProperty gasDurationOnPlayer1;

    SerializedProperty projectileHit2;
    SerializedProperty damageFrequency2;
    SerializedProperty frequencyToReapplyGas2;
    SerializedProperty gasDuration2;
    SerializedProperty gasSize2;
    SerializedProperty gasDurationOnPlayer2;

    SerializedProperty projectileHit3;
    SerializedProperty damageFrequency3;
    SerializedProperty frequencyToReapplyGas3;
    SerializedProperty gasDuration3;
    SerializedProperty gasSize3;
    SerializedProperty gasDurationOnPlayer3;

    GameObject Player;
    [MenuItem("Window/Character Editor")]
    public static void OpenWindow()
    {
        GetWindow<CharacterEditor>("Characters");

    }
    private void OnEnable()
    {
        SetSerializedObjectsAndPlayers();
        headings.normal.textColor = Color.yellow;
    }
    private void OnGUI()
    {
        SetSerializedObjectsAndPlayers();

        GUILayout.BeginHorizontal();
        toolbarSel = GUILayout.Toolbar(toolbarSel, toolbarStrings);
        GUILayout.EndHorizontal();

        if (toolbarSel == 0)
        {
            GetEvolutionInformation(rayType, "Ray");
            DisplayEvolutionLayout("Ray");
        }
        else if (toolbarSel == 1)
        {
            GetEvolutionInformation(lionType, "Lion");
            DisplayEvolutionLayout("Lion");

        }
        else if (toolbarSel == 2)
        {
            GetEvolutionInformation(dragonType, "Dragon");
            DisplayEvolutionLayout("Dragon");
        }


    }
    void SetSerializedObjectsAndPlayers()
    {
        if (Player == null)
        {
            Player = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Player.prefab", typeof(GameObject));
            evolutions = Player.GetComponentsInChildren<Evolutions>(true);
            dragonType.Clear();
            rayType.Clear();
            lionType.Clear();

            dragonAbility.Clear();
            rayAbility.Clear();
            lionAbility.Clear();

            UpdateLists();
        }
    }
    private void UpdateLists()
    {
        for (int i = 0; i < evolutions.Length; i++)
        {
            string evoName = evolutions[i].name;

            if (evoName.Contains("Dragon") || evoName.Contains("Green"))
            {
                dragonType.Add(evolutions[i]);
                dragonAbility.Add(evolutions[i].GetComponent<DragonAbility>());
            }
            else if (evoName.Contains("Ray") || evoName.Contains("Blue"))
            {
                rayType.Add(evolutions[i]);
                rayAbility.Add(evolutions[i].GetComponent<LaserAbility>());

            }
            else if (evoName.Contains("Lion") || evoName.Contains("Red") || evoName.Contains("Dracon"))
            {
                lionType.Add(evolutions[i]);
                lionAbility.Add(evolutions[i].GetComponent<AoeExplosionAbility>());
            }
        }
    }
    void GetEvolutionInformation(List<Evolutions> evolutionType, string type)
    {
        for (int i = 0; i < evolutionType.Count; i++)
        {
            if (i == 0)
            {
                evolution0 = new SerializedObject(evolutionType[i]);
                evolution0.Update();
                maxHealthEvo0 = evolution0.FindProperty("MaxHealth");
                expWorthEvo0 = evolution0.FindProperty("ExpWorth");
                healAfterStunnedEvo0 = evolution0.FindProperty("AmountToHealAfterStunned");
            }
            else if (i == 1)
            {
                evolution1 = new SerializedObject(evolutionType[i]);
                evolution1.Update();
                maxHealthEvo1 = evolution1.FindProperty("MaxHealth");
                expWorthEvo1 = evolution1.FindProperty("ExpWorth");
                healAfterStunnedEvo1 = evolution1.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo1 = evolution1.FindProperty("AmountToHealWhenEvolveing");
            }
            else if (i == 2)
            {
                evolution2 = new SerializedObject(evolutionType[i]);
                evolution2.Update();
                maxHealthEvo2 = evolution2.FindProperty("MaxHealth");
                expWorthEvo2 = evolution2.FindProperty("ExpWorth");
                healAfterStunnedEvo2 = evolution2.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo2 = evolution2.FindProperty("AmountToHealWhenEvolveing");
            }
            else if (i == 3)
            {
                evolution3 = new SerializedObject(evolutionType[i]);
                evolution3.Update();
                maxHealthEvo3 = evolution3.FindProperty("MaxHealth");
                expWorthEvo3 = evolution3.FindProperty("ExpWorth");
                healAfterStunnedEvo3 = evolution3.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo3 = evolution3.FindProperty("AmountToHealWhenEvolveing");
            }

            if (type == "Ray")
            {
                GetRayAbilityVariables(i);
            }
            else if (type == "Dragon")
            {
                GetDragonAbilityVariables(i);
            }
            else if (type == "Lion")
            {
                GetLionAbilityVariables(i);
            }

        }
    }

    void DisplayEvolutionLayout(string evolutionType)
    {
        GUILayout.Space(20f);

        EditorGUILayout.BeginHorizontal();


        GUILayout.Label("Evolution Name", EditorStyles.boldLabel, textFields);
        GUILayout.Space(70f);
        GUILayout.Label(evolution0.targetObject.name, EditorStyles.boldLabel, textFields);
        GUILayout.Space(50f);
        GUILayout.Label(evolution1.targetObject.name, EditorStyles.boldLabel, textFields);

        GUILayout.Space(50f);
        GUILayout.Label(evolution2.targetObject.name, EditorStyles.boldLabel, textFields);
        GUILayout.Space(50f);
        GUILayout.Label(evolution3.targetObject.name, EditorStyles.boldLabel, textFields);
        GUILayout.Space(50f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20f);

        GUILayout.Label("Health Variables", headings);
        GUILayout.Space(20f);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(20f);

        GUILayout.Label("Max Health", EditorStyles.boldLabel, textFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(maxHealthEvo0, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(maxHealthEvo1, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(maxHealthEvo2, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(maxHealthEvo3, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal(textFields);

        GUILayout.Space(20f);

        GUILayout.Label("EXP Value", EditorStyles.boldLabel, textFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(expWorthEvo0, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(expWorthEvo1, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(expWorthEvo2, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(expWorthEvo3, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(20f);

        GUILayout.Label("Health Regained Stun", EditorStyles.boldLabel, textFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(healAfterStunnedEvo0, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(healAfterStunnedEvo1, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(healAfterStunnedEvo2, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(healAfterStunnedEvo3, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(20f);

        GUILayout.Label("Health When Evolving", EditorStyles.boldLabel, textFields);
        GUILayout.Space(252f);
        EditorGUILayout.PropertyField(healWhenEvolvingEvo1, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(healWhenEvolvingEvo2, GUIContent.none, propertyFields);
        GUILayout.Space(50f);
        EditorGUILayout.PropertyField(healWhenEvolvingEvo3, GUIContent.none, propertyFields);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(50f);

        if (evolutionType == "Ray")
        {


            GUILayout.Label("Ability Variables", headings, textFields);
            GUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Projectile Speed", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileSpeedEvo0, GUIContent.none, propertyFields);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Damage", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Cooldown", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Charge Up Time", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);        
            EditorGUILayout.PropertyField(chargeUpTime1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(chargeUpTime2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(chargeUpTime3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Laser Duration", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(laserDuration1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(laserDuration2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(laserDuration3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Auto Shoot Time", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(autoShootTimer1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(autoShootTimer2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(autoShootTimer3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Laser Frequency", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(laserFrequency1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(laserFrequency2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(laserFrequency3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

        }
        else if (evolutionType == "Lion")
        {
            GUILayout.Label("Ability Variables", headings, textFields);
            GUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Damage", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Cooldown", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Projectile Speed", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileSpeedEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileSpeedEvo1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileSpeedEvo2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileSpeedEvo3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("AOE Radius", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(aoeRadius1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(aoeRadius2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(aoeRadius3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("AOE Damage", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(aoeDamage1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(aoeDamage2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(aoeDamage3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

        }
        else if (evolutionType == "Dragon")
        {
            GUILayout.Label("Ability Variables", headings, textFields);
            GUILayout.Space(20f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Cooldown", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(cooldownEvo0, GUIContent.none, propertyFields);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Projectile Speed", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileSpeedEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Hit Damage", EditorStyles.boldLabel, textFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageEvo0, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileHit1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileHit2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(projectileHit3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Damage Frequency", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(damageFrequency1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageFrequency2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(damageFrequency3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Reapply Frequency", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(frequencyToReapplyGas1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(frequencyToReapplyGas2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(frequencyToReapplyGas3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Gas Duration", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(gasDuration1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(gasDuration2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(gasDuration3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("Gas Radius", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(gasSize1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(gasSize2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(gasSize3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            GUILayout.Label("gas Duration On Player", EditorStyles.boldLabel, textFields);
            GUILayout.Space(252f);
            EditorGUILayout.PropertyField(gasDurationOnPlayer1, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(gasDurationOnPlayer2, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.PropertyField(gasDurationOnPlayer3, GUIContent.none, propertyFields);
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();

        }
        evolution0.ApplyModifiedProperties();
        evolution1.ApplyModifiedProperties();
        evolution2.ApplyModifiedProperties();
        evolution3.ApplyModifiedProperties();
    }
    void GetLionAbilityVariables(int level)
    {
        if (level == 0)
        {
            abilityEvolution0 = new SerializedObject(lionType[level].GetComponent<AbilityBase>());
            damageEvo0 = abilityEvolution0.FindProperty("damage");
            cooldownEvo0 = abilityEvolution0.FindProperty("shootCooldown");
            projectileSpeedEvo0 = abilityEvolution0.FindProperty("ProjectileSpeed");
        }
        else if (level == 1)
        {
            abilityEvolution1 = new SerializedObject(lionType[level].GetComponent<AoeExplosionAbility>());
            damageEvo1 = abilityEvolution1.FindProperty("damage");
            cooldownEvo1 = abilityEvolution1.FindProperty("shootCooldown");
            projectileSpeedEvo1 = abilityEvolution1.FindProperty("ProjectileSpeed");
            aoeRadius1 = abilityEvolution1.FindProperty("aoeRadius");
            aoeDamage1 = abilityEvolution1.FindProperty("aoeDamage");


        }
        else if (level == 2)
        {
            abilityEvolution2 = new SerializedObject(lionType[level].GetComponent<AoeExplosionAbility>());
            damageEvo2 = abilityEvolution2.FindProperty("damage");
            cooldownEvo2 = abilityEvolution2.FindProperty("shootCooldown");
            projectileSpeedEvo2 = abilityEvolution2.FindProperty("ProjectileSpeed");
            aoeRadius2 = abilityEvolution2.FindProperty("aoeRadius");
            aoeDamage2 = abilityEvolution2.FindProperty("aoeDamage");


        }
        else if (level == 3)
        {
            abilityEvolution3 = new SerializedObject(lionType[level].GetComponent<AoeExplosionAbility>());
            damageEvo3 = abilityEvolution3.FindProperty("damage");
            cooldownEvo3 = abilityEvolution3.FindProperty("shootCooldown");
            projectileSpeedEvo3 = abilityEvolution3.FindProperty("ProjectileSpeed");
            aoeRadius3 = abilityEvolution3.FindProperty("aoeRadius");
            aoeDamage3 = abilityEvolution3.FindProperty("aoeDamage");
        }
    }
    void GetDragonAbilityVariables(int level)
    {
        if (level == 0)
        {
            abilityEvolution0 = new SerializedObject(dragonType[level].GetComponent<AbilityBase>());
            damageEvo0 = abilityEvolution0.FindProperty("damage");
            cooldownEvo0 = abilityEvolution0.FindProperty("shootCooldown");
            projectileSpeedEvo0 = abilityEvolution0.FindProperty("ProjectileSpeed");
        }
        else if (level == 1)
        {
            abilityEvolution1 = new SerializedObject(dragonType[level].GetComponent<DragonAbility>());
            projectileHit1 = abilityEvolution1.FindProperty("projectileHitDmg");
            damageFrequency1 = abilityEvolution1.FindProperty("damageFrequency");
            frequencyToReapplyGas1 = abilityEvolution1.FindProperty("frequencyToReapplyGas");
            gasDuration1 = abilityEvolution1.FindProperty("gasDuration");
            gasSize1 = abilityEvolution1.FindProperty("gasSize");
            gasDurationOnPlayer1 = abilityEvolution1.FindProperty("gasDurationOnPlayer");
        }
        else if (level == 2)
        {
            abilityEvolution2 = new SerializedObject(dragonType[level].GetComponent<DragonAbility>());
            projectileHit2 = abilityEvolution2.FindProperty("projectileHitDmg");
            damageFrequency2 = abilityEvolution2.FindProperty("damageFrequency");
            frequencyToReapplyGas2 = abilityEvolution2.FindProperty("frequencyToReapplyGas");
            gasDuration2 = abilityEvolution2.FindProperty("gasDuration");
            gasSize2 = abilityEvolution2.FindProperty("gasSize");
            gasDurationOnPlayer2 = abilityEvolution2.FindProperty("gasDurationOnPlayer");
        }
        else if (level == 3)
        {
            abilityEvolution3 = new SerializedObject(dragonType[level].GetComponent<DragonAbility>());
            projectileHit3 = abilityEvolution3.FindProperty("projectileHitDmg");
            damageFrequency3 = abilityEvolution3.FindProperty("damageFrequency");
            frequencyToReapplyGas3 = abilityEvolution3.FindProperty("frequencyToReapplyGas");
            gasDuration3 = abilityEvolution3.FindProperty("gasDuration");
            gasSize3 = abilityEvolution3.FindProperty("gasSize");
            gasDurationOnPlayer3 = abilityEvolution3.FindProperty("gasDurationOnPlayer");
        }
    }
    void GetRayAbilityVariables(int level)
    {
        if (level == 0)
        {
            abilityEvolution0 = new SerializedObject(rayType[level].GetComponent<AbilityBase>());
            damageEvo0 = abilityEvolution0.FindProperty("damage");
            cooldownEvo0 = abilityEvolution0.FindProperty("shootCooldown");
            projectileSpeedEvo0 = abilityEvolution0.FindProperty("ProjectileSpeed");
        }
        else if (level == 1)
        {
            abilityEvolution1 = new SerializedObject(rayType[level].GetComponent<LaserAbility>());
            damageEvo1 = abilityEvolution1.FindProperty("damage");
            cooldownEvo1 = abilityEvolution1.FindProperty("shootCooldown");
            chargeUpTime1 = abilityEvolution1.FindProperty("ChargeupTime");
            laserDuration1 = abilityEvolution1.FindProperty("baseLaserDuration");
            laserFrequency1 = abilityEvolution1.FindProperty("damageFrequency");
            autoShootTimer1 = abilityEvolution1.FindProperty("ShootAutomaticallyAt");
        }
        else if (level == 2)
        {
            abilityEvolution2 = new SerializedObject(rayType[level].GetComponent<LaserAbility>());
            damageEvo2 = abilityEvolution2.FindProperty("damage");
            cooldownEvo2 = abilityEvolution2.FindProperty("shootCooldown");
            chargeUpTime2 = abilityEvolution1.FindProperty("ChargeupTime");
            laserDuration2 = abilityEvolution1.FindProperty("baseLaserDuration");
            laserFrequency2 = abilityEvolution1.FindProperty("damageFrequency");
            autoShootTimer2 = abilityEvolution1.FindProperty("ShootAutomaticallyAt");
        }
        else if (level == 3)
        {
            abilityEvolution3 = new SerializedObject(rayType[level].GetComponent<LaserAbility>());
            damageEvo3 = abilityEvolution3.FindProperty("damage");
            cooldownEvo3 = abilityEvolution3.FindProperty("shootCooldown");
            chargeUpTime3 = abilityEvolution1.FindProperty("ChargeupTime");
            laserDuration3 = abilityEvolution1.FindProperty("baseLaserDuration");
            laserFrequency3 = abilityEvolution1.FindProperty("damageFrequency");
            autoShootTimer3 = abilityEvolution1.FindProperty("ShootAutomaticallyAt");
        }
    }
}
