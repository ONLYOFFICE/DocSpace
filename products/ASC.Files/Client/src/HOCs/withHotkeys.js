import React from "react";
import { useHotkeys } from "react-hotkeys-hook";
import { observer, inject } from "mobx-react";

const withHotkeys = (Component) => {
  const WithHotkeys = (props) => {
    const {
      t,
      setSelected,
      setHotkeyPanelVisible,
      confirmDelete,
      setDeleteDialogVisible,
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
    } = props;

    console.log("props", props.sectionWidth);

    const hotkeysFilter = {
      filter: (ev) =>
        ev.target?.type === "checkbox" || ev.target?.tagName !== "INPUT",
      filterPreventDefault: false,
      enableOnTags: ["INPUT"],
    };

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
    useHotkeys("shift+a", selectAll, hotkeysFilter);

    //Deselect all files and folders
    useHotkeys("shift+n", () => setSelected("none"), hotkeysFilter);

    //Move down without changing selection
    useHotkeys("ctrl+DOWN", moveCaretBottom, hotkeysFilter);

    //Move up without changing selection
    useHotkeys("ctrl+UP", moveCaretUpper, hotkeysFilter);

    //Move left without changing selection
    useHotkeys("ctrl+LEFT", moveCaretLeft, hotkeysFilter);

    //Move right without changing selection
    useHotkeys("ctrl+RIGHT", moveCaretRight, hotkeysFilter);

    //Open item
    useHotkeys("Enter", openItem, hotkeysFilter);

    //Delete selection
    useHotkeys(
      "Delete, shift+3",
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

    //TODO: Undo the last action
    useHotkeys(
      "Ctrl+z",
      () => alert("Undo the last action"),
      hotkeysFilter,
      []
    );

    //TODO: Redo the last undone action
    useHotkeys(
      "Ctrl+Shift+z",
      () => alert("Redo the last undone action"),
      hotkeysFilter,
      []
    );

    //Open hotkeys panel
    useHotkeys(
      "Ctrl+num_divide, Ctrl+/",
      () => setHotkeyPanelVisible(true),
      hotkeysFilter
    );

    return <Component {...props} />;
  };

  return inject(
    ({
      filesStore,
      dialogsStore,
      settingsStore,
      filesActionsStore,
      hotkeyStore,
    }) => {
      const { setSelected } = filesStore;

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
      } = hotkeyStore;

      const { setHotkeyPanelVisible, setDeleteDialogVisible } = dialogsStore;
      const { isAvailableOption, deleteAction } = filesActionsStore;

      return {
        setSelected,

        setHotkeyPanelVisible,
        setDeleteDialogVisible,
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
      };
    }
  )(observer(WithHotkeys));
};

export default withHotkeys;
