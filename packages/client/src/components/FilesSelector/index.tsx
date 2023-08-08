import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

// @ts-ignore
import Loaders from "@docspace/common/components/Loaders";
import { FolderType } from "@docspace/common/constants";

import Aside from "@docspace/components/aside";
import Backdrop from "@docspace/components/backdrop";
import Selector from "@docspace/components/selector";
// @ts-ignore
import toastr from "@docspace/components/toast/toastr";

import EmptyScreenFilterAltSvgUrl from "PUBLIC_DIR/images/empty_screen_filter_alt.svg?url";
import EmptyScreenFilterAltDarkSvgUrl from "PUBLIC_DIR/images/empty_screen_filter_alt_dark.svg?url";
import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";
import EmptyScreenAltSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_alt_dark.svg?url";

import {
  BreadCrumb,
  FilesSelectorProps,
  Item,
  Security,
} from "./FilesSelector.types";

import useRootHelper from "./helpers/useRootHelper";
import useRoomsHelper from "./helpers/useRoomsHelper";
import useLoadersHelper from "./helpers/useLoadersHelper";
import useFilesHelper from "./helpers/useFilesHelper";
import { getAcceptButtonLabel, getHeaderLabel, getIsDisabled } from "./utils";

const FilesSelector = ({
  isPanelVisible = false,
  withoutBasicSelection = false,
  withoutImmediatelyClose = false,
  isThirdParty = false,
  isEditorDialog = false,

  filterParam,

  onClose,

  isMove,
  isCopy,
  isRestoreAll,

  currentFolderId,
  fromFolderId,
  parentId,
  rootFolderType,

  treeFolders,

  theme,

  selection,
  disabledItems,
  isFolderActions,
  setIsFolderActions,
  setConflictDialogData,
  checkFileConflicts,
  itemOperationToFolder,
  clearActiveOperations,
  setMovingInProgress,
  setMoveToPanelVisible,
  setCopyPanelVisible,
  setRestoreAllPanelVisible,

  onSelectFolder,
  onSetBaseFolderPath,
  onSetNewFolderPath,
  onSelectTreeNode,
  onSave,
  onSelectFile,

  withFooterInput,
  withFooterCheckbox,
  footerInputHeader,
  currentFooterInputValue,
  footerCheckboxLabel,

  descriptionText,
  setSelectedItems,
}: FilesSelectorProps) => {
  const { t } = useTranslation(["Files", "Common", "Translations"]);

  const [breadCrumbs, setBreadCrumbs] = React.useState<BreadCrumb[]>([]);
  const [items, setItems] = React.useState<Item[] | null>(null);

  const [selectedItemType, setSelectedItemType] = React.useState<
    "rooms" | "files" | undefined
  >(undefined);
  const [selectedItemId, setSelectedItemId] = React.useState<
    number | string | undefined
  >(undefined);
  const [selectedItemSecurity, setSelectedItemSecurity] = React.useState<
    Security | undefined
  >(undefined);
  const [selectedTreeNode, setSelectedTreeNode] = React.useState(null);
  const [selectedFileInfo, setSelectedFileInfo] = React.useState<{
    id: number | string;
    title: string;
    path?: string[];
  } | null>(null);

  const [total, setTotal] = React.useState<number>(0);
  const [hasNextPage, setHasNextPage] = React.useState<boolean>(false);

  const [searchValue, setSearchValue] = React.useState<string>("");

  const [isRequestRunning, setIsRequestRunning] =
    React.useState<boolean>(false);

  const {
    setIsBreadCrumbsLoading,
    isNextPageLoading,
    setIsNextPageLoading,
    isFirstLoad,
    setIsFirstLoad,
    showBreadCrumbsLoader,
    showLoader,
  } = useLoadersHelper({ items });

  const { isRoot, setIsRoot, getRootData } = useRootHelper({
    setIsBreadCrumbsLoading,
    setBreadCrumbs,
    setTotal,
    setItems,
    treeFolders,
    setHasNextPage,
    setIsNextPageLoading,
  });

  const { getRoomList } = useRoomsHelper({
    setIsBreadCrumbsLoading,
    setBreadCrumbs,
    setIsNextPageLoading,
    setHasNextPage,
    setTotal,
    setItems,
    isFirstLoad,
    setIsRoot,
    searchValue,
  });

  const { getFileList } = useFilesHelper({
    setIsBreadCrumbsLoading,
    setBreadCrumbs,
    setIsNextPageLoading,
    setHasNextPage,
    setTotal,
    setItems,
    selectedItemId,
    isFirstLoad,
    setIsRoot,
    searchValue,
    disabledItems,
    setSelectedItemSecurity,
    isThirdParty,
    onSelectTreeNode,
    setSelectedTreeNode,
    filterParam,
  });

  const onSelectAction = (item: Item) => {
    if (item.isFolder) {
      setIsFirstLoad(true);
      setItems(null);
      setBreadCrumbs((value) => [
        ...value,
        {
          label: item.label,
          id: item.id,
          isRoom:
            item.parentId === 0 && item.rootFolderType === FolderType.Rooms,
        },
      ]);
      setSelectedItemId(item.id);
      setSearchValue("");

      if (item.parentId === 0 && item.rootFolderType === FolderType.Rooms) {
        setSelectedItemType("rooms");
        getRoomList(0, false, null);
      } else {
        setSelectedItemType("files");
        getFileList(0, item.id, false, null);
      }
    } else {
      setSelectedFileInfo({ id: item.id, title: item.title });
    }
  };

  React.useEffect(() => {
    if (!withoutBasicSelection) {
      onSelectFolder && onSelectFolder(currentFolderId);
      onSetBaseFolderPath && onSetBaseFolderPath(currentFolderId);
    }
    if (!currentFolderId) {
      getRootData();
    } else {
      setSelectedItemId(currentFolderId);
      if (
        parentId === 0 &&
        rootFolderType === FolderType.Rooms &&
        !isThirdParty
      ) {
        setSelectedItemType("rooms");
        getRoomList(0, true);
      } else {
        setSelectedItemType("files");
        getFileList(0, currentFolderId, true);
      }
    }
  }, []);

  const onClickBreadCrumb = (item: BreadCrumb) => {
    if (!isFirstLoad) {
      setSearchValue("");
      setIsFirstLoad(true);

      if (+item.id === 0) {
        setSelectedItemSecurity(undefined);
        setSelectedItemType(undefined);
        getRootData();
      } else {
        setItems(null);

        const idx = breadCrumbs.findIndex(
          (value) => value.id.toString() === item.id.toString()
        );

        const newBreadCrumbs = breadCrumbs.map((item) => ({ ...item }));

        newBreadCrumbs.splice(idx + 1, newBreadCrumbs.length - idx - 1);

        setBreadCrumbs(newBreadCrumbs);
        setSelectedItemId(item.id);
        if (item.isRoom) {
          setSelectedItemType("rooms");
          getRoomList(0, false, null);
        } else {
          setSelectedItemType("files");
          getFileList(0, item.id, false, null);
        }
      }
    }
  };

  const onCloseAction = () => {
    if (onClose) {
      onClose();
    } else {
      if (isCopy) {
        setCopyPanelVisible(false);
        setIsFolderActions(false);
      } else if (isRestoreAll) {
        setRestoreAllPanelVisible(false);
      } else {
        setMoveToPanelVisible(false);
      }
    }
  };

  const onSearchAction = (value: string) => {
    setIsFirstLoad(true);
    setItems(null);
    if (selectedItemType === "rooms") {
      getRoomList(0, false, value === "" ? null : value);
    } else {
      getFileList(0, selectedItemId, false, value === "" ? null : value);
    }

    setSearchValue(value);
  };

  const onClearSearchAction = () => {
    setIsFirstLoad(true);
    setItems(null);
    if (selectedItemType === "rooms") {
      getRoomList(0, false, null);
    } else {
      getFileList(0, selectedItemId, false, null);
    }

    setSearchValue("");
  };

  const onAcceptAction = (
    items: any,
    accessRights: any,
    fileName: string,
    isChecked: boolean
  ) => {
    if ((isMove || isCopy || isRestoreAll) && !isEditorDialog) {
      const folderTitle = breadCrumbs[breadCrumbs.length - 1].label;

      let fileIds: any[] = [];
      let folderIds: any[] = [];

      for (let item of selection) {
        if (item.fileExst || item.contentLength) {
          fileIds.push(item.id);
        } else if (item.id === selectedItemId) {
          toastr.error(t("Translations:MoveToFolderMessage"));
        } else {
          folderIds.push(item.id);
        }
      }

      if (isFolderActions) {
        fileIds = [];
        folderIds = [];

        folderIds.push(currentFolderId);
      }

      if (folderIds.length || fileIds.length) {
        const operationData = {
          destFolderId: selectedItemId,
          folderIds,
          fileIds,
          deleteAfter: false,
          isCopy,
          folderTitle,
          translations: {
            copy: t("Common:CopyOperation"),
            move: t("Translations:MoveToOperation"),
          },
        };

        setIsRequestRunning(true);
        setSelectedItems();
        checkFileConflicts(selectedItemId, folderIds, fileIds)
          .then(async (conflicts: any) => {
            if (conflicts.length) {
              setConflictDialogData(conflicts, operationData);
              setIsRequestRunning(false);
            } else {
              setIsRequestRunning(false);
              onCloseAction();
              const move = !isCopy;
              if (move) setMovingInProgress(move);
              sessionStorage.setItem("filesSelectorPath", `${selectedItemId}`);
              await itemOperationToFolder(operationData);
            }
          })
          .catch((e: any) => {
            toastr.error(e);
            setIsRequestRunning(false);
            clearActiveOperations(fileIds, folderIds);
          });
      } else {
        toastr.error(t("Common:ErrorEmptyList"));
      }
    } else {
      setIsRequestRunning(true);
      onSetNewFolderPath && onSetNewFolderPath(selectedItemId);
      onSelectFolder && onSelectFolder(selectedItemId);
      onSave &&
        selectedItemId &&
        onSave(null, selectedItemId, fileName, isChecked);
      onSelectTreeNode && onSelectTreeNode(selectedTreeNode);

      const info: {
        id: string | number;
        title: string;
        path?: string[];
      } = {
        id: selectedFileInfo?.id || "",
        title: selectedFileInfo?.title || "",
        path: [],
      };

      breadCrumbs.forEach((item, index) => {
        if (index !== 0 && info.path) info.path.push(item.label);
      });

      onSelectFile && selectedFileInfo && onSelectFile(info);
      !withoutImmediatelyClose && onCloseAction();
    }
  };

  const headerLabel = getHeaderLabel(
    t,
    isCopy,
    isRestoreAll,
    isMove,
    filterParam
  );

  const acceptButtonLabel = getAcceptButtonLabel(
    t,
    isCopy,
    isRestoreAll,
    isMove,
    filterParam
  );

  const isDisabled = getIsDisabled(
    isFirstLoad,
    fromFolderId === selectedItemId,
    selectedItemType === "rooms",
    isRoot,
    isCopy,
    isMove,
    isRestoreAll,
    isRequestRunning,
    selectedItemSecurity,
    filterParam,
    !!selectedFileInfo
  );

  return (
    <>
      <Backdrop
        visible={isPanelVisible}
        isAside
        withBackground
        zIndex={210}
        onClick={onCloseAction}
      />
      <Aside
        visible={isPanelVisible}
        withoutBodyScroll
        zIndex={310}
        onClose={onCloseAction}
      >
        <Selector
          headerLabel={headerLabel}
          withoutBackButton
          searchPlaceholder={t("Common:Search")}
          searchValue={searchValue}
          onSearch={onSearchAction}
          onClearSearch={onClearSearchAction}
          items={items ? items : []}
          onSelect={onSelectAction}
          acceptButtonLabel={acceptButtonLabel}
          onAccept={onAcceptAction}
          withCancelButton
          cancelButtonLabel={t("Common:CancelButton")}
          onCancel={onCloseAction}
          emptyScreenImage={
            theme.isBase ? EmptyScreenAltSvgUrl : EmptyScreenAltSvgDarkUrl
          }
          emptyScreenHeader={t("SelectorEmptyScreenHeader")}
          emptyScreenDescription=""
          searchEmptyScreenImage={
            theme.isBase
              ? EmptyScreenFilterAltSvgUrl
              : EmptyScreenFilterAltDarkSvgUrl
          }
          searchEmptyScreenHeader={t("Common:NotFoundTitle")}
          searchEmptyScreenDescription={t("EmptyFilterDescriptionText")}
          withBreadCrumbs
          breadCrumbs={breadCrumbs}
          onSelectBreadCrumb={onClickBreadCrumb}
          isLoading={showLoader}
          isBreadCrumbsLoading={showBreadCrumbsLoader}
          withSearch={
            !isRoot && items ? items.length > 0 : !isRoot && isFirstLoad
          }
          rowLoader={
            <Loaders.SelectorRowLoader
              isMultiSelect={false}
              isUser={isRoot}
              isContainer={showLoader}
            />
          }
          searchLoader={<Loaders.SelectorSearchLoader />}
          breadCrumbsLoader={<Loaders.SelectorBreadCrumbsLoader />}
          alwaysShowFooter={true}
          isNextPageLoading={isNextPageLoading}
          hasNextPage={hasNextPage}
          totalItems={total}
          loadNextPage={
            isRoot
              ? null
              : selectedItemType === "rooms"
              ? getRoomList
              : getFileList
          }
          disableAcceptButton={isDisabled}
          withFooterInput={withFooterInput}
          withFooterCheckbox={withFooterCheckbox}
          footerInputHeader={footerInputHeader}
          currentFooterInputValue={currentFooterInputValue}
          footerCheckboxLabel={footerCheckboxLabel}
          descriptionText={
            !filterParam ? "" : descriptionText ?? t("Common:SelectDOCXFormat")
          }
          acceptButtonId={isMove || isCopy ? "select-file-modal-submit" : ""}
          cancelButtonId={isMove || isCopy ? "select-file-modal-cancel" : ""}
        />
      </Aside>
    </>
  );
};

