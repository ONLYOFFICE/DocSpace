import { combineUrl } from "@docspace/common/utils";
import { isDesktop } from "@docspace/components/utils/device";
import { makeAutoObservable } from "mobx";
import config from "PACKAGE_FILE";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import toastr from "@docspace/components/toast/toastr";
import { RoomsType } from "@docspace/common/constants";
import { encryptionUploadDialog } from "../helpers/desktop";
import getFilesFromEvent from "@docspace/components/drag-and-drop/get-files-from-event";

class HotkeyStore {
  filesStore;
  dialogsStore;
  settingsStore;
  filesActionsStore;
  treeFoldersStore;
  uploadDataStore;
  selectedFolderStore;

  elemOffset = 0;
  hotkeysClipboardAction = null;

  constructor(
    filesStore,
    dialogsStore,
    settingsStore,
    filesActionsStore,
    treeFoldersStore,
    uploadDataStore,
    selectedFolderStore
  ) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.dialogsStore = dialogsStore;
    this.settingsStore = settingsStore;
    this.filesActionsStore = filesActionsStore;
    this.treeFoldersStore = treeFoldersStore;
    this.uploadDataStore = uploadDataStore;
    this.selectedFolderStore = selectedFolderStore;
  }

  scrollToCaret = () => {
    const { offsetTop, item } = this.getItemOffset();
    const scroll = document.getElementsByClassName("section-scroll")[0];
    const scrollRect = scroll?.getBoundingClientRect();

    if (item && item[0]) {
      const el = item[0];
      const rect = el.getBoundingClientRect();

      const rectHeight =
        this.filesStore.viewAs === "table" ? rect.height * 2 : rect.height;

      if (
        scrollRect.top + scrollRect.height - rect.height > rect.top &&
        scrollRect.top < rect.top + el.offsetHeight - rectHeight
      ) {
        // console.log("element is visible");
      } else {
        scroll.scrollTo(0, offsetTop - scrollRect.height / 2);
        // console.log("element is not visible");
      }
    } else {
      scroll?.scrollTo(0, this.elemOffset - scrollRect.height / 2);
    }
  };

  activateHotkeys = (e) => {
    const infiniteLoaderComponent = document.getElementsByClassName(
      "ReactVirtualized__List"
    )[0];

    if (infiniteLoaderComponent) {
      infiniteLoaderComponent.tabIndex = -1;
    }

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
      this.setCaret(selection[0], !(e.ctrlKey || e.metaKey || e.shiftKey));
      this.filesStore.setHotkeyCaretStart(selection[0]);
    }

    if (!hotkeyCaret || isDefaultKeys) return e;
  };

  setCaret = (caret, withScroll = true) => {
    //TODO: inf-scroll
    // const id = caret.isFolder ? `folder_${caret.id}` : `file_${caret.id}`;
    // const elem = document.getElementById(id);
    // if (!elem) return;

    this.filesStore.setHotkeyCaret(caret);
    withScroll && this.scrollToCaret();

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
      const scroll = document.querySelector(
        "#sectionScroll > .scroll-wrapper > .scroller"
      );
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
    const { selection, setSelection, hotkeyCaret, setHotkeyCaretStart } =
      this.filesStore;

    const index = selection.findIndex(
      (f) => f.id === hotkeyCaret?.id && f.isFolder === hotkeyCaret?.isFolder
    );
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
    const { hotkeyCaret, filesList, setHotkeyCaretStart, selection, viewAs } =
      this.filesStore;
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) {
      this.selectFirstFile();

      setHotkeyCaretStart(filesList[0]);
    } else if (this.prevFile) {
      this.setSelectionWithCaret([this.prevFile]);
    }
  };

  selectRight = () => {
    const { hotkeyCaret, filesList, setHotkeyCaretStart, selection, viewAs } =
      this.filesStore;
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
      filesList,
    } = this.filesStore;

    if (!hotkeyCaretStart) {
      setHotkeyCaretStart(hotkeyCaret);
    }
    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();

    if (viewAs === "tile") {
      if (
        this.nextForTileDown.id === hotkeyCaret.id &&
        this.nextForTileDown.isFolder === hotkeyCaret.isFolder
      )
        return;

      setSelection(this.selectionsDown);
      this.setCaret(this.nextForTileDown);
    } else if (this.nextFile) {
      if (selection.findIndex((f) => f.id === this.nextFile.id) !== -1) {
        const startIndex = filesList.findIndex(
          (f) =>
            f.id === hotkeyCaretStart.id &&
            f.isFolder === hotkeyCaretStart.isFolder
        );

        if (startIndex > this.caretIndex) {
          deselectFile(hotkeyCaret);
        }
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
      filesList,
    } = this.filesStore;

    if (!hotkeyCaretStart) {
      setHotkeyCaretStart(hotkeyCaret);
    }
    if (!hotkeyCaret && !selection.length) this.selectFirstFile();

    if (viewAs === "tile") {
      if (
        this.prevForTileUp.id === hotkeyCaret.id &&
        this.prevForTileUp.isFolder === hotkeyCaret.isFolder
      )
        return;

      setSelection(this.selectionsUp);
      this.setCaret(this.prevForTileUp);
    } else if (this.prevFile) {
      if (
        selection.findIndex(
          (f) =>
            f.id === this.prevFile.id && f.isFolder === this.prevFile.isFolder
        ) !== -1
      ) {
        const startIndex = filesList.findIndex(
          (f) =>
            f.id === hotkeyCaretStart.id &&
            f.isFolder === hotkeyCaretStart.isFolder
        );

        if (startIndex < this.caretIndex) {
          deselectFile(hotkeyCaret);
        }
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
      hotkeyCaretStart,
      filesList,
    } = this.filesStore;
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();

    const nextFile = this.nextFile;
    if (!nextFile) return;

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) =>
        f.id === hotkeyCaretStart?.id &&
        f.isFolder === hotkeyCaretStart?.isFolder
    );

    const nextCaretIndex = this.caretIndex + 1;
    let nextForTileRight = selection;

    let iNext = hotkeyCaretStartIndex;
    if (iNext < nextCaretIndex) {
      while (iNext !== nextCaretIndex + 1) {
        if (filesList[iNext]) {
          if (
            nextForTileRight.findIndex(
              (f) =>
                f.id === filesList[iNext].id &&
                f.isFolder === filesList[iNext].isFolder
            ) !== -1
          ) {
            nextForTileRight.filter(
              (f) =>
                f.id === filesList[iNext].id &&
                f.isFolder === filesList[iNext].isFolder
            );
          } else {
            nextForTileRight.push(filesList[iNext]);
          }
        }
        iNext++;
      }
    }

    if (this.caretIndex < hotkeyCaretStartIndex) {
      const idx = nextForTileRight.findIndex(
        (f) => f.id === hotkeyCaret.id && f.isFolder === hotkeyCaret.isFolder
      );
      nextForTileRight = nextForTileRight.filter((_, index) => index !== idx);
    }

    setSelection(nextForTileRight);

    this.setCaret(nextFile);
  };

  multiSelectLeft = () => {
    const {
      selection,
      setSelection,
      hotkeyCaret,
      viewAs,
      filesList,
      hotkeyCaretStart,
    } = this.filesStore;
    if (viewAs !== "tile") return;

    if (!hotkeyCaret && !selection.length) return this.selectFirstFile();

    const prevFile = this.prevFile;
    if (!prevFile) return;

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) =>
        f.id === hotkeyCaretStart?.id &&
        f.isFolder === hotkeyCaretStart?.isFolder
    );

    const prevCaretIndex = this.caretIndex - 1;
    let prevForTileLeft = selection;

    let iPrev = hotkeyCaretStartIndex;
    if (iPrev > prevCaretIndex) {
      while (iPrev !== prevCaretIndex - 1) {
        if (filesList[iPrev]) {
          if (
            prevForTileLeft.findIndex(
              (f) =>
                f.id === filesList[iPrev].id &&
                f.isFolder === filesList[iPrev].isFolder
            ) !== -1
          ) {
            prevForTileLeft.filter(
              (f) =>
                f.id === filesList[iPrev].id &&
                f.isFolder === filesList[iPrev].isFolder
            );
          } else {
            prevForTileLeft.push(filesList[iPrev]);
          }
        }
        iPrev--;
      }
    }

    if (this.caretIndex > hotkeyCaretStartIndex) {
      const idx = prevForTileLeft.findIndex(
        (f) => f.id === hotkeyCaret.id && f.isFolder === hotkeyCaret.isFolder
      );
      prevForTileLeft = prevForTileLeft.filter((_, index) => index !== idx);
    }

    setSelection(prevForTileLeft);
    this.setCaret(prevFile);
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
    const { filesList, hotkeyCaret, setHotkeyCaretStart, setSelected } =
      this.filesStore;

    setSelected("all");
    if (!hotkeyCaret) {
      this.setCaret(filesList[0]);
      setHotkeyCaretStart(filesList[0]);
    }
  };

  goToHomePage = (navigate) => {
    const { filter, categoryType } = this.filesStore;

    const filterParamsStr = filter.toUrlParams();

    const url = getCategoryUrl(categoryType, filter.folder);

    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `${url}?${filterParamsStr}`
      )
    );
  };

  uploadFile = (isFolder, navigate, t) => {
    if (isFolder) {
      if (this.treeFoldersStore.isPrivacyFolder) return;
      const folderInput = document.getElementById("customFolderInput");
      folderInput && folderInput.click();
    } else {
      if (this.treeFoldersStore.isPrivacyFolder) {
        encryptionUploadDialog((encryptedFile, encrypted) => {
          encryptedFile.encrypted = encrypted;
          this.goToHomePage(navigate);
          this.uploadDataStore.startUpload([encryptedFile], null, t);
        });
      } else {
        const fileInput = document.getElementById("customFileInput");
        fileInput && fileInput.click();
      }
    }
  };

  copyToClipboard = (t, isCut) => {
    const { selection, setHotkeysClipboard } = this.filesStore;

    const canCopy = selection.every((s) => s.security?.Copy);
    const canMove = selection.every((s) => s.security?.Move);

    if (!canCopy || (isCut && !canMove) || !selection.length) return;

    setHotkeysClipboard();
    this.hotkeysClipboardAction = isCut ? "move" : "copy";

    const copyText = `${t("AddedToClipboard")}: ${selection.length}`;
    toastr.success(copyText);
  };

  moveFilesFromClipboard = (t) => {
    let fileIds = [];
    let folderIds = [];

    const { id: selectedItemId, roomType, security } = this.selectedFolderStore;
    const { activeFiles, activeFolders, hotkeysClipboard } = this.filesStore;
    const { checkFileConflicts, setSelectedItems, setConflictDialogData } =
      this.filesActionsStore;
    const { itemOperationToFolder, clearActiveOperations } =
      this.uploadDataStore;

    const isCopy = this.hotkeysClipboardAction === "copy";
    const selections = isCopy
      ? hotkeysClipboard
      : hotkeysClipboard.filter((f) => f && !f?.isEditing);

    if (!selections.length) return;

    if (!security.CopyTo || !security.MoveTo) return;

    const isPublic = roomType === RoomsType.PublicRoom;

    for (let item of selections) {
      if (item.fileExst || item.contentLength) {
        const fileInAction = activeFiles.includes(item.id);
        !fileInAction && fileIds.push(item.id);
      } else if (item.id === selectedItemId) {
        toastr.error(t("MoveToFolderMessage"));
      } else {
        const folderInAction = activeFolders.includes(item.id);

        !folderInAction && folderIds.push(item.id);
      }
    }

    if (folderIds.length || fileIds.length) {
      const operationData = {
        destFolderId: selectedItemId,
        folderIds,
        fileIds,
        deleteAfter: false,
        isCopy,
        translations: {
          copy: t("Common:CopyOperation"),
          move: t("Translations:MoveToOperation"),
        },
      };

      if (isPublic) {
        this.dialogsStore.setMoveToPublicRoomVisible(true, operationData);
        return;
      }

      const fileTitle = hotkeysClipboard.find((f) => f.title)?.title;
      setSelectedItems(fileTitle, hotkeysClipboard.length);
      checkFileConflicts(selectedItemId, folderIds, fileIds)
        .then(async (conflicts) => {
          if (conflicts.length) {
            setConflictDialogData(conflicts, operationData);
          } else {
            if (!isCopy) this.filesStore.setMovingInProgress(!isCopy);
            await itemOperationToFolder(operationData);
          }
        })
        .catch((e) => {
          toastr.error(e);
          clearActiveOperations(fileIds, folderIds);
        })
        .finally(() => {
          this.filesStore.setHotkeysClipboard([]);
        });
    } else {
      toastr.error(t("Common:ErrorEmptyList"));
    }
  };

  uploadClipboardFiles = async (t, event) => {
    const { uploadEmptyFolders } = this.filesActionsStore;
    const { startUpload } = this.uploadDataStore;
    const currentFolderId = this.selectedFolderStore.id;

    if (this.filesStore.hotkeysClipboard.length) {
      return this.moveFilesFromClipboard(t);
    }

    const files = await getFilesFromEvent(event);

    const emptyFolders = files.filter((f) => f.isEmptyDirectory);

    if (emptyFolders.length > 0) {
      uploadEmptyFolders(emptyFolders, currentFolderId).then(() => {
        const onlyFiles = files.filter((f) => !f.isEmptyDirectory);
        if (onlyFiles.length > 0) startUpload(onlyFiles, currentFolderId, t);
      });
    } else {
      startUpload(files, currentFolderId, t);
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
    let selectionsDown = JSON.parse(JSON.stringify(selection));

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) =>
        f.id === hotkeyCaretStart?.id &&
        f.isFolder === hotkeyCaretStart?.isFolder
    );

    const firstSelectionIndex = filesList.findIndex(
      (f) => f.id === selection[0]?.id && f.isFolder === selection[0]?.isFolder
    );

    const nextForTileDownIndex = filesList.findIndex(
      (f) =>
        f.id === this.nextForTileDown?.id &&
        f.isFolder === this.nextForTileDown?.isFolder
    );

    let nextForTileDownItemIndex = nextForTileDownIndex;

    const itemIndexDown =
      hotkeyCaretStartIndex !== -1 &&
      hotkeyCaretStartIndex < firstSelectionIndex
        ? hotkeyCaretStartIndex
        : firstSelectionIndex;

    if (itemIndexDown !== -1 && viewAs === "tile") {
      if (nextForTileDownItemIndex === -1) {
        nextForTileDownItemIndex = itemIndexDown + this.countTilesInRow;
      }

      let itemIndex = this.caretIndex;

      while (itemIndex !== nextForTileDownItemIndex) {
        const fileIndex = selectionsDown.findIndex(
          (f) =>
            f.id === filesList[itemIndex].id &&
            f.isFolder === filesList[itemIndex].isFolder
        );

        if (fileIndex === -1) {
          selectionsDown.push(filesList[itemIndex]);
        } else {
          if (hotkeyCaretStartIndex > itemIndex) {
            selectionsDown = selectionsDown.filter(
              (_, index) => index !== fileIndex
            );
          }
        }

        itemIndex++;
      }

      if (
        selectionsDown.findIndex(
          (f) =>
            f.id === this.nextForTileDown.id &&
            f.isFolder === this.nextForTileDown.isFolder
        ) === -1
      ) {
        selectionsDown.push(this.nextForTileDown);
      }
    }

    return selectionsDown;
  }

  get selectionsUp() {
    const { filesList, viewAs, selection, hotkeyCaretStart } = this.filesStore;

    let selectionsUp = JSON.parse(JSON.stringify(selection));

    const hotkeyCaretStartIndex = filesList.findIndex(
      (f) =>
        f.id === hotkeyCaretStart?.id &&
        f.isFolder === hotkeyCaretStart?.isFolder
    );

    const firstSelectionIndex = filesList.findIndex(
      (f) => f.id === selection[0]?.id && f.isFolder === selection[0]?.isFolder
    );

    const prevForTileUpIndex = filesList.findIndex(
      (f) =>
        f.id === this.prevForTileUp?.id &&
        f.isFolder === this.prevForTileUp?.isFolder
    );
    let prevForTileUpItemIndex = prevForTileUpIndex;

    const itemIndexUp =
      hotkeyCaretStartIndex !== -1 &&
      hotkeyCaretStartIndex > firstSelectionIndex
        ? hotkeyCaretStartIndex
        : firstSelectionIndex;

    if (itemIndexUp !== -1 && viewAs === "tile") {
      if (prevForTileUpItemIndex === -1) {
        prevForTileUpItemIndex = itemIndexUp - this.countTilesInRow;
      }

      let itemIndex = this.caretIndex;

      while (itemIndex !== prevForTileUpItemIndex) {
        const fileIndex = selectionsUp.findIndex(
          (f) =>
            f.id === filesList[itemIndex].id &&
            f.isFolder === filesList[itemIndex].isFolder
        );

        if (fileIndex === -1) {
          selectionsUp.push(filesList[itemIndex]);
        } else {
          if (hotkeyCaretStartIndex < itemIndex) {
            selectionsUp = selectionsUp.filter(
              (_, index) => index !== fileIndex
            );
          }
        }

        itemIndex--;
      }

      if (
        selectionsUp.findIndex(
          (f) =>
            f.id === this.prevForTileUp.id &&
            f.isFolder === this.prevForTileUp.isFolder
        ) === -1
      ) {
        selectionsUp.push(this.prevForTileUp);
      }
    }

    return selectionsUp;
  }
}

export default HotkeyStore;
