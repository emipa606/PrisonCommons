# GitHub CoPilot Instructions for Prison Commons (Continued)

## Mod Overview and Purpose
The "Prison Commons (Continued)" mod enhances the prison management experience in the game RimWorld. Originally developed by Ben Lubars, this updated version introduces the concept of "prison commons" areas, allowing mod users to mark any enclosed gather spot (such as a table, sculpture, or campfire) as a place where prisoners can freely mingle. This feature integrates these spots into the prison environment, providing more controlled interactions and reducing issues related to high door counts during prison breaks.

## Key Features and Systems
- **Prison Commons Areas:** Transform areas with gather spots into parts of a prison where prisoners can move freely. This reduces the reliance on multiple doors and alleviates prison break issues.
- **Prisoner Movement:** Enhances prisoner AI to recognize prison commons as accessible areas.
- **Compatibility and Extensions:** Works in harmony with other mods for a seamless experience. Integrates prison management tools for improved gameplay.

## Coding Patterns and Conventions
- **File Naming:** The filenames reflect their function and are typically in the format `<ClassPurpose>_<MethodPurpose>.cs` (e.g., `Building_Door_PawnCanOpen.cs`).
- **Class Structure:** All classes are static or internal where appropriate, signifying their utility-focused role within the mod’s workflow.
- **Method Conventions:** Methods follow clear naming practices to denote their role or action, making the mod’s function easy to navigate (e.g., `TryMakeJob`, `InitiatePrisonBreakMtbDays`).

## XML Integration
The configuration and management of object properties and game integration are defined through XML files. Pay close attention to how XML elements correspond to game objects in the C# code:
- **Data Definitions:** Use XML files to define new game entities and attributes that are accessed by C# logic.
- **Mod Extensions:** Extend existing game behaviors using XML-backed configuration files to maintain flexibility and minimal redundancy in the code.

## Harmony Patching
Harmony is used extensively to integrate the mod’s features into RimWorld’s existing codebase:
- **Patch Classes:** Use Harmony’s `PatchAll` method in static classes to modify game methods with care.
- **Method Prefix and Postfix:** Employ prefixes and postfixes to enhance or modify existing methods without replacing them entirely. This approach ensures compatibility with other mods.

## Suggestions for Copilot
- **Anticipate Common Patterns:** Utilize existing class and method structures to expand the codebase efficiently. Copilot can suggest code that adheres to current patterns and naming conventions.
- **XML Snippets:** Use Copilot to generate XML snippets for alignment with C# methods, ensuring seamless integration and maintaining overall organization.
- **Patch Example Suggestions:** When using Harmony, leverage Copilot to create example patch methods that demonstrate the correct usage of prefixes, postfixes, and transpilers.
- **Error Handling Tips:** Include practical suggestions for tracing and handling errors through robust logging and testing with isolated mod environments before integration. 

By following this comprehensive guide, you can use GitHub Copilot to efficiently generate and maintain code for the "Prison Commons (Continued)" mod while ensuring high compatibility and functionality within the RimWorld ecosystem.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