export default inject(
  (
    {
      auth,
      selectedFolderStore,
      filesActionsStore,
      uploadDataStore,
      treeFoldersStore,
      dialogsStore,
      filesStore,
    }: any,
    { isCopy, isRestoreAll, isMove, isPanelVisible, id, passedFoldersTree }: any
  ) => {
    const { id: selectedId, parentId, rootFolderType } = selectedFolderStore;

    const { setConflictDialogData, checkFileConflicts, setSelectedItems } =
      filesActionsStore;
    const { itemOperationToFolder, clearActiveOperations } = uploadDataStore;

    const sessionPath = window.sessionStorage.getItem("filesSelectorPath");

    const fromFolderId = id
      ? id
      : passedFoldersTree?.length > 0
      ? passedFoldersTree[0].id
      : rootFolderType === FolderType.Archive ||
        rootFolderType === FolderType.TRASH
      ? undefined
      : selectedId;

    const currentFolderId =
      sessionPath && (isMove || isCopy || isRestoreAll)
        ? +sessionPath
        : fromFolderId;

    const { treeFolders } = treeFoldersStore;

    const {
      moveToPanelVisible,
      setMoveToPanelVisible,
      copyPanelVisible,
      setCopyPanelVisible,
      restoreAllPanelVisible,
      setRestoreAllPanelVisible,
      conflictResolveDialogVisible,
      isFolderActions,
      setIsFolderActions,
    } = dialogsStore;

    const { theme } = auth.settingsStore;

    const { selection, bufferSelection, filesList, setMovingInProgress } =
      filesStore;

    const selections =
      isMove || isCopy || isRestoreAll
        ? isRestoreAll
          ? filesList
          : selection.length > 0 && selection[0] != null
          ? selection
          : bufferSelection != null
          ? [bufferSelection]
          : []
        : [];

    const selectionsWithoutEditing = isRestoreAll
      ? filesList
      : isCopy
      ? selections
      : selections.filter((f: any) => f && !f?.isEditing);

    const disabledItems: any[] = [];

    selectionsWithoutEditing.forEach((item: any) => {
      if (item?.isFolder && item?.id) {
        disabledItems.push(item.id);
      }
    });

    return {
      currentFolderId,
      fromFolderId,
      parentId,
      rootFolderType,
      treeFolders,
      isPanelVisible: isPanelVisible
        ? isPanelVisible
        : (moveToPanelVisible || copyPanelVisible || restoreAllPanelVisible) &&
          !conflictResolveDialogVisible,
      setMoveToPanelVisible,
      theme,
      selection: selectionsWithoutEditing,
      disabledItems,
      isFolderActions,
      setConflictDialogData,
      checkFileConflicts,
      itemOperationToFolder,
      clearActiveOperations,
      setMovingInProgress,
      setCopyPanelVisible,
      setRestoreAllPanelVisible,
      setIsFolderActions,
      setSelectedItems,
    };
  }
)(observer(FilesSelector));
