import React from "react";
import {
  Backdrop,
  Heading,
  Aside,
  IconButton,
  Checkbox,
  Button,
  DropDown,
  DropDownItem,
  Textarea,
} from "asc-web-components";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
import { constants, toastr } from "asc-web-common";

import {
  StyledAsidePanel,
  StyledContent,
  StyledFooter,
  StyledHeaderContent,
  StyledSharingBody,
} from "../StyledPanels";
import { AddUsersPanel, AddGroupsPanel, EmbeddingPanel } from "../index";
import SharingRow from "./SharingRow";
//import { setEncryptionAccess } from "../../../helpers/desktop";
import { inject, observer } from "mobx-react";
const { ShareAccessRights } = constants;

const SharingBodyStyle = { height: `calc(100vh - 156px)` };

class SharingPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showActionPanel: false,
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
    };

    this.ref = React.createRef();
    this.scrollRef = React.createRef();
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onCloseActionPanel = (e) => {
    if (this.ref.current.contains(e.target)) return;
    this.setState({ showActionPanel: !this.state.showActionPanel });
  };

  onToggleLink = (item) => {
    const { shareDataItems } = this.state;
    const { DenyAccess, ReadOnly } = ShareAccessRights;

    const rights = item.access !== DenyAccess ? DenyAccess : ReadOnly;
    const newDataItems = JSON.parse(JSON.stringify(shareDataItems));

    newDataItems[0].access = rights;
    this.setState({
      shareDataItems: newDataItems,
    });
  };

  updateRowData = (newRowData) => {
    const { files, folders, setFiles, setFolders } = this.props;

    for (let item of newRowData) {
      if (!item.fileExst && item.foldersCount) {
        let folderIndex = folders.findIndex((x) => x.id === item.id);
        if (folderIndex !== -1) {
          folders[folderIndex] = item;
        }
      } else {
        let fileIndex = files.findIndex((x) => x.id === item.id);
        if (fileIndex !== -1) {
          files[fileIndex] = item;
        }
      }
    }

    setFiles(files);
    setFolders(folders);
  };

  onSaveClick = () => {
    const {
      baseShareData,
      isNotifyUsers,
      message,
      shareDataItems,
      filesOwnerId,
    } = this.state;
    const {
      selection,
      setIsLoading,
      isPrivacy,
      replaceFileStream,
      t,
      uploadPanelVisible,
      updateUploadedItem,
      uploadSelection,
      isDesktop,
      setEncryptionAccess,
      setShareFiles,
    } = this.props;

    const folderIds = [];
    const fileIds = [];
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
    const owner = shareDataItems.find((x) => x.isOwner);
    const ownerId =
      filesOwnerId !== owner.sharedTo.id ? owner.sharedTo.id : null;

    setIsLoading(true);
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
      .then((res) => {
        if (ownerId) {
          this.updateRowData(res[0]);
        }
        if (isPrivacy && isDesktop) {
          if (share.length === 0) return Promise.resolve();
          selection.forEach((item) => {
            return setEncryptionAccess(item).then((encryptedFile) => {
              if (!encryptedFile) return Promise.resolve();

              toastr.info(t("EncryptedFileSaving"));

              const title = item.title;

              return replaceFileStream(item.id, encryptedFile, true, true).then(
                () =>
                  toastr.success(
                    <Trans i18nKey="EncryptedFileSharing" ns="SharingPanel">
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
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  onNotifyUsersChange = () =>
    this.setState({ isNotifyUsers: !this.state.isNotifyUsers });

  onShowUsersPanel = () =>
    this.setState({
      showAddUsersPanel: !this.state.showAddUsersPanel,
      showActionPanel: false,
    });

  onChangeItemAccess = (e) => {
    const id = e.currentTarget.dataset.id;
    const access = e.currentTarget.dataset.access;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id && !x.isOwner);

    if (elem.access !== +access) {
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
    const { selection } = this.props;
    const folderId = [];
    const fileId = [];

    for (let item of selection) {
      if (item.access === 1 || item.access === 0) {
        if (item.fileExst) {
          fileId.push(item.id);
        } else {
          folderId.push(item.id);
        }
      }
    }

    return [folderId, fileId];
  };

  getShareData = () => {
    const returnValue = this.getData();
    const folderId = returnValue[0];
    const fileId = returnValue[1];
    const {
      getAccessOption,
      getExternalAccessOption,
      selection,
      setIsLoading,
      getShareUsers,
    } = this.props;

    if (folderId.length !== 0 || fileId.length !== 0) {
      setIsLoading(true);
      getShareUsers(folderId, fileId)
        .then((shareDataItems) => {
          const baseShareData = JSON.parse(JSON.stringify(shareDataItems));
          const accessOptions = getAccessOption(selection);
          const externalAccessOptions = getExternalAccessOption(selection);
          const filesOwner = shareDataItems.find((x) => x.isOwner);
          const filesOwnerId = filesOwner ? filesOwner.sharedTo.id : null;

          this.setState({
            baseShareData,
            shareDataItems,
            accessOptions,
            externalAccessOptions,
            showPanel: true,
            filesOwnerId,
          });
        })
        .catch((err) => {
          toastr.error(err);
          this.onClose();
        })
        .finally(() => setIsLoading(false));
    }
  };

  getInternalLink = () => {
    const { homepage, selection } = this.props;
    const item = selection[0];
    const isFile = !!item.fileExst;

    if (selection.length !== 1) return null;

    return isFile
      ? item.canOpenPlayer
        ? `${window.location.href}&preview=${item.id}`
        : item.webUrl
      : `${window.location.origin + homepage}/filter?folder=${item.id}`;
  };

  onShowEmbeddingPanel = (link) =>
    this.setState({
      showEmbeddingPanel: !this.state.showEmbeddingPanel,
      shareLink: link,
    });

  onShowGroupsPanel = () =>
    this.setState({
      showAddGroupsPanel: !this.state.showAddGroupsPanel,
      showActionPanel: false,
    });

  onShowChangeOwnerPanel = () =>
    this.setState({
      showChangeOwnerPanel: !this.state.showChangeOwnerPanel,
      showActionPanel: false,
    });

  onChangeMessage = (e) => this.setState({ message: e.target.value });

  setShareDataItems = (shareDataItems) => this.setState({ shareDataItems });

  onClose = () => {
    this.props.setSharingPanelVisible(!this.props.sharingPanelVisible);
    this.props.selectUploadedFile([]);
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

    if (this.state.message === prevState.message) {
      this.scrollRef.current.view.focus();
    }
  }

  render() {
    //console.log("Sharing panel render");
    const {
      t,
      isMyId,
      selection,
      groupsCaption,
      canShareOwnerChange,
      isLoading,
      uploadPanelVisible,
    } = this.props;
    const {
      showActionPanel,
      isNotifyUsers,
      shareDataItems,
      message,
      showAddUsersPanel,
      showAddGroupsPanel,
      showEmbeddingPanel,
      showChangeOwnerPanel,
      shareLink,
      showPanel,
      accessOptions,
      externalAccessOptions,
    } = this.state;

    const visible = showPanel;
    const zIndex = 310;
    const onPlusClickProp = !isLoading ? { onClick: this.onPlusClick } : {};
    const internalLink = selection.length === 1 && this.getInternalLink();

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent isDisabled={isLoading}>
            <StyledHeaderContent>
              {uploadPanelVisible && (
                <IconButton
                  size="16"
                  iconName="ArrowPathIcon"
                  onClick={this.onClose}
                  color="A3A9AE"
                />
              )}
              <Heading className="sharing_panel-header" size="medium" truncate>
                {t("SharingSettingsTitle")}
              </Heading>
              <div className="sharing_panel-icons-container">
                <div ref={this.ref} className="sharing_panel-drop-down-wrapper">
                  <IconButton
                    size="17"
                    iconName="PlusIcon"
                    className="sharing_panel-plus-icon"
                    {...onPlusClickProp}
                    color="A3A9AE"
                    isDisabled={isLoading}
                  />

                  <DropDown
                    directionX="right"
                    className="sharing_panel-drop-down"
                    open={showActionPanel}
                    manualY="30px"
                    clickOutsideAction={this.onCloseActionPanel}
                  >
                    <DropDownItem
                      label={t("LinkText")}
                      onClick={this.onShowUsersPanel}
                    />
                    <DropDownItem
                      label={t("AddGroupsForSharingButton")}
                      onClick={this.onShowGroupsPanel}
                    />
                  </DropDown>
                </div>

                {/*<IconButton
                  size="16"
                  iconName="KeyIcon"
                  onClick={this.onKeyClick}
                />*/}
              </div>
            </StyledHeaderContent>
            <StyledSharingBody
              ref={this.scrollRef}
              stype="mediumBlack"
              style={SharingBodyStyle}
            >
              {shareDataItems.map((item, index) => (
                <SharingRow
                  t={t}
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
                />
              ))}
              {isNotifyUsers && (
                <div className="sharing_panel-text-area">
                  <Textarea
                    placeholder={t("AddShareMessage")}
                    onChange={this.onChangeMessage}
                    value={message}
                    isDisabled={isLoading}
                  />
                </div>
              )}
            </StyledSharingBody>
            <StyledFooter>
              <Checkbox
                isChecked={isNotifyUsers}
                label={t("Notify users")}
                onChange={this.onNotifyUsersChange}
                className="sharing_panel-checkbox"
                isDisabled={isLoading}
              />
              <Button
                className="sharing_panel-button"
                label={t("AddButton")}
                size="big"
                primary
                onClick={this.onSaveClick}
                isDisabled={isLoading}
              />
            </StyledFooter>
          </StyledContent>
        </Aside>

        {showAddUsersPanel && (
          <AddUsersPanel
            onSharingPanelClose={this.onClose}
            onClose={this.onShowUsersPanel}
            visible={showAddUsersPanel}
            shareDataItems={shareDataItems}
            setShareDataItems={this.setShareDataItems}
            groupsCaption={groupsCaption}
            accessOptions={accessOptions}
            isMultiSelect
          />
        )}

        {showAddGroupsPanel && (
          <AddGroupsPanel
            onSharingPanelClose={this.onClose}
            onClose={this.onShowGroupsPanel}
            visible={showAddGroupsPanel}
            shareDataItems={shareDataItems}
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
            shareDataItems={shareDataItems}
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

const SharingPanel = withTranslation("SharingPanel")(SharingPanelComponent);

export default inject(
  (
    {
      auth,
      initFilesStore,
      filesStore,
      uploadDataStore,
      dialogsStore,
      treeFoldersStore,
    },
    { uploadPanelVisible }
  ) => {
    const { replaceFileStream, setEncryptionAccess } = auth;
    const { customNames, isDesktopClient, homepage } = auth.settingsStore;
    const { setIsLoading, isLoading } = initFilesStore;
    const {
      files,
      folders,
      selection,
      canShareOwnerChange,
      getAccessOption,
      getExternalAccessOption,
      setFiles,
      setFolders,
      getShareUsers,
      setShareFiles,
    } = filesStore;
    const { isPrivacyFolder } = treeFoldersStore;
    const { sharingPanelVisible, setSharingPanelVisible } = dialogsStore;
    const {
      uploadSelection,
      selectUploadedFile,
      updateUploadedItem,
    } = uploadDataStore;

    return {
      isMyId: auth.userStore.user.id,
      groupsCaption: customNames.groupsCaption,
      isDesktop: isDesktopClient,
      homepage,
      files,
      folders,
      selection: uploadPanelVisible ? uploadSelection : selection,
      isLoading,
      isPrivacy: isPrivacyFolder,
      sharingPanelVisible,
      uploadSelection,
      canShareOwnerChange,

      setIsLoading,
      setSharingPanelVisible,
      selectUploadedFile,
      updateUploadedItem,
      replaceFileStream,
      setEncryptionAccess,
      getAccessOption,
      getExternalAccessOption,
      setFiles,
      setFolders,
      getShareUsers,
      setShareFiles,
    };
  }
)(withRouter(observer(SharingPanel)));
