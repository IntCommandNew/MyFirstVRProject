# MyFirstVRProject!

A small VR darts game built with **Unity**, **OpenXR** and the **XR Interaction Toolkit**.

The menu can be opened using the secondary action button of the left controller (for the XR Device Simulator, the button is mapped to [Shift + 2])

Link to demo video: https://www.youtube.com/watch?v=yamxD-bvmUE

## AI-generated content

The **ring radii** in `DartBoard.cs` (the `rings` array) were **calculated by Claude (Anthropic)** from the model's source files. The ring boundaries were measured on the dart model's texture and converted to fractions of the board's radius using the mesh's UV mapping, so the scoring matches the rings painted on the board.

## Credits

- **Table:** [Table](https://sketchfab.com/3d-models/table-1132fa2850a24917892733566bd68e74) from Sketchfab. (note: disabled in unity editor)
- **Darts and dartboard:** [Stylized Dart](https://sketchfab.com/3d-models/stylized-dart-0a2b3b229615424fb19073d4c48a35b6) from Sketchfab.
- **Scoring ring values:** calculated by Claude (Anthropic).
- **Furniture:** [Kenney Furniture Kit](https://kenney.nl/assets/furniture-kit)
- **Floor texture:** [Floor Texture](https://www.boracity.com/textures/wood/dark-oak-wood-planks)
- **Game panel UI:** [Panel Asset](https://freegameui.net/assets/panels/panel_rect_slate-silver_01/)
- **Explosion texture:** [Explosion grid texture](https://github.com/Tvtig/RocketLauncher/tree/main/Assets/Tvtig/Rocket%20Launcher/Art/Textures/Explosion)