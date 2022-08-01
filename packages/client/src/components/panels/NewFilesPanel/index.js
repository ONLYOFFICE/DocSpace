import React from "react";
import { withRouter } from "react-router";
import Backdrop from "@docspace/components/backdrop";
import Link from "@docspace/components/link";
import Loader from "@docspace/components/loader";
import Text from "@docspace/components/text";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import Row from "@docspace/components/row";
import Box from "@docspace/components/box";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";
import toastr from "client/toastr";
import { ReactSVG } from "react-svg";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledFooter,
  StyledSharingBody,
} from "../StyledPanels";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

const SharingBodyStyle = { height: `calc(100vh - 156px)` };

class NewFilesPanel extends React.Component {
  state = { readingFiles: [], inProgress: false };

  onClose = () => {
    this.props.setNewFilesPanelVisible(false);
  };

  getItemIcon = (item) => {
    const extension = item.fileExst;
    const icon = extension
      ? this.props.getIcon(24, extension)
      : this.props.getFolderIcon(item.providerKey, 24);

    return (
      <ReactSVG
        beforeInjection={(svg) => svg.setAttribute("style", "margin-top: 4px")}
        src={icon}
        loading={this.svgLoader}
      />
    );
  };

  onMarkAsRead = () => {
    const fileIds = [];
    const folderIds = [];

    for (let item of this.props.newFiles) {
      if (item.fileExst) fileIds.push(item.id);
      else folderIds.push(item.id);
    }

    this.setState({ inProgress: true });

    this.props
      .markAsRead(folderIds, fileIds)
      .then(() => this.setNewBadgeCount())
      .then(() => {
        const { hasNew, refreshFiles } = this.props;

        return hasNew ? refreshFiles() : Promise.resolve();
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        this.setState({ inProgress: false });
        this.onClose();
      });
  };

  onNewFileClick = (e) => {
    const { id, extension: fileExst } = e.target.dataset;

    const { /* updateFolderBadge, */ markAsRead } = this.props;
    const readingFiles = this.state.readingFiles;

    const fileIds = fileExst ? [id] : [];
    const folderIds = fileExst ? [] : [id];

    if (readingFiles.includes(id)) return this.onFileClick(item);
    markAsRead(folderIds, fileIds, item)
      .then(() => {
        //updateFolderBadge(folderId, 1);

        readingFiles.push(id);
        this.setState({ readingFiles });
        this.onFileClick(item);
      })
      .catch((err) => toastr.error(err));
  };

  onFileClick = (item) => {
    const { id, fileExst, webUrl, fileType, providerKey } = item;
    const {
      filter,
      //setMediaViewerData,
      fetchFiles,
      addFileToRecentlyViewed,
    } = this.props;

    if (!fileExst) {
      fetchFiles(id, filter)
        .catch((err) => toastr.error(err))
        .finally(() => this.onClose());
    } else {
      const canEdit = [5, 6, 7].includes(fileType); //TODO: maybe dirty
      const isMedia = [2, 3, 4].includes(fileType);

      if (canEdit && providerKey) {
        return addFileToRecentlyViewed(id)
          .then(() => console.log("Pushed to recently viewed"))
          .catch((e) => console.error(e))
          .finally(
            window.open(
              combineUrl(
                AppServerConfig.proxyURL,
                config.homepage,
                `/doceditor?fileId=${id}`
              ),
              "_blank"
            )
          );
      }

      if (isMedia) {
        //const mediaItem = { visible: true, id };
        //setMediaViewerData(mediaItem);
        return;
      }

      return window.open(webUrl, "_blank");
    }
  };

  setNewBadgeCount = () => {
    const {
      newFilesIds,
      updateFoldersBadge,
      updateFilesBadge,
      updateRootBadge,
      updateFolderBadge,
      pathParts,
      newFiles,
    } = this.props;

    const filesCount = newFiles.length;
    updateRootBadge(+newFilesIds[0], filesCount);

    if (newFilesIds.length <= 1) {
      if (pathParts[0] === +newFilesIds[0]) {
        updateFoldersBadge();
        updateFilesBadge();
      }
    } else {
      updateFolderBadge(newFilesIds[newFilesIds.length - 1], filesCount);
    }
  };

