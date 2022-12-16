import React from "react";
//import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";
import axios from "axios";
import toastr from "@docspace/components/toast/toastr";
import Section from "@docspace/common/components/Section";
import {
  showLoader,
  hideLoader,
  frameCallbackData,
  frameCallCommand,
  getObjectByLocation,
} from "@docspace/common/utils";
import FilesFilter from "@docspace/common/api/files/filter";
import { getGroup } from "@docspace/common/api/groups";
import { getUserById } from "@docspace/common/api/people";
import { withTranslation, Trans } from "react-i18next";

import {
  SectionBodyContent,
  SectionFilterContent,
  SectionHeaderContent,
  SectionPagingContent,
} from "./Section";
import MediaViewer from "./MediaViewer";
import DragTooltip from "../../components/DragTooltip";
import { observer, inject } from "mobx-react";
//import config from "PACKAGE_FILE";
import { Consumer } from "@docspace/components/utils/context";
import { Events } from "@docspace/common/constants";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { getCategoryType } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";
import { InfoPanelBodyContent, InfoPanelHeaderContent } from "./InfoPanel";

class PureHome extends React.Component {
  componentDidMount() {
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
    } = this.props;

    if (!window.location.href.includes("#preview")) {
      localStorage.removeItem("isFirstUrl");
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
          })
          .catch((err) => {
            toastr.error(err);
            this.fetchDefaultFiles();
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
        this.fetchDefaultRooms();

        return;
      }
    } else {
      filterObj = FilesFilter.getFilter(window.location);

      if (!filterObj) {
        setIsLoading(true);
        this.fetchDefaultFiles();

        return;
      }
    }

    if (!filterObj) return;

    if (isRooms && alreadyFetchingRooms) return setAlreadyFetchingRooms(false);

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

    window.addEventListener("message", this.handleMessage, false);
  }

  fetchDefaultFiles = () => {
    const { fetchFiles, setIsLoading, setFirstLoad } = this.props;
    const filterObj = FilesFilter.getDefault();
    const folderId = filterObj.folder;

    fetchFiles(folderId).finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  };

  fetchDefaultRooms = () => {
    const { fetchRooms, setIsLoading, setFirstLoad } = this.props;

    fetchRooms().finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  };

  onDrop = (files, uploadToFolder) => {
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

  showOperationToast = (type, qty, title) => {
    const { t } = this.props;
    switch (type) {
      case "move":
        if (qty > 1) {
          return toastr.success(
            <Trans t={t} i18nKey="MoveItems" ns="Files">
              {{ qty }} elements has been moved
            </Trans>
          );
        }
        return toastr.success(
          <Trans t={t} i18nKey="MoveItem" ns="Files">
            {{ title }} moved
          </Trans>
        );
      case "duplicate":
        if (qty > 1) {
          return toastr.success(
            <Trans t={t} i18nKey="CopyItems" ns="Files">
              {{ qty }} elements copied
            </Trans>
          );
        }
        return toastr.success(
          <Trans t={t} i18nKey="CopyItem" ns="Filesы">
            {{ title }} copied
          </Trans>
        );
      default:
        break;
    }
  };

  showUploadPanel = () => {
    const {
      uploaded,
      converted,
      setUploadPanelVisible,
      clearPrimaryProgressData,
      primaryProgressDataVisible,
    } = this.props;
    setUploadPanelVisible(true);

    if (primaryProgressDataVisible && uploaded && converted)
      clearPrimaryProgressData();
  };
  componentDidUpdate(prevProps) {
    const {
      isProgressFinished,
      secondaryProgressDataStoreIcon,
      itemsSelectionLength,
      itemsSelectionTitle,
    } = this.props;

    if (this.props.isHeaderVisible !== prevProps.isHeaderVisible) {
      this.props.setHeaderVisible(this.props.isHeaderVisible);
    }
    if (
      isProgressFinished &&
      isProgressFinished !== prevProps.isProgressFinished
    ) {
      this.showOperationToast(
        secondaryProgressDataStoreIcon,
        itemsSelectionLength,
        itemsSelectionTitle
      );
    }
  }

  componentWillUnmount() {
    window.removeEventListener("message", this.handleMessage, false);
  }

  handleMessage = async (e) => {
    const {
      setFrameConfig,
      user,
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
    } = this.props;

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

  render() {
    //console.log("Home render");
    const {
      viewAs,

      firstLoad,
      isHeaderVisible,
      isPrivacyFolder,
      isRecycleBinFolder,
      isErrorRoomNotAvailable,

      primaryProgressDataVisible,
      primaryProgressDataPercent,
      primaryProgressDataIcon,
      primaryProgressDataAlert,
      clearUploadedFilesHistory,

      secondaryProgressDataStoreVisible,
      secondaryProgressDataStorePercent,
      secondaryProgressDataStoreIcon,
      secondaryProgressDataStoreAlert,

      dragging,
      tReady,
      personal,
      checkedMaintenance,
      setMaintenanceExist,
      snackbarExist,
      isFrame,
      showTitle,
      showFilter,
      frameConfig,
      withPaging,
      isEmptyPage,
    } = this.props;

    if (window.parent && !frameConfig) {
      frameCallCommand("setConfig");
    }

    return (
      <>
        <MediaViewer />
        <DragTooltip />
        <Section
          withPaging={withPaging}
          dragging={dragging}
          withBodyScroll
          withBodyAutoFocus={!isMobile}
          uploadFiles
          onDrop={isRecycleBinFolder || isPrivacyFolder ? null : this.onDrop}
          setSelections={this.props.setSelections}
          showPrimaryProgressBar={primaryProgressDataVisible}
          primaryProgressBarValue={primaryProgressDataPercent}
          primaryProgressBarIcon={primaryProgressDataIcon}
          showPrimaryButtonAlert={primaryProgressDataAlert}
          showSecondaryProgressBar={secondaryProgressDataStoreVisible}
          secondaryProgressBarValue={secondaryProgressDataStorePercent}
          secondaryProgressBarIcon={secondaryProgressDataStoreIcon}
          showSecondaryButtonAlert={secondaryProgressDataStoreAlert}
          clearUploadedFilesHistory={clearUploadedFilesHistory}
          viewAs={viewAs}
          hideAside={
            primaryProgressDataVisible || secondaryProgressDataStoreVisible //TODO: use hideArticle action
          }
          isLoaded={!firstLoad}
          isHeaderVisible={isHeaderVisible}
          onOpenUploadPanel={this.showUploadPanel}
          firstLoad={firstLoad}
        >
          {!isErrorRoomNotAvailable && (
            <Section.SectionHeader>
              {isFrame ? (
                showTitle && <SectionHeaderContent />
              ) : (
                <SectionHeaderContent />
              )}
            </Section.SectionHeader>
          )}

          {!isEmptyPage && (
            <Section.SectionFilter>
              {isFrame ? (
                showFilter && <SectionFilterContent />
              ) : (
                <SectionFilterContent />
              )}
            </Section.SectionFilter>
          )}

          <Section.SectionBody>
            <Consumer>
              {(context) => (
                <>
                  <SectionBodyContent sectionWidth={context.sectionWidth} />
                </>
              )}
            </Consumer>
          </Section.SectionBody>

          <Section.InfoPanelHeader>
            <InfoPanelHeaderContent />
          </Section.InfoPanelHeader>

          <Section.InfoPanelBody>
            <InfoPanelBodyContent />
          </Section.InfoPanelBody>

          {withPaging && (
            <Section.SectionPaging>
              <SectionPagingContent tReady={tReady} />
            </Section.SectionPaging>
          )}
        </Section>
      </>
    );
  }
}

const Home = withTranslation("Files")(PureHome);

export default inject(
  ({
    auth,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    mediaViewerDataStore,
    settingsStore,
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
      setSelections,
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

      disableDrag,
      isErrorRoomNotAvailable,
    } = filesStore;

    const { gallerySelected } = oformsStore;

    const {
      isRecycleBinFolder,
      isPrivacyFolder,

      expandedKeys,
      setExpandedKeys,
      isRoomsFolder,
      isArchiveFolder,
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
    } = secondaryProgressDataStore;

    const {
      setUploadPanelVisible,
      startUpload,
      uploaded,
      converted,
    } = uploadDataStore;

    const { uploadEmptyFolders } = filesActionsStore;

    const selectionLength = isProgressFinished ? selection.length : null;
    const selectionTitle = isProgressFinished
      ? filesStore.selectionTitle
      : null;

    const { setToPreviewFile, playlist } = mediaViewerDataStore;

    const {
      checkedMaintenance,
      setMaintenanceExist,
      snackbarExist,
      isHeaderVisible,
      setHeaderVisible,
      personal,
      setFrameConfig,
      frameConfig,
      isFrame,
      withPaging,
    } = auth.settingsStore;

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
      checkedMaintenance,
      setMaintenanceExist,
      snackbarExist,

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
      setSelections,
      startUpload,
      uploadEmptyFolders,
      isHeaderVisible,
      setHeaderVisible,
      personal,
      setToPreviewFile,
      playlist,

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
    };
  }
)(withRouter(observer(Home)));
