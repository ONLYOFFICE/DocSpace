import React from "react";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";

import toastr from "studio/toastr";
import {
  AppServerConfig,
  FileAction,
  FileStatus,
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
      let titleWithoutExt = props.titleWithoutExt;
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
      const {
        fileActionId,
        fileActionExt,
        setIsUpdatingRowItem,
        isUpdatingRowItem,
        isEdit,
        titleWithoutExt,
      } = this.props;
      if (fileActionId === -1 && fileActionExt !== prevProps.fileActionExt) {
        const itemTitle = getDefaultFileName(fileActionExt);
        this.setState({ itemTitle });
      }
      if (fileActionId === null && prevProps.fileActionId !== fileActionId) {
        isUpdatingRowItem && setIsUpdatingRowItem(false);
      }

      if (!isEdit && titleWithoutExt !== this.state.itemTitle) {
        this.setState({ itemTitle: titleWithoutExt });
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
        addActiveItems,
        clearActiveOperations,
      } = this.props;

      const { itemTitle } = this.state;
      const originalTitle = getTitleWithoutExst(item);

      setIsLoading(true);
      let timerId;

      const isSameTitle =
        originalTitle.trim() === itemTitle.trim() || itemTitle.trim() === "";

      const isFile = item.fileExst || item.contentLength;

      if (isSameTitle) {
        this.setState({
          itemTitle: originalTitle,
        });
        return editCompleteAction(fileActionId, item, isSameTitle);
      } else {
        timerId = setTimeout(() => {
          isFile ? addActiveItems([item.id]) : addActiveItems(null, [item.id]);
        }, 500);
      }

      isFile
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
            .finally(() => {
              clearTimeout(timerId);
              timerId = null;
              clearActiveOperations([item.id]);

              setIsLoading(false);
            })
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
            .finally(() => {
              clearTimeout(timerId);
              timerId = null;
              clearActiveOperations(null, [item.id]);

              setIsLoading(false);
            });
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
      const {
        fileActionType,
        setIsUpdatingRowItem,
        addActiveItems,
        item,
      } = this.props;

      setIsUpdatingRowItem(true);

      if (fileActionType === FileAction.Create) {
        !item.fileExst && !item.contentLength
          ? addActiveItems(null, [item.id])
          : addActiveItems([item.id]);
        this.createItem(e, open);
      } else {
        this.updateItem(e);
      }
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
        setConvertPasswordDialogVisible,
        setFormCreationInfo,
        setIsUpdatingRowItem,
        clearActiveOperations,
        addActiveItems,
        fileCopyAs,
      } = this.props;
      const { itemTitle } = this.state;

      const isMakeFormFromFile = fileActionTemplateId ? true : false;

      let title = itemTitle;

      if (isLoading) return;

      setIsLoading(true);

      const itemId = e.currentTarget.dataset.itemid;

      let createdFileId, createdFolderId;

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

      if (!item.fileExst && !item.contentLength) {
        createFolder(item.parentId, title)
          .then((folder) => {
            createdFolderId = folder.id;
            addActiveItems(null, [folder.id]);
          })
          .then(() => this.completeAction(itemId))
          .catch((e) => toastr.error(e))
          .finally(() => {
            const folderIds = [+itemId];
            createdFolderId && folderIds.push(createdFolderId);

            clearActiveOperations(null, folderIds);

            return setIsLoading(false);
          });
      } else {
        if (isMakeFormFromFile) {
          fileCopyAs(
            fileActionTemplateId,
            `${title}.${item.fileExst}`,
            item.parentId
          )
            .then((file) => {
              createdFileId = file.id;
              addActiveItems([file.id]);

              open && openDocEditor(file.id, file.providerKey, tab);
            })
            .then(() => this.completeAction(itemId))
            .catch((err) => {
              console.log("err", err);
              const isPasswordError = new RegExp(/\(password\)*$/);

              if (isPasswordError.test(err)) {
                toastr.error(
                  t("Translations:FileProtected"),
                  t("Common:Warning")
                );
                setIsUpdatingRowItem(false);

                setFormCreationInfo({
                  newTitle: `${title}.${item.fileExst}`,
                  fromExst: ".docx",
                  toExst: item.fileExst,
                  open,
                  actionId: itemId,
                  fileInfo: {
                    id: fileActionTemplateId,
                    folderId: item.parentId,
                    fileExst: item.fileExst,
                  },
                });
                setConvertPasswordDialogVisible(true);

                open && openDocEditor(null, null, tab);
              }
            })
            .finally(() => {
              const fileIds = [+itemId];
              createdFileId && fileIds.push(createdFileId);

              clearActiveOperations(fileIds);

              return setIsLoading(false);
            });
        } else {
          createFile(item.parentId, `${title}.${item.fileExst}`)
            .then((file) => {
              createdFileId = file.id;
              addActiveItems([file.id]);

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
              const fileIds = [+itemId];
              createdFileId && fileIds.push(createdFileId);

              clearActiveOperations(fileIds);

              return setIsLoading(false);
            });
        }
      }
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
        isDesktop,
        isTrashFolder,
        item,
        onFilesClick,
        t,
        viewAs,
        viewer,
        isUpdatingRowItem,
        passwordEntryProcess,
        isEdit,
        titleWithoutExt,
      } = this.props;
      const { access, createdBy, fileExst, fileStatus, href, icon, id } = item;

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

      const newItems =
        item.new || (fileStatus & FileStatus.IsNew) === FileStatus.IsNew;
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
          isUpdatingRowItem={isUpdatingRowItem}
          passwordEntryProcess={passwordEntryProcess}
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
    (
      {
        filesActionsStore,
        filesStore,
        treeFoldersStore,
        auth,
        dialogsStore,
        uploadDataStore,
      },
      { item }
    ) => {
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
        setIsUpdatingRowItem,
        isUpdatingRowItem,
        passwordEntryProcess,
        addActiveItems,
      } = filesStore;
      const { clearActiveOperations, fileCopyAs } = uploadDataStore;
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

      const {
        setConvertPasswordDialogVisible,
        setConvertItem,
        setFormCreationInfo,
      } = dialogsStore;

      const isEdit =
        item.id === fileActionId && item.fileExst === fileActionExt;

      const titleWithoutExt = getTitleWithoutExst(item);

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
        setConvertPasswordDialogVisible,
        setConvertItem,
        setFormCreationInfo,
        setIsUpdatingRowItem,
        isUpdatingRowItem,
        passwordEntryProcess,
        addActiveItems,
        clearActiveOperations,
        fileCopyAs,
        isEdit,
        titleWithoutExt,
      };
    }
  )(observer(WithContent));
}
