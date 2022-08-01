import React from "react";
import Backdrop from "@docspace/components/backdrop";
import Button from "@docspace/components/button";

import Aside from "@docspace/components/aside";
import SaveCancelButton from "@docspace/components/save-cancel-buttons";
import { withTranslation, Trans } from "react-i18next";
import toastr from "client/toastr";
import { ShareAccessRights } from "@docspace/common/constants";
import { StyledAsidePanel } from "../StyledPanels";
import { AddUsersPanel, AddGroupsPanel, EmbeddingPanel } from "../index";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { isMobile, isMobileOnly } from "react-device-detect";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";
import ModalDialog from "@docspace/components/modal-dialog";
import EmbeddingBody from "../EmbeddingPanel/EmbeddingBody";

import { StyledContent, StyledModalFooter } from "./StyledSharingPanel";

import Header from "./Header";
import Body from "./Body";
import Footer from "./Footer";
import SharingPanelLoader from "@docspace/common/components/Loaders/SharingPanelLoader";
import SharingPanelLoaderModal from "@docspace/common/components/Loaders/SharingPanelLoader/modal";

// const SharingBodyStyle = { height: `calc(100vh - 156px)` };

class SharingPanelComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isNotifyUsers: false,
      shareDataItems: [],
      baseShareData: [],
      message: "",
      showAddUsersPanel: false,
      showEmbeddingPanel: false,
      showAddGroupsPanel: false,
      showChangeOwnerPanel: false,
      shareLink: "",
      isLoadedShareData: false,
      showPanel: false,
      accessOptions: [],
      filesOwnerId: null,
      isUpdated: false,
      isLoading: false,
      showEmbeddingContent: false,
      baseExternalAccess: null,
    };

    this.scrollRef = React.createRef();
  }

  isUpdateAccessInfo = (selectedAccess) => {
    const { baseExternalAccess, isUpdated } = this.state;

    if (+baseExternalAccess !== +selectedAccess) {
      !isUpdated && this.setState({ isUpdated: true });
    } else {
      isUpdated && this.setState({ isUpdated: false });
    }
  };
  onToggleLink = (item) => {
    const { shareDataItems } = this.state;
    const { isPersonal } = this.props;
    const { DenyAccess, ReadOnly } = ShareAccessRights;

    const rights = item.access !== DenyAccess ? DenyAccess : ReadOnly;
    const newDataItems = JSON.parse(JSON.stringify(shareDataItems));
    newDataItems[0].access = rights;

    isPersonal && this.isUpdateAccessInfo(rights);

    this.setState({
      shareDataItems: newDataItems,
      showEmbeddingContent: false,
    });
  };

  updateRowData = (newRowData) => {
    const { getFileInfo, getFolderInfo, isFolderActions, id } = this.props;

    if (isFolderActions) {
      return getFolderInfo(id);
    }

    for (let item of newRowData) {
      !item.fileExst ? getFolderInfo(item.id) : getFileInfo(item.id);
    }
  };

  onSaveClick = () => {
    const { baseShareData, shareDataItems, filesOwnerId } = this.state;
    const { selection, isFolderActions } = this.props;

    let folderIds = [];
    let fileIds = [];
    const share = [];

    let externalAccess = null;

    for (let item of shareDataItems) {
      const baseItem = baseShareData.find(
        (x) => x.sharedTo.id === item.sharedTo.id
      );

      if (
        (baseItem &&
          baseItem.access !== item.access &&
          !item.sharedTo.shareLink) ||
        (!item.isOwner && !baseItem)
      ) {
        share.push({ shareTo: item.sharedTo.id, access: item.access });
      }

      if (item.sharedTo.shareLink && item.access !== baseItem.access) {
        externalAccess = item.access;
      }
    }

    for (let item of baseShareData) {
      const baseItem = shareDataItems.find(
        (x) => x.sharedTo.id === item.sharedTo.id
      );
      if (!baseItem) {
        share.push({
          shareTo: item.sharedTo.id,
          access: ShareAccessRights.None,
        });
      }
    }

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    if (isFolderActions) {
      folderIds = [];
      fileIds = [];

      folderIds.push(selection[0]);
    }

    const owner = shareDataItems.find((x) => x.isOwner);
    const ownerId =
      filesOwnerId !== owner.sharedTo.id ? owner.sharedTo.id : null;

    this.setShareInfo(folderIds, fileIds, share, externalAccess, ownerId);
  };

  setShareInfo = (folderIds, fileIds, share, externalAccess, ownerId) => {
    const { isNotifyUsers, message } = this.state;

    const {
      selection,
      isPrivacy,
      replaceFileStream,
      t,
      uploadPanelVisible,
      updateUploadedItem,
      uploadSelection,
      isDesktop,
      setEncryptionAccess,
      setShareFiles,
      onSuccess,
      setIsFolderActions,
    } = this.props;

    this.onClose();

    setShareFiles(
      folderIds,
      fileIds,
      share,
      isNotifyUsers,
      message,
      externalAccess,
      ownerId
    )
      .then(() => {
        if (!ownerId) {
          this.updateRowData(selection);
        }
        if (isPrivacy && isDesktop) {
          if (share.length === 0) return Promise.resolve();
          selection.forEach((item) => {
            return setEncryptionAccess(item).then((encryptedFile) => {
              if (!encryptedFile) return Promise.resolve();

              toastr.info(t("Translations:EncryptedFileSaving"));

              const title = item.title;

              return replaceFileStream(item.id, encryptedFile, true, true).then(
                () =>
                  toastr.success(
                    <Trans
                      t={t}
                      i18nKey="EncryptedFileSharing"
                      ns="SharingPanel"
                    >
                      File {{ title }} successfully shared
                    </Trans>
                  )
              );
            });
          });
        }

        if (uploadPanelVisible && uploadSelection) {
          return updateUploadedItem(selection[0].id);
        }
        return Promise.resolve();
      })
      .then(() => onSuccess && onSuccess())
      .catch((err) => toastr.error(err))
      .finally(() => {
        setIsFolderActions(false);
      });
  };

  onNotifyUsersChange = () =>
    this.setState({
      isNotifyUsers: !this.state.isNotifyUsers,
    });

  onShowUsersPanel = () =>
    this.setState({
      showAddUsersPanel: !this.state.showAddUsersPanel,
    });

  onShowGroupsPanel = () =>
    this.setState({
      showAddGroupsPanel: !this.state.showAddGroupsPanel,
    });

  onShowChangeOwnerPanel = () => {
    this.setState({
      showChangeOwnerPanel: !this.state.showChangeOwnerPanel,
    });
  };

  onShowEmbeddingPanel = (link) =>
    this.setState({
      showEmbeddingPanel: !this.state.showEmbeddingPanel,
      shareLink: link,
    });

  onShowEmbeddingContainer = (link) =>
    this.setState({
      showEmbeddingContent: !this.state.showEmbeddingContent,
      shareLink: link,
    });

  onChangeItemAccess = (e) => {
    const { isPersonal } = this.props;
    const id = e.currentTarget.dataset.id;
    const access = e.currentTarget.dataset.access;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id && !x.isOwner);

    if (elem.access !== +access) {
      isPersonal && this.isUpdateAccessInfo(access);

      elem.access = +access;
      this.setState({ shareDataItems });
    }
  };

  onRemoveUserItemClick = (e) => {
    const id = e.currentTarget.dataset.for;
    const shareDataItems = this.state.shareDataItems.slice(0);

    const index = shareDataItems.findIndex((x) => x.sharedTo.id === id);
    if (index !== -1) {
      shareDataItems.splice(index, 1);
      this.setState({ shareDataItems });
    }
  };

  getData = () => {
    const { selection, id, access } = this.props;

    let folderId = [];
    let fileId = [];

    for (let item of selection) {
      if (item.access === 1 || item.access === 0) {
        if (item.fileExst) {
          fileId.push(item.id);
        } else {
          folderId.push(item.id);
        }
      }
    }

    if (this.props.isFolderActions) {
      folderId = [];
      fileId = [];

      folderId = access === 1 || access === 0 ? [id] : [];
    }

    return [folderId, fileId];
  };

  getShareData = () => {
    const returnValue = this.getData();
    const folderId = returnValue[0];
    const fileId = returnValue[1];

    if (folderId.length !== 0 || fileId.length !== 0) {
      this.setState(
        {
          isLoading: true,
        },
        function () {
          this.getShareUsers(folderId, fileId);
        }
      );
    }
  };

  getShareUsers = (folderId, fileId) => {
    const {
      getAccessOption,
      getExternalAccessOption,
      selection,
      getShareUsers,
      isPersonal,
    } = this.props;

    getShareUsers(folderId, fileId)
      .then((shareDataItems) => {
        const baseShareData = JSON.parse(JSON.stringify(shareDataItems));
        const accessOptions = getAccessOption(selection);

        const externalAccessOptions = getExternalAccessOption(selection);
        const filesOwner = shareDataItems.find((x) => x.isOwner);
        const filesOwnerId = filesOwner ? filesOwner.sharedTo.id : null;

        const baseExternalAccess = isPersonal
          ? shareDataItems.find((x) => x.sharedTo.shareLink)?.access
          : null;

        this._isMounted &&
          this.setState({
            baseShareData,
            shareDataItems,
            accessOptions,
            externalAccessOptions,
            //showPanel: true,
            filesOwnerId,
            baseExternalAccess,
          });
      })

      .catch((err) => {
        toastr.error(err);
        this.onClose();
      })
      .finally(() => {
        setTimeout(() => {
          if (this._isMounted)
            return this.setState({
              isLoading: false,
            });
        }, 500);
      });
  };

  getInternalLink = () => {
    const { homepage, selection } = this.props;

    const item = selection[0];
    const isFile = !!item?.fileExst;

    if (selection.length !== 1) return null;

    return isFile
      ? item.canOpenPlayer
        ? `${window.location.href}&preview=${item?.id}`
        : item.webUrl
      : `${window.location.origin + homepage}/filter?folder=${item?.id}`; //TODO: Change url by category
  };

  onChangeMessage = (e) => {
    this.setState({ message: e.target.value });
  };

  setShareDataItems = (shareDataItems) => {
    this.setState({ shareDataItems });
  };

  onClose = () => {
    const {
      onCancel,
      setSharingPanelVisible,
      selectUploadedFile,
      setSelection,
      setBufferSelection,
    } = this.props;

    setSharingPanelVisible(false);

    setSelection([]);

    selectUploadedFile([]);
    setBufferSelection(null);
    onCancel && onCancel();
  };

  componentDidMount() {
    const { settings, setFilesSettings } = this.props;
    settings && setFilesSettings(settings); // Remove after initialization settings in Editor

    this.getShareData();

    this._isMounted = true;
    document.addEventListener("keyup", this.onKeyPress);
    window.addEventListener("popstate", () => this.onClose());
  }

  componentWillUnmount() {
    this._isMounted = false;
    document.removeEventListener("keyup", this.onKeyPress);
    window.removeEventListener("popstate", () => this.onClose());
  }

  onKeyPress = (event) => {
    const {
      showAddUsersPanel,
      showEmbeddingPanel,
      showAddGroupsPanel,
      showChangeOwnerPanel,
    } = this.state;
    if (
      showAddUsersPanel ||
      showEmbeddingPanel ||
      showAddGroupsPanel ||
      showChangeOwnerPanel
    )
      return;
    if (event.key === "Esc" || event.key === "Escape") {
      this.onClose();
    }
  };

  componentDidUpdate(prevProps, prevState) {
    if (
      this.state.showPanel !== prevState.showPanel &&
      this.state.showPanel === false
    ) {
      this.onClose();
    }

    if (this.state.message === prevState.message && this.scrollRef.current) {
      this.scrollRef.current.view.focus();
    }
  }

  render() {
    //console.log("Sharing panel render");
    const {
      t,
      isPersonal,
      isMyId,
      selection,
      groupsCaption,
      canShareOwnerChange,
      uploadPanelVisible,
      documentTitle,
      sharingPanelVisible,
      isPrivacy,
      theme,
      isShared,
    } = this.props;
    const {
      isNotifyUsers,
      shareDataItems,
      message,
      showAddUsersPanel,
      showAddGroupsPanel,
      showEmbeddingPanel,
      showChangeOwnerPanel,
      shareLink,
      //showPanel,
      accessOptions,
      externalAccessOptions,
      showEmbeddingContent,
      isLoading,
    } = this.state;

    const visible = sharingPanelVisible;

    const zIndex = 310;

    const isEncrypted =
      isPrivacy || (selection.length && selection[0]?.encrypted);

    const internalLink =
      selection.length === 1 && !isEncrypted && this.getInternalLink();

    const filteredShareDataItems = [];
    const externalItem = [];
    const owner = [];
    const shareGroups = [];
    const shareUsers = [];

    shareDataItems.forEach((item) => {
      if (item?.sharedTo?.shareLink) {
        return externalItem.push(item);
      }

      if (item?.isOwner) {
        item.isUser = true;
        return owner.push(item);
      }

      if (
        item?.sharedTo?.userName ||
        (item?.sharedTo?.label && item.sharedTo.avatarUrl)
      ) {
        item.isUser = true;
        shareUsers.push(item);
      } else {
        item.isGroup = true;
        shareGroups.push(item);
      }
    });

    filteredShareDataItems.push(
      ...externalItem,
      ...owner,
      ...shareGroups,
      ...shareUsers
    );

    return (
      <>
        {isPersonal ? (
          <>
            {isLoading ? (
              isMobileOnly ? (
                <ModalDialog
                  displayType="modal"
                  visible={visible}
                  withoutCloseButton={true}
                  withoutBodyScroll={true}
                  scale={true}
                  onClose={this.onClose}
                  isPersonal={isPersonal}
                  modalBodyPadding="12px 0 0"
                >
                  <ModalDialog.Body>
                    <SharingPanelLoaderModal isShared={isShared} />
                  </ModalDialog.Body>
                </ModalDialog>
              ) : (
                <>
                  <ModalDialog
                    displayType="modal"
                    visible={visible}
                    withoutCloseButton={true}
                    withoutBodyScroll={true}
                    scale={true}
                    onClose={this.onClose}
                    width={"400px"}
                    isPersonal={isPersonal}
                    modalBodyPadding="12px 0 0"
                  >
                    <ModalDialog.Body>
                      <SharingPanelLoaderModal isShared={isShared} />
                    </ModalDialog.Body>
                  </ModalDialog>
                </>
              )
            ) : isMobileOnly ? (
              <ModalDialog
                displayType="modal"
                visible={visible}
                withoutCloseButton={false}
                withoutBodyScroll={true}
                scale={true}
                onClose={this.onClose}
                modalBodyPadding="12px 0 0"
                isPersonal={isPersonal}
              >
                <ModalDialog.Header>
                  <Header
                    t={t}
                    uploadPanelVisible={showEmbeddingContent}
                    isPersonal={isPersonal}
                    isEncrypted={isEncrypted}
                    onClose={this.onShowEmbeddingContainer}
                    onShowUsersPanel={this.onShowUsersPanel}
                    onShowGroupsPanel={this.onShowGroupsPanel}
                    label={t("EmbeddingPanel:EmbeddingDocument")}
                  />
                </ModalDialog.Header>

                <ModalDialog.Body>
                  {showEmbeddingContent ? (
                    <EmbeddingBody
                      isPersonal={isPersonal}
                      theme={theme}
                      embeddingLink={externalItem[0].sharedTo.shareLink}
                    />
                  ) : (
                    <Body
                      isShared={isShared}
                      t={t}
                      isPersonal={isPersonal}
                      selection={selection}
                      externalItem={externalItem[0]}
                      onToggleLink={this.onToggleLink}
                      onShowEmbeddingPanel={this.onShowEmbeddingContainer}
                      onChangeItemAccess={this.onChangeItemAccess}
                      accessOptions={accessOptions}
                      externalAccessOptions={externalAccessOptions}
                    />
                  )}
                </ModalDialog.Body>

                {!showEmbeddingContent && (
                  <ModalDialog.Footer>
                    <StyledModalFooter>
                      <SaveCancelButton
                        saveButtonLabel={t("Common:SaveButton")}
                        onSaveClick={this.onSaveClick}
                        cancelButtonLabel={t("Common:CancelButton")}
                        onCancelClick={this.onClose}
                        showReminder={true}
                      />
                    </StyledModalFooter>
                  </ModalDialog.Footer>
                )}
              </ModalDialog>
            ) : (
              <ModalDialog
                displayType="modal"
                visible={visible}
                withoutCloseButton={false}
                withoutBodyScroll={true}
                scale={true}
                onClose={this.onClose}
                width={"400px"}
                modalBodyPadding="12px 0 0"
                isPersonal={isPersonal}
              >
                <ModalDialog.Header>
                  <Header
                    t={t}
                    uploadPanelVisible={showEmbeddingContent}
                    isPersonal={isPersonal}
                    isEncrypted={isEncrypted}
                    onClose={this.onShowEmbeddingContainer}
                    onShowUsersPanel={this.onShowUsersPanel}
                    onShowGroupsPanel={this.onShowGroupsPanel}
                    label={t("EmbeddingPanel:EmbeddingDocument")}
                  />
                </ModalDialog.Header>

                <ModalDialog.Body>
                  {showEmbeddingContent ? (
                    <EmbeddingBody
                      isPersonal={isPersonal}
                      theme={theme}
                      embeddingLink={externalItem[0].sharedTo.shareLink}
                    />
                  ) : (
                    <Body
                      isShared={isShared}
                      t={t}
                      isPersonal={isPersonal}
                      selection={selection}
                      externalItem={externalItem[0]}
                      onToggleLink={this.onToggleLink}
                      onShowEmbeddingPanel={this.onShowEmbeddingContainer}
                      onChangeItemAccess={this.onChangeItemAccess}
                      accessOptions={accessOptions}
                      externalAccessOptions={externalAccessOptions}
                    />
                  )}
                </ModalDialog.Body>

                {!showEmbeddingContent && (
                  <ModalDialog.Footer>
                    <StyledModalFooter>
                      <SaveCancelButton
                        saveButtonLabel={t("Common:SaveButton")}
                        onSaveClick={this.onSaveClick}
                        cancelButtonLabel={t("Common:CancelButton")}
                        onCancelClick={this.onClose}
                        showReminder={this.state.isUpdated}
                        cancelEnable={true}
                      />
                    </StyledModalFooter>
                  </ModalDialog.Footer>
                )}
              </ModalDialog>
            )}
          </>
        ) : (
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
              withoutBodyScroll={true}
              onClose={this.onClose}
            >
              {!isLoading ? (
                <StyledContent isNotifyUsers={isNotifyUsers}>
                  <Header
                    t={t}
                    uploadPanelVisible={uploadPanelVisible}
                    isPersonal={isPersonal}
                    isEncrypted={isEncrypted}
                    onClose={this.onClose}
                    onShowUsersPanel={this.onShowUsersPanel}
                    onShowGroupsPanel={this.onShowGroupsPanel}
                  />

                  <Body
                    t={t}
                    isPersonal={isPersonal}
                    selection={selection}
                    externalItem={externalItem[0]}
                    owner={owner[0]}
                    shareGroups={shareGroups}
                    shareUsers={shareUsers}
                    isMyId={isMyId}
                    onToggleLink={this.onToggleLink}
                    onShowEmbeddingPanel={this.onShowEmbeddingPanel}
                    onChangeItemAccess={this.onChangeItemAccess}
                    accessOptions={accessOptions}
                    externalAccessOptions={externalAccessOptions}
                    canShareOwnerChange={canShareOwnerChange}
                    internalLink={internalLink}
                    onRemoveUserClick={this.onRemoveUserItemClick}
                    onShowChangeOwnerPanel={this.onShowChangeOwnerPanel}
                    documentTitle={documentTitle}
                  />

                  <Footer
                    t={t}
                    isPersonal={isPersonal}
                    message={message}
                    onChangeMessage={this.onChangeMessage}
                    isNotifyUsers={isNotifyUsers}
                    onNotifyUsersChange={this.onNotifyUsersChange}
                    onSaveClick={this.onSaveClick}
                  />
                </StyledContent>
              ) : (
                <SharingPanelLoader />
              )}
            </Aside>

            {showAddUsersPanel && (
              <AddUsersPanel
                onSharingPanelClose={this.onClose}
                onClose={this.onShowUsersPanel}
                visible={showAddUsersPanel}
                shareDataItems={filteredShareDataItems}
                setShareDataItems={this.setShareDataItems}
                groupsCaption={groupsCaption}
                accessOptions={accessOptions}
                isMultiSelect
                isEncrypted={isEncrypted}
              />
            )}

            {showAddGroupsPanel && (
              <AddGroupsPanel
                onSharingPanelClose={this.onClose}
                onClose={this.onShowGroupsPanel}
                visible={showAddGroupsPanel}
                shareDataItems={filteredShareDataItems}
                setShareDataItems={this.setShareDataItems}
                accessOptions={accessOptions}
                isMultiSelect
              />
            )}

            {showChangeOwnerPanel && (
              <AddUsersPanel
                onSharingPanelClose={this.onClose}
                onClose={this.onShowChangeOwnerPanel}
                visible={showChangeOwnerPanel}
                shareDataItems={filteredShareDataItems}
                setShareDataItems={this.setShareDataItems}
              />
            )}

            {showEmbeddingPanel && (
              <EmbeddingPanel
                visible={showEmbeddingPanel}
                onSharingPanelClose={this.onClose}
                onClose={this.onShowEmbeddingPanel}
                embeddingLink={externalItem[0].sharedTo.shareLink}
              />
            )}
          </StyledAsidePanel>
        )}
      </>
    );
  }
}

