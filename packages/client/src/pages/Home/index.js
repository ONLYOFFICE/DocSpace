import React from "react";
import { useLocation, Outlet } from "react-router-dom";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import axios from "axios";

import {
  showLoader,
  hideLoader,
  frameCallbackData,
  frameCallCommand,
  getObjectByLocation,
} from "@docspace/common/utils";

import FilesFilter from "@docspace/common/api/files/filter";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import AccountsFilter from "@docspace/common/api/people/filter";
import { getGroup } from "@docspace/common/api/groups";
import { getUserById } from "@docspace/common/api/people";
import { Events } from "@docspace/common/constants";
import Section from "@docspace/common/components/Section";

import toastr from "@docspace/components/toast/toastr";

import DragTooltip from "SRC_DIR/components/DragTooltip";
import { getCategoryType, setDocumentTitle } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

import {
  SectionFilterContent,
  SectionHeaderContent,
  SectionPagingContent,
} from "./Section";
import AccountsDialogs from "./Section/AccountsBody/Dialogs";

import MediaViewer from "./MediaViewer";
import SelectionArea from "./SelectionArea";
import { InfoPanelBodyContent, InfoPanelHeaderContent } from "./InfoPanel";
import { RoomSearchArea } from "@docspace/common/constants";

const PureHome = (props) => {
    const {
      fetchFiles,
      fetchRooms,
      alreadyFetchingRooms,
      setAlreadyFetchingRooms,
      //homepage,
      setIsLoading,
      setFirstLoad,
      setToPreviewFile,
      playlist,

      getFileInfo,
      gallerySelected,
      setIsUpdatingRowItem,
      setIsPreview,
      selectedFolderStore,
    t,
    startUpload,
    setDragging,
    dragging,
    uploadEmptyFolders,
    disableDrag,
    uploaded,
    converted,
    setUploadPanelVisible,
    clearPrimaryProgressData,
    primaryProgressDataVisible,
    isProgressFinished,
    secondaryProgressDataStoreIcon,
    itemsSelectionLength,
    itemsSelectionTitle,
    setItemsSelectionTitle,
    refreshFiles,
    isHeaderVisible,
    setHeaderVisible,
    setFrameConfig,
    user,
    folders,
    files,
    selection,
    filesList,
    removeFirstUrl,

    createFile,
    createFolder,
    createRoom,

    setViewAs,
    viewAs,

    firstLoad,

    isPrivacyFolder,
    isRecycleBinFolder,
    isErrorRoomNotAvailable,

    primaryProgressDataPercent,
    primaryProgressDataIcon,
    primaryProgressDataAlert,
    clearUploadedFilesHistory,

    secondaryProgressDataStoreVisible,
    secondaryProgressDataStorePercent,

    secondaryProgressDataStoreAlert,

    tReady,
    isFrame,
    showTitle,
    showFilter,
    frameConfig,
    withPaging,
    isEmptyPage,
    isLoadedEmptyPage,

    setPortalTariff,

    accountsViewAs,
    fetchPeople,
    setSelectedNode,
    onClickBack,
  } = props;

  const location = useLocation();

  const isAccountsPage = location.pathname.includes("accounts");
  const isSettingsPage = location.pathname.includes("settings");

  React.useEffect(() => {
    if (isAccountsPage || isSettingsPage) return;

    if (!window.location.href.includes("#preview")) {
      // localStorage.removeItem("isFirstUrl");
      // Media viewer
      removeFirstUrl();
    }

    const categoryType = getCategoryType(location);

    let filterObj = null;
    let isRooms = false;

    if (window.location.href.indexOf("/#preview") > 1 && playlist.length < 1) {
      const pathname = window.location.href;
      const fileId = pathname.slice(pathname.indexOf("#preview") + 9);

      setTimeout(() => {
        getFileInfo(fileId)
          .then((data) => {
            const canOpenPlayer =
              data.viewAccessability.ImageView ||
              data.viewAccessability.MediaView;
            const file = { ...data, canOpenPlayer };
            setToPreviewFile(file, true);
            setIsPreview(true);
          })
          .catch((err) => {
            toastr.error(err);
            fetchDefaultFiles();
          });
      }, 1);

      return;
    }

    const isRoomFolder = getObjectByLocation(window.location)?.folder;

    if (
      (categoryType == CategoryType.Shared ||
        categoryType == CategoryType.SharedRoom ||
        categoryType == CategoryType.Archive) &&
      !isRoomFolder
    ) {
      filterObj = RoomsFilter.getFilter(window.location);

      isRooms = true;

      if (!filterObj) {
        setIsLoading(true);

        if (window.location.pathname.indexOf("/rooms/archived") !== -1) {
          fetchArchiveDefaultRooms();

          return;
        }
        fetchDefaultRooms();

        return;
      }
    } else {
      filterObj = FilesFilter.getFilter(window.location);

      if (!filterObj) {
        setIsLoading(true);
        fetchDefaultFiles();

        return;
      }
    }

    if (!filterObj) return;

    if (isRooms && alreadyFetchingRooms && selectedFolderStore.title)
      return setAlreadyFetchingRooms(false);

    let dataObj = { filter: filterObj };

    if (filterObj && filterObj.authorType) {
      const authorType = filterObj.authorType;
      const indexOfUnderscore = authorType.indexOf("_");
      const type = authorType.slice(0, indexOfUnderscore);
      const itemId = authorType.slice(indexOfUnderscore + 1);

      if (itemId) {
        dataObj = {
          type,
          itemId,
          filter: filterObj,
        };
      } else {
        filterObj.authorType = null;
        dataObj = { filter: filterObj };
      }
    }

    if (filterObj && filterObj.subjectId) {
      const type = "user";
      const itemId = filterObj.subjectId;

      if (itemId) {
        dataObj = {
          type,
          itemId,
          filter: filterObj,
        };
      } else {
        filterObj.subjectId = null;
        dataObj = { filter: filterObj };
      }
    }

    if (!dataObj) return;

    const { filter, itemId, type } = dataObj;
    const newFilter = filter
      ? filter.clone()
      : isRooms
      ? RoomsFilter.getDefault()
      : FilesFilter.getDefault();
    const requests = [Promise.resolve(newFilter)];

    if (type === "group") {
      requests.push(getGroup(itemId));
    } else if (type === "user") {
      requests.push(getUserById(itemId));
    }

    setIsLoading(true);

    axios
      .all(requests)
      .catch((err) => {
        if (isRooms) {
          Promise.resolve(RoomsFilter.getDefault());
        } else {
          Promise.resolve(FilesFilter.getDefault());
        }

        //console.warn("Filter restored by default", err);
      })
      .then((data) => {
        const filter = data[0];
        const result = data[1];
        if (result) {
          const type = result.displayName ? "user" : "group";
          const selectedItem = {
            key: result.id,
            label: type === "user" ? result.displayName : result.name,
            type,
          };
          if (!isRooms) {
            filter.selectedItem = selectedItem;
          }
        }

        if (filter) {
          if (isRooms) {
            return fetchRooms(null, filter);
          } else {
            const folderId = filter.folder;
            return fetchFiles(folderId, filter);
          }
        }

        return Promise.resolve();
      })
      .then(() => {
        if (gallerySelected) {
          setIsUpdatingRowItem(false);

          const event = new Event(Events.CREATE);

          const payload = {
            extension: "docxf",
            id: -1,
            fromTemplate: true,
            title: gallerySelected.attributes.name_form,
          };

          event.payload = payload;

          window.dispatchEvent(event);
        }
      })
      .finally(() => {
        setIsLoading(false);
        setFirstLoad(false);
        setAlreadyFetchingRooms(false);
      });

    window.addEventListener("message", handleMessage, false);

    return () => {
      window.removeEventListener("message", handleMessage, false);
    };
  }, []);

  const fetchDefaultFiles = () => {
    const filterObj = FilesFilter.getDefault();
    const folderId = filterObj.folder;

    fetchFiles(folderId).finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  };

  const fetchDefaultRooms = () => {
    fetchRooms().finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  };

  const fetchArchiveDefaultRooms = () => {
    const { fetchRooms, setIsLoading, setFirstLoad } = this.props;

    const filter = RoomsFilter.getDefault();
    filter.searchArea = RoomSearchArea.Archive;

    fetchRooms(null, filter).finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  };

  const onDrop = (files, uploadToFolder) => {
    const {
      t,
      startUpload,
      setDragging,
      dragging,
      uploadEmptyFolders,
      disableDrag,
    } = this.props;
    dragging && setDragging(false);

    if (disableDrag) return;

    const emptyFolders = files.filter((f) => f.isEmptyDirectory);

    if (emptyFolders.length > 0) {
      uploadEmptyFolders(emptyFolders, uploadToFolder).then(() => {
        const onlyFiles = files.filter((f) => !f.isEmptyDirectory);
        if (onlyFiles.length > 0) startUpload(onlyFiles, uploadToFolder, t);
      });
    } else {
      startUpload(files, uploadToFolder, t);
    }
  };

  const showOperationToast = (type, qty, title) => {
    switch (type) {
      case "move":
        if (qty > 1) {
          return (
            toastr.success(
            <Trans t={t} i18nKey="MoveItems" ns="Files">
              {{ qty }} elements has been moved
            </Trans>
            ),
            refreshFiles()
          );
        }
        return (
          toastr.success(
          <Trans t={t} i18nKey="MoveItem" ns="Files">
            {{ title }} moved
          </Trans>
          ),
          refreshFiles()
        );

      case "duplicate":
        if (qty > 1) {
          return (
            toastr.success(
            <Trans t={t} i18nKey="CopyItems" ns="Files">
              {{ qty }} elements copied
            </Trans>
            ),
            refreshFiles()
          );
        }
        return (
          toastr.success(
          <Trans t={t} i18nKey="CopyItem" ns="Files">
            {{ title }} copied
          </Trans>
          ),
          refreshFiles()
        );

      default:
        break;
    }
  };

  const showUploadPanel = () => {
    setUploadPanelVisible(true);

    if (primaryProgressDataVisible && uploaded && converted)
      clearPrimaryProgressData();
  };

  const prevProps = React.useRef({
    isHeaderVisible: isHeaderVisible,
    isProgressFinished: isProgressFinished,
  });

  React.useEffect(() => {
    if (isHeaderVisible !== prevProps.current.isHeaderVisible) {
      setHeaderVisible(isHeaderVisible);
    }

    if (
      isProgressFinished &&
      itemsSelectionTitle &&
      isProgressFinished !== prevProps.current.isProgressFinished
    ) {
      showOperationToast(
        secondaryProgressDataStoreIcon,
        itemsSelectionLength,
        itemsSelectionTitle
      );
      setItemsSelectionTitle(null);
    }
  }, [
    isAccountsPage,
    isHeaderVisible,
    setHeaderVisible,
    isProgressFinished,
    refreshFiles,
    itemsSelectionTitle,
    showOperationToast,
    setItemsSelectionTitle,
  ]);

  React.useEffect(() => {
    prevProps.current.isHeaderVisible = isHeaderVisible;
    prevProps.current.isProgressFinished = isProgressFinished;
  }, [isHeaderVisible, isProgressFinished]);

  const handleMessage = async (e) => {
    const eventData = typeof e.data === "string" ? JSON.parse(e.data) : e.data;

    if (eventData.data) {
      const { data, methodName } = eventData.data;

      let res;

      switch (methodName) {
        case "setConfig":
          res = await setFrameConfig(data);
          break;
        case "getFolderInfo":
          res = selectedFolderStore;
          break;
        case "getFolders":
          res = folders;
          break;
        case "getFiles":
          res = files;
          break;
        case "getList":
          res = filesList;
          break;
        case "getSelection":
          res = selection;
          break;
        case "getUserInfo":
          res = user;
          break;
        case "openModal": {
          const { type, options } = data;

          if (type === "CreateFile" || type === "CreateFolder") {
            const item = new Event(Events.CREATE);

            const payload = {
              extension: options,
              id: -1,
            };

            item.payload = payload;

            window.dispatchEvent(item);
          }

          if (type === "CreateRoom") {
            const room = new Event(Events.ROOM_CREATE);

            window.dispatchEvent(room);
          }
          break;
        }
        case "createFile":
          {
            const { folderId, title, templateId, formId } = data;
            res = await createFile(folderId, title, templateId, formId);

            refreshFiles();
          }
          break;
        case "createFolder":
          {
            const { parentFolderId, title } = data;
            res = await createFolder(parentFolderId, title);

            refreshFiles();
          }
          break;
        case "createRoom":
          {
            const { title, type } = data;
            res = await createRoom(title, type);

            refreshFiles();
          }
          break;
        case "setListView":
          {
            setViewAs(data);
          }
          break;
        default:
          res = "Wrong method";
      }

      frameCallbackData(res);
    }
  };

  if (window.parent && !frameConfig && !isAccountsPage && !isSettingsPage) {
    frameCallCommand("setConfig");
  }

  React.useEffect(() => {
    window.addEventListener("popstate", onClickBack);

    return () => {
      window.removeEventListener("popstate", onClickBack);
    };
  }, []);

  React.useEffect(() => {
    if (!isAccountsPage) return;
    if (location.pathname.indexOf("/accounts/filter") > -1) {
      setSelectedNode(["accounts", "filter"]);

      const newFilter = AccountsFilter.getFilter(location);
      //console.log("PEOPLE URL changed", pathname, newFilter);
      fetchPeople(newFilter, true).catch((err) => {
        if (err?.response?.status === 402) setPortalTariff();
      });
    }
  }, [isAccountsPage, location, setSelectedNode]);

  React.useEffect(() => {
    if (!isSettingsPage) return;
    setDocumentTitle(t("Common:Settings"));
  }, [t, tReady, isSettingsPage]);

  let sectionProps = {};

  if (isSettingsPage) {
    sectionProps.isInfoPanelAvailable = false;
    sectionProps.viewAs = "settings";
  } else {
    sectionProps = {
      withPaging,
      withBodyScroll: true,
      withBodyAutoFocus: !isMobile,
      firstLoad,
      isLoaded: !firstLoad,
      viewAs: accountsViewAs,
    };

    if (!isAccountsPage) {
      sectionProps.dragging = dragging;
      sectionProps.uploadFiles = true;
      sectionProps.onDrop =
        isRecycleBinFolder || isPrivacyFolder ? null : onDrop;

      sectionProps.clearUploadedFilesHistory = clearUploadedFilesHistory;
      sectionProps.viewAs = viewAs;
      sectionProps.hideAside =
        primaryProgressDataVisible || secondaryProgressDataStoreVisible;
      sectionProps.isHeaderVisible = isHeaderVisible;

      sectionProps.isEmptyPage = isEmptyPage;
    }
  }

  sectionProps.onOpenUploadPanel = showUploadPanel;
  sectionProps.showPrimaryProgressBar = primaryProgressDataVisible;
  sectionProps.primaryProgressBarValue = primaryProgressDataPercent;
  sectionProps.primaryProgressBarIcon = primaryProgressDataIcon;
  sectionProps.showPrimaryButtonAlert = primaryProgressDataAlert;
  sectionProps.showSecondaryProgressBar = secondaryProgressDataStoreVisible;
  sectionProps.secondaryProgressBarValue = secondaryProgressDataStorePercent;
  sectionProps.secondaryProgressBarIcon = secondaryProgressDataStoreIcon;
  sectionProps.showSecondaryButtonAlert = secondaryProgressDataStoreAlert;

    return (
      <>
      {isSettingsPage ? (
        <></>
      ) : isAccountsPage ? (
        <AccountsDialogs />
      ) : (
        <>
        <DragTooltip />
        <SelectionArea />
        </>
      )}
      <MediaViewer />
      <Section {...sectionProps}>
        {(!isErrorRoomNotAvailable || isAccountsPage || isSettingsPage) && (
            <Section.SectionHeader>
              {isFrame ? (
                showTitle && <SectionHeaderContent />
              ) : (
                <SectionHeaderContent />
              )}
            </Section.SectionHeader>
          )}

        {((!isEmptyPage && !isErrorRoomNotAvailable) || isAccountsPage) &&
          !isSettingsPage && (
            <Section.SectionFilter>
              {isFrame ? (
                showFilter && <SectionFilterContent />
              ) : (
                <SectionFilterContent />
              )}
            </Section.SectionFilter>
          )}

          <Section.SectionBody>
          <Outlet />
          </Section.SectionBody>

          <Section.InfoPanelHeader>
            <InfoPanelHeaderContent />
          </Section.InfoPanelHeader>
          <Section.InfoPanelBody>
            <InfoPanelBodyContent />
          </Section.InfoPanelBody>

        {withPaging && !isSettingsPage && (
            <Section.SectionPaging>
              <SectionPagingContent tReady={tReady} />
            </Section.SectionPaging>
          )}
        </Section>
      </>
    );
};

