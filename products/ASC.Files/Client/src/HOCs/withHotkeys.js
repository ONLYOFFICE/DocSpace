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
        else if (nextFile) setSelection([nextFile]);
      },
      [nextFile, selection, firstFile]
    );

    //Select upper item
    // TODO: tile view
    useHotkeys(
      "k, UP",
      () => {
        if (!selection.length) setSelection([firstFile]);
        else if (prevFile) setSelection([prevFile]);
      },
      [prevFile, selection, firstFile]
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
    // TODO: tile view
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
    // TODO: tile view
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

    //Expand Selection LEFT
    // TODO: tile view
    useHotkeys(
      "shift+LEFT",
      () => {
        // if (!selection.length) setSelection([firstFile]);
        // else if (nextFile) setSelection([nextFile]);
      },
      [
        /* nextFile, selection, firstFile */
      ]
    );

    //Expand Selection RIGHT
    // TODO: tile view
    useHotkeys(
      "shift+RIGHT",
      () => {
        // if (!selection.length) setSelection([firstFile]);
        // else if (nextFile) setSelection([nextFile]);
      },
      [
        /* nextFile, selection, firstFile */
      ]
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

  return inject(({ filesStore }) => {
    const {
      selection,
      setSelection,
      filesList,
      setSelected,
      hotkeyCaret,
      setHotkeyCaret,
      deselectFile,
    } = filesStore;

    let prevCaretIndex, nextCaretIndex, prevFile, nextFile;

    if (filesList && selection.length && hotkeyCaret) {
      prevCaretIndex = filesList.findIndex((f) => f.id === hotkeyCaret.id);
      nextCaretIndex = filesList.findIndex((f) => f.id === hotkeyCaret.id);

      if (prevCaretIndex !== -1) {
        prevCaretIndex -= 1;
        prevFile = filesList[prevCaretIndex];
      }

      if (nextCaretIndex !== -1) {
        nextCaretIndex += 1;
        nextFile = filesList[nextCaretIndex];
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
    };
  })(observer(WithHotkeys));
};

export default withHotkeys;
