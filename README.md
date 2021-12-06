# Colony Simulator

## To Do's
- Restrict camera movement based on size of map
- Create resource production
- Implement player ability to delete saved maps

## Reminders
Possible things to improve/make more efficient
- When adding a new tile type
  - Add it to the TileType enum
  - Create a material variable for it
  - Add a line to PopulateTileMatDict() in WorldGenerator.cs to pair the tile type and material
  - Create a TILE_TYPE % varible in WorldGenerator.cs for randomly generated maps
  - Add a calculation to use the TILE_TYPE % when creating a random map
