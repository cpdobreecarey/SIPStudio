# Changelog

# 2025-12-04 **2026.0.0**

Initial publication of SIPStudio Revit add-in.

### Features

- **Link DWG – Preconfigured Import Options**
  - Opens file dialog filtered to **DWG** files only.
  - **Import settings:**
    - Auto-correct lines slightly off axis.
    - Black and white colour mode.
    - Origin to origin placement.
    - Current view only.
    - Import units set to millimetres.
    - Visible layers only.
  - **Visual Graphic Overrides applied to imported DWG:**
    - Transparency: 100%
    - Projection line colour: **`#408080`**
    - Draw layer: Foreground

- **Orient 3D View to Plane**
  - Orients the active 3D view normal to the selected plane and auto-centres the object on screen.
  - Available only in 3D views.
  - Prompts user to select a planar face in the model.
  - Command has no ribbon button; invoked via keyboard shortcut **`OP`**.

- **SIP Project Template Configuration**
  - Configures Revit to use the bundled SIP template as the default project template.
  - Users must first load the add-in, open a document, and then restart Revit for changes to take effect.