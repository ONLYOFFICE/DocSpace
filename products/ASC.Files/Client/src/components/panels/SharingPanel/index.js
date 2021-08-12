import React from "react";
import Backdrop from "@appserver/components/backdrop";
import Heading from "@appserver/components/heading";
import Aside from "@appserver/components/aside";
import IconButton from "@appserver/components/icon-button";
import Checkbox from "@appserver/components/checkbox";
import Button from "@appserver/components/button";
import DropDown from "@appserver/components/drop-down";
import DropDownItem from "@appserver/components/drop-down-item";
import Textarea from "@appserver/components/textarea";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { withTranslation, Trans } from "react-i18next";
import toastr from "studio/toastr";
import { ShareAccessRights } from "@appserver/common/constants";
import {
  StyledAsidePanel,
  StyledContent,
  StyledFooter,
  StyledHeaderContent,
  StyledSharingBody,
} from "../StyledPanels";
import { AddUsersPanel, AddGroupsPanel, EmbeddingPanel } from "../index";
import SharingRow from "./SharingRow";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { isMobile } from "react-device-detect";
import Loaders from "@appserver/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

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
    const { getFileInfo, getFolderInfo } = this.props;

    for (let item of newRowData) {
      !item.fileExst ? getFolderInfo(item.id) : getFileInfo(item.id);
    }
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
      onSuccess,
    } = this.props;

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
    const {
      getAccessOption,
      getExternalAccessOption,
      selection,
      setIsLoading,
      getShareUsers,
    } = this.props;

    const returnValue = this.getData();
    const folderId = returnValue[0];
    const fileId = returnValue[1];

    if (folderId.length !== 0 || fileId.length !== 0) {
      !isMobile && setIsLoading(true);
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
            //showPanel: true,
            filesOwnerId,
          });
        })

        .catch((err) => {
          toastr.error(err);
          this.onClose();
        })
        .finally(() => !isMobile && setIsLoading(false));
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
    const { onCancel, setSharingPanelVisible, selectUploadedFile } = this.props;

    setSharingPanelVisible(false);
    selectUploadedFile([]);
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
      tReady,
      isPersonal,
      isMyId,
      selection,
      groupsCaption,
      canShareOwnerChange,
      isLoading,
      uploadPanelVisible,
      documentTitle,
      sharingPanelVisible,
      isPrivacy,
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
      //showPanel,
      accessOptions,
      externalAccessOptions,
    } = this.state;

    const visible = sharingPanelVisible;
    const zIndex = 310;
    const onPlusClickProp = !isLoading ? { onClick: this.onPlusClick } : {};

    const isEncrypted =
      isPrivacy || (selection.length && selection[0].encrypted);

    const internalLink =
      selection.length === 1 && !isEncrypted && this.getInternalLink();

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
                  iconName="/static/images/arrow.path.react.svg"
                  onClick={this.onClose}
                  color="A3A9AE"
                />
              )}
              <Heading className="sharing_panel-header" size="medium" truncate>
                {t("SharingSettingsTitle")}
              </Heading>
              {!isPersonal && (
                <div className="sharing_panel-icons-container">
                  <div
                    ref={this.ref}
                    className="sharing_panel-drop-down-wrapper"
                  >
                    <IconButton
                      size="17"
                      iconName="/static/images/actions.header.touch.react.svg"
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
                    {!isEncrypted && (
                      <DropDownItem
                        label={t("AddGroupsForSharingButton")}
                        onClick={this.onShowGroupsPanel}
                      />
                    )}
                    </DropDown>
                  </div>

                  {/*<IconButton
                  size="16"
                  iconName="images/key.react.svg"
                  onClick={this.onKeyClick}
                />*/}
                </div>
              )}
            </StyledHeaderContent>
            <StyledSharingBody
              ref={this.scrollRef}
              stype="mediumBlack"
              style={SharingBodyStyle}
            >
              {!isLoading ? (
                shareDataItems.map((item, index) => (
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
                  />
                ))
              ) : (
                <div key="loader" className="panel-loader-wrapper">
                  <Loader type="oval" size="16px" className="panel-loader" />
                  <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                    "Common:LoadingDescription"
                  )}`}</Text>
                </div>
              )}
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
              {!isPersonal && (
                <Checkbox
                  isChecked={isNotifyUsers}
                  label={t("Notify users")}
                  onChange={this.onNotifyUsersChange}
                  className="sharing_panel-checkbox"
                  isDisabled={isLoading}
                />
              )}
              <Button
                className="sharing_panel-button"
                label={t("Common:SaveButton")}
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
            isEncrypted={isEncrypted}
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

const SharingPanel = inject(
  (
    { auth, filesStore, uploadDataStore, dialogsStore, treeFoldersStore },
    { uploadPanelVisible }
  ) => {
    const { replaceFileStream, setEncryptionAccess } = auth;
    const { personal, customNames, isDesktopClient } = auth.settingsStore;

    const {
      selection,
      canShareOwnerChange,
      getAccessOption,
      getExternalAccessOption,
      setFile,
      setFolder,
      getShareUsers,
      setShareFiles,
      setIsLoading,
      getFileInfo,
      getFolderInfo,
      isLoading,
    } = filesStore;
    const { isPrivacyFolder } = treeFoldersStore;
    const { setSharingPanelVisible, sharingPanelVisible } = dialogsStore;
    const {
      selectedUploadFile,
      selectUploadedFile,
      updateUploadedItem,
    } = uploadDataStore;

    return {
      isPersonal: personal,
      isMyId: auth.userStore.user && auth.userStore.user.id,
      groupsCaption: customNames.groupsCaption,
      isDesktop: isDesktopClient,
      homepage: config.homepage,
      selection: uploadPanelVisible ? selectedUploadFile : selection,
      isLoading,
      isPrivacy: isPrivacyFolder,
      selectedUploadFile,
      canShareOwnerChange,

      setIsLoading,
      setSharingPanelVisible,
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
    };
  }
)(
  observer(
    withTranslation(["SharingPanel", "Common", "Translations"])(
      withLoader(SharingPanelComponent)(<Loaders.DialogAsideLoader isPanel />)
    )
  )
);

class Panel extends React.Component {
  static convertSharingUsers = (shareDataItems) => {
    const t = i18n.getFixedT(null, ["SharingPanel", "Common"]);
    let sharingSettings = [];
    for (let i = 1; i < shareDataItems.length; i++) {
      let resultAccess =
        shareDataItems[i].access === 1
          ? t("Common:FullAccess")
          : shareDataItems[i].access === 2
          ? t("ReadOnly")
          : shareDataItems[i].access === 3
          ? t("DenyAccess")
          : shareDataItems[i].access === 5
          ? t("Common:Review")
          : shareDataItems[i].access === 6
          ? t("Comment")
          : shareDataItems[i].access === 7
          ? t("FormFilling")
          : shareDataItems[i].access === 8
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
