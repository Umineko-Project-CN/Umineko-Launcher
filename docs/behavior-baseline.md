# Behavior Baseline

The upgrade and refactor work keeps the current launcher behavior stable until a later UI migration.

## Startup

- Enforces a single running launcher instance.
- Verifies that `ons.cfg` exists before the main UI opens.
- Loads the saved game configuration at startup.
- Sets `CurrentUICulture` from the saved language selection.

## Main launcher flows

- Shows the localized splash screen, then opens the main window.
- Loads remote news and update status from the existing update XML feed.
- Lets the player:
  - launch the game
  - run a verify launch
  - open the language dialog
  - open the about popup
  - open the config popup
  - open the feedback link

## Configuration

- Reads and writes the existing `ons.cfg` format.
- Preserves unsupported config lines when saving.
- Supports display resolution, display mode, scale, legacy OP, and script language.
- Keeps current language switching behavior, including the special handling for missing CHT resources.

## Updating

- Uses the existing update XML schema and checksum/version rules.
- Supports:
  - no update required
  - incremental launcher/script/resource updates
  - manual update requirement for major resource version jumps
- Downloads update packages, shows progress, then invokes the bundled `ZipExtractor`.

## Dialogs and windows

- Keeps the current modal dialogs and popups:
  - message dialog
  - language dialog
  - download dialog
  - config popup
  - about popup
