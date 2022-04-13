import React from "react";
import Backdrop from "@appserver/components/backdrop";

import Aside from "@appserver/components/aside";
import Scrollbar from "@appserver/components/scrollbar";
import Text from "@appserver/components/text";
import Loader from "@appserver/components/loader";

import { withTranslation, Trans } from "react-i18next";
import toastr from "studio/toastr";
import { ShareAccessRights } from "@appserver/common/constants";
import { StyledAsidePanel } from "../StyledPanels";
import { AddUsersPanel, AddGroupsPanel, EmbeddingPanel } from "../index";
import SharingRow from "./SharingRow";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { isMobile, isMobileOnly } from "react-device-detect";
import Loaders from "@appserver/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";
import ModalDialog from "@appserver/components/modal-dialog";

import { StyledContent, StyledBodyContent } from "./StyledSharingPanel";

import Header from "./Header";
import Footer from "./Footer";

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
    console.log(e);
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
      .finally(() =>
        this.setState({
          isLoading: false,
        })
      );
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
      : `${window.location.origin + homepage}/filter?folder=${item?.id}`;
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
    this.getShareData();

    document.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    document.removeEventListener("keyup", this.onKeyPress);
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
      isUpdated,
      isLoading,
    } = this.state;

    const visible = sharingPanelVisible;

    const zIndex = 310;

    const onPlusClickProp = !isLoading ? { onClick: this.onPlusClick } : {};

    const isEncrypted =
      isPrivacy || (selection.length && selection[0]?.encrypted);

    const internalLink =
      selection.length === 1 && !isEncrypted && this.getInternalLink();

    const filteredShareDataItems = [];
    const owner = [];
    const shareGroups = [];
    const shareUsers = [];

    shareDataItems.forEach((item, index) => {
      if (item?.sharedTo?.shareLink) {
        return filteredShareDataItems.push(item);
      }

      if (item?.isOwner) {
        item.isUser = true;
        return owner.push(item);
      }

      if (item?.sharedTo?.userName || item?.sharedTo?.label) {
        item.isUser = true;
        shareUsers.push(item);
      } else {
        item.isGroup = true;
        shareGroups.push(item);
      }
    });

    if (shareGroups[shareGroups.length - 1]) {
      shareGroups[shareGroups.length - 1].isEndOfBlock = true;
    }

    filteredShareDataItems.push(...owner, ...shareGroups, ...shareUsers);

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
            <Header
              t={t}
              uploadPanelVisible={uploadPanelVisible}
              isPersonal={isPersonal}
              isEncrypted={isEncrypted}
              onClose={this.onClose}
              onShowUsersPanel={this.onShowUsersPanel}
              onShowGroupsPanel={this.onShowGroupsPanel}
            />
            <StyledBodyContent>
              <Scrollbar ref={this.scrollRef} stype="mediumBlack">
                {!isLoading ? (
                  filteredShareDataItems.length > 0 ? (
                    filteredShareDataItems.map((item, index) => (
                      <SharingRow
                        t={t}
                        isPersonal={isPersonal}
                        index={index}
                        key={`${item.sharedTo.id}_${index}`}
                        selection={selection}
                        item={item}
                        isMyId={isMyId}
                        accessOptions={accessOptions}
                        externalAccessOptions={externalAccessOptions}
                        canShareOwnerChange={canShareOwnerChange}
                        onChangeItemAccess={this.onChangeItemAccess}
                        internalLink={internalLink}
                        onRemoveUserClick={this.onRemoveUserItemClick}
                        onShowEmbeddingPanel={this.onShowEmbeddingPanel}
                        onToggleLink={this.onToggleLink}
                        onShowChangeOwnerPanel={this.onShowChangeOwnerPanel}
                        isLoading={isLoading}
                        documentTitle={documentTitle}
                        isInternalLinkOnly={false}
                      />
                    ))
                  ) : (
                    <SharingRow
                      t={t}
                      isPersonal={isPersonal}
                      internalLink={internalLink}
                      isInternalLinkOnly={true}
                    />
                  )
                ) : (
                  <div key="loader" className="panel-loader-wrapper">
                    <Loader type="oval" size="16px" className="panel-loader" />
                    <Text as="span">
                      {`${t("Common:LoadingProcessing")} 
                      ${t("Common:LoadingDescription")}`}
                    </Text>
                  </div>
                )}
              </Scrollbar>
            </StyledBodyContent>

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
            embeddingLink={shareLink}
          />
        )}
      </StyledAsidePanel>
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
    },
    { uploadPanelVisible }
  ) => {
    const { replaceFileStream, setEncryptionAccess } = auth;
    const { personal, customNames, isDesktopClient } = auth.settingsStore;
    const { user } = auth.userStore;

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
    };
  }
)(
  observer(
    withTranslation([
      "SharingPanel",
      "Common",
      "Translations",
      "Home",
      "ChangeOwnerPanel",
    ])(withLoader(SharingPanelComponent)(<Loaders.DialogAsideLoader isPanel />))
  )
);

class Panel extends React.Component {
  static convertSharingUsers = (shareDataItems) => {
    const t = i18n.getFixedT(null, [
      "SharingPanel",
      "Common",
      "Translations",
      "Home",
      "ChangeOwnerPanel",
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
