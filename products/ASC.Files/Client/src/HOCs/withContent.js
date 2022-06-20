import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "studio/toastr";
import {
  AppServerConfig,
  FileAction,
  FileStatus,
  ShareAccessRights,
} from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import getCorrectDate from "@appserver/components/utils/getCorrectDate";
import { LANGUAGE } from "@appserver/common/constants";
import config from "../../package.json";
import EditingWrapperComponent from "../components/EditingWrapperComponent";
import { getTitleWithoutExst } from "../helpers/files-helpers";
import { getDefaultFileName } from "../helpers/utils";
import ItemIcon from "../components/ItemIcon";

export default function withContent(WrappedContent) {
  class WithContent extends React.Component {
    constructor(props) {
      super(props);

      let titleWithoutExt = props.titleWithoutExt;

      this.state = { itemTitle: titleWithoutExt };
    }

    componentDidUpdate(prevProps) {
      const { titleWithoutExt } = this.props;

      if (titleWithoutExt !== this.state.itemTitle) {
        this.setState({ itemTitle: titleWithoutExt });
      }
    }

    getStatusByDate = (create) => {
      const { culture, item, personal } = this.props;
      const { created, updated } = item;

      const locale = personal ? localStorage.getItem(LANGUAGE) : culture;

      const date = create ? created : updated;

      const dateLabel = getCorrectDate(locale, date);

      return dateLabel;
    };

    render() {
      const {
        element,
        isDesktop,
        isTrashFolder,
        item,
        onFilesClick,
        t,

        viewer,

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

      return (
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
        openDocEditor,
        renameFolder,
        setIsLoading,
        updateFile,
        viewAs,
        setIsUpdatingRowItem,
        isUpdatingRowItem,
        passwordEntryProcess,
        addActiveItems,
        gallerySelected,
        setCreatedItem,
      } = filesStore;
      const { clearActiveOperations, fileCopyAs } = uploadDataStore;
      const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

      const { replaceFileStream, setEncryptionAccess } = auth;

      const {
        culture,
        personal,
        folderFormValidation,
        isDesktopClient,
      } = auth.settingsStore;

      const {
        setConvertPasswordDialogVisible,
        setConvertItem,
        setFormCreationInfo,
      } = dialogsStore;

      const titleWithoutExt = getTitleWithoutExst(item, false);

      return {
        createFile,
        createFolder,
        culture,
        editCompleteAction,

        folderFormValidation,
        homepage: config.homepage,
        isDesktop: isDesktopClient,
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

        titleWithoutExt,

        gallerySelected,
        setCreatedItem,
        personal,
      };
    }
  )(observer(WithContent));
}
