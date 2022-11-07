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

  elemOffset = 0;

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

  scrollToCaret = () => {
    const { offsetTop, item } = this.getItemOffset();
    const scroll = document.getElementsByClassName("section-scroll")[0];
    const scrollRect = scroll.getBoundingClientRect();

    if (item && item[0]) {
      const el = item[0];
      const rect = el.getBoundingClientRect();

      if (
        scrollRect.top + scrollRect.height - rect.height > rect.top &&
        scrollRect.top < rect.top + el.offsetHeight
      ) {
        //console.log("element is visible");
      } else {
        scroll.scrollTo(0, offsetTop - scrollRect.height / 2);
        //console.log("element is not visible");
      }
    } else {
      scroll.scrollTo(0, this.elemOffset - scrollRect.height / 2);
    }
  };

  activateHotkeys = (e) => {
    if (
      this.dialogsStore.someDialogIsOpen ||
      (e.target?.tagName === "INPUT" && e.target.type !== "checkbox") ||
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

    const { selection: s, hotkeyCaret, filesList } = this.filesStore;
    const selection = s.length ? s : filesList;

    if (!hotkeyCaret) {
      const scroll = document.getElementsByClassName("section-scroll");
      scroll && scroll[0] && scroll[0].focus();
    }

    if (!hotkeyCaret && selection.length) {
      this.setCaret(selection[0]);
      this.filesStore.setHotkeyCaretStart(selection[0]);
    }

    if (!hotkeyCaret || isDefaultKeys) return e;
  };

  setCaret = (caret) => {
    //TODO: inf-scroll
    // const id = caret.isFolder ? `folder_${caret.id}` : `file_${caret.id}`;
    // const elem = document.getElementById(id);
    // if (!elem) return;

    this.filesStore.setHotkeyCaret(caret);
    this.scrollToCaret();

    const { offsetTop } = this.getItemOffset();
    if (offsetTop) this.elemOffset = offsetTop;
  };

  getItemOffset = () => {
    const { hotkeyCaret, viewAs } = this.filesStore;

    let item = document.getElementsByClassName(
      `${hotkeyCaret.id}_${hotkeyCaret.fileExst}`
    );

    if (viewAs === "table") {
      item = item && item[0]?.getElementsByClassName("table-container_cell");
    }

    if (item && item[0]) {
      const el = item[0];

      const offset = el.closest(".window-item")?.offsetTop;

      const offsetTop = offset
        ? offset
        : viewAs === "tile"
        ? el.parentElement.parentElement.offsetTop
        : el.offsetTop;

      return { offsetTop, item };
    }

    return { offsetTop: null, item: null };
  };

  selectFirstFile = () => {
    const { filesList } = this.filesStore;

    if (filesList.length) {
      // scroll to first element
      const scroll = document.querySelector("#sectionScroll > .scroll-body");
      scroll.scrollTo(0, 0);

      this.filesStore.setSelection([filesList[0]]);
      this.setCaret(filesList[0]);
      this.filesStore.setHotkeyCaretStart(filesList[0]);
    }
  };

  setSelectionWithCaret = (selection) => {
    this.filesStore.setSelection(selection);
    this.setCaret(selection[0]);
    this.filesStore.setHotkeyCaretStart(selection[0]);
  };

  selectFile = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
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
        this.setCaret(selection[0]);
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
      this.setCaret(this.nextForTileDown);
    } else if (this.nextFile) {
      if (selection.findIndex((f) => f.id === this.nextFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...selection, ...[this.nextFile]]);
      }
      this.setCaret(this.nextFile);
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
      this.setCaret(this.prevForTileUp);
    } else if (this.prevFile) {
      if (selection.findIndex((f) => f.id === this.prevFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...[this.prevFile], ...selection]);
      }

      this.setCaret(this.prevFile);
    }
  };

  multiSelectRight = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      viewAs,
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
      this.setCaret(nextFile);
    } else if (nextFile) {
      if (selection.findIndex((f) => f.id === nextFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...selection, ...[nextFile]]);
      }

      this.setCaret(nextFile);
    }
  };

  multiSelectLeft = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      viewAs,
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
      this.setCaret(prevFile);
    } else if (prevFile) {
      if (selection.findIndex((f) => f.id === prevFile.id) !== -1) {
        deselectFile(hotkeyCaret);
      } else {
        setSelection([...[prevFile], ...selection]);
      }

      this.setCaret(prevFile);
    }
  };

  moveCaretBottom = () => {
    const { viewAs } = this.filesStore;

    if (viewAs === "tile") this.setCaret(this.nextForTileDown);
    else if (this.nextFile) this.setCaret(this.nextFile);
  };

  moveCaretUpper = () => {
    const { viewAs } = this.filesStore;

    if (viewAs === "tile") this.setCaret(this.prevForTileUp);
    else if (this.prevFile) this.setCaret(this.prevFile);
  };

  moveCaretLeft = () => {
    if (this.prevFile) this.setCaret(this.prevFile);
  };

  moveCaretRight = () => {
    if (this.nextFile) this.setCaret(this.nextFile);
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
      setHotkeyCaretStart,
      setSelected,
    } = this.filesStore;

    setSelected("all");
    if (!hotkeyCaret) {
      this.setCaret(filesList[0]);
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

    const item = hotkeyCaret
      ? hotkeyCaret
      : selection.length
      ? selection.length === 1
        ? selection[0]
        : selection[selection.length - 1]
      : null;

    const caretIndex = filesList.findIndex(
      (f) => f.id === item?.id && f.isFolder === item?.isFolder
    );

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

    if (nextForTileDown.isFolder === undefined) {
      nextForTileDown.isFolder = !!nextForTileDown.parentId;
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

    if (prevForTileUp.isFolder === undefined) {
      prevForTileUp.isFolder = !!prevForTileUp.parentId;
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