const SharingPanel = inject(
  (
    {
      auth,
      filesStore,
      uploadDataStore,
      dialogsStore,
      treeFoldersStore,
      selectedFolderStore,
      settingsStore,
    },
    { uploadPanelVisible }
  ) => {
    const { replaceFileStream, setEncryptionAccess } = auth;
    const { personal, customNames, isDesktopClient } = auth.settingsStore;
    const { setFilesSettings } = settingsStore;

    const { id, access } = selectedFolderStore;

    const {
      selection,
      bufferSelection,
      canShareOwnerChange,
      getAccessOption,
      getExternalAccessOption,
      setFile,
      setFolder,
      getShareUsers,
      setShareFiles,
      setSelection,
      getFileInfo,
      getFolderInfo,
      setBufferSelection,
    } = filesStore;
    const { isPrivacyFolder } = treeFoldersStore;
    const {
      setSharingPanelVisible,
      sharingPanelVisible,
      setIsFolderActions,
      isFolderActions,
    } = dialogsStore;
    const {
      selectedUploadFile,
      selectUploadedFile,
      updateUploadedItem,
    } = uploadDataStore;

    const isShared =
      selection.length > 0 && selection[0].shared
        ? selection[0].shared
        : bufferSelection?.shared
        ? bufferSelection.shared
        : false;

    return {
      theme: auth.settingsStore.theme,
      isPersonal: personal,
      isMyId: auth.userStore.user && auth.userStore.user.id,
      groupsCaption: customNames.groupsCaption,
      isDesktop: isDesktopClient,
      homepage: config.homepage,
      selection: uploadPanelVisible
        ? selectedUploadFile
        : selection.length
        ? selection
        : [bufferSelection],
      isPrivacy: isPrivacyFolder,
      isFolderActions,
      selectedUploadFile,
      canShareOwnerChange,

      setSharingPanelVisible,
      setIsFolderActions,
      setSelection,
      sharingPanelVisible,
      selectUploadedFile,
      updateUploadedItem,
      replaceFileStream,
      setEncryptionAccess,
      getAccessOption,
      getExternalAccessOption,
      setFile,
      setFolder,
      getShareUsers,
      setShareFiles,
      getFileInfo,
      getFolderInfo,
      id,
      setBufferSelection,
      access,
      isShared: isShared,
      setFilesSettings,
    };
  }
)(
  observer(
    withTranslation([
      "SharingPanel",
      "Common",
      "Translations",
      "Files",
      "ChangeOwnerPanel",
      "EmbeddingPanel",
    ])(withLoader(SharingPanelComponent)(<Loaders.DialogAsideLoader isPanel />))
  )
);

