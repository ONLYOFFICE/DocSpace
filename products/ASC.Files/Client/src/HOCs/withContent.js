import React from "react";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";

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

      const { item, fileActionId, fileActionExt, fileActionTemplateId } = props;
      let titleWithoutExt = getTitleWithoutExst(item);
      if (
        fileActionId === -1 &&
        item.id === fileActionId &&
        fileActionTemplateId === null
      ) {
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
        createFolder,
        fileActionTemplateId,
        isDesktop,
        isLoading,
        isPrivacy,
        item,
        openDocEditor,
        replaceFileStream,
        setEncryptionAccess,
        setIsLoading,
        t,
      } = this.props;
      const { itemTitle } = this.state;

      let title = itemTitle;

      if (isLoading) return;

      setIsLoading(true);

      const itemId = e.currentTarget.dataset.itemid;

      if (itemTitle.trim() === "") {
        title =
          fileActionTemplateId === null
            ? getDefaultFileName(item.fileExst)
            : getTitleWithoutExst(item);

        this.setState({
          itemTitle: title,
        });
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
        ? createFolder(item.parentId, title)
            .then(() => this.completeAction(itemId))
            .catch((e) => toastr.error(e))
            .finally(() => {
              return setIsLoading(false);
            })
        : createFile(
            item.parentId,
            `${title}.${item.fileExst}`,
            fileActionTemplateId
          )
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

    getStatusByDate = (create) => {
      const { culture, item } = this.props;
      const { created, updated } = item;

      const options = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "numeric",
      };

      const date = create ? created : updated;

      const dateLabel = new Date(date)
        .toLocaleString(culture, options)
        .replace(",", "");

      return dateLabel;
    };

    render() {
      const { itemTitle } = this.state;
      const {
        element,
        fileActionExt,
        fileActionId,
        isDesktop,
        isTrashFolder,
        item,
        onFilesClick,
        t,
        viewAs,
        viewer,
      } = this.props;
      const { access, createdBy, fileExst, fileStatus, href, icon, id } = item;

      const titleWithoutExt = getTitleWithoutExst(item);

      const isEdit = id === fileActionId && fileExst === fileActionExt;

      const updatedDate = this.getStatusByDate(false);
      const createdDate = this.getStatusByDate(true);

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
        linkStyles.href = href;
      }

      const newItems = item.new || fileStatus === 2;
      const showNew = !!newItems;
      const elementIcon = element ? (
        element
      ) : (
        <ItemIcon id={id} icon={icon} fileExst={fileExst} />
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
        createFile,
        createFolder,
        isLoading,
        openDocEditor,
        renameFolder,
        setIsLoading,
        updateFile,
        viewAs,
      } = filesStore;
      const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

      const {
        extension: fileActionExt,
        id: fileActionId,
        templateId: fileActionTemplateId,
        type: fileActionType,
      } = filesStore.fileActionStore;
      const { replaceFileStream, setEncryptionAccess } = auth;
      const {
        culture,
        folderFormValidation,
        isDesktopClient,
      } = auth.settingsStore;

      return {
        createFile,
        createFolder,
        culture,
        editCompleteAction,
        fileActionExt,
        fileActionId,
        fileActionTemplateId,
        fileActionType,
        folderFormValidation,
        homepage: config.homepage,
        isDesktop: isDesktopClient,
        isLoading,
        isPrivacy: isPrivacyFolder,
        isTrashFolder: isRecycleBinFolder,
        openDocEditor,
        renameFolder,
        replaceFileStream,
        setEncryptionAccess,
        setIsLoading,
        updateFile,
        viewAs,
        viewer: auth.userStore.user,
      };
    }
  )(observer(WithContent));
}
