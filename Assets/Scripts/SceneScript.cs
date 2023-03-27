using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneScript : MonoBehaviour
{
    [System.Serializable]
    public class PlanetStats
    {
        public string Name;
        public DVec3 PhysicPosition;
        public DVec3 Velocity;
        public DVec3 Acceleration;
        public float Mass;
        public float radius;
        public string Prefab;

        public void ToPlanetData(ref PlanetData planet)
        {
            planet.PlanetName = Name;
            planet.PhysicPosition = PhysicPosition;
            planet.transform.position = planet.PhysicPosition.AsVector();
            planet.Velocity = Velocity;
            planet.Acceleration = Acceleration;
            planet.Mass = Mass;
            planet.radius = radius;
            planet.Placed = true;
            planet.Prefab = Prefab;
        }

        public void FromPlanetData(PlanetData planet)
        {
            Name = planet.PlanetName;
            PhysicPosition = planet.PhysicPosition;
            Velocity = planet.Velocity;
            Acceleration = planet.Acceleration;
            Mass = planet.Mass;
            radius = planet.radius;
            Prefab = planet.Prefab;
        }
    }
    [System.Serializable]
    public class PlanetStatsManager
    {
        public List<PlanetStats> Planets = new List<PlanetStats>();

        public void ToPlanetDataManager(ref PlanetDataManager manager)
        {
            var list = manager.GetAllPlanets();
            for (int i = 0; i < Planets.Count; i++)
            {
                var gameObject = Instantiate((GameObject)Resources.Load("Prefabs/" + Planets[i].Prefab, typeof(GameObject)));
                gameObject.transform.parent = manager.transform;
                var newPlanet = gameObject.GetComponent(typeof(PlanetData)) as PlanetData;

                Planets[i].ToPlanetData(ref newPlanet);
                list.Add(newPlanet);
            }
        }

        public void FromPlanetDataManager(PlanetDataManager manager)
        {
            var list = manager.GetAllPlanets();
            for (int i = 0; i < list.Count; i++)
            {
                PlanetStats newPlanet = new PlanetStats();
                newPlanet.FromPlanetData(list[i]);
                Planets.Add(newPlanet);
            }
        }
    }
    [SerializeField] TMPro.TMP_InputField field;
    [SerializeField] SceneButton ExampleButton;
    [SerializeField] List<SceneButton> Buttons = new List<SceneButton>();
    [SerializeField] GameObject ScrollView;
    string path = Application.streamingAssetsPath + "/Resources/Scenes/";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonListClicked()
    {
        if (ScrollView.activeSelf)
        {
            HideScrollView();
        }
        else
        {
            ShowScrollView();
        }
    }

    public void OnEnterInputLoad()
    {
        var inspector = FindObjectOfType<Inspector>();
        inspector.EnterInputField();
    }

    public void ShowScrollView()
    {
        ScrollView.SetActive(true);

        // Get all files in the specified folder path with the ".json" extension
        string[] jsonFiles = Directory.GetFiles(path, "*.json");

        // Output the number of found JSON files
        Debug.Log("Found " + jsonFiles.Length + " JSON file(s) in " + path);

        // Output the file paths of all found JSON files
        foreach (string file in jsonFiles)
        {
            var button = Instantiate(ExampleButton, ExampleButton.transform.parent);
            button.name = file.Substring(file.LastIndexOf('/') + 1);
            button.name = button.name.Substring(0, button.name.LastIndexOf('.'));
            button.gameObject.SetActive(true);
            var textMeshPro = button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            if (textMeshPro)
                textMeshPro.text = button.name;
            Buttons.Add(button);
        }
        ExampleButton.gameObject.SetActive(false);
    }

    public void HideScrollView()
    {
        ScrollView.SetActive(false);
        foreach (var button in Buttons)
        {
            Destroy(button.gameObject);
        }
        Buttons.Clear();
    }

    public void OnExitInputLoad()
    {
        var inspector = FindObjectOfType<Inspector>();
        inspector.ExitInputField();
    }

    public void SaveClick()
    {
        SaveScene(field.text);
    }

    public void LoadClick()
    {
        LoadScene(field.text);
    }

    public void SaveScene(string filename)
    {
        if (field.text.Length == 0) return;
        // Serialize the object into JSON and save string.
        string jsonString = "";
        var manager = FindObjectOfType<PlanetDataManager>();
        var planetmanager = new PlanetStatsManager();
        planetmanager.FromPlanetDataManager(manager);
        jsonString = JsonUtility.ToJson(planetmanager);

        // Write JSON to file.
        File.WriteAllText(path + filename + ".json", jsonString);
    }

    public void LoadScene(string filename)
    {
        if (File.Exists(path + filename + ".json"))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(path + filename + ".json");

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            var stats = JsonUtility.FromJson<PlanetStatsManager>(fileContents);

            var manager = FindObjectOfType<PlanetDataManager>();

            foreach (var planet in manager.GetAllPlanets())
            {
                Destroy(planet.gameObject);
            }

            stats.ToPlanetDataManager(ref manager);
            print(manager);
        }
    }
}
