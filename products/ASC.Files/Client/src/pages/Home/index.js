import React from "react";
//import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";
import axios from "axios";
import toastr from "@appserver/components/toast/toastr";
import PageLayout from "@appserver/common/components/PageLayout";
import { showLoader, hideLoader } from "@appserver/common/utils";
import FilesFilter from "@appserver/common/api/files/filter";
import { getGroup } from "@appserver/common/api/groups";
import { getUserById } from "@appserver/common/api/people";
import { withTranslation, Trans } from "react-i18next";
import {
  ArticleBodyContent,
  ArticleHeaderContent,
  ArticleMainButtonContent,
} from "../../components/Article";
import {
  SectionBodyContent,
  SectionFilterContent,
  SectionHeaderContent,
  SectionPagingContent,
} from "./Section";

import { createTreeFolders } from "../../helpers/files-helpers";
import MediaViewer from "./MediaViewer";
import DragTooltip from "../../components/DragTooltip";
import { observer, inject } from "mobx-react";
import config from "../../../package.json";

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
      mediaViewersFormatsStore,
      getFileInfo,
    } = this.props;

    const reg = new RegExp(`${homepage}((/?)$|/filter)`, "gm"); //TODO: Always find?
    const match = window.location.pathname.match(reg);
    let filterObj = null;

    if (window.location.href.indexOf("/files/#preview") > 1) {
      const pathname = window.location.href;
      const fileId = pathname.slice(pathname.indexOf("#preview") + 9);

      getFileInfo(fileId)
        .then((data) => {
          const canOpenPlayer = mediaViewersFormatsStore.isMediaOrImage(
            data.fileExst
          );
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
      filterObj = FilesFilter.getFilter(window.location);

      if (!filterObj) {
        setIsLoading(true);
        this.fetchDefaultFiles();

        return;
      }
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
        console.warn("Filter restored by default", err);
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
    } = this.props;
    return (
      <>
        <MediaViewer />
        <DragTooltip />
        <PageLayout
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
            secondaryProgressDataStoreVisible
          }
          isLoaded={!firstLoad}
          isHeaderVisible={isHeaderVisible}
          onOpenUploadPanel={this.showUploadPanel}
          firstLoad={firstLoad}
          dragging={dragging}
        >
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>

          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>

          <PageLayout.ArticleBody>
            <ArticleBodyContent onTreeDrop={this.onDrop} />
          </PageLayout.ArticleBody>
          <PageLayout.SectionHeader>
            <SectionHeaderContent />
          </PageLayout.SectionHeader>

          <PageLayout.SectionFilter>
            <SectionFilterContent />
          </PageLayout.SectionFilter>

          <PageLayout.SectionBody>
            <SectionBodyContent />
          </PageLayout.SectionBody>

          <PageLayout.SectionPaging>
            <SectionPagingContent tReady={tReady} />
          </PageLayout.SectionPaging>
        </PageLayout>
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
    formatsStore,
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
    } = filesStore;

    const { mediaViewersFormatsStore } = formatsStore;

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

    const { setToPreviewFile } = mediaViewerDataStore;
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
      setToPreviewFile,
      mediaViewersFormatsStore,
      getFileInfo,
    };
  }
)(withRouter(observer(Home)));
