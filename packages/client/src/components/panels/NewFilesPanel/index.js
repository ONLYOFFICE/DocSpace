import React from "react";
import Backdrop from "@docspace/components/backdrop";
import Link from "@docspace/components/link";
import Loader from "@docspace/components/loader";
import Text from "@docspace/components/text";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import Row from "@docspace/components/row";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
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
import config from "PACKAGE_FILE";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";
import {
  getCategoryTypeByFolderType,
  getCategoryUrl,
} from "SRC_DIR/helpers/utils";
import FilesFilter from "@docspace/common/api/files/filter";

const SharingBodyStyle = { height: `calc(100vh - 156px)` };

class NewFilesPanel extends React.Component {
  state = { readingFiles: [], inProgress: false };

  onClose = () => {
    if (this.state.inProgress) return;
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
    const { inProgress, readingFiles } = this.state;
    if (inProgress) return;

    this.setState({ inProgress: true });

    const files = [];
    const folders = [];

    for (let item of this.props.newFiles) {
      if (item.fileExst) files.push(item);
      else folders.push(item);
    }

    const fileIds = files
      .filter((f) => !readingFiles.includes(f.id.toString()))
      .map((f) => f.id);

    const folderIds = folders
      .filter((f) => !readingFiles.includes(f.id.toString()))
      .map((f) => f.id);

    this.props
      .markAsRead(folderIds, fileIds)
      //.then(() => this.setNewBadgeCount())
      .then(() => {
        const { hasNew, refreshFiles } = this.props;

        return Promise.resolve(); //hasNew ? refreshFiles() :
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        this.setState({ inProgress: false }, () => {
          this.onClose();
        });
      });
  };

  onNewFileClick = (e) => {
    if (this.state.inProgress) return;

    this.setState({ inProgress: true });

    const { id, extension: fileExst } = e.target.dataset;

    const { /* updateFolderBadge, */ markAsRead, newFiles, refreshFiles } =
      this.props;
    const readingFiles = this.state.readingFiles;

    const fileIds = fileExst ? [id] : [];
    const folderIds = fileExst ? [] : [id];

    const item = newFiles.find((file) => file.id.toString() === id);

    if (readingFiles.includes(id)) {
      this.setState({ inProgress: false });
      return this.onFileClick(item);
    }

    markAsRead(folderIds, fileIds, item)
      .then(() => {
        //updateFolderBadge(folderId, 1);

        readingFiles.push(id);
        this.setState({ readingFiles, inProgress: false });

        this.onFileClick(item);
      })
      .then(() => {
        // refreshFiles();
      })
      .catch((err) => toastr.error(err));
  };

  onFileClick = (item) => {
    const {
      id,
      fileExst,
      webUrl,
      fileType,
      providerKey,
      rootFolderType,

      title,
    } = item;
    const {
      setMediaViewerData,

      addFileToRecentlyViewed,
      playlist,

      currentFolderId,
      setIsLoading,
    } = this.props;

    if (!fileExst) {
      const categoryType = getCategoryTypeByFolderType(rootFolderType, id);

      const state = { title, rootFolderType, isRoot: false };
      setIsLoading(true);

      const url = getCategoryUrl(categoryType, id);

      const filter = FilesFilter.getDefault();
      filter.folder = id;

      window.DocSpace.navigate(`${url}?${filter.toUrlParams()}`, { state });

      this.setState({ inProgress: false }, () => {
        this.onClose();
      });
    } else {
      const canEdit = [5, 6, 7].includes(fileType); //TODO: maybe dirty

      const isMediaActive = playlist.findIndex((el) => el.fileId === id) !== -1;

      const isMedia =
        item?.viewAccessability?.ImageView ||
        item?.viewAccessability?.MediaView;

      if (canEdit && providerKey) {
        return addFileToRecentlyViewed(id)
          .then(() => console.log("Pushed to recently viewed"))
          .catch((e) => console.error(e))
          .finally(
            window.open(
              combineUrl(
                window.DocSpaceConfig?.proxy?.url,
                config.homepage,
                `/doceditor?fileId=${id}`
              ),
              window.DocSpaceConfig?.editor?.openOnNewPage ? "_blank" : "_self"
            )
          );
      }

      if (isMedia) {
        if (currentFolderId !== item.folderId) {
          const categoryType = getCategoryTypeByFolderType(
            rootFolderType,
            item.folderId
          );

          const state = {
            title: "",
            rootFolderType,

            isRoot: false,
          };
          setIsLoading(true);

          const url = getCategoryUrl(categoryType, item.folderId);

          const filter = FilesFilter.getDefault();
          filter.folder = id;

          window.DocSpace.navigate(`${url}?${filter.toUrlParams()}`, { state });

          const mediaItem = { visible: true, id };
          setMediaViewerData(mediaItem);

          this.setState({ inProgress: false }, () => {
            this.onClose();
          });
        } else {
          const mediaItem = { visible: true, id };
          setMediaViewerData(mediaItem);

          return this.onClose();
        }

        return;
      }

      return window.open(webUrl, "_blank");
    }
  };

