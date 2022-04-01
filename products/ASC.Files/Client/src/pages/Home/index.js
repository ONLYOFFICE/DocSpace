import React from "react";
//import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";
import axios from "axios";
import toastr from "@appserver/components/toast/toastr";
import Section from "@appserver/common/components/Section";
import { showLoader, hideLoader } from "@appserver/common/utils";
import FilesFilter from "@appserver/common/api/files/filter";
import { getGroup } from "@appserver/common/api/groups";
import { getUserById } from "@appserver/common/api/people";
import { withTranslation, Trans } from "react-i18next";

import {
  SectionBodyContent,
  SectionFilterContent,
  SectionHeaderContent,
  SectionPagingContent,
  Bar,
} from "./Section";

import { ArticleMainButtonContent } from "../../components/Article";

import { createTreeFolders } from "../../helpers/files-helpers";
import MediaViewer from "./MediaViewer";
import DragTooltip from "../../components/DragTooltip";
import { observer, inject } from "mobx-react";
import config from "../../../package.json";
import { Consumer } from "@appserver/components/utils/context";

class PureHome extends React.Component {
  componentDidMount() {
    const {
      fetchFiles,
      homepage,
      setIsLoading,
      setFirstLoad,
      expandedKeys,
      setExpandedKeys,
      setToPreviewFile,
      playlist,
      isMediaOrImage,
      getFileInfo,
      setIsPrevSettingsModule,
      isPrevSettingsModule,
    } = this.props;

    if (!window.location.href.includes("#preview")) {
      localStorage.removeItem("isFirstUrl");
    }

    const reg = new RegExp(`${homepage}((/?)$|/filter)`, "gmi"); //TODO: Always find?
    const match = window.location.pathname.match(reg);
    let filterObj = null;

    if (
      window.location.href.indexOf("/files/#preview") > 1 &&
      playlist.length < 1
    ) {
      const pathname = window.location.href;
      const fileId = pathname.slice(pathname.indexOf("#preview") + 9);

      getFileInfo(fileId)
        .then((data) => {
          const canOpenPlayer = isMediaOrImage(data.fileExst);
          const file = { ...data, canOpenPlayer };
          setToPreviewFile(file, true);
        })
        .catch((err) => {
          toastr.error(err);
          this.fetchDefaultFiles();
        });

      return;
    }

    if (match && match.length > 0) {
      if (window.location.href.includes("#preview")) {
        return;
      }

      filterObj = FilesFilter.getFilter(window.location);

      if (!filterObj) {
        setIsLoading(true);
        this.fetchDefaultFiles();

        return;
      }
    }

    if (isPrevSettingsModule) {
      setIsPrevSettingsModule(false);
      return;
    }

    if (!filterObj) return;

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

    if (!dataObj) return;

    const { filter, itemId, type } = dataObj;
    const newFilter = filter ? filter.clone() : FilesFilter.getDefault();
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
        Promise.resolve(FilesFilter.getDefault());
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
          filter.selectedItem = selectedItem;
        }

        if (filter) {
          const folderId = filter.folder;
          //console.log("filter", filter);

          return fetchFiles(folderId, filter).then((data) => {
            const pathParts = data.selectedFolder.pathParts;
            const newExpandedKeys = createTreeFolders(pathParts, expandedKeys);
            setExpandedKeys(newExpandedKeys);
          });
        }

