# Colony Simulator

## Gameplay

### Improvements
- Improvements can be built if the player has enough of the right resources (ex: a farm costs 10 wood)
    - Some improvements are resource tile dependant (ex: mines can only be built on mountains)
- Improvements can be destroyed after being built
- Producing improvements generate their specific resource and add it to the user's current amount
- Types
    - Houses: add space for more colonists to settle and live in
    - Farms: produce food
    - Mines: produce stone
    - Lumber Mills: produce wood

### Map Creation
- A random map can be generated, using Perlin Noise for resource tile spawning
- Maps can be saved from the pause menu
    - The current map will be logged as well as the player's built improvements, current resource amounts, and production amounts
- Saved maps can be loaded, restoring all logged info
- Saved maps can be deleted from the map select screen

### Camera Controls
- Panning: WASD
- Zooming: R/F

## To-Do's

### Future Ideas
**Colonists**
- Having individual colonists that run around to perform tasks (building, moving/producing resources, etc)
- Colonists have needs that determine a happiness amount 
    - More happiness, the more new colonists are likely to settle
    - Less happiness, the more current colonists are likely to leave the colony

**More Resources/Improvements** 
- Add more rare resources and more complex improvements that provide further benefit
    - Resource tile and improvement to mine Metal

**Camera**
- Readd camera rotating (using Q/E)

### Things to Improve

**Art**
- Tiles and improvement object and UI assets
- Colonists assets (when added)

**Adding a New Resource**
- Add a new ResourceType enum value in ImprovementManager
- Create a text object in the scene to display the current and secondary amounts of the resource 
- Add a public GameObj variable for the text object in ImprovementManager
- Add a new resourceDescription in GameManager, including the text object variable from the UIManager

**Adding a New Improvement**
- Add a new ImprovementType enum value in ImprovementManager
- Create the improvement prefab, attaching the Producer script if appicable
- Add a public GameObj variable in ImprovementManager
- Add a new improvementDescription in in GameManager, also adding its resource costs 
