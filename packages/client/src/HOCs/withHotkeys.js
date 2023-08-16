import React, { useEffect } from "react";
import { useHotkeys } from "react-hotkeys-hook";
import { observer, inject } from "mobx-react";
import { Events } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";
import throttle from "lodash/throttle";

const withHotkeys = (Component) => {
  const WithHotkeys = (props) => {
    const {
      t,
      history,
      setSelected,
      viewAs,
      setViewAs,
      setHotkeyPanelVisible,
      confirmDelete,
      setDeleteDialogVisible,
      setSelectFileDialogVisible,
      deleteAction,
      isAvailableOption,

      selectFile,
      selectBottom,
      selectUpper,
      selectLeft,
      selectRight,
      multiSelectBottom,
      multiSelectUpper,
      multiSelectRight,
      multiSelectLeft,
      moveCaretBottom,
      moveCaretUpper,
      moveCaretLeft,
      moveCaretRight,
      openItem,
      selectAll,
      activateHotkeys,
      backToParentFolder,

      uploadFile,
      someDialogIsOpen,
      enabledHotkeys,
      mediaViewerIsVisible,

      isFavoritesFolder,
      isRecentFolder,
      isTrashFolder,
      isArchiveFolder,
      isRoomsFolder,

      selection,
      setFavoriteAction,
      filesIsLoading,

      isVisitor,
      deleteRooms,
      archiveRooms,
      isGracePeriod,
      setInviteUsersWarningDialogVisible,

      security,
    } = props;

    const hotkeysFilter = {
      filter: (ev) =>
        ev.target?.type === "checkbox" || ev.target?.tagName !== "INPUT",
      filterPreventDefault: false,
      enableOnTags: ["INPUT"],
      enabled:
        !someDialogIsOpen &&
        enabledHotkeys &&
        !mediaViewerIsVisible &&
        !filesIsLoading,
      // keyup: true,
      // keydown: false,
    };

    const onKeyDown = (e) => activateHotkeys(e);

    const folderWithNoAction =
      isFavoritesFolder ||
      isRecentFolder ||
      isTrashFolder ||
      isArchiveFolder ||
      isRoomsFolder ||
      isVisitor ||
      !security?.Create;

    const onCreate = (extension) => {
      if (folderWithNoAction) return;
      const event = new Event(Events.CREATE);

      const payload = {
        extension: extension,
        id: -1,
      };

      event.payload = payload;

      window.dispatchEvent(event);
    };

    const onCreateRoom = () => {
      if (!isVisitor && isRoomsFolder && security?.Create) {
        if (isGracePeriod) {
          setInviteUsersWarningDialogVisible(true);
          return;
        }

        const event = new Event(Events.ROOM_CREATE);
        window.dispatchEvent(event);
      }
    };

    useEffect(() => {
      const throttledKeyDownEvent = throttle(onKeyDown, 300);

      window.addEventListener("keydown", throttledKeyDownEvent);

      return () =>
        window.removeEventListener("keypress", throttledKeyDownEvent);
    });

    //Select/deselect item
    useHotkeys("x", selectFile);

    useHotkeys(
      "*",
      (e) => {
        if (e.shiftKey || e.ctrlKey) return;

        switch (e.key) {
          case "ArrowDown":
          case "j": {
            return selectBottom();
          }

          case "ArrowUp":
          case "k": {
            return selectUpper();
          }

          case "ArrowRight":
          case "l": {
            return selectRight();
          }

          case "ArrowLeft":
          case "h": {
            return selectLeft();
          }

          default:
            break;
        }
      },
      hotkeysFilter
    );

    // //Select bottom element
    // useHotkeys("j, DOWN", selectBottom, hotkeysFilter);

    // //Select upper item
    // useHotkeys("k, UP", selectUpper, hotkeysFilter);

    // //Select item on the left
    // useHotkeys("h, LEFT", selectLeft, hotkeysFilter);

    // //Select item on the right
    // useHotkeys("l, RIGHT", selectRight, hotkeysFilter);

    //Expand Selection DOWN
    useHotkeys("shift+DOWN", multiSelectBottom, hotkeysFilter);

    //Expand Selection UP
    useHotkeys("shift+UP", multiSelectUpper, hotkeysFilter);

    //Expand Selection RIGHT
    useHotkeys("shift+RIGHT", () => multiSelectRight(), hotkeysFilter);

    //Expand Selection LEFT
    useHotkeys("shift+LEFT", () => multiSelectLeft(), hotkeysFilter);

    //Select all files and folders
    useHotkeys("shift+a, ctrl+a", selectAll, hotkeysFilter);

    //Deselect all files and folders
    useHotkeys("shift+n, ESC", () => setSelected("none"), hotkeysFilter);

    //Move down without changing selection
    useHotkeys("ctrl+DOWN, command+DOWN", moveCaretBottom, hotkeysFilter);

    //Move up without changing selection
    useHotkeys("ctrl+UP, command+UP", moveCaretUpper, hotkeysFilter);

    //Move left without changing selection
    useHotkeys("ctrl+LEFT, command+LEFT", moveCaretLeft, hotkeysFilter);

    //Move right without changing selection
    useHotkeys("ctrl+RIGHT, command+RIGHT", moveCaretRight, hotkeysFilter);

    //Open item
    useHotkeys("Enter", openItem, hotkeysFilter);

    //Back to parent folder
    useHotkeys("Backspace", backToParentFolder, hotkeysFilter);

    //Change viewAs
    useHotkeys(
      "v",
      () => (viewAs === "tile" ? setViewAs("table") : setViewAs("tile")),
      hotkeysFilter
    );

    //Crete document
    useHotkeys("Shift+d", () => onCreate("docx"), {
      ...hotkeysFilter,
      ...{ keyup: true },
    });

    //Crete spreadsheet
    useHotkeys("Shift+s", () => onCreate("xlsx"), {
      ...hotkeysFilter,
      ...{ keyup: true },
    });

    //Crete presentation
    useHotkeys("Shift+p", () => onCreate("pptx"), {
      ...hotkeysFilter,
      ...{ keyup: true },
    });

    //Crete form template
    useHotkeys("Shift+o", () => onCreate("docxf"), {
      ...hotkeysFilter,
      ...{ keyup: true },
    });

    //Crete form template from file
    useHotkeys(
      "Alt+Shift+o",
      () => {
        if (folderWithNoAction) return;
        setSelectFileDialogVisible(true);
      },

      hotkeysFilter
    );

    //Crete folder
    useHotkeys("Shift+f", () => onCreate(null), {
      ...hotkeysFilter,
      ...{ keyup: true },
    });

    //Crete room
    useHotkeys("Shift+r", () => onCreateRoom(), {
      ...hotkeysFilter,
      ...{ keyup: true },
    });

    //Delete selection
    useHotkeys(
      "delete, shift+3, command+delete, command+Backspace",
      () => {
        if (isArchiveFolder) {
          isAvailableOption("unarchive") && deleteRooms(t);
          return;
        }

        if (isRoomsFolder) {
          isAvailableOption("archive") && archiveRooms("archive");
          return;
        }

        if (isAvailableOption("delete")) {
          if (isRecentFolder) return;

          if (isFavoritesFolder) {
            const items = selection.map((item) => item.id);

            setFavoriteAction("remove", items)
              .then(() => toastr.success(t("RemovedFromFavorites")))
              .catch((err) => toastr.error(err));

            return;
          }

          if (confirmDelete) {
            setDeleteDialogVisible(true);
          } else {
            const translations = {
              deleteOperation: t("Translations:DeleteOperation"),
              deleteFromTrash: t("Translations:DeleteFromTrash"),
              deleteSelectedElem: t("Translations:DeleteSelectedElem"),
              FileRemoved: t("Files:FileRemoved"),
              FolderRemoved: t("Files:FolderRemoved"),
            };
            deleteAction(translations).catch((err) => toastr.error(err));
          }
        }
      },
      hotkeysFilter,
      [confirmDelete]
    );

    // //TODO: Undo the last action
    // useHotkeys(
    //   "Ctrl+z, command+z",
    //   () => alert("Undo the last action"),
    //   hotkeysFilter,
    //   []
    // );

    // //TODO: Redo the last undone action
    // useHotkeys(
    //   "Ctrl+Shift+z, command+Shift+z",
    //   () => alert("Redo the last undone action"),
    //   hotkeysFilter,
    //   []
    // );

    //Open hotkeys panel
    useHotkeys(
      "Ctrl+num_divide, Ctrl+/, command+/",
      () => setHotkeyPanelVisible(true),
      hotkeysFilter
    );

    //Upload file
    useHotkeys(
      "Shift+u",
      () => {
        if (folderWithNoAction) return;
        uploadFile(false, history, t);
      },

      hotkeysFilter
    );

    //Upload folder
    useHotkeys(
      "Shift+i",
      () => {
        if (folderWithNoAction) return;
        uploadFile(true);
      },

      hotkeysFilter
    );

    return <Component {...props} />;
  };

  return inject(
    ({
      auth,
      filesStore,
      dialogsStore,
      settingsStore,
      filesActionsStore,
      hotkeyStore,
      mediaViewerDataStore,
      treeFoldersStore,
      selectedFolderStore,
    }) => {
      const {
        setSelected,
        viewAs,
        setViewAs,
        enabledHotkeys,
        selection,
        filesIsLoading,
      } = filesStore;

      const {
        selectFile,
        selectBottom,
        selectUpper,
        selectLeft,
        selectRight,
        multiSelectBottom,
        multiSelectUpper,
        multiSelectRight,
        multiSelectLeft,
        moveCaretBottom,
        moveCaretUpper,
        moveCaretLeft,
        moveCaretRight,
        openItem,
        selectAll,
        activateHotkeys,
        uploadFile,
      } = hotkeyStore;

      const {
        setDeleteDialogVisible,
        setSelectFileDialogVisible,
        someDialogIsOpen,
        setInviteUsersWarningDialogVisible,
      } = dialogsStore;
      const {
        isAvailableOption,
        deleteAction,
        backToParentFolder,
        setFavoriteAction,
        deleteRooms,
        archiveRooms,
      } = filesActionsStore;

      const { visible: mediaViewerIsVisible } = mediaViewerDataStore;
      const { setHotkeyPanelVisible } = auth.settingsStore;
      const { isVisitor } = auth.userStore.user;
      const { isGracePeriod } = auth.currentTariffStatusStore;

      const {
        isFavoritesFolder,
        isRecentFolder,
        isTrashFolder,
        isArchiveFolder,
        isRoomsFolder,
      } = treeFoldersStore;

      const security = selectedFolderStore.security;

      return {
        setSelected,
        viewAs,
        setViewAs,

        setHotkeyPanelVisible,
        setDeleteDialogVisible,
        setSelectFileDialogVisible,
        confirmDelete: settingsStore.confirmDelete,
        deleteAction,
        isAvailableOption,

        selectFile,
        selectBottom,
        selectUpper,
        selectLeft,
        selectRight,
        multiSelectBottom,
        multiSelectUpper,
        multiSelectRight,
        multiSelectLeft,
        moveCaretBottom,
        moveCaretUpper,
        moveCaretLeft,
        moveCaretRight,
        openItem,
        selectAll,
        activateHotkeys,
        backToParentFolder,

        uploadFile,
        someDialogIsOpen,
        enabledHotkeys,
        mediaViewerIsVisible,

        isFavoritesFolder,
        isRecentFolder,
        isTrashFolder,
        isArchiveFolder,
        isRoomsFolder,

        selection,
        setFavoriteAction,
        filesIsLoading,

        isVisitor,
        deleteRooms,
        archiveRooms,

        isGracePeriod,
        setInviteUsersWarningDialogVisible,

        security,
      };
    }
  )(observer(WithHotkeys));
};

export default withHotkeys;
