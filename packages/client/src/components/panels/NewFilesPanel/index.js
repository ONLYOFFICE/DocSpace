import { useState, useEffect, useMemo } from "react";
import Backdrop from "@docspace/components/backdrop";
import Loader from "@docspace/components/loader";
import Text from "@docspace/components/text";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import Row from "@docspace/components/row";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import Portal from "@docspace/components/portal";
import { isMobileOnly } from "react-device-detect";
import { ReactSVG } from "react-svg";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledFooter,
  StyledSharingBody,
  StyledLink,
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

const NewFilesPanel = (props) => {
  const {
    setNewFilesPanelVisible,
    getIcon,
    getFolderIcon,
    newFiles,
    markAsRead,
    setMediaViewerData,
    addFileToRecentlyViewed,
    playlist,
    currentFolderId,
    setIsLoading,
    t,
    visible,
    isLoading,
  } = props;

  const [listFiles, setListFiles] = useState(newFiles);
  const [inProgress, setInProgress] = useState(false);
  const [currentOpenFileId, setCurrentOpenFileId] = useState(null);

  const onClose = () => {
    if (inProgress) return;
    setNewFilesPanelVisible(false);
  };

  const getItemIcon = (item) => {
    const extension = item.fileExst;
    const icon = extension
      ? getIcon(24, extension)
      : getFolderIcon(item.providerKey, 24);

    const svgLoader = () => <div style={{ width: "24px" }} />;

    return (
      <ReactSVG
        beforeInjection={(svg) => svg.setAttribute("style", "margin-top: 4px")}
        src={icon}
        loading={svgLoader}
      />
    );
  };

  const onMarkAsRead = () => {
    if (inProgress) return;
    setInProgress(true);

    const files = [];
    const folders = [];

    for (let item of listFiles) {
      if (item.fileExst) files.push(item);
      else folders.push(item);
    }

    const fileIds = files.map((f) => f.id);
    const folderIds = folders.map((f) => f.id);

    markAsRead(folderIds, fileIds)
      .then(() => {
        return Promise.resolve(); //hasNew ? refreshFiles() :
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        setInProgress(false);
        onClose();
      });
  };

  const onNewFileClick = (e) => {
    if (inProgress) return;

    setInProgress(true);

    const { id, extension: fileExst } = e.target.dataset;

    setCurrentOpenFileId(id);

    const fileIds = fileExst ? [id] : [];
    const folderIds = fileExst ? [] : [id];

    const item = newFiles.find((file) => file.id.toString() === id);

    markAsRead(folderIds, fileIds, item)
      .then(() => {
        onFileClick(item);

        const newListFiles = listFiles.filter(
          (file) => file.id.toString() !== id
        );

        setListFiles(newListFiles);
        if (!newListFiles.length) onClose();
      })
      .catch((err) => {
        toastr.error(err);
      })
      .finally(() => {
        setInProgress(false);
        setCurrentOpenFileId(null);
      });
  };

  const onFileClick = (item) => {
    const {
      id,
      fileExst,
      webUrl,
      fileType,
      providerKey,
      rootFolderType,

      title,
    } = item;

    if (!fileExst) {
      const categoryType = getCategoryTypeByFolderType(rootFolderType, id);

      const state = { title, rootFolderType, isRoot: false };
      setIsLoading(true);

      const url = getCategoryUrl(categoryType, id);

      const filter = FilesFilter.getDefault();
      filter.folder = id;

      window.DocSpace.navigate(`${url}?${filter.toUrlParams()}`, { state });

      setInProgress(false);
      onClose();
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
          filter.folder = item.folderId;

          window.DocSpace.navigate(`${url}?${filter.toUrlParams()}`, { state });

          const mediaItem = { visible: true, id };
          setMediaViewerData(mediaItem);

          setInProgress(false);
          onClose();
        } else {
          const mediaItem = { visible: true, id };
          setMediaViewerData(mediaItem);

          return onClose();
        }

        return;
      }

      return window.open(webUrl, "_blank");
    }
  };

  const filesListNode = useMemo(() => {
    return listFiles.map((file) => {
      const element = getItemIcon(file);

      return (
        <Row
          key={file.id}
          element={element}
          inProgress={currentOpenFileId === file.id.toString()}
        >
          <StyledLink
            onClick={onNewFileClick}
            containerWidth="100%"
            type="page"
            fontWeight={600}
            isTextOverflow
            truncate
            title={file.title}
            fontSize="14px"
            className="files-new-link"
            data-id={file.id}
            data-extension={file.fileExst}
          >
            {file.title}
          </StyledLink>
        </Row>
      );
    });
  }, [onNewFileClick, getItemIcon, currentOpenFileId]);

  const element = (
    <StyledAsidePanel visible={visible}>
      <Backdrop
        onClick={onClose}
        visible={visible}
        zIndex={310}
        isAside={true}
      />
      <Aside className="header_aside-panel" visible={visible} onClose={onClose}>
        <StyledContent>
          <StyledHeaderContent>
            <Heading className="files-operations-header" size="medium" truncate>
              {t("NewFiles")}
            </Heading>
          </StyledHeaderContent>
          {!isLoading ? (
            <StyledBody className="files-operations-body">
              <StyledSharingBody stype="mediumBlack" style={SharingBodyStyle}>
                {filesListNode}
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
              label={t("Viewed")}
              size="normal"
              primary
              onClick={onMarkAsRead}
              isLoading={inProgress}
            />
            <Button
              className="new_files_panel-button"
              label={t("Common:CloseButton")}
              size="normal"
              isDisabled={inProgress}
              onClick={onClose}
            />
          </StyledFooter>
        </StyledContent>
      </Aside>
    </StyledAsidePanel>
  );

  return isMobileOnly ? <Portal element={element} /> : element;
};

export default inject(
  ({
    auth,
    filesStore,
    mediaViewerDataStore,
    filesActionsStore,
    selectedFolderStore,
    dialogsStore,
    settingsStore,
    clientLoadingStore,
  }) => {
    const { addFileToRecentlyViewed, hasNew, refreshFiles } = filesStore;

    const { setIsSectionFilterLoading, isLoading } = clientLoadingStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
    };

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
