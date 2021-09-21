
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
    
    SerializedObject evolution0;
    SerializedProperty maxHealthEvo0;
    SerializedProperty expWorthEvo0;
    SerializedProperty healAfterStunnedEvo0;
    SerializedProperty healWhenEvolvingEvo0;

    SerializedObject evolution1;
    SerializedProperty maxHealthEvo1;
    SerializedProperty expWorthEvo1;
    SerializedProperty healAfterStunnedEvo1;
    SerializedProperty healWhenEvolvingEvo1;

    SerializedObject evolution2;
    SerializedProperty maxHealthEvo2;
    SerializedProperty expWorthEvo2;
    SerializedProperty healAfterStunnedEvo2;
    SerializedProperty healWhenEvolvingEvo2;

    SerializedObject evolution3;
    SerializedProperty maxHealthEvo3;
    SerializedProperty expWorthEvo3;
    SerializedProperty healAfterStunnedEvo3;
    SerializedProperty healWhenEvolvingEvo3;


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
            DisplayType(rayType);
        
        if (toolbarSel == 1)        
            DisplayType(lionType);
        
        if (toolbarSel == 2)       
            DisplayType(dragonType);
        
        GUILayout.BeginHorizontal();

       
        GUILayout.EndHorizontal();

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
    void DisplayType(List<Evolutions> evolutionType)
    {
        for (int i = 0; i < evolutionType.Count; i++)
        {
            if (i == 0)
            {
                evolution0 = new SerializedObject(evolutionType[i]);
                
                maxHealthEvo0 = evolution0.FindProperty("MaxHealth");
                expWorthEvo0 = evolution0.FindProperty("ExpWorth");
                healAfterStunnedEvo0 = evolution0.FindProperty("AmountToHealAfterStunned");
                GUILayout.Label(evolution0.targetObject.name);

                EditorGUILayout.PropertyField(maxHealthEvo0);
                EditorGUILayout.PropertyField(expWorthEvo0);
                EditorGUILayout.PropertyField(healAfterStunnedEvo0);

            }
            if (i == 1)
            {
                evolution1 = new SerializedObject(evolutionType[i]);
                maxHealthEvo1 = evolution1.FindProperty("MaxHealth");
                expWorthEvo1 = evolution1.FindProperty("ExpWorth");
                healAfterStunnedEvo1 = evolution1.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo1 = evolution1.FindProperty("AmountToHealWhenEvolveing");
                GUILayout.Label(evolution1.targetObject.name);
                EditorGUILayout.PropertyField(maxHealthEvo1);
                EditorGUILayout.PropertyField(expWorthEvo1);
                EditorGUILayout.PropertyField(healAfterStunnedEvo1);
                EditorGUILayout.PropertyField(healWhenEvolvingEvo1);
            }
            if (i == 2)
            {
                evolution2 = new SerializedObject(evolutionType[i]);
                maxHealthEvo2 = evolution2.FindProperty("MaxHealth");
                expWorthEvo2 = evolution2.FindProperty("ExpWorth");
                healAfterStunnedEvo2 = evolution2.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo2 = evolution2.FindProperty("AmountToHealWhenEvolveing");
                GUILayout.Label(evolution2.targetObject.name);
                EditorGUILayout.PropertyField(maxHealthEvo2);
                EditorGUILayout.PropertyField(expWorthEvo2);
                EditorGUILayout.PropertyField(healAfterStunnedEvo2);
                EditorGUILayout.PropertyField(healWhenEvolvingEvo2);
            }
            if (i == 3)
            {
                evolution3 = new SerializedObject(evolutionType[i]);
                maxHealthEvo3 = evolution3.FindProperty("MaxHealth");
                expWorthEvo3 = evolution3.FindProperty("ExpWorth");
                healAfterStunnedEvo3 = evolution3.FindProperty("AmountToHealAfterStunned");
                healWhenEvolvingEvo3 = evolution3.FindProperty("AmountToHealWhenEvolveing");

                GUILayout.Label(evolution3.targetObject.name);
                EditorGUILayout.PropertyField(maxHealthEvo3);
                EditorGUILayout.PropertyField(expWorthEvo3);
                EditorGUILayout.PropertyField(healAfterStunnedEvo3);
                EditorGUILayout.PropertyField(healWhenEvolvingEvo3);
            }
        } 
    }
    void DisplayHealthVariables(Evolutions evoStats)
    {
        GUILayout.Label("Health Variables");

    }
    void DisplayLionAbilityVariables(AoeExplosionAbility lionAbility)
    {
        GUILayout.Label("Lion Abilities Variables");

    }
    void DisplayDragonAbilityVariables(DragonAbility dragonAbility)
    {
        GUILayout.Label("Dragon Abilities Variables");

    }
    void DisplayRayAbilityVariables(LaserAbility rayAbility)
    {
        GUILayout.Label("Ray Abilities Variables");

    }
}
