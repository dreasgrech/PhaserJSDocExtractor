# PhaserJSDocExtractor

If you're one of the still remaining ~four of us luddites who's still writing JavaScript without TypeScript, you might encounter an issue where not all the defined object types in [Phaser](https://phaser.io/)'s [jsdoc](https://jsdoc.app/) blocks are correctly resolved, meaning that your IDE won't be able to navigate to certain type definitions or provide you with code completion.

This is because not all of Phaser's jsdoc typedefs are exported in the final distributed JavaScript file (ex: phaser-3.60.0.js).  An example is `Phaser.Types.Physics.Matter.MatterBody`, or just about anything in the `Phaser.Types`, and that's because that particular namespace is not part of the exported typedefs.

![image](https://github.com/dreasgrech/PhaserJSDocExtractor/assets/501697/bd37abd4-5843-4964-8676-1324b45a7d5c)

This program extracts all the `@typedef` (and `@callback`) jsdoc blocks in the Phaser source code and outputs them in a single JavaScript file.  Including this file in your project will then provide you with all the type definition metadata needed for code completion and other good stuff.

![image](https://github.com/dreasgrech/PhaserJSDocExtractor/assets/501697/9a72a75f-bd0d-41e3-9ada-3b6fc4baf9e2)
