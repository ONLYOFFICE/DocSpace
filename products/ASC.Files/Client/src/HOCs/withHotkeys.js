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
    } = props;

    //Select item
    useHotkeys(
      "x",
      () => (selection.length ? setSelection([]) : setSelection([firstFile])),
      [selection]
    );

    //Select bottom element
    // TODO: tile view
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
    // TODO: tile view
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
        if (!selection.length) setSelection([firstFile]);
        else if (prevFile) setSelection([prevFile]);
      },
      [prevFile, selection, firstFile]
    );

    //Select item on the right
    useHotkeys(
      "l, RIGHT",
      () => {
        if (!selection.length) setSelection([firstFile]);
        else if (nextFile) setSelection([nextFile]);
      },
      [nextFile, selection, firstFile]
    );

    //Expand Selection DOWN
    useHotkeys(
      "shift+DOWN",
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

    //Expand Selection UP
    useHotkeys(
      "shift+UP",
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

    //Expand Selection RIGHT
    // TODO: tile view
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
    // TODO: tile view
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

    return <Component {...props} />;
  };

  return inject(({ filesStore }, { sectionWidth }) => {
    const {
      selection,
      setSelection,
      filesList,
      setSelected,
      hotkeyCaret,
      setHotkeyCaret,
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
        const indexForNextTile =
          caretIndex + countTilesInRow - countOfMissingFiles;

        nextForTileDown =
          foldersLength - caretIndex - 1 <= division || division === 0
            ? filesList[indexForNextTile]
              ? filesList[indexForNextTile]
              : files[0]
            : folders[foldersLength - 1];
      }

      //Prev tile
      if (prevForTileUp.isFolder !== isFolder) {
        const indexForPrevTile =
          caretIndex - countTilesInRow + countOfMissingFiles;

        prevForTileUp = filesList[indexForPrevTile]
          ? filesList[indexForPrevTile].isFolder
            ? filesList[indexForPrevTile]
            : folders[foldersLength - 1]
          : folders[foldersLength - 1];
      }
    }

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
    };
  })(observer(WithHotkeys));
};

export default withHotkeys;
