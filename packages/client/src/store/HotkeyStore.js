import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import { combineUrl } from "@docspace/common/utils";
import { isDesktop } from "@docspace/components/utils/device";
import { makeAutoObservable } from "mobx";
import config from "PACKAGE_FILE";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import { encryptionUploadDialog } from "../helpers/desktop";

class HotkeyStore {
  filesStore;
  dialogsStore;
  settingsStore;
  filesActionsStore;
  treeFoldersStore;
  uploadDataStore;

  constructor(
    filesStore,
    dialogsStore,
    settingsStore,
    filesActionsStore,
    treeFoldersStore,
    uploadDataStore
  ) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.dialogsStore = dialogsStore;
    this.settingsStore = settingsStore;
    this.filesActionsStore = filesActionsStore;
    this.treeFoldersStore = treeFoldersStore;
    this.uploadDataStore = uploadDataStore;
  }

  activateHotkeys = (e) => {
    if (
      this.dialogsStore.someDialogIsOpen ||
      e.target?.tagName === "INPUT" ||
      e.target?.tagName === "TEXTAREA"
    )
      return e;

    const isDefaultKeys =
      ["PageUp", "PageDown", "Home", "End"].indexOf(e.code) > -1;

    if (
      ["Space", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"].indexOf(
        e.code
      ) > -1
    ) {
      e.preventDefault();
    }

    const { selection: s, hotkeyCaret, viewAs, filesList } = this.filesStore;
    const selection = s.length ? s : filesList;

    if (!hotkeyCaret) {
      const scroll = document.getElementsByClassName("section-scroll");
      scroll && scroll[0] && scroll[0].focus();
    }

    if (!hotkeyCaret && selection.length) {
      this.filesStore.setHotkeyCaret(selection[0]);
      this.filesStore.setHotkeyCaretStart(selection[0]);
    }

    if (!hotkeyCaret || isDefaultKeys) return;

    let item = document.getElementsByClassName(
      `${hotkeyCaret.id}_${hotkeyCaret.fileExst}`
    );

    if (viewAs === "table") {
      item = item && item[0]?.getElementsByClassName("table-container_cell");
    }

    if (item && item[0]) {
      const el = item[0];
      const rect = el.getBoundingClientRect();
      const scroll = document.getElementsByClassName("section-scroll")[0];
      const scrollRect = scroll.getBoundingClientRect();

      if (
        scrollRect.top + scrollRect.height - rect.height > rect.top &&
        scrollRect.top < rect.top + el.offsetHeight
      ) {
        //console.log("element is visible");
      } else {
        const offset = el.closest(".window-item")?.offsetTop;
        const offsetTop = offset
          ? offset
          : viewAs === "tile"
          ? el.parentElement.parentElement.offsetTop
          : el.offsetTop;

        scroll.scrollTo(0, offsetTop - scrollRect.height / 2);
        //console.log("element is not visible");
      }
    }
  };

  selectFirstFile = () => {
    const { filesList } = this.filesStore;

    if (filesList.length) {
      this.filesStore.setSelection([filesList[0]]);
      this.filesStore.setHotkeyCaret(filesList[0]);
      this.filesStore.setHotkeyCaretStart(filesList[0]);
    }
  };

  setSelectionWithCaret = (selection) => {
    this.filesStore.setSelection(selection);
    this.filesStore.setHotkeyCaret(selection[0]);
    this.filesStore.setHotkeyCaretStart(selection[0]);
  };

  selectFile = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      setHotkeyCaret,
      setHotkeyCaretStart,
    } = this.filesStore;

    const index = selection.findIndex((f) => f.id === hotkeyCaret?.id);
    if (index !== -1) {
      const newSelection = selection;
      newSelection.splice(index, 1);
      setSelection(newSelection);
    } else if (hotkeyCaret) {
      const newSelection = selection;
      newSelection.push(hotkeyCaret);
      setSelection(newSelection);
      setHotkeyCaretStart(hotkeyCaret);
    } else {
      if (selection.length) {
        setHotkeyCaret(selection[0]);
        setHotkeyCaretStart(selection[0]);
      } else this.selectFirstFile();
    }
  };

  selectBottom = () => {
    const { viewAs, hotkeyCaret, selection } = this.filesStore;

    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();
    else if (viewAs === "tile")
      this.setSelectionWithCaret([this.nextForTileDown]);
    else if (this.nextFile) this.setSelectionWithCaret([this.nextFile]);
  };

  selectUpper = () => {
    const { hotkeyCaret, viewAs, selection } = this.filesStore;

    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();
    else if (viewAs === "tile")
      this.setSelectionWithCaret([this.prevForTileUp]);
    else if (this.prevFile) this.setSelectionWithCaret([this.prevFile]);
  };

  selectLeft = () => {
    const {
      hotkeyCaret,
      filesList,
      setHotkeyCaretStart,
      selection,
      viewAs,
    } = this.filesStore;
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) {
      this.selectFirstFile();

      setHotkeyCaretStart(filesList[0]);
    } else if (this.prevFile) {
      this.setSelectionWithCaret([this.prevFile]);
    }
  };

  selectRight = () => {
    const {
      hotkeyCaret,
      filesList,
      setHotkeyCaretStart,
      selection,
      viewAs,
    } = this.filesStore;
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) {
      this.selectFirstFile();
      setHotkeyCaretStart(filesList[0]);
    } else if (this.nextFile) {
      this.setSelectionWithCaret([this.nextFile]);
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
    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();

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
    if (!hotkeyCaret && !selection.length) this.selectFirstFile();

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
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();

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
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();

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

  goToHomePage = (history) => {
    const { filter, categoryType } = this.filesStore;

    const filterParamsStr = filter.toUrlParams();

    const url = getCategoryUrl(categoryType, filter.folder);

    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `${url}?${filterParamsStr}`
      )
    );
  };

  uploadFile = (isFolder, history, t) => {
    if (isFolder) {
      if (this.treeFoldersStore.isPrivacyFolder) return;
      const folderInput = document.getElementById("customFolderInput");
      folderInput && folderInput.click();
    } else {
      if (this.treeFoldersStore.isPrivacyFolder) {
        encryptionUploadDialog((encryptedFile, encrypted) => {
          encryptedFile.encrypted = encrypted;
          this.goToHomePage(history);
          this.uploadDataStore.startUpload([encryptedFile], null, t);
        });
      } else {
        const fileInput = document.getElementById("customFileInput");
        fileInput && fileInput.click();
      }
    }
  };

  get countTilesInRow() {
    const isDesktopView = isDesktop();
    const tileGap = isDesktopView ? 16 : 14;
    const minTileWidth = 216 + tileGap;
    const sectionPadding = isDesktopView ? 24 : 16;

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
    } else return false;
  }

  get caretIndex() {
    const { filesList, hotkeyCaret, selection } = this.filesStore;
    const id =
      selection.length && selection.length === 1 && !hotkeyCaret
        ? selection[0].id
        : hotkeyCaret?.id;
    const caretIndex = filesList.findIndex((f) => f.id === id);

    if (caretIndex !== -1) return caretIndex;
    else return null;
  }

  get nextFile() {
    const { filesList } = this.filesStore;

    if (this.caretIndex !== -1) {
      const nextCaretIndex = this.caretIndex + 1;
      return filesList[nextCaretIndex];
    } else return null;
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
    const { filesList } = this.filesStore;

    if (this.caretIndex !== -1) {
      const prevCaretIndex = this.caretIndex - 1;
      return filesList[prevCaretIndex];
    } else return null;
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
