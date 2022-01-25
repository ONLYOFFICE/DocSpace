import React from "react";
import { useHotkeys } from "react-hotkeys-hook";
import { observer, inject } from "mobx-react";

const withHotkeys = (Component) => {
  const WithHotkeys = (props) => {
    const { selection, setSelection, firstFile, nextFile, prevFile } = props;

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

    return <Component {...props} />;
  };

  return inject(({ filesStore }) => {
    const { selection, setSelection, filesList } = filesStore;

    const indexOfCurrentFile =
      filesList &&
      selection.length &&
      filesList.findIndex((f) => f.id === selection[selection.length - 1].id);

    const isValidIndex =
      indexOfCurrentFile !== null && indexOfCurrentFile !== -1;

    const nextIndex = isValidIndex ? indexOfCurrentFile + 1 : null;
    const prevIndex = isValidIndex ? indexOfCurrentFile - 1 : null;

    const nextFile = nextIndex !== null ? filesList[nextIndex] : null;
    const prevFile = prevIndex !== null ? filesList[prevIndex] : null;

    return {
      selection,
      setSelection,
      firstFile: filesList[0],
      nextFile,
      prevFile,
    };
  })(observer(WithHotkeys));
};

export default withHotkeys;