        return Promise.resolve();
      })
      .finally(() => {
        setIsLoading(false);
        setFirstLoad(false);
      });
  }

  fetchDefaultFiles = () => {
    const { isVisitor, fetchFiles, setIsLoading, setFirstLoad } = this.props;
    const filterObj = FilesFilter.getDefault();
    const folderId = isVisitor ? "@common" : filterObj.folder;

    fetchFiles(folderId).finally(() => {
      setIsLoading(false);
      setFirstLoad(false);
    });
  };

  onDrop = (files, uploadToFolder) => {
    const { t, startUpload, setDragging, dragging } = this.props;
    dragging && setDragging(false);
    startUpload(files, uploadToFolder, t);
  };

  showOperationToast = (type, qty, title) => {
    const { t } = this.props;
    switch (type) {
      case "move":
        if (qty > 1) {
          return toastr.success(
            <Trans t={t} i18nKey="MoveItems" ns="Home">
              {{ qty }} elements has been moved
            </Trans>
          );
        }
        return toastr.success(
          <Trans t={t} i18nKey="MoveItem" ns="Home">
            {{ title }} moved
          </Trans>
        );
      case "duplicate":
        if (qty > 1) {
          return toastr.success(
            <Trans t={t} i18nKey="CopyItems" ns="Home">
              {{ qty }} elements copied
            </Trans>
          );
        }
        return toastr.success(
          <Trans t={t} i18nKey="CopyItem" ns="Home">
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
      uploadPanelVisible,
      setUploadPanelVisible,
      clearPrimaryProgressData,
      primaryProgressDataVisible,
    } = this.props;
    setUploadPanelVisible(!uploadPanelVisible);

    if (primaryProgressDataVisible && uploaded && converted)
      clearPrimaryProgressData();
  };
  componentDidUpdate(prevProps) {
    const {
      isProgressFinished,
      secondaryProgressDataStoreIcon,
      selectionLength,
      selectionTitle,
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
        selectionLength,
        selectionTitle
      );
    }
  }

  render() {
    //console.log("Home render");
    const {
      viewAs,
      fileActionId,
      firstLoad,
      isHeaderVisible,
      isPrivacyFolder,
      isRecycleBinFolder,

      primaryProgressDataVisible,
      primaryProgressDataPercent,
      primaryProgressDataIcon,
      primaryProgressDataAlert,

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
    } = this.props;
    return (
      <>
        <MediaViewer />
        <DragTooltip />
        <Section
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
          viewAs={viewAs}
          hideAside={
            !!fileActionId ||
            primaryProgressDataVisible ||
            secondaryProgressDataStoreVisible //TODO: use hideArticle action
          }
          isLoaded={!firstLoad}
          isHeaderVisible={isHeaderVisible}
          onOpenUploadPanel={this.showUploadPanel}
          firstLoad={firstLoad}
        >
          <Section.SectionHeader>
            <SectionHeaderContent />
          </Section.SectionHeader>

          <Section.SectionBar>
            {checkedMaintenance && !snackbarExist && (
              <Bar
                firstLoad={firstLoad}
                personal={personal}
                setMaintenanceExist={setMaintenanceExist}
              />
            )}
          </Section.SectionBar>

          <Section.SectionFilter>
            <SectionFilterContent />
          </Section.SectionFilter>

          <Section.SectionBody>
            <Consumer>
              {(context) => (
                <>
                  <SectionBodyContent sectionWidth={context.sectionWidth} />
                </>
              )}
            </Consumer>
          </Section.SectionBody>

          <Section.SectionPaging>
            <SectionPagingContent tReady={tReady} />
          </Section.SectionPaging>
        </Section>
      </>
    );
  }
}

const Home = withTranslation("Home")(PureHome);

export default inject(
  ({
    auth,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    mediaViewerDataStore,
    settingsStore,
  }) => {
    const {
      secondaryProgressDataStore,
      primaryProgressDataStore,
    } = uploadDataStore;
    const {
      firstLoad,
      setFirstLoad,
      fetchFiles,
      fileActionStore,
      selection,
      setSelections,
      dragging,
      setDragging,
      setIsLoading,
      isLoading,
      viewAs,
      getFileInfo,
      setIsPrevSettingsModule,
      isPrevSettingsModule,
    } = filesStore;

    const { id } = fileActionStore;
    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      expandedKeys,
      setExpandedKeys,
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
    } = secondaryProgressDataStore;

    const {
      setUploadPanelVisible,
      startUpload,
      uploaded,
      converted,
    } = uploadDataStore;

    const selectionLength = isProgressFinished ? selection.length : null;
    const selectionTitle = isProgressFinished
      ? filesStore.selectionTitle
      : null;

    const { setToPreviewFile, playlist } = mediaViewerDataStore;
    if (!firstLoad) {
      if (isLoading) {
        showLoader();
      } else {
        hideLoader();
      }
    }

    return {
      homepage: config.homepage,
      firstLoad,
      dragging,
      fileActionId: id,
      viewAs,
      uploaded,
      converted,
      isRecycleBinFolder,
      isPrivacyFolder,
      isVisitor: auth.userStore.user.isVisitor,
      checkedMaintenance: auth.settingsStore.checkedMaintenance,
      setMaintenanceExist: auth.settingsStore.setMaintenanceExist,
      snackbarExist: auth.settingsStore.snackbarExist,
      expandedKeys,

      primaryProgressDataVisible,
      primaryProgressDataPercent,
      primaryProgressDataIcon,
      primaryProgressDataAlert,
      clearPrimaryProgressData,

      secondaryProgressDataStoreVisible,
      secondaryProgressDataStorePercent,
      secondaryProgressDataStoreIcon,
      secondaryProgressDataStoreAlert,

      selectionLength,
      isProgressFinished,
      selectionTitle,

      setExpandedKeys,
      setFirstLoad,
      setDragging,
      setIsLoading,
      fetchFiles,
      setUploadPanelVisible,
      setSelections,
      startUpload,
      isHeaderVisible: auth.settingsStore.isHeaderVisible,
      setHeaderVisible: auth.settingsStore.setHeaderVisible,
      personal: auth.settingsStore.personal,
      setToPreviewFile,
      playlist,
      isMediaOrImage: settingsStore.isMediaOrImage,
      getFileInfo,

      setIsPrevSettingsModule,
      isPrevSettingsModule,
    };
  }
)(withRouter(observer(Home)));