  // setNewBadgeCount = () => {
  //   const {
  //     newFilesIds,
  //     updateFoldersBadge,
  //     updateFilesBadge,
  //     updateRootBadge,
  //     updateFolderBadge,
  //     pathParts,
  //     newFiles,
  //   } = this.props;

  //   const { readingFiles } = this.state;

  //   const filesCount = newFiles.filter(
  //     (f) => !readingFiles.includes(f.id.toString())
  //   ).length;
  // updateRootBadge(+newFilesIds[0], filesCount);

  // if (newFilesIds.length <= 1) {
  //   if (pathParts[0] === +newFilesIds[0]) {
  //     updateFoldersBadge();
  //     updateFilesBadge();
  //   }
  // } else {
  //   updateFolderBadge(newFilesIds[newFilesIds.length - 1], filesCount);
  // }
  //};

  render() {
    //console.log("NewFiles panel render");
    const { t, visible, isLoading, newFiles, theme } = this.props;
    const { inProgress } = this.state;
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
                          color={theme.filesPanels.color}
                          isTextOverflow
                          truncate
                          title={file.title}
                          fontSize="14px"
                          className="files-new-link"
                          data-id={file.id}
                          data-extension={file.fileExst}
                        >
                          {file.title}
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
                className="new_files_panel-button new_file_panel-first-button"
                label={t("MarkAsRead")}
                size="normal"
                primary
                onClick={this.onMarkAsRead}
                isLoading={inProgress}
              />
              <Button
                className="new_files_panel-button"
                label={t("Common:CloseButton")}
                size="normal"
                isDisabled={inProgress}
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
    clientLoadingStore,
  }) => {
    const {
      addFileToRecentlyViewed,

      hasNew,
      refreshFiles,
    } = filesStore;

    const { setIsSectionFilterLoading, isLoading } = clientLoadingStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
    };

    //const { updateRootBadge } = treeFoldersStore;
    const { playlist, setMediaViewerData, setCurrentItem } =
      mediaViewerDataStore;
    const { getIcon, getFolderIcon } = settingsStore;
    const { markAsRead } = filesActionsStore;
    const { pathParts, id: currentFolderId } = selectedFolderStore;

    const {
      setNewFilesPanelVisible,
      newFilesPanelVisible: visible,
      newFilesIds,
      newFiles,
    } = dialogsStore;

    return {
      pathParts,
      visible,
      newFiles,
      newFilesIds,
      isLoading,

      playlist,
      setCurrentItem,
      currentFolderId,

      setMediaViewerData,
      addFileToRecentlyViewed,
      getIcon,
      getFolderIcon,
      markAsRead,
      setNewFilesPanelVisible,

      theme: auth.settingsStore.theme,
      hasNew,
      refreshFiles,

      setIsLoading,
    };
  }
)(
  withTranslation(["NewFilesPanel", "Common"])(
    withLoader(observer(NewFilesPanel))(<Loaders.DialogAsideLoader isPanel />)
  )
);
