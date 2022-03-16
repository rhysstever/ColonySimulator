using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum TileType
{
    Plains,
    Forest,
    Mountains,
    Ocean
}

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator instance = null;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    [SerializeField] // Map parent
    private GameObject tilesParent, improvementParent;

    [SerializeField] // Prefabs
    private GameObject tilePrefab, forestPrefab, mountainsPrefab;

    [SerializeField] // Resource tile materials
    private Material plainsMat, forestMat, mountainsMat, oceanMat;
    
    public Dictionary<TileType, Material> tileMaterials;

    // World Settings
    public int size;
    private float oceansPercent, mountainsPercent, forestPercent;
    private float oceansNoiseMod, mountainsNoiseMod, forestNoiseMod;

    public List<int> savedMapsNums;
    private string mapFilePath;

    // Start is called before the first frame update
    void Start()
    {
        oceansPercent = 0.25f;
        mountainsPercent = 0.2f;
        forestPercent = 0.35f;

        oceansNoiseMod = 0.15f;
        mountainsNoiseMod = 0.25f;
        forestNoiseMod = 0.35f;

        tileMaterials = new Dictionary<TileType, Material>();
        PopulateTileMatDict();

        mapFilePath = "Assets/Resources/Maps";
        savedMapsNums = GetSavedMapNums();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Create a random world with randomized parameters (size and tile type %s)
    /// </summary>
    public void CreateNewRandomMap()
	{
        // Randomize the size and ocean and grassland percentages
        size = Random.Range(25, 50);

        // Set tile percentages & noise modifiers
        oceansPercent = 0.25f;
        mountainsPercent = 0.2f;
        forestPercent = 0.35f;

        oceansNoiseMod = 0.15f;
        mountainsNoiseMod = 0.25f;
        forestNoiseMod = 0.35f;

        ClearMap();

        // Loop to create a random map based on the given percentages
        for(int y = 0; y < size; y++)
		{
            for(int x = 0; x < size; x++)
            {
                TileType tileType = GetRandomTileType(x, y);
                CreateWorldTile(tilesParent, new Vector2(x, y), tileType);
			}
		}

        // Center the camera and change the gameState
        CenterCamera();
        GameManager.instance.ChangeGameState(GameState.game);
    }

    /// <summary>
    /// Correlates a tile type to a material
    /// (To be used when a tile is created)
    /// </summary>
    private void PopulateTileMatDict()
    {
        tileMaterials.Add(TileType.Plains, plainsMat);
        tileMaterials.Add(TileType.Forest, forestMat);
        tileMaterials.Add(TileType.Mountains, mountainsMat);
        tileMaterials.Add(TileType.Ocean, oceanMat);
    }

    /// <summary>
    /// Loads a world from the "Maps" folder
    /// </summary>
    /// <param name="mapNum">The index of the map trying to be loaded</param>
    public void LoadWorld(int mapNum)
    {
        // Create the file path string
        string filePath = mapFilePath + "/mapSavedAt" + mapNum + ".txt";

        // End early if the file does not exist
        if(!File.Exists(filePath))
            return;

        // Clear the current map
        ClearMap();

        // Create the StreamReader and read the file line by line
        StreamReader reader = new StreamReader(filePath);
        int lineNum = -1;

        // Load current amount of resources
        string resLine = reader.ReadLine();
        string[] resourceAmounts = resLine.Split(' ');
        for(int i = 0; i < resourceAmounts.Length - 1; i++)
            GameManager.instance.resources[(ResourceType)i].AddAmount(int.Parse(resourceAmounts[i]));

        while(!reader.EndOfStream)
        {
            // Read the next line
            lineNum++;
            string line = reader.ReadLine();
            string[] arrOfSingleLine = line.Split(' ');
            // Loop through the line (technically looping through each character of the one word)
            for(int i = 0; i < arrOfSingleLine.Length; i++)
            {
                GameObject newTile;
                // Create the tile
                switch(arrOfSingleLine[i][0])
                {
                    case 'O':
                        newTile = CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Ocean);
                        break;
                    case 'F':
                        newTile = CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Forest);
                        break;
                    case 'M':
                        newTile = CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Mountains);
                        break;
                    case 'P':
                    default:
                        newTile = CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Plains);
                        break;
                }

                // If there is no improvement on the tile, continue to next tile
                if(arrOfSingleLine[i].Length == 1)
                    continue;

                // Create the improvement
                switch(arrOfSingleLine[i][1])
                {
                    case 'H':
                        ImprovementManager.instance.BuildImprovement(ImprovementType.House, newTile, true);
                        break;
                    case 'F':
                        ImprovementManager.instance.BuildImprovement(ImprovementType.Farm, newTile, true);
                        break;
                    case 'M':
                        ImprovementManager.instance.BuildImprovement(ImprovementType.Mine, newTile, true);
                        break;
                    default:
                        Debug.Log("Improvement on " + new Vector2(i, lineNum));
                        break;
                }
			}
        }

        reader.Close();

        // Update world info
        size = lineNum + 1;
        forestPercent = -1;
        mountainsPercent = -1;
        oceansPercent = -1;

        // Recenter the camera and change the gameState
        CenterCamera();
        GameManager.instance.ChangeGameState(GameState.game);
    }

    /// <summary>
    /// Saves the current world as a text file in the "Maps" folder
    /// </summary>
    public void SaveWorld()
	{
        if(tilesParent.transform.childCount == 0)
		{
            Debug.Log("No map to save!");
            return;
		}

        // Create the new save file
        string mapName = "SavedAt" + (int)Time.time;
        string filePath = mapFilePath + "/map" + mapName + ".txt";
        StreamWriter writer = File.CreateText(filePath);

        // Log resource stats
        foreach(ResourceType resourceType in GameManager.instance.resources.Keys)
            writer.Write(GameManager.instance.resources[resourceType].CurrentAmount + " ");
        writer.WriteLine();
        
        // Loop through each tile and write a character based on the tile type
        foreach(Transform childTrans in tilesParent.transform)
		{
            // Get the correct letter based on the tile's type
            string tileStr = "P";
            switch(childTrans.GetComponent<Tile>().tileType)
            {
                case TileType.Forest:
                    tileStr = "F";
                    break;
                case TileType.Mountains:
                    tileStr = "M";
                    break;
                case TileType.Ocean:
                    tileStr = "O";
                    break;
                case TileType.Plains:
                default:
                    break;
            }

            if(childTrans.GetComponent<Tile>().improvement != null)
			{
                switch(childTrans.GetComponent<Tile>().improvement.GetComponent<Improvement>().type)
                {
                    case ImprovementType.House:
                        tileStr += "H";
                        break;
                    case ImprovementType.Farm:
                        tileStr += "F";
                        break;
                    case ImprovementType.Mine:
                        tileStr += "M";
                        break;
                }
			}

            // If the tile's x-value is on the edge, prep the next letter to be on a new line
			if((int)childTrans.GetComponent<Tile>().coordinates.x == size - 1)
                writer.WriteLine(tileStr);
            // Otherwise, write the letter on the back of the current line
			else
                writer.Write(tileStr + " ");
		}
        
        writer.Close();
        savedMapsNums = GetSavedMapNums();
    }

    /// <summary>
    /// A helper method that creates a tile and sets all needed information
    /// </summary>
    /// <param name="parentGameObj">The parent of the tile</param>
    /// <param name="posCoordinates">The tile's coordinates</param>
    /// <param name="tileType">The tile's type</param>
    private GameObject CreateWorldTile(GameObject parentGameObj, Vector2 posCoordinates, TileType tileType)
	{
        // Create a 3D position based on the given x-y 
        Vector3 spawnPosition = new Vector3(posCoordinates.x, tilePrefab.transform.position.y, posCoordinates.y);

        // Create the tile as a child of the map parent
        GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, parentGameObj.transform);

        newTile.name = tileType.ToString() + " Tile";
        // Set coordinates and tileType
        newTile.GetComponent<Tile>().coordinates = posCoordinates;
        newTile.GetComponent<Tile>().tileType = tileType;
        // Change material based on tile type
        newTile.GetComponent<Tile>().ChangeMaterial(tileMaterials[tileType]);
        if(tileType == TileType.Forest || tileType == TileType.Mountains)
            CreateResourceObj(newTile, tileType);
        // Set select event
        newTile.GetComponent<Selectable>().unityEvent = TileSelector.instance.selectEvent;
        // Ensure the tile markers are inactive
        newTile.GetComponent<Tile>().Select(false);

        return newTile;
	}

    /// <summary>
    /// Creates a resource object on a map tile
    /// </summary>
    /// <param name="tile">The gameObject tile that the resource will be on</param>
    /// <param name="tileType">The tile type of the tyle</param>
    private void CreateResourceObj(GameObject tile, TileType tileType)
	{
        GameObject newResource = null;
        switch(tileType)
        {
            case TileType.Forest:
                newResource = Instantiate(forestPrefab, tile.transform);
                newResource.transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                break;
            case TileType.Mountains:
                newResource = Instantiate(mountainsPrefab, tile.transform);
                break;
        }
        // End early if no resource object was created
        if(newResource == null)
            return;
        // Set initial data
        newResource.name = tileType.ToString();
        newResource.GetComponent<Resource>().tile = tile;
        newResource.GetComponent<Selectable>().unityEvent = TileSelector.instance.selectEvent;
        tile.GetComponent<Tile>().resource = newResource;
	}

    /// <summary>
    /// A helper method that gets a random tile type
    /// </summary>
    /// <param name="x">The x-index of the tile</param>
    /// <param name="y">The y-index of the tile</param>
    /// <returns>A random tile type</returns>
    private TileType GetRandomTileType(int x, int y)
	{
        // Get a perlin noise'd number (scalars needed for noise)
        float oceanNoiseNum = Mathf.PerlinNoise(x * oceansNoiseMod, y * oceansNoiseMod);

        if(oceanNoiseNum < oceansPercent)
            return TileType.Ocean;

        float mountainsNoiseNum = Mathf.PerlinNoise(x * mountainsNoiseMod, y * mountainsNoiseMod);
        if(mountainsNoiseNum < mountainsPercent)
            return TileType.Mountains;

        float forestNoiseNum = Mathf.PerlinNoise(x * forestNoiseMod, y * forestNoiseMod);
        if(forestNoiseNum < forestPercent)
            return TileType.Forest;

        return TileType.Plains;
    }

    /// <summary>
    /// Deletes a saved map file
    /// </summary>
    /// <param name="mapNum">The index of the map that will be deleted</param>
    public void DeleteMap(int mapNum)
	{
        if (mapNum == -1)
            return;

        // Delete the map file
        string filePath = mapFilePath + "/mapSavedAt" + mapNum + ".txt";
        File.Delete(filePath);

        // Update savedMapsNums
        savedMapsNums = GetSavedMapNums();
        UIManager.instance.CreateMapLoadButtons();
    }

    /// <summary>
    /// A helper method that resets and centers the camera
    /// </summary>
    private void CenterCamera()
    {
        CameraManager.instance.ResetCamera();
        CameraManager.instance.MoveCamera(
            new Vector3(
                (float)size / 2 - 0.5f,
                0,
                (float)size / 2 + 1.0f));
    }

    /// <summary>
    /// A helper method that clears the current map
    /// </summary>
    private void ClearMap()
	{
        // Loop through each tile and destroy it
        foreach(Transform tileTrans in tilesParent.transform)
            Destroy(tileTrans.gameObject);

		// Loop through each improvement, within its parentObj and destroy it
		foreach(Transform subParentTrans in improvementParent.transform)
            foreach(Transform improvementTrans in subParentTrans)
                Destroy(improvementTrans.gameObject);

        // Zero out all resource amounts
        foreach(ResourceType resourceType in GameManager.instance.resources.Keys)
            GameManager.instance.resources[resourceType].ResetAmount();

        // Give the player 15 wood to start
        GameManager.instance.UpdateResourceAmount(ResourceType.Wood, 15);

        // Update UI
        UIManager.instance.UpdateResourcesUI();
    }

    /// <summary>
    /// A helper method to get the number of saved maps
    /// </summary>
    /// <returns>A list of the numbers of every saved map</returns>
    private List<int> GetSavedMapNums()
	{
        DirectoryInfo dir = new DirectoryInfo(mapFilePath);
        FileInfo[] fileInfos = dir.GetFiles("*.txt");   // ignores non .txt files (mostly .meta)
        List<int> mapNums = new List<int>();
        foreach (FileInfo fileInfo in fileInfos)
        {
            // fileInfo.Name    ->  gets name of file
            // Substring(10)    ->  trims string to map#.txt
            // .Split('.')[0]   ->  "trims" string to map#
            int mapNum = int.Parse(fileInfo.Name.Substring(10).Split('.')[0]);
            mapNums.Add((mapNum));
        }

        return mapNums;
    }

    /// <summary>
    /// A helper method to find the right empty parent gameObj for the right improvement
    /// </summary>
    /// <param name="improvementType">The type of improvement</param>
    /// <returns>The empty parent gameObj for that improvement</returns>
    public GameObject FindImprovementSubParent(ImprovementType improvementType)
	{
        foreach(Transform subParentTrans in improvementParent.transform)
            if(subParentTrans.gameObject.name.ToLower() == improvementType.ToString().ToLower() + "s")
                return subParentTrans.gameObject;

        return null;
    }
}
