# PhaserJSDocExtractor

If you're one of the still remaining ~four of us luddites who's still writing JavaScript without TypeScript, you might encounter an issue where not all the defined object types in [Phaser](https://phaser.io/)'s [jsdoc](https://jsdoc.app/) blocks are correctly resolved, meaning that your IDE won't be able to navigate to certain type definitions or provide you with code completion.

This is because not all of Phaser's jsdoc typedefs are exported in the final distributed JavaScript file (ex: phaser-3.60.0.js).  An example is `Phaser.Types.Physics.Matter.MatterBody`, or just about anything in the `Phaser.Types`, and that's because that particular namespace is not part of the exported typedefs.

![image](https://github.com/dreasgrech/PhaserJSDocExtractor/assets/501697/79c1c8c6-8c95-4b52-92cf-ba5b4e0f37b3)

This program extracts all the `@typedef` (and `@callback`) jsdoc blocks in the Phaser source code and outputs them in a single JavaScript file.  Including this file in your project will then provide you with all the type definition metadata needed for code completion and other good stuff.

![image](https://github.com/dreasgrech/PhaserJSDocExtractor/assets/501697/d27748fa-65b6-42ba-838f-cdf34fbafb3f)