class Panel extends React.Component {
  static convertSharingUsers = (shareDataItems) => {
    const t = i18n.getFixedT(null, [
      "SharingPanel",
      "Common",
      "Translations",
      "Files",
      "ChangeOwnerPanel",
      "EmbeddingPanel",
    ]);
    const {
      FullAccess,
      DenyAccess,
      ReadOnly,
      Review,
      Comment,
      FormFilling,
      CustomFilter,
    } = ShareAccessRights;

    let sharingSettings = [];

    for (let i = 1; i < shareDataItems.length; i++) {
      let resultAccess =
        shareDataItems[i].access === FullAccess
          ? t("Common:FullAccess")
          : shareDataItems[i].access === ReadOnly
          ? t("ReadOnly")
          : shareDataItems[i].access === DenyAccess
          ? t("DenyAccess")
          : shareDataItems[i].access === Review
          ? t("Common:Review")
          : shareDataItems[i].access === Comment
          ? t("Comment")
          : shareDataItems[i].access === FormFilling
          ? t("FormFilling")
          : shareDataItems[i].access === CustomFilter
          ? t("CustomFilter")
          : "";

      let obj = {
        user:
          shareDataItems[i].sharedTo.displayName ||
          shareDataItems[i].sharedTo.name,
        permissions: resultAccess,
      };
      sharingSettings.push(obj);
    }
    return sharingSettings;
  };

  render() {
    return (
      <I18nextProvider i18n={i18n}>
        <SharingPanel {...this.props} />
      </I18nextProvider>
    );
  }
}

export default Panel;
