import React from "react";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";
import { isMobile } from "react-device-detect";

import toastr from "studio/toastr";
import {
  AppServerConfig,
  FileAction,
  ShareAccessRights,
} from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";

import config from "../../package.json";
import EditingWrapperComponent from "../components/EditingWrapperComponent";
import { getTitleWithoutExst } from "../helpers/files-helpers";
import { getDefaultFileName } from "../helpers/utils";
import ItemIcon from "../components/ItemIcon";
export default function withContent(WrappedContent) {
  class WithContent extends React.Component {
    constructor(props) {
      super(props);

      const { item, fileActionId, fileActionExt } = props;
      let titleWithoutExt = getTitleWithoutExst(item);
      if (fileActionId === -1 && item.id === fileActionId) {
        titleWithoutExt = getDefaultFileName(fileActionExt);
      }

      this.state = { itemTitle: titleWithoutExt };
    }

    componentDidUpdate(prevProps) {
      const { fileActionId, fileActionExt } = this.props;
      if (fileActionId === -1 && fileActionExt !== prevProps.fileActionExt) {
        const itemTitle = getDefaultFileName(fileActionExt);
        this.setState({ itemTitle });
      }
    }

    completeAction = (id) => {
      const { editCompleteAction, item } = this.props;

      const isCancel =
        (id.currentTarget && id.currentTarget.dataset.action === "cancel") ||
        id.keyCode === 27;
      editCompleteAction(id, item, isCancel);
    };

    updateItem = () => {
      const {
        t,
        updateFile,
        renameFolder,
        item,
        setIsLoading,
        fileActionId,
        editCompleteAction,
      } = this.props;

      const { itemTitle } = this.state;
      const originalTitle = getTitleWithoutExst(item);

      setIsLoading(true);
      const isSameTitle =
        originalTitle.trim() === itemTitle.trim() || itemTitle.trim() === "";
      if (isSameTitle) {
        this.setState({
          itemTitle: originalTitle,
        });
        return editCompleteAction(fileActionId, item, isSameTitle);
      }

      item.fileExst || item.contentLength
        ? updateFile(fileActionId, itemTitle)
            .then(() => this.completeAction(fileActionId))
            .then(() =>
              toastr.success(
                t("FileRenamed", {
                  oldTitle: item.title,
                  newTitle: itemTitle + item.fileExst,
                })
              )
            )
            .catch((err) => toastr.error(err))
            .finally(() => setIsLoading(false))
        : renameFolder(fileActionId, itemTitle)
            .then(() => this.completeAction(fileActionId))
            .then(() =>
              toastr.success(
                t("FolderRenamed", {
                  folderTitle: item.title,
                  newFoldedTitle: itemTitle,
                })
              )
            )
            .catch((err) => toastr.error(err))
            .finally(() => setIsLoading(false));
    };

    cancelUpdateItem = (e) => {
      const { item } = this.props;

      const originalTitle = getTitleWithoutExst(item);
      this.setState({
        itemTitle: originalTitle,
      });

      return this.completeAction(e);
    };

    onClickUpdateItem = (e, open = true) => {
      const { fileActionType } = this.props;
      fileActionType === FileAction.Create
        ? this.createItem(e, open)
        : this.updateItem(e);
    };

    createItem = (e, open) => {
      const {
        createFile,
        item,
        setIsLoading,
        isLoading,
        openDocEditor,
        isPrivacy,
        isDesktop,
        replaceFileStream,
        t,
        setEncryptionAccess,
        createFolder,
      } = this.props;
      const { itemTitle } = this.state;

      if (isLoading) return;

      setIsLoading(true);

      const itemId = e.currentTarget.dataset.itemid;

      if (itemTitle.trim() === "") {
        toastr.warning(t("CreateWithEmptyTitle"));
        return this.completeAction(itemId);
      }

      let tab =
        !isDesktop && item.fileExst && open
          ? window.open(
              combineUrl(
                AppServerConfig.proxyURL,
                config.homepage,
                "/doceditor"
              ),
              "_blank"
            )
          : null;

      !item.fileExst && !item.contentLength
        ? createFolder(item.parentId, itemTitle)
            .then(() => this.completeAction(itemId))
            .catch((e) => toastr.error(e))
            .finally(() => {
              return setIsLoading(false);
            })
        : createFile(item.parentId, `${itemTitle}.${item.fileExst}`)
            .then((file) => {
              if (isPrivacy) {
                return setEncryptionAccess(file).then((encryptedFile) => {
                  if (!encryptedFile) return Promise.resolve();
                  toastr.info(t("Translations:EncryptedFileSaving"));
                  return replaceFileStream(
                    file.id,
                    encryptedFile,
                    true,
                    false
                  ).then(
                    () => open && openDocEditor(file.id, file.providerKey, tab)
                  );
                });
              }
              return open && openDocEditor(file.id, file.providerKey, tab);
            })
            .then(() => this.completeAction(itemId))
            .catch((e) => toastr.error(e))
            .finally(() => {
              return setIsLoading(false);
            });
    };

    renameTitle = (e) => {
      const { t, folderFormValidation } = this.props;

      let title = e.target.value;
      //const chars = '*+:"<>?|/'; TODO: think how to solve problem with interpolation escape values in i18n translate

      if (title.match(folderFormValidation)) {
        toastr.warning(t("ContainsSpecCharacter"));
      }
      title = title.replace(folderFormValidation, "_");
      return this.setState({ itemTitle: title });
    };

    getStatusByDate = () => {
      const { culture, t, item, sectionWidth, viewAs } = this.props;
      const { created, updated, version, fileExst } = item;

      const title =
        version > 1
          ? t("TitleModified")
          : fileExst
          ? t("TitleUploaded")
          : t("TitleCreated");

      const date = fileExst ? updated : created;
      const dateLabel = new Date(date).toLocaleString(culture);
      const mobile =
        (sectionWidth && sectionWidth <= 375) || isMobile || viewAs === "table";

      return mobile ? dateLabel : `${title}: ${dateLabel}`;
    };

    getTableStatusByDate = (create) => {
      const { created, updated } = this.props.item;

      const date = create ? created : updated;
      const dateLabel = new Date(date).toLocaleString(this.props.culture);
      return dateLabel;
    };

    render() {
      const { itemTitle } = this.state;
      const {
        item,
        fileActionId,
        fileActionExt,
        viewer,
        t,
        isTrashFolder,
        onFilesClick,
        viewAs,
        element,
        isDesktop,
      } = this.props;
      const {
        id,
        fileExst,
        updated,
        createdBy,
        access,
        fileStatus,
        href,
      } = item;

      const titleWithoutExt = getTitleWithoutExst(item);

      const isEdit = id === fileActionId && fileExst === fileActionExt;

      const updatedDate =
        viewAs === "table"
          ? this.getTableStatusByDate(false)
          : updated && this.getStatusByDate();
      const createdDate = this.getTableStatusByDate(true);

      const fileOwner =
        createdBy &&
        ((viewer.id === createdBy.id && t("Common:MeLabel")) ||
          createdBy.displayName);

      const accessToEdit =
        access === ShareAccessRights.FullAccess || // only badges?
        access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)

      const linkStyles = isTrashFolder //|| window.innerWidth <= 1024
        ? { noHover: true }
        : { onClick: onFilesClick };

      if (!isDesktop && !isTrashFolder) {
        linkStyles.href = item.href;
      }

      const newItems = item.new || fileStatus === 2;
      const showNew = !!newItems;
      const elementIcon = element ? (
        element
      ) : (
        <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />
      );

      return isEdit ? (
        <EditingWrapperComponent
          className={"editing-wrapper-component"}
          elementIcon={elementIcon}
          itemTitle={itemTitle}
          itemId={id}
          viewAs={viewAs}
          renameTitle={this.renameTitle}
          onClickUpdateItem={this.onClickUpdateItem}
          cancelUpdateItem={this.cancelUpdateItem}
        />
      ) : (
        <WrappedContent
          titleWithoutExt={titleWithoutExt}
          updatedDate={updatedDate}
          createdDate={createdDate}
          fileOwner={fileOwner}
          accessToEdit={accessToEdit}
          linkStyles={linkStyles}
          newItems={newItems}
          showNew={showNew}
          isTrashFolder={isTrashFolder}
          onFilesClick={onFilesClick}
          {...this.props}
        />
      );
    }
  }

  return inject(
    ({ filesActionsStore, filesStore, treeFoldersStore, auth }, {}) => {
      const { editCompleteAction } = filesActionsStore;
      const {
        setIsLoading,
        isLoading,
        openDocEditor,
        updateFile,
        renameFolder,
        createFile,
        createFolder,
        viewAs,
      } = filesStore;
      const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

      const {
        type: fileActionType,
        extension: fileActionExt,
        id: fileActionId,
      } = filesStore.fileActionStore;
      const { replaceFileStream, setEncryptionAccess } = auth;
      const {
        culture,
        isDesktopClient,
        folderFormValidation,
      } = auth.settingsStore;

      return {
        setIsLoading,
        isLoading,
        isTrashFolder: isRecycleBinFolder,
        openDocEditor,
        updateFile,
        renameFolder,
        fileActionId,
        editCompleteAction,
        fileActionType,
        createFile,
        isPrivacy: isPrivacyFolder,
        isDesktop: isDesktopClient,
        replaceFileStream,
        setEncryptionAccess,
        createFolder,
        fileActionExt,
        culture,
        homepage: config.homepage,
        viewer: auth.userStore.user,
        viewAs,
        folderFormValidation,
      };
    }
  )(observer(WithContent));
}
