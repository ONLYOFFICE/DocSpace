import React, { useEffect } from "react";
import { useHotkeys } from "react-hotkeys-hook";
import { observer, inject } from "mobx-react";
import { FileAction } from "@appserver/common/constants";

const withHotkeys = (Component) => {
  const WithHotkeys = (props) => {
    const {
      t,
      history,
      setSelected,
      viewAs,
      setViewAs,
      setAction,
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

      hideArticle,
      uploadFile,
      someDialogIsOpen,
    } = props;

    const hotkeysFilter = {
      filter: (ev) =>
        ev.target?.type === "checkbox" || ev.target?.tagName !== "INPUT",
      filterPreventDefault: false,
      enableOnTags: ["INPUT"],
      enabled: !someDialogIsOpen,
    };

    const onKeyDown = (e) => activateHotkeys(e);

    useEffect(() => {
      window.addEventListener("keydown", onKeyDown);

      return () => window.removeEventListener("keypress", onKeyDown);
    });

    //Select/deselect item
    useHotkeys("x", selectFile);

    //Select bottom element
    useHotkeys("j, DOWN", selectBottom, hotkeysFilter);

    //Select upper item
    useHotkeys("k, UP", selectUpper, hotkeysFilter);

    //Select item on the left
    useHotkeys("h, LEFT", selectLeft, hotkeysFilter);

    //Select item on the right
    useHotkeys("l, RIGHT", selectRight, hotkeysFilter);

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
    useHotkeys(
      "Shift+d",
      () => setAction({ type: FileAction.Create, extension: "docx", id: -1 }),
      hotkeysFilter
    );

    //Crete spreadsheet
    useHotkeys(
      "Shift+s",
      () => setAction({ type: FileAction.Create, extension: "xlsx", id: -1 }),
      hotkeysFilter
    );

    //Crete presentation
    useHotkeys(
      "Shift+p",
      () => setAction({ type: FileAction.Create, extension: "pptx", id: -1 }),
      hotkeysFilter
    );

    //Crete form template
    useHotkeys(
      "Shift+o",
      () => setAction({ type: FileAction.Create, extension: "docxf", id: -1 }),
      hotkeysFilter
    );

    //Crete form template from file
    useHotkeys(
      "Alt+Shift+o",
      () => {
        hideArticle();
        setSelectFileDialogVisible(true);
      },
      hotkeysFilter
    );

    //Crete folder
    useHotkeys(
      "Shift+f",
      () => setAction({ type: FileAction.Create, id: -1 }),
      hotkeysFilter
    );

    //Delete selection
    useHotkeys(
      "delete, shift+3, command+delete, command+Backspace",
      () => {
        if (isAvailableOption("delete")) {
          if (confirmDelete) setDeleteDialogVisible(true);
          else {
            const translations = {
              deleteOperation: t("Translations:DeleteOperation"),
              deleteFromTrash: t("Translations:DeleteFromTrash"),
              deleteSelectedElem: t("Translations:DeleteSelectedElem"),
              FileRemoved: t("Home:FileRemoved"),
              FolderRemoved: t("Home:FolderRemoved"),
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
    useHotkeys("Shift+u", () => uploadFile(false, history, t), hotkeysFilter);

    //Upload folder
    useHotkeys("Shift+i", () => uploadFile(true), hotkeysFilter);

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
    }) => {
      const { hideArticle } = auth.settingsStore;
      const { setSelected, viewAs, setViewAs, fileActionStore } = filesStore;
      const { setAction } = fileActionStore;

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
        setHotkeyPanelVisible,
        setDeleteDialogVisible,
        setSelectFileDialogVisible,
        someDialogIsOpen,
      } = dialogsStore;
      const {
        isAvailableOption,
        deleteAction,
        backToParentFolder,
      } = filesActionsStore;

      return {
        setSelected,
        viewAs,
        setViewAs,
        setAction,

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

        hideArticle,
        uploadFile,
        someDialogIsOpen,
      };
    }
  )(observer(WithHotkeys));
};

export default withHotkeys;