const Home = withTranslation(["Files", "People"])(PureHome);

export default inject(
  ({
    auth,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    mediaViewerDataStore,
    peopleStore,
    filesActionsStore,
    oformsStore,
  }) => {
    const {
      secondaryProgressDataStore,
      primaryProgressDataStore,
      clearUploadedFilesHistory,
    } = uploadDataStore;
    const {
      firstLoad,
      setFirstLoad,
      fetchFiles,
      fetchRooms,
      alreadyFetchingRooms,
      setAlreadyFetchingRooms,
      selection,
      dragging,
      setDragging,
      setIsLoading,
      isLoading,
      viewAs,
      getFileInfo,
      setIsUpdatingRowItem,

      folders,
      files,
      filesList,
      selectedFolderStore,
      createFile,
      createFolder,
      createRoom,
      refreshFiles,
      setViewAs,
      isEmptyPage,
      isLoadedEmptyPage,
      disableDrag,
      isErrorRoomNotAvailable,
      setIsPreview,
    } = filesStore;

    const { gallerySelected } = oformsStore;

    const {
      isRecycleBinFolder,
      isPrivacyFolder,

      expandedKeys,
      setExpandedKeys,
      isRoomsFolder,
      isArchiveFolder,
      setSelectedNode,
    } = treeFoldersStore;

    const {
      visible: primaryProgressDataVisible,
      percent: primaryProgressDataPercent,
      icon: primaryProgressDataIcon,
      alert: primaryProgressDataAlert,
      clearPrimaryProgressData,
    } = primaryProgressDataStore;

    const {
      visible: secondaryProgressDataStoreVisible,
      percent: secondaryProgressDataStorePercent,
      icon: secondaryProgressDataStoreIcon,
      alert: secondaryProgressDataStoreAlert,
      isSecondaryProgressFinished: isProgressFinished,
      itemsSelectionLength,
      itemsSelectionTitle,
      setItemsSelectionTitle,
    } = secondaryProgressDataStore;

    const { setUploadPanelVisible, startUpload, uploaded, converted } =
      uploadDataStore;

    const { uploadEmptyFolders, onClickBack } = filesActionsStore;

    const selectionLength = isProgressFinished ? selection.length : null;
    const selectionTitle = isProgressFinished
      ? filesStore.selectionTitle
      : null;

    const { setToPreviewFile, playlist, removeFirstUrl } = mediaViewerDataStore;

    const { settingsStore, currentTariffStatusStore } = auth;

    const { setPortalTariff } = currentTariffStatusStore;

    const {
      isHeaderVisible,
      setHeaderVisible,
      setFrameConfig,
      frameConfig,
      isFrame,
      withPaging,
      showCatalog,
    } = settingsStore;

    const {
      usersStore,

      viewAs: accountsViewAs,
    } = peopleStore;

    const { getUsersList: fetchPeople } = usersStore;

    if (!firstLoad) {
      if (isLoading) {
        showLoader();
      } else {
        hideLoader();
      }
    }

    return {
      //homepage: config.homepage,
      firstLoad,
      dragging,
      viewAs,
      uploaded,
      converted,
      isRecycleBinFolder,
      isPrivacyFolder,
      isVisitor: auth.userStore.user.isVisitor,

      primaryProgressDataVisible,
      primaryProgressDataPercent,
      primaryProgressDataIcon,
      primaryProgressDataAlert,
      clearPrimaryProgressData,

      clearUploadedFilesHistory,

      secondaryProgressDataStoreVisible,
      secondaryProgressDataStorePercent,
      secondaryProgressDataStoreIcon,
      secondaryProgressDataStoreAlert,

      selectionLength,
      isProgressFinished,
      selectionTitle,

      itemsSelectionLength,
      setItemsSelectionTitle,
      itemsSelectionTitle,
      isErrorRoomNotAvailable,
      isRoomsFolder,
      isArchiveFolder,

      disableDrag,

      setExpandedKeys,
      setFirstLoad,
      setDragging,
      setIsLoading,
      fetchFiles,
      fetchRooms,
      alreadyFetchingRooms,
      setAlreadyFetchingRooms,
      setUploadPanelVisible,
      startUpload,
      uploadEmptyFolders,
      isHeaderVisible,
      setHeaderVisible,
      setToPreviewFile,
      setIsPreview,
      playlist,
      removeFirstUrl,

      getFileInfo,
      gallerySelected,
      setIsUpdatingRowItem,

      setFrameConfig,
      frameConfig,
      isFrame,
      showTitle: frameConfig?.showTitle,
      showFilter: frameConfig?.showFilter,
      user: auth.userStore.user,
      folders,
      files,
      selection,
      filesList,
      selectedFolderStore,
      createFile,
      createFolder,
      createRoom,
      refreshFiles,
      setViewAs,
      withPaging,
      isEmptyPage,
      isLoadedEmptyPage,
      setPortalTariff,

      accountsViewAs,
      fetchPeople,
      setSelectedNode,
      onClickBack,
    };
  }
)(observer(Home));
