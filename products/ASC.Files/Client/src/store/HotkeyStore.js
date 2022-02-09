import { isDesktop } from "@appserver/components/utils/device";
import { makeAutoObservable } from "mobx";

class HotkeyStore {
  filesStore;
  dialogsStore;
  settingsStore;
  filesActionsStore;

  constructor(filesStore, dialogsStore, settingsStore, filesActionsStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.dialogsStore = dialogsStore;
    this.settingsStore = settingsStore;
    this.filesActionsStore = filesActionsStore;
  }

  selectFirstFile = () => {
    this.filesStore.setSelection([this.filesStore.filesList[0]]);
  };

  selectFile = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      setHotkeyCaretStart,
    } = this.filesStore;

    const index = selection.findIndex((f) => f.id === hotkeyCaret.id);
    if (index !== -1) {
      const newSelection = selection;
      newSelection.splice(index, 1);
      setSelection(newSelection);
    } else {
      const newSelection = selection;
      newSelection.push(hotkeyCaret);
      setSelection(newSelection);
      setHotkeyCaretStart(hotkeyCaret);
    }
  };

  selectBottom = () => {
    const { setSelection, viewAs, hotkeyCaret } = this.filesStore;

    if (!hotkeyCaret) return this.selectFirstFile();
    else if (viewAs === "tile") setSelection([this.nextForTileDown]);
    else if (this.nextFile) setSelection([this.nextFile]);
  };

  selectUpper = () => {
    const { hotkeyCaret, setSelection, viewAs } = this.filesStore;

    if (!hotkeyCaret) return this.selectFirstFile();
    else if (viewAs === "tile") setSelection([this.prevForTileUp]);
    else if (this.prevFile) setSelection([this.prevFile]);
  };

  selectLeft = () => {
    const {
      hotkeyCaret,
      setSelection,
      filesList,
      setHotkeyCaretStart,
    } = this.filesStore;
    if (!hotkeyCaret) {
      this.selectFirstFile();

      setHotkeyCaretStart(filesList[0]);
    } else if (this.prevFile) {
      setSelection([this.prevFile]);
    }
  };

  selectRight = () => {
    const {
      hotkeyCaret,
      setSelection,
      filesList,
      setHotkeyCaretStart,
    } = this.filesStore;

    if (!hotkeyCaret) {
      this.selectFirstFile();
      setHotkeyCaretStart(filesList[0]);
    } else if (this.nextFile) {
      setSelection([this.nextFile]);
    }
  };

  multiSelectBottom = () => {
    const {
      selection,
      setSelection,
      hotkeyCaretStart,
      setHotkeyCaretStart,
      hotkeyCaret,
      viewAs,
      setHotkeyCaret,
      deselectFile,
    } = this.filesStore;

    if (!hotkeyCaretStart) {
      setHotkeyCaretStart(hotkeyCaret);
    }
    if (!hotkeyCaret) return this.selectFirstFile();

    if (viewAs === "tile") {
      if (this.nextForTileDown.id === hotkeyCaret.id) return;

      setSelection([
        ...this.selectionsDown,
        ...[hotkeyCaretStart ? hotkeyCaretStart : hotkeyCaret],
      ]);
      setHotkeyCaret(this.nextForTileDown);
    } else if (this.nextFile) {
      if (selection.findIndex((f) => f.id === this.nextFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...selection, ...[this.nextFile]]);
      }
      setHotkeyCaret(this.nextFile);
    }
  };

  multiSelectUpper = () => {
    const {
      selection,
      setSelection,
      hotkeyCaretStart,
      setHotkeyCaretStart,
      hotkeyCaret,
      viewAs,
      setHotkeyCaret,
      deselectFile,
    } = this.filesStore;

    if (!hotkeyCaretStart) {
      setHotkeyCaretStart(hotkeyCaret);
    }
    if (!hotkeyCaret) this.selectFirstFile();

    if (viewAs === "tile") {
      if (this.prevForTileUp.id === hotkeyCaret.id) return;

      setSelection([
        ...this.selectionsUp,
        ...[hotkeyCaretStart ? hotkeyCaretStart : hotkeyCaret],
      ]);
      setHotkeyCaret(this.prevForTileUp);
    } else if (this.prevFile) {
      if (selection.findIndex((f) => f.id === this.prevFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...[this.prevFile], ...selection]);
      }

      setHotkeyCaret(this.prevFile);
    }
  };

  multiSelectRight = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      viewAs,
      setHotkeyCaret,
      deselectFile,
      hotkeyCaretStart,
      filesList,
    } = this.filesStore;

    if (!hotkeyCaret) return this.selectFirstFile();

    const nextFile = this.nextFile;
    if (!nextFile) return;

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) => f.id === hotkeyCaretStart?.id
    );

    const nextCaretIndex = this.caretIndex + 1;
    const nextForTileRight = [];

    let iNext = hotkeyCaretStartIndex;
    if (iNext < nextCaretIndex) {
      while (iNext !== nextCaretIndex + 1) {
        if (filesList[iNext]) nextForTileRight.push(filesList[iNext]);
        iNext++;
      }
    }

    let counterToRight = 0;
    if (this.caretIndex < hotkeyCaretStartIndex) {
      counterToRight = hotkeyCaretStartIndex - this.caretIndex;
    }

    while (counterToRight !== 0) {
      nextForTileRight.push(
        filesList[hotkeyCaretStartIndex - counterToRight + 1]
      );
      counterToRight--;
    }

    if (viewAs === "tile") {
      setSelection(nextForTileRight);
      setHotkeyCaret(nextFile);
    } else if (nextFile) {
      if (selection.findIndex((f) => f.id === nextFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...selection, ...[nextFile]]);
      }

      setHotkeyCaret(nextFile);
    }
  };

  multiSelectLeft = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      viewAs,
      setHotkeyCaret,
      deselectFile,
      filesList,
      hotkeyCaretStart,
    } = this.filesStore;

    if (!hotkeyCaret) return this.selectFirstFile();

    const prevFile = this.prevFile;
    if (!prevFile) return;

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) => f.id === hotkeyCaretStart?.id
    );

    const prevCaretIndex = this.caretIndex - 1;
    const prevForTileLeft = [];

    let iPrev = hotkeyCaretStartIndex;
    if (iPrev > prevCaretIndex) {
      while (iPrev !== prevCaretIndex - 1) {
        if (filesList[iPrev]) prevForTileLeft.push(filesList[iPrev]);
        iPrev--;
      }
    }

    let counterToLeft = 0;
    if (this.caretIndex > hotkeyCaretStartIndex) {
      counterToLeft = this.caretIndex - hotkeyCaretStartIndex;
    }

    while (counterToLeft > 0) {
      prevForTileLeft.push(
        filesList[hotkeyCaretStartIndex + counterToLeft - 1]
      );
      counterToLeft--;
    }

    if (viewAs === "tile") {
      setSelection(prevForTileLeft);
      setHotkeyCaret(prevFile);
    } else if (prevFile) {
      if (selection.findIndex((f) => f.id === prevFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...[prevFile], ...selection]);
      }

      setHotkeyCaret(prevFile);
    }
  };

  moveCaretBottom = () => {
    const { viewAs, setHotkeyCaret } = this.filesStore;

    if (viewAs === "tile") setHotkeyCaret(this.nextForTileDown);
    else if (this.nextFile) setHotkeyCaret(this.nextFile);
  };

  moveCaretUpper = () => {
    const { viewAs, setHotkeyCaret } = this.filesStore;

    if (viewAs === "tile") setHotkeyCaret(this.prevForTileUp);
    else if (this.prevFile) setHotkeyCaret(this.prevFile);
  };

  moveCaretLeft = () => {
    if (this.prevFile) this.filesStore.setHotkeyCaret(this.prevFile);
  };

  moveCaretRight = () => {
    if (this.nextFile) this.filesStore.setHotkeyCaret(this.nextFile);
  };

  openItem = () => {
    const { selection } = this.filesStore;
    selection.length === 1 &&
      !this.dialogsStore.someDialogIsOpen &&
      this.filesActionsStore.openFileAction(selection[0]);
  };

  selectAll = () => {
    const {
      filesList,
      hotkeyCaret,
      setHotkeyCaret,
      setHotkeyCaretStart,
      setSelected,
    } = this.filesStore;

    setSelected("all");
    if (!hotkeyCaret) {
      setHotkeyCaret(filesList[0]);
      setHotkeyCaretStart(filesList[0]);
    }
  };

  get countTilesInRow() {
    const isDesktopView = isDesktop();
    const tileGap = isDesktopView ? 16 : 12;
    const minTileWidth = 220 + tileGap;
    const sectionPadding = isDesktopView ? 24 : 18;

    const body = document.getElementById("section");
    const sectionWidth = body ? body.offsetWidth - sectionPadding : 0;

    return Math.floor(sectionWidth / minTileWidth);
  }

  get division() {
    const { folders } = this.filesStore;
    return folders.length % this.countTilesInRow;
  }

  get countOfMissingFiles() {
    return this.division ? this.countTilesInRow - this.division : 0;
  }

  get caretIsFolder() {
    const { filesList } = this.filesStore;

    if (this.caretIndex !== -1) {
      return filesList[this.caretIndex].isFolder;
    } else {
      console.log("TODO");
      return false;
    }
  }

  get caretIndex() {
    const { filesList, hotkeyCaret } = this.filesStore;
    const caretIndex = filesList.findIndex((f) => f.id === hotkeyCaret.id);

    if (caretIndex !== -1) return caretIndex;
    else {
      console.log("TODO");
      return null;
    }
  }

  get nextFile() {
    const { filesList } = this.filesStore;

    if (this.caretIndex !== -1) {
      const nextCaretIndex = this.caretIndex + 1;
      return filesList[nextCaretIndex];
    } else {
      console.log("TODO");
      return null;
    }
  }

  get nextForTileDown() {
    const { filesList, folders, files } = this.filesStore;

    const nextTileFile = filesList[this.caretIndex + this.countTilesInRow];
    const foldersLength = folders.length;

    let nextForTileDown = nextTileFile
      ? nextTileFile
      : filesList[filesList.length - 1];

    //Next tile

    if (nextForTileDown.isFolder !== this.caretIsFolder) {
      let indexForNextTile =
        this.caretIndex + this.countTilesInRow - this.countOfMissingFiles;

      nextForTileDown =
        foldersLength - this.caretIndex - 1 <= this.division ||
        this.division === 0
          ? filesList[indexForNextTile]
            ? filesList[indexForNextTile]
            : files[0]
          : folders[foldersLength - 1];
    } else if (!nextTileFile) {
      // const pp = filesList.findIndex((f) => f.id === nextForTileDown?.id);
      // if (pp < this.caretIndex + this.countTilesInRow) {
      //   nextForTileDown = hotkeyCaret;
      // }
    }

    return nextForTileDown;
  }

  get prevFile() {
    const { filesList, selection, hotkeyCaret } = this.filesStore;

    if (filesList.length && selection.length && hotkeyCaret) {
      if (this.caretIndex !== -1) {
        const prevCaretIndex = this.caretIndex - 1;
        return filesList[prevCaretIndex];
      } else {
        console.log("TODO");
        return null;
      }
    }
  }

  get prevForTileUp() {
    const { filesList, folders, hotkeyCaret } = this.filesStore;
    const foldersLength = folders.length;

    const prevTileFile = filesList[this.caretIndex - this.countTilesInRow];
    let prevForTileUp = prevTileFile ? prevTileFile : filesList[0];

    if (prevForTileUp.isFolder !== this.caretIsFolder) {
      let indexForPrevTile =
        this.caretIndex - this.countTilesInRow + this.countOfMissingFiles;

      prevForTileUp = filesList[indexForPrevTile]
        ? filesList[indexForPrevTile].isFolder
          ? filesList[indexForPrevTile]
          : folders[foldersLength - 1]
        : folders[foldersLength - 1];
    } else if (!prevTileFile) {
      prevForTileUp = hotkeyCaret;
    }
    return prevForTileUp;
  }

  get selectionsDown() {
    const { filesList, hotkeyCaretStart, viewAs, selection } = this.filesStore;
    const selectionsDown = [];

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) => f.id === hotkeyCaretStart?.id
    );

    const firstSelectionIndex = filesList.findIndex(
      (f) => f.id === selection[0]?.id
    );

    const nextForTileDownIndex = filesList.findIndex(
      (f) => f.id === this.nextForTileDown?.id
    );
    let nextForTileDownItemIndex = nextForTileDownIndex;

    const itemIndexDown =
      hotkeyCaretStartIndex !== -1 &&
      hotkeyCaretStartIndex < firstSelectionIndex
        ? hotkeyCaretStartIndex
        : firstSelectionIndex;

    if (itemIndexDown !== -1 && viewAs === "tile") {
      if (nextForTileDownItemIndex === -1)
        nextForTileDownItemIndex = itemIndexDown + this.countTilesInRow;

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
    }
    return selectionsDown;
  }

  get selectionsUp() {
    const { filesList, viewAs, selection, hotkeyCaretStart } = this.filesStore;
    const selectionsUp = [];

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) => f.id === hotkeyCaretStart?.id
    );

    const firstSelectionIndex = filesList.findIndex(
      (f) => f.id === selection[0]?.id
    );

    const prevForTileUpIndex = filesList.findIndex(
      (f) => f.id === this.prevForTileUp?.id
    );
    let prevForTileUpItemIndex = prevForTileUpIndex;

    const itemIndexUp =
      hotkeyCaretStartIndex !== -1 &&
      hotkeyCaretStartIndex > firstSelectionIndex
        ? hotkeyCaretStartIndex
        : firstSelectionIndex;

    if (itemIndexUp !== -1 && viewAs === "tile") {
      if (prevForTileUpItemIndex === -1)
        prevForTileUpItemIndex = itemIndexUp - this.countTilesInRow;

      if (prevForTileUpItemIndex < itemIndexUp) {
        while (prevForTileUpItemIndex !== itemIndexUp) {
          if (prevForTileUpItemIndex - hotkeyCaretStartIndex <= 0)
            selectionsUp.push(filesList[prevForTileUpItemIndex]);
          prevForTileUpItemIndex++;
        }
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
    return selectionsUp;
  }
}

export default HotkeyStore;