  render() {
    //console.log("NewFiles panel render");
    const { t, visible, isLoading, newFiles, theme } = this.props;
    const zIndex = 310;

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside
          className="header_aside-panel"
          visible={visible}
          onClose={this.onClose}
        >
          <StyledContent>
            <StyledHeaderContent>
              <Heading
                className="files-operations-header"
                size="medium"
                truncate
              >
                {t("NewFiles")}
              </Heading>
            </StyledHeaderContent>
            {!isLoading ? (
              <StyledBody className="files-operations-body">
                <StyledSharingBody stype="mediumBlack" style={SharingBodyStyle}>
                  {newFiles.map((file) => {
                    const element = this.getItemIcon(file);
                    return (
                      <Row key={file.id} element={element}>
                        <Link
                          onClick={this.onNewFileClick}
                          containerWidth="100%"
                          type="page"
                          fontWeight={600}
                          color="#333"
                          isTextOverflow
                          truncate
                          title={file.title}
                          fontSize="14px"
                          className="files-new-link"
                          data-id={file.id}
                          data-extension={file.fileExst}
                        >
                          <Link
                            containerWidth="100%"
                            type="page"
                            fontWeight="bold"
                            color={theme.filesPanels.color}
                            isTextOverflow
                            truncate
                            title={file.title}
                            fontSize="14px"
                            className="files-new-link"
                          >
                            {file.title}
                          </Link>
                        </Link>
                      </Row>
                    );
                  })}
                </StyledSharingBody>
              </StyledBody>
            ) : (
              <div key="loader" className="panel-loader-wrapper">
                <Loader type="oval" size="16px" className="panel-loader" />
                <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                  "Common:LoadingDescription"
                )}`}</Text>
              </div>
            )}
            <StyledFooter>
              <Button
                className="new_files_panel-button"
                label={t("MarkAsRead")}
                size="normal"
                primary
                onClick={this.onMarkAsRead}
                isLoading={this.state.inProgress}
              />
              <Button
                className="sharing_panel-button"
                label={t("Common:CloseButton")}
                size="normal"
                onClick={this.onClose}
              />
            </StyledFooter>
          </StyledContent>
        </Aside>
      </StyledAsidePanel>
    );
  }
}

export default inject(
  ({
    auth,
    filesStore,
    mediaViewerDataStore,
    treeFoldersStore,
    filesActionsStore,
    selectedFolderStore,
    dialogsStore,
    settingsStore,
  }) => {
    const {
      fetchFiles,
      filter,
      addFileToRecentlyViewed,
      //setIsLoading,
      isLoading,
      updateFilesBadge,
      updateFolderBadge,
      updateFoldersBadge,
      hasNew,
      refreshFiles,
    } = filesStore;
    const { updateRootBadge } = treeFoldersStore;
    const { setMediaViewerData } = mediaViewerDataStore;
    const { getIcon, getFolderIcon } = settingsStore;
    const { markAsRead } = filesActionsStore;
    const { pathParts } = selectedFolderStore;

    const {
      setNewFilesPanelVisible,
      newFilesPanelVisible: visible,
      newFilesIds,
      newFiles,
    } = dialogsStore;

    return {
      filter,
      pathParts,
      visible,
      newFiles,
      newFilesIds,
      isLoading,

      //setIsLoading,
      fetchFiles,
      setMediaViewerData,
      addFileToRecentlyViewed,
      getIcon,
      getFolderIcon,
      markAsRead,
      setNewFilesPanelVisible,
      updateRootBadge,
      updateFolderBadge,
      updateFoldersBadge,
      updateFilesBadge,

      theme: auth.settingsStore.theme,
      hasNew,
      refreshFiles,
    };
  }
)(
  withRouter(
    withTranslation(["NewFilesPanel", "Common"])(
      withLoader(observer(NewFilesPanel))(<Loaders.DialogAsideLoader isPanel />)
    )
  )
);
