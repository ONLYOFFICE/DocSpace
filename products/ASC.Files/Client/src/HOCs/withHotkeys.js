import React from "react";
import { useHotkeys } from "react-hotkeys-hook";
import { observer, inject } from "mobx-react";

const withHotkeys = (Component) => {
  const WithHotkeys = (props) => {
    const {
      selection,
      setSelection,
      firstFile,
      nextFile,
      prevFile,
      setSelected,
      setHotkeyCaret,
      deselectFile,
      hotkeyCaret,
      nextForTileDown,
      prevForTileUp,
      viewAs,
      selectionsDown,
      selectionsUp,
      hotkeyCaretStart,
      setHotkeyCaretStart,
      setHotkeyPanelVisible,
    } = props;

    //Select item
    useHotkeys(
      "x",
      () => {
        if (selection.length) setSelection([]);
        else {
          setSelection([firstFile]);
          setHotkeyCaretStart(null);
        }
      },
      [selection, firstFile]
    );

    //Select bottom element
    useHotkeys(
      "j, DOWN",
      () => {
        if (!selection.length) setSelection([firstFile]);
        else if (viewAs === "tile") setSelection([nextForTileDown]);
        else if (nextFile) setSelection([nextFile]);
      },
      [nextFile, selection, firstFile, nextForTileDown]
    );

    //Select upper item
    useHotkeys(
      "k, UP",
      () => {
        if (!selection.length) setSelection([firstFile]);
        else if (viewAs === "tile") setSelection([prevForTileUp]);
        else if (prevFile) setSelection([prevFile]);
      },
      [prevFile, selection, firstFile, prevForTileUp]
    );

    //Select item on the left
    useHotkeys(
      "h, LEFT",
      () => {
        if (!selection.length) {
          setSelection([firstFile]);
          setHotkeyCaretStart(firstFile);
        } else if (prevFile) {
          setSelection([prevFile]);
          setHotkeyCaretStart(prevFile);
        }
      },
      [prevFile, selection, firstFile]
    );

    //Select item on the right
    useHotkeys(
      "l, RIGHT",
      () => {
        if (!selection.length) {
          setSelection([firstFile]);
          setHotkeyCaretStart(firstFile);
        } else if (nextFile) {
          setSelection([nextFile]);
          setHotkeyCaretStart(nextFile);
        }
      },
      [nextFile, selection, firstFile]
    );

    //Expand Selection DOWN
    useHotkeys(
      "shift+DOWN",
      () => {
        if (!hotkeyCaretStart) setHotkeyCaretStart(hotkeyCaret);
        if (!selection.length) setSelection([firstFile]);

        if (viewAs === "tile") {
          if (nextForTileDown.id === hotkeyCaret.id) return;
          setSelection([
            ...selectionsDown,
            ...[hotkeyCaretStart ? hotkeyCaretStart : hotkeyCaret],
          ]);
          setHotkeyCaret(nextForTileDown);
        } else if (nextFile) {
          if (selection.findIndex((f) => f.id === nextFile.id) !== -1) {
            deselectFile(hotkeyCaret);
          } else {
            setSelection([...selection, ...[nextFile]]);
          }
          setHotkeyCaret(nextFile);
        }
      },
      [
        viewAs,
        nextFile,
        selection,
        firstFile,
        nextForTileDown,
        selectionsDown,
        hotkeyCaretStart,
        hotkeyCaret,
      ]
    );

    //Expand Selection UP
    useHotkeys(
      "shift+UP",
      () => {
        if (!hotkeyCaretStart) setHotkeyCaretStart(hotkeyCaret);
        if (!selection.length) setSelection([firstFile]);

        if (viewAs === "tile") {
          if (prevForTileUp.id === hotkeyCaret.id) return;
          setSelection([
            ...selectionsUp,
            ...[hotkeyCaretStart ? hotkeyCaretStart : hotkeyCaret],
          ]);
          setHotkeyCaret(prevForTileUp);
        } else if (prevFile) {
          if (selection.findIndex((f) => f.id === prevFile.id) !== -1) {
            deselectFile(hotkeyCaret);
          } else {
            setSelection([...[prevFile], ...selection]);
          }

          setHotkeyCaret(prevFile);
        }
      },
      [
        viewAs,
        prevFile,
        selection,
        firstFile,
        prevForTileUp,
        selectionsUp,
        hotkeyCaret,
        hotkeyCaretStart,
      ]
    );

    //Expand Selection RIGHT
    useHotkeys(
      "shift+RIGHT",
      () => {
        if (!selection.length) setSelection([firstFile]);
        else if (nextFile) {
          if (selection.findIndex((f) => f.id === nextFile.id) !== -1) {
            deselectFile(hotkeyCaret);
          } else {
            setSelection([...selection, ...[nextFile]]);
          }
          setHotkeyCaret(nextFile);
        }
      },
      [nextFile, selection, firstFile]
    );

    //Expand Selection LEFT
    useHotkeys(
      "shift+LEFT",
      () => {
        if (!selection.length) setSelection([firstFile]);
        else if (prevFile) {
          if (selection.findIndex((f) => f.id === prevFile.id) !== -1) {
            deselectFile(hotkeyCaret);
          } else {
            setSelection([...[prevFile], ...selection]);
          }

          setHotkeyCaret(prevFile);
        }
      },
      [prevFile, selection, firstFile]
    );

    //Select all files and folders
    useHotkeys("shift+a", () => {
      setSelected("all");
    });

    //Deselect all files and folders
    useHotkeys("shift+n", () => {
      setSelected("none");
    });

    //Open hotkeys panel
    useHotkeys("Ctrl+num_divide, Ctrl+/", () => setHotkeyPanelVisible(true));

    return <Component {...props} />;
  };

  return inject(({ filesStore, dialogsStore }, { sectionWidth }) => {
    const {
      selection,
      setSelection,
      filesList,
      setSelected,
      hotkeyCaret,
      setHotkeyCaret,
      setHotkeyCaretStart,
      hotkeyCaretStart,
      deselectFile,
      viewAs,
      files,
      folders,
    } = filesStore;

    const minTileWidth = 220 + 16;
    const countTilesInRow = Math.floor(sectionWidth / minTileWidth);
    const foldersLength = folders.length;
    const division = foldersLength % countTilesInRow;

    let caretIndex = -1;
    let prevCaretIndex, nextCaretIndex, prevFile, nextFile;
    let indexForNextTile, indexForPrevTile;
    let nextForTileDown, prevForTileUp;

    if (filesList.length && selection.length && hotkeyCaret) {
      caretIndex = filesList.findIndex((f) => f.id === hotkeyCaret.id);

      if (caretIndex !== -1) {
        prevCaretIndex = caretIndex - 1;
        prevFile = filesList[prevCaretIndex];
      }

      if (caretIndex !== -1) {
        nextCaretIndex = caretIndex + 1;
        nextFile = filesList[nextCaretIndex];
      }

      //TODO: table view
      // console.log("sectionWidth", sectionWidth);
      // console.log("countTilesInRow", countTilesInRow);

      //Tile view
      const nextTileFile = filesList[caretIndex + countTilesInRow];
      const prevTileFile = filesList[caretIndex - countTilesInRow];

      nextForTileDown = nextTileFile
        ? nextTileFile
        : filesList[filesList.length - 1];
      prevForTileUp = prevTileFile ? prevTileFile : filesList[0];

      const isFolder =
        caretIndex !== -1 ? filesList[caretIndex].isFolder : false;

      const countOfMissingFiles = division ? countTilesInRow - division : 0;

      //Next tile
      if (nextForTileDown.isFolder !== isFolder) {
        indexForNextTile = caretIndex + countTilesInRow - countOfMissingFiles;

        nextForTileDown =
          foldersLength - caretIndex - 1 <= division || division === 0
            ? filesList[indexForNextTile]
              ? filesList[indexForNextTile]
              : files[0]
            : folders[foldersLength - 1];
      } else if (!nextTileFile) {
        // const pp = filesList.findIndex((f) => f.id === nextForTileDown?.id);
        // if (pp < caretIndex + countTilesInRow) {
        //   nextForTileDown = hotkeyCaret;
        // }
      }

      //Prev tile
      if (prevForTileUp.isFolder !== isFolder) {
        indexForPrevTile = caretIndex - countTilesInRow + countOfMissingFiles;

        prevForTileUp = filesList[indexForPrevTile]
          ? filesList[indexForPrevTile].isFolder
            ? filesList[indexForPrevTile]
            : folders[foldersLength - 1]
          : folders[foldersLength - 1];
      } else if (!prevTileFile) {
        prevForTileUp = hotkeyCaret;
      }
    }

    //shift

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) => f.id === hotkeyCaretStart?.id
    );

    const selectionsDown = [];
    const selectionsUp = [];
    const firstSelectionIndex = filesList.findIndex(
      (f) => f.id === selection[0]?.id
    );
    //shift select down

    const nextForTileDownIndex = filesList.findIndex(
      (f) => f.id === nextForTileDown?.id
    );
    let nextForTileDownItemIndex = nextForTileDownIndex;

    const itemIndexDown =
      hotkeyCaretStartIndex !== -1 &&
      hotkeyCaretStartIndex < firstSelectionIndex
        ? hotkeyCaretStartIndex
        : firstSelectionIndex;

    if (itemIndexDown !== -1 && viewAs === "tile") {
      if (nextForTileDownItemIndex === -1)
        nextForTileDownItemIndex = itemIndexDown + countTilesInRow;

      if (nextForTileDownItemIndex > itemIndexDown) {
        while (nextForTileDownItemIndex !== itemIndexDown) {
          if (hotkeyCaretStartIndex - nextForTileDownItemIndex <= 0)
            selectionsDown.push(filesList[nextForTileDownItemIndex]);
          nextForTileDownItemIndex--;
        }
      }

      let counterToDown = 0;
      if (nextForTileDownIndex < hotkeyCaretStartIndex) {
        counterToDown = hotkeyCaretStartIndex - nextForTileDownIndex;
      }

      while (counterToDown !== 0) {
        selectionsDown.push(filesList[hotkeyCaretStartIndex - counterToDown]);
        counterToDown--;
      }
      //console.log("selectionsDown", selectionsDown);
    }

    const prevForTileUpIndex = filesList.findIndex(
      (f) => f.id === prevForTileUp?.id
    );
    let prevForTileUpItemIndex = prevForTileUpIndex;

    const itemIndexUp =
      hotkeyCaretStartIndex !== -1 &&
      hotkeyCaretStartIndex > firstSelectionIndex
        ? hotkeyCaretStartIndex
        : firstSelectionIndex;

    //shift select up
    if (itemIndexUp !== -1 && viewAs === "tile") {
      if (prevForTileUpItemIndex === -1)
        prevForTileUpItemIndex = itemIndexUp - countTilesInRow;

      if (prevForTileUpItemIndex < itemIndexUp) {
        while (prevForTileUpItemIndex !== itemIndexUp) {
          if (prevForTileUpItemIndex - hotkeyCaretStartIndex <= 0)
            selectionsUp.push(filesList[prevForTileUpItemIndex]);
          prevForTileUpItemIndex++;
        }

        let counterToUp = 0;
        if (prevForTileUpIndex > hotkeyCaretStartIndex) {
          counterToUp = prevForTileUpIndex - hotkeyCaretStartIndex;
        }

        while (counterToUp !== 0) {
          selectionsUp.push(filesList[hotkeyCaretStartIndex + counterToUp]);
          counterToUp--;
        }
      }
    }

    //console.log("selectionsUp", selectionsUp);
    return {
      selection,
      setSelection,
      firstFile: filesList[0],
      nextFile,
      prevFile,
      setSelected,
      hotkeyCaret,
      setHotkeyCaret,
      deselectFile,
      nextForTileDown,
      prevForTileUp,
      viewAs,
      selectionsDown,
      selectionsUp,
      setHotkeyCaretStart,
      hotkeyCaretStart,

      setHotkeyPanelVisible: dialogsStore.setHotkeyPanelVisible,
    };
  })(observer(WithHotkeys));
};

export default withHotkeys;
