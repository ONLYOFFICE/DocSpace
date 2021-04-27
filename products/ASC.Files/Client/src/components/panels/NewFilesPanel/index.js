import React from "react";
import { withRouter } from "react-router";
import Backdrop from "@appserver/components/backdrop";
import Link from "@appserver/components/link";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import Heading from "@appserver/components/heading";
import Aside from "@appserver/components/aside";
import Row from "@appserver/components/row";
import Box from "@appserver/components/box";
import RowContainer from "@appserver/components/row-container";
import Button from "@appserver/components/button";
import { withTranslation } from "react-i18next";
import { getNewFiles, markAsRead } from "@appserver/common/api/files";
import toastr from "studio/toastr";
import { ReactSVG } from "react-svg";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledFooter,
} from "../StyledPanels";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../package.json";
class NewFilesPanel extends React.Component {
  constructor(props) {
    super(props);

    this.state = { files: [], readingFiles: [] };
  }

  componentDidMount() {
    const { newFilesIds, setIsLoading } = this.props;
    setIsLoading(true);
    getNewFiles(newFilesIds[newFilesIds.length - 1])
      .then((files) => this.setState({ files }))
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  }

  onClose = () => {
    this.props.setNewFilesPanelVisible(false);
  };

  getItemIcon = (item, isEdit) => {
    const extension = item.fileExst;
    const icon = extension
      ? this.props.getFileIcon(extension, 24)
      : this.props.getFolderIcon(item.providerKey, 24);

    return (
      <ReactSVG
        beforeInjection={(svg) => {
          svg.setAttribute("style", "margin-top: 4px");
          isEdit && svg.setAttribute("style", "margin-left: 24px");
        }}
        src={icon}
        loading={this.svgLoader}
      />
    );
  };

  onMarkAsRead = () => {
    const fileIds = [];

    for (let item of this.state.files) {
      fileIds.push(`${item.id}`);
    }

    markAsRead([], fileIds)
      .then(() => this.setNewBadgeCount())
      .catch((err) => toastr.error(err))
      .finally(() => this.onClose());
  };

  onNewFileClick = (item) => {
    const {
      updateFileBadge,
      updateFolderBadge,
      updateRootBadge,
      markAsRead,
      newFilesIds,
    } = this.props;
    const readingFiles = this.state.readingFiles;

    markAsRead([], [item.id])
      .then(() => {
        // TODO: How update row folder badge count? Fetch?

        if (readingFiles.includes(item.id)) return this.onFileClick(item);

        updateRootBadge(+newFilesIds[0], 1);
        updateFolderBadge(item.folderId, 1);
        updateFileBadge(item.id);

        readingFiles.push(item.id);
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
      fetchFiles(id, filter).catch((err) => toastr.error(err));
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
    } = this.props;

    const filesCount = this.state.files.length;
    updateRootBadge(+newFilesIds[0], filesCount);

    if (newFilesIds.length <= 1) {
      if (pathParts[0] === +newFilesIds[0]) {
        updateFoldersBadge();
        updateFilesBadge();
      }
    } else {
      updateFolderBadge(+newFilesIds[newFilesIds.length - 1], filesCount);
    }
  };

  render() {
    //console.log("NewFiles panel render");
    const { t, visible, onClose, isLoading } = this.props;
    const { files } = this.state;
    const zIndex = 310;

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside className="header_aside-panel" visible={visible}>
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
                <RowContainer useReactWindow>
                  {files.map((file) => {
                    const element = this.getItemIcon(file);
                    return (
                      <Row key={file.id} element={element}>
                        <Box
                          onClick={this.onNewFileClick.bind(this, file)}
                          marginProp="auto 0"
                        >
                          <Link
                            containerWidth="100%"
                            type="page"
                            fontWeight="bold"
                            color="#333"
                            isTextOverflow
                            truncate
                            title={file.title}
                            fontSize="14px"
                            className="files-new-link"
                          >
                            {file.title}
                          </Link>
                        </Box>
                      </Row>
                    );
                  })}
                </RowContainer>
              </StyledBody>
            ) : (
              <div key="loader" className="panel-loader-wrapper">
                <Loader type="oval" size="16px" className="panel-loader" />
                <Text as="span">{t("LoadingLabel")}</Text>
              </div>
            )}
            <StyledFooter>
              <Button
                className="new_files_panel-button"
                label={t("MarkAsRead")}
                size="big"
                primary
                onClick={this.onMarkAsRead}
              />
              <Button
                className="sharing_panel-button"
                label={t("CloseButton")}
                size="big"
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
    filesStore,
    mediaViewerDataStore,
    treeFoldersStore,
    formatsStore,
    filesActionsStore,
    selectedFolderStore,
    dialogsStore,
  }) => {
    const {
      fetchFiles,
      filter,
      addFileToRecentlyViewed,
      setIsLoading,
      isLoading,
      updateFileBadge,
      updateFilesBadge,
      updateFolderBadge,
      updateFoldersBadge,
    } = filesStore;
    const { updateRootBadge } = treeFoldersStore;
    const { setMediaViewerData } = mediaViewerDataStore;
    const { getFileIcon, getFolderIcon } = formatsStore.iconFormatsStore;
    const { markAsRead } = filesActionsStore;
    const { pathParts } = selectedFolderStore;

    const {
      setNewFilesPanelVisible,
      newFilesPanelVisible: visible,
      newFilesIds,
    } = dialogsStore;

    return {
      filter,
      pathParts,
      visible,
      newFilesIds,

      isLoading,
      setIsLoading,
      fetchFiles,
      setMediaViewerData,
      addFileToRecentlyViewed,
      getFileIcon,
      getFolderIcon,
      markAsRead,
      setNewFilesPanelVisible,
      updateRootBadge,
      updateFileBadge,
      updateFolderBadge,
      updateFoldersBadge,
      updateFilesBadge,
    };
  }
)(withRouter(withTranslation("NewFilesPanel")(observer(NewFilesPanel))));
