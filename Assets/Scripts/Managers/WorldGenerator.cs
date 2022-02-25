using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum TileType
{
    Plains,
    Grassland,
    Ocean
}

public class WorldGenerator : MonoBehaviour
{
    // Map parent
    public GameObject tilesParent;

    // Main tile prefab
    public GameObject tilePrefab;
    
    // Tile materials
    public Dictionary<TileType, Material> tileMaterials;
    public Material plainsMat;
    public Material grasslandMat;
    public Material oceanMat;

    // World Settings
    public int size;
    public float grasslandPerc;
    public float oceanPerc;

    public int savedMapCount;
    private string mapFilePath;

    // Start is called before the first frame update
    void Start()
    {
        mapFilePath = "Assets/Resources/Maps";
        savedMapCount = GetSavedMapCount();

        tileMaterials = new Dictionary<TileType, Material>();
        PopulateTileMatDict();

        // TODO: Add when more tile types are added (make more efficient?)
        if(grasslandPerc > 1)
            grasslandPerc = 1;
        if(oceanPerc > 1)
            oceanPerc = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Correlates a tile type to a material
    /// (To be used when a tile is created)
    /// </summary>
    private void PopulateTileMatDict()
	{
        // TODO: Add when more tile types are added
        tileMaterials.Add(TileType.Plains, plainsMat);
        tileMaterials.Add(TileType.Grassland, grasslandMat);
        tileMaterials.Add(TileType.Ocean, oceanMat);
    }

    /// <summary>
    /// Create a random world with randomized parameters (size and tile type %s)
    /// </summary>
    public void CreateNewRandomMap()
	{
        // Randomize the size and ocean and grassland percentages
        size = Random.Range(25, 50);
        oceanPerc = Random.Range(0.1f, 0.3f);
        grasslandPerc = Random.Range(0.1f, 0.5f);

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
        GetComponent<GameManager>().ChangeGameState(GameState.game);
	}

    /// <summary>
    /// Loads a world from the "Maps" folder
    /// </summary>
    /// <param name="mapIndex">The index of the map trying to be loaded</param>
    public void LoadWorld(int mapIndex)
    {
        // Create the file path string
        string filePath = mapFilePath + "/map" + mapIndex + ".txt";

        // End early if the file does not exist
        if(!File.Exists(filePath))
            return;

        // Clear the current map
        ClearMap();

        // Create the StreamReader and read the file line by line
        StreamReader reader = new StreamReader(filePath);
        int lineNum = -1;
        while(!reader.EndOfStream)
        {
            // Read the next line
            lineNum++;
            string testLine = reader.ReadLine();
            // Loop through the line (technically looping through each character of the one word)
            for(int i = 0; i < testLine.Length; i++)
			{
                char c = testLine[i];
                // Create a tile based on the character
                switch(c)
                {
                    case 'O':
                        CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Ocean);
                        break;
                    case 'G':
                        CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Grassland);
                        break;
                    case 'P':
                    default:
                        CreateWorldTile(tilesParent, new Vector2(i, lineNum), TileType.Plains);
                        break;
                }
			}
        }

        reader.Close();

        // Update world info
        size = lineNum + 1;
        grasslandPerc = -1;
        oceanPerc = -1;

        // Recenter the camera and change the gameState
        CenterCamera();
        GetComponent<GameManager>().ChangeGameState(GameState.game);
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
        savedMapCount++;
        string filePath = mapFilePath + "/map" + savedMapCount + ".txt";
        StreamWriter writer = File.CreateText(filePath);
        
        // Loop through each tile and write a character based on the tile type
        foreach(Transform childTrans in tilesParent.transform)
		{
            // Get the correct letter based on the tile's type
            string newLetter = "P";
            switch(childTrans.GetComponent<Tile>().tileType)
            {
                case TileType.Grassland:
                    newLetter = "G";
                    break;
                case TileType.Ocean:
                    newLetter = "O";
                    break;
                case TileType.Plains:
                default:
                    break;
            }

            // If the tile's x-value is on the edge, prep the next letter to be on a new line
			if((int)childTrans.GetComponent<Tile>().coordinates.x == size - 1)
                writer.WriteLine(newLetter);
            // Otherwise, write the letter on the back of the current line
			else
                writer.Write(newLetter);
		}
        
        writer.Close();
    }

    /// <summary>
    /// A helper method that creates a tile and sets all needed information
    /// </summary>
    /// <param name="parentGameObj">The parent of the tile</param>
    /// <param name="posCoordinates">The tile's coordinates</param>
    /// <param name="tileType">The tile's type</param>
    private void CreateWorldTile(GameObject parentGameObj, Vector2 posCoordinates, TileType tileType)
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
        // Set select event
        newTile.GetComponent<Selectable>().unityEvent = GetComponent<TileSelector>().selectEvent;

        // Ensure the tile markers are inactive
        newTile.GetComponent<Tile>().Select(false);
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
        float randNoiseNum = Mathf.PerlinNoise(x * 0.15f, y * 0.15f);

        if(randNoiseNum <= oceanPerc)
            return TileType.Ocean;
        else
		{
            if(randNoiseNum <= grasslandPerc)
                return TileType.Grassland;
            else
                return TileType.Plains;
        }
    }

    /// <summary>
    /// Deletes a saved map file
    /// </summary>
    /// <param name="mapIndex">The index of the map that will be deleted</param>
    public void DeleteMap(int mapIndex)
	{
        // Delete the map file
        string filePath = mapFilePath + "/map" + mapIndex + ".txt";
        File.Delete(filePath);

        // Recalculate savedMapCount
        savedMapCount = GetSavedMapCount();
    }

    /// <summary>
    /// A helper method that resets and centers the camera
    /// </summary>
    private void CenterCamera()
    {
        GetComponent<CameraManager>().ResetCamera();
        GetComponent<CameraManager>().MoveCamera(
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
        // Loop through each child of the map parent and destroy it
        foreach(Transform childTrans in tilesParent.transform)
            Destroy(childTrans.gameObject);
    }

    /// <summary>
    /// A helper method to get the number of saved maps
    /// </summary>
    /// <returns></returns>
    private int GetSavedMapCount()
	{
        DirectoryInfo dir = new DirectoryInfo(mapFilePath);
        FileInfo[] fileInfos = dir.GetFiles("*.txt");   // ignores non .txt files (mostly .meta)
        return fileInfos.Length;
    }
}
