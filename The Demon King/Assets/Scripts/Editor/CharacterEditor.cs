
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterEditor : EditorWindow
{
    string[] toolbarStrings = {"Ray Demon", "Lion Demon", "Dragon Demon" };

    List<Evolutions> dragonType = new List<Evolutions>();
    List<Evolutions> lionType = new List<Evolutions>();
    List<Evolutions> rayType = new List<Evolutions>();

    List<AbilityBase> dragonAbility = new List<AbilityBase>();
    List<AbilityBase> lionAbility = new List<AbilityBase>();
    List<AbilityBase> rayAbility = new List<AbilityBase>();

    string slime;
    string child;
    string adult;
    string king;

    int toolbarSel = 0;
    float maxHealth = 2;
    GameObject Player;
    [MenuItem("Window/Character Editor")]
    public static void OpenWindow()
    {
        GetWindow<CharacterEditor>("Characters");
    }
    private void OnGUI()
    {
        dragonType.Clear();
        rayType.Clear();
        lionType.Clear();

        //dragonAbility.Clear();
        //rayAbility.Clear();
        //lionAbility.Clear();

        Player = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Player.prefab", typeof(GameObject));
        Evolutions[] evolutions = Player.GetComponentsInChildren<Evolutions>(true);
        AbilityBase[] ability = Player.GetComponentsInChildren<AbilityBase>(true);

        for (int i = 0; i < evolutions.Length; i++)
        {
            string evoName = evolutions[i].name;

            if (evoName.Contains("Dragon") || evoName.Contains("Green"))
            {
                dragonType.Add(evolutions[i]);
                //dragonAbility.Add(ability[i]);
            }

            if (evoName.Contains("Ray") || evoName.Contains("Blue"))
            {
                rayType.Add(evolutions[i]);
                //rayAbility.Add(ability[i]);

            }

            if (evoName.Contains("Lion") || evoName.Contains("Red") || evoName.Contains("Dracon"))
            {
                lionType.Add(evolutions[i]);
                //lionAbility.Add(ability[i]);
            }
        }

        GUILayout.BeginHorizontal();
        toolbarSel = GUILayout.Toolbar(toolbarSel, toolbarStrings);
        GUILayout.EndHorizontal();

        if (toolbarSel == 0)
        {
            GUILayout.Label(rayType[0].gameObject.name);


            GUILayout.Label(rayType[1].gameObject.name);
            GUILayout.Label(rayType[2].gameObject.name);
            GUILayout.Label(rayType[3].gameObject.name);
        }

        if (toolbarSel == 1)
        {
            GUILayout.Label(lionType[0].gameObject.name);
            GUILayout.Label(lionType[1].gameObject.name);
            GUILayout.Label(lionType[2].gameObject.name);
            GUILayout.Label(lionType[3].gameObject.name);
        }

        if (toolbarSel == 2)
        {
            GUILayout.Label(dragonType[0].gameObject.name);
            GUILayout.Label(dragonType[1].gameObject.name);
            GUILayout.Label(dragonType[2].gameObject.name);
            GUILayout.Label(dragonType[3].gameObject.name);
        }

        if (evolutions.Length != 0)
        {
            maxHealth = EditorGUILayout.FloatField(evolutions[toolbarSel].MaxHealth);
        }
    }

}
