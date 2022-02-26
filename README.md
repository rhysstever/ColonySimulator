# Colony Simulator

## To Do's
- Implement player ability to delete saved maps (deleting works for the last map only)

## Processes
Things that can be improved/simplified
- When adding a new tile type  
    - Add it to the TileType enum
    - Create a material variable for it
    - Add a line to PopulateTileMatDict() in WorldGenerator.cs to pair the tile type and material
    - Create a TILE_TYPE % varible in WorldGenerator.cs for randomly generated maps
    - Add a calculation to use the TILE_TYPE % when creating a random map
- When adding a new resource
      - Create a (int, int) variable for it in GameManager.cs
    - Create Text UI on the Canvas and in UIManager.cs
    - Update the text UI based on the value from the GameManager.cs in UpdateResourcesUI()
    - Create an improvement prefab for the producer of this resource
    - Add the producer as a value in the ImprovementType enum in ImprovementManager.cs
    - Add that prefab to the ImprovementManager.cs
    - Add a public helper method: BuildIMPROVEMENT_NAME() in ImprovementManager.cs
    - Add another case in the switch statement in BuildImprovement() in ImprovementManager.cs for this ImprovementType
    - Add a button to create the improvement in the selectedObjectPanel on the Canvas; make it a child of the tileButtons object

## Completed
- Map Creation/Generation
    - A map is randomly generated using Perlin Noise
    - Resource map tiles will have resource objects (ex: forests, mountains, etc)
    - Maps can be saved by the user
- Gameplay
    - Improvements can be built on correct tiles (ex: mines on mountain tiles, nothing on oceans)
    - Improvements can be destroyed 
    - Producing improvements produce their specific resource and give it to the user
- UI
    - Different UI panels/elements for each game state
    - Player stats update as they produce more resources
    - The camera can be moved via user input
        - Panning: WASD
        - Rotating: QE
        - Zoom: RF

