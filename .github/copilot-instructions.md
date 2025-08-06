# GitHub Copilot Instructions for Prison Commons (Continued)

## Mod Overview and Purpose
The 'Prison Commons (Continued)' mod is an update to the original mod by Ben Lubars, enhancing the gameplay experience in RimWorld by introducing the concept of a prison commons area. This mod allows players to designate any enclosed Gather Spot (such as tables, sculptures, or campfires) as a part of the prison's communal space. Within these designated areas, prisoners can freely enter and exit through doors without restrictions, thereby adding depth to prison management dynamics in the game. 

The mod aims to provide a more realistic and immersive environment for managing prisoners while addressing common challenges, like the infamous prison breaks due to an excessive number of doors.

## Key Features and Systems
- **Prison Commons Assignment**: Marking various enclosed objects as part of the prison commons.
- **Door Accessibility for Prisoners**: Ensuring doors within these commons areas do not impede prisoner movement.
- **Enhanced Prison Break Dynamics**: Tweaked algorithms for prison break initiation related to prisoner freedom and accessibility within designated commons.
- **Integrated Social and Warden Duties**: Adjustments to warden behavior and prisoner interactions considering the new commons areas.

## Coding Patterns and Conventions
- **Namespace and Class Definitions**: All logic is encapsulated in static classes or as extensions of existing RimWorld classes. Ensure that new functionality is internal to prevent cross-assembly dependencies unless public access is necessary.
- **Method Definitions**: Use concise and descriptive method names indicating the main purpose.
- **Maintainability**: Regularly refactor code for clarity and maintainability; avoid code duplication where possible by creating utility methods.

## XML Integration
- This mod leverages RimWorld's XML structure primarily for defining object properties and integrating with existing game settings. Ensure XML files are updated with new definitions for objects or features introduced, maintaining consistency with the mod's purpose.
- Properly define `Defs` to use RimWorld's data-driven configuration pattern, allowing for easy mod configuration changes.

## Harmony Patching
- Use Harmony patches to ensure compatibility with the core game and other mods. Harmony is used extensively in this mod to override or extend existing game functions without altering the original game code.
- Ensure all Harmony patches are well-documented within your codebase, noting the original method and the purpose of the patch.
  
## Suggestions for Copilot
- **Class and Method Auto-completion**: Take advantage of Copilot to quickly scaffold new classes and methods following the existing mod patterns, especially when adding new features.
- **Harmony Patch Syntax**: Utilize prompts for standard Harmony patching practices, ensuring consistent and correct implementation.
- **XML File Assistance**: Use Copilot to generate required XML snippets for new object definitions or adjustments in the mod.
- **Code Suggestions and Refactoring**: Allow Copilot to suggest improvements and optimizations to existing methods, focusing on readabilities, such as simplifying complex LINQ queries or streamlining conditionals.

By adhering to the guidelines and conventions outlined in this document, contributors can maintain a high standard of quality and consistency across the 'Prison Commons (Continued)' mod codebase, ensuring an enriched player experience.
