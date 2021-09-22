
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
    SerializedProperty chargeUpTime;
    SerializedProperty laserDuration;
    SerializedProperty laserFrequency;

    //Poison Ability
    SerializedProperty damageOverTime;
    SerializedProperty damageFrequency;
    SerializedProperty frequencyToReapplyGas;
    SerializedProperty gasDuration;
    SerializedProperty gasSize;
    SerializedProperty gasDurationOnPlayer;

    GameObject Player;
    [MenuItem("Window/Character Editor")]
    public static void OpenWindow()
    {
        GetWindow<CharacterEditor>("Characters");
    }
    private void OnGUI()
    {
        Player = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Player.prefab", typeof(GameObject));
        evolutions = Player.GetComponentsInChildren<Evolutions>(true);

        GUILayout.BeginHorizontal();
        toolbarSel = GUILayout.Toolbar(toolbarSel, toolbarStrings);
        GUILayout.EndHorizontal();

        if (toolbarSel == 0)
        {
            DisplayType(rayType, "Ray");
        }
        else if (toolbarSel == 1)
        {
            DisplayType(lionType, "Lion");

        }
        else if (toolbarSel == 2)
        {
            DisplayType(dragonType,"Dragon");

        }
    }
    private void OnValidate()
    {
        
    }

    private void OnHierarchyChange()
    {
        dragonType.Clear();
        rayType.Clear();
        lionType.Clear();

        dragonAbility.Clear();
        rayAbility.Clear();
        lionAbility.Clear();

        UpdateLists();
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
    void DisplayType(List<Evolutions> evolutionType, string type)
    {
        for (int i = 0; i < evolutionType.Count; i++)
        {
            if (i == 0)
            {
                evolution0 = new SerializedObject(evolutionType[i]);
                maxHealthEvo0 = evolution0.FindProperty("MaxHealth");
                expWorthEvo0 = evolution0.FindProperty("ExpWorth");
                healAfterStunnedEvo0 = evolution0.FindProperty("AmountToHealAfterStunned");

                GUILayout.Label(evolution0.targetObject.name, EditorStyles.boldLabel);
                GUILayout.Label("Health Variables", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(maxHealthEvo0);
                EditorGUILayout.PropertyField(expWorthEvo0);
                EditorGUILayout.PropertyField(healAfterStunnedEvo0);
            }
            else if (i == 1)
            {
                evolution1 = new SerializedObject(evolutionType[i]);
                maxHealthEvo1 = evolution1.FindProperty("MaxHealth");
                expWorthEvo1 = evolution1.FindProperty("ExpWorth");
                healAfterStunnedEvo1 = evolution1.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo1 = evolution1.FindProperty("AmountToHealWhenEvolveing");

                GUILayout.Label(evolution1.targetObject.name, EditorStyles.boldLabel);
                GUILayout.Label("Health Variables", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(maxHealthEvo1);
                EditorGUILayout.PropertyField(expWorthEvo1);
                EditorGUILayout.PropertyField(healAfterStunnedEvo1);
                EditorGUILayout.PropertyField(healWhenEvolvingEvo1);
            }
            else if (i == 2)
            {
                evolution2 = new SerializedObject(evolutionType[i]);
                maxHealthEvo2 = evolution2.FindProperty("MaxHealth");
                expWorthEvo2 = evolution2.FindProperty("ExpWorth");
                healAfterStunnedEvo2 = evolution2.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo2 = evolution2.FindProperty("AmountToHealWhenEvolveing");

                GUILayout.Label(evolution2.targetObject.name, EditorStyles.boldLabel);
                GUILayout.Label("Health Variables", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(maxHealthEvo2);
                EditorGUILayout.PropertyField(expWorthEvo2);
                EditorGUILayout.PropertyField(healAfterStunnedEvo2);
                EditorGUILayout.PropertyField(healWhenEvolvingEvo2);
            }
            else if (i == 3)
            {
                evolution3 = new SerializedObject(evolutionType[i]);           
                maxHealthEvo3 = evolution3.FindProperty("MaxHealth");
                expWorthEvo3 = evolution3.FindProperty("ExpWorth");
                healAfterStunnedEvo3 = evolution3.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo3 = evolution3.FindProperty("AmountToHealWhenEvolveing");

                GUILayout.Label(evolution3.targetObject.name);
                GUILayout.Label("Health Variables", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(maxHealthEvo3);
                EditorGUILayout.PropertyField(expWorthEvo3);
                EditorGUILayout.PropertyField(healAfterStunnedEvo3);
                EditorGUILayout.PropertyField(healWhenEvolvingEvo3);
            }


            if (type == "Ray")
            {
                //DisplayRayAbilityVariables(i);
            }
            else if(type == "Dragon")
            {
                //DisplayDragonAbilityVariables(i);
            }
            else if(type == "Lion")
            {
                DisplayLionAbilityVariables(i);
            }
        } 
    }
    void DisplayLionAbilityVariables(int level)
    {
        if (level == 0 )
        {
            abilityEvolution0 = new SerializedObject(lionType[level].GetComponent<AbilityBase>());
            damageEvo0 = abilityEvolution0.FindProperty("damage");
            cooldownEvo0 = abilityEvolution0.FindProperty("shootCooldown");
            projectileSpeedEvo0 = abilityEvolution0.FindProperty("ProjectileSpeed");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo0);
            EditorGUILayout.PropertyField(cooldownEvo0);
            EditorGUILayout.PropertyField(projectileSpeedEvo0);
        }
        else if(level == 1)
        {
            abilityEvolution1 = new SerializedObject(lionType[level].GetComponent<AoeExplosionAbility>());
            damageEvo1 = abilityEvolution1.FindProperty("damage");
            cooldownEvo1 = abilityEvolution1.FindProperty("shootCooldown");
            projectileSpeedEvo1 = abilityEvolution1.FindProperty("ProjectileSpeed");
            aoeRadius1 = abilityEvolution1.FindProperty("aoeRadius");
            aoeDamage1 = abilityEvolution1.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo1);
            EditorGUILayout.PropertyField(cooldownEvo1);
            EditorGUILayout.PropertyField(projectileSpeedEvo1);
            EditorGUILayout.PropertyField(aoeRadius1);
            EditorGUILayout.PropertyField(aoeDamage1);
        }
        else if (level == 2)
        {
            abilityEvolution2 = new SerializedObject(lionType[level].GetComponent<AoeExplosionAbility>());
            damageEvo2 = abilityEvolution2.FindProperty("damage");
            cooldownEvo2 = abilityEvolution2.FindProperty("shootCooldown");
            projectileSpeedEvo2 = abilityEvolution2.FindProperty("ProjectileSpeed");
            aoeRadius2 = abilityEvolution2.FindProperty("aoeRadius");
            aoeDamage2 = abilityEvolution2.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo2);
            EditorGUILayout.PropertyField(cooldownEvo2);
            EditorGUILayout.PropertyField(projectileSpeedEvo2);
            EditorGUILayout.PropertyField(aoeRadius2);
            EditorGUILayout.PropertyField(aoeDamage2);
        }
        else if(level == 3)
        {
            abilityEvolution3 = new SerializedObject(lionType[level].GetComponent<AoeExplosionAbility>());
            damageEvo3 = abilityEvolution3.FindProperty("damage");
            cooldownEvo3 = abilityEvolution3.FindProperty("shootCooldown");
            projectileSpeedEvo3 = abilityEvolution3.FindProperty("ProjectileSpeed");
            aoeRadius3 = abilityEvolution3.FindProperty("aoeRadius");
            aoeDamage3 = abilityEvolution3.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo3);
            EditorGUILayout.PropertyField(cooldownEvo3);
            EditorGUILayout.PropertyField(projectileSpeedEvo3);
            EditorGUILayout.PropertyField(aoeRadius3);
            EditorGUILayout.PropertyField(aoeDamage3);
        }
    }
    void DisplayDragonAbilityVariables(int level)
    {
        GUILayout.Label("Dragon Abilities Variables");
        if (level == 0)
        {
            abilityEvolution0 = new SerializedObject(dragonType[level].GetComponent<AbilityBase>());
            damageEvo0 = abilityEvolution0.FindProperty("damage");
            cooldownEvo0 = abilityEvolution0.FindProperty("shootCooldown");
            projectileSpeedEvo0 = abilityEvolution0.FindProperty("ProjectileSpeed");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo0);
            EditorGUILayout.PropertyField(cooldownEvo0);
            EditorGUILayout.PropertyField(projectileSpeedEvo0);
        }
        else if (level == 1)
        {
            abilityEvolution1 = new SerializedObject(dragonType[level].GetComponent<DragonGasEffect>());
            damageEvo1 = abilityEvolution1.FindProperty("damage");
            cooldownEvo1 = abilityEvolution1.FindProperty("shootCooldown");
            projectileSpeedEvo1 = abilityEvolution1.FindProperty("ProjectileSpeed");
            aoeRadius1 = abilityEvolution1.FindProperty("aoeRadius");
            aoeDamage1 = abilityEvolution1.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo1);
            EditorGUILayout.PropertyField(cooldownEvo1);
            EditorGUILayout.PropertyField(projectileSpeedEvo1);
            EditorGUILayout.PropertyField(aoeRadius1);
            EditorGUILayout.PropertyField(aoeDamage1);
        }
        else if (level == 2)
        {
            abilityEvolution2 = new SerializedObject(dragonType[level].GetComponent<DragonGasEffect>());
            damageEvo2 = abilityEvolution2.FindProperty("damage");
            cooldownEvo2 = abilityEvolution2.FindProperty("shootCooldown");
            projectileSpeedEvo2 = abilityEvolution2.FindProperty("ProjectileSpeed");
            aoeRadius2 = abilityEvolution2.FindProperty("aoeRadius");
            aoeDamage2 = abilityEvolution2.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo2);
            EditorGUILayout.PropertyField(cooldownEvo2);
            EditorGUILayout.PropertyField(projectileSpeedEvo2);
            EditorGUILayout.PropertyField(aoeRadius2);
            EditorGUILayout.PropertyField(aoeDamage2);
        }
        else if (level == 3)
        {
            abilityEvolution3 = new SerializedObject(dragonType[level].GetComponent<DragonGasEffect>());
            damageEvo3 = abilityEvolution3.FindProperty("damage");
            cooldownEvo3 = abilityEvolution3.FindProperty("shootCooldown");
            projectileSpeedEvo3 = abilityEvolution3.FindProperty("ProjectileSpeed");
            aoeRadius3 = abilityEvolution3.FindProperty("aoeRadius");
            aoeDamage3 = abilityEvolution3.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo3);
            EditorGUILayout.PropertyField(cooldownEvo3);
            EditorGUILayout.PropertyField(projectileSpeedEvo3);
            EditorGUILayout.PropertyField(aoeRadius3);
            EditorGUILayout.PropertyField(aoeDamage3);
        }
    }
    void DisplayRayAbilityVariables(int level)
    {
        GUILayout.Label("Ray Abilities Variables");
        if (level == 0)
        {
            abilityEvolution0 = new SerializedObject(rayType[level].GetComponent<AbilityBase>());
            damageEvo0 = abilityEvolution0.FindProperty("damage");
            cooldownEvo0 = abilityEvolution0.FindProperty("shootCooldown");
            projectileSpeedEvo0 = abilityEvolution0.FindProperty("ProjectileSpeed");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo0);
            EditorGUILayout.PropertyField(cooldownEvo0);
            EditorGUILayout.PropertyField(projectileSpeedEvo0);
        }
        else if (level == 1)
        {
            abilityEvolution1 = new SerializedObject(rayType[level].GetComponent<LaserAbility>());
            damageEvo1 = abilityEvolution1.FindProperty("damage");
            cooldownEvo1 = abilityEvolution1.FindProperty("shootCooldown");
            projectileSpeedEvo1 = abilityEvolution1.FindProperty("ProjectileSpeed");
            aoeRadius1 = abilityEvolution1.FindProperty("aoeRadius");
            aoeDamage1 = abilityEvolution1.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo1);
            EditorGUILayout.PropertyField(cooldownEvo1);
            EditorGUILayout.PropertyField(projectileSpeedEvo1);
            EditorGUILayout.PropertyField(aoeRadius1);
            EditorGUILayout.PropertyField(aoeDamage1);
        }
        else if (level == 2)
        {
            abilityEvolution2 = new SerializedObject(rayType[level].GetComponent<LaserAbility>());
            damageEvo2 = abilityEvolution2.FindProperty("damage");
            cooldownEvo2 = abilityEvolution2.FindProperty("shootCooldown");
            projectileSpeedEvo2 = abilityEvolution2.FindProperty("ProjectileSpeed");
            aoeRadius2 = abilityEvolution2.FindProperty("aoeRadius");
            aoeDamage2 = abilityEvolution2.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo2);
            EditorGUILayout.PropertyField(cooldownEvo2);
            EditorGUILayout.PropertyField(projectileSpeedEvo2);
            EditorGUILayout.PropertyField(aoeRadius2);
            EditorGUILayout.PropertyField(aoeDamage2);
        }
        else if (level == 3)
        {
            abilityEvolution3 = new SerializedObject(rayType[level].GetComponent<LaserAbility>());
            damageEvo3 = abilityEvolution3.FindProperty("damage");
            cooldownEvo3 = abilityEvolution3.FindProperty("shootCooldown");
            projectileSpeedEvo3 = abilityEvolution3.FindProperty("ProjectileSpeed");
            aoeRadius3 = abilityEvolution3.FindProperty("aoeRadius");
            aoeDamage3 = abilityEvolution3.FindProperty("aoeDamage");

            GUILayout.Label("Ability Variables", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(damageEvo3);
            EditorGUILayout.PropertyField(cooldownEvo3);
            EditorGUILayout.PropertyField(projectileSpeedEvo3);
            EditorGUILayout.PropertyField(aoeRadius3);
            EditorGUILayout.PropertyField(aoeDamage3);
        }
    }
}
