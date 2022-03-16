# Colony Simulator

## To Do's
- Implement player ability to delete saved maps (deleting works for the last map only)

## Completed
- Map Creation/Generation
    - A map is randomly generated using Perlin Noise
    - Resource map tiles will have resource objects (ex: forests, mountains, etc)
    - Maps can be saved and loaded by the user
- Gameplay
    - Improvements can be built on correct tiles (ex: mines on mountain tiles, nothing on oceans)
        - Houses: add space for more colonists to settle and live in
        - Farms: produce food
        - Mines: produce stone
        - Lumber Mills: produce woods
    - Improvements can be destroyed 
    - Producing improvements produce their specific resource and give it to the user
- UI
    - Different UI panels/elements for each game state
    - Player stats update as they produce more resources
    - The camera can be moved via user input
        - Panning: WASD
        - Zoom: RF
