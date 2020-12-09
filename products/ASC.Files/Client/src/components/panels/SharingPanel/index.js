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
  ComboBox,
  Icons,
} from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { utils as commonUtils, constants, toastr, store } from "asc-web-common";
import {
  getShareUsers,
  setShareFiles,
  setSharingPanelVisible,
} from "../../../store/files/actions";
import {
  getAccessOption,
  getExternalAccessOption,
  getSelection,
  getSharePanelVisible,
} from "../../../store/files/selectors";
import {
  StyledAsidePanel,
  StyledContent,
  StyledFooter,
  StyledHeaderContent,
  StyledSharingBody,
} from "../StyledPanels";
import { AddUsersPanel, AddGroupsPanel, EmbeddingPanel } from "../index";
import SharingRow from "./SharingRow";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "SharingPanel",
  localesPath: "panels/SharingPanel",
});
const { changeLanguage } = commonUtils;
const { ShareAccessRights } = constants;
const {
  getCurrentUserId,
  getSettingsCustomNamesGroupsCaption,
} = store.auth.selectors;

const SharingBodyStyle = { height: `calc(100vh - 156px)` };

class SharingPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = {
      showActionPanel: false,
      isNotifyUsers: false,
      shareDataItems: [],
      baseShareData: [],
      message: "",
      showAddUsersPanel: false,
      showEmbeddingPanel: false,
      showAddGroupsPanel: false,
      accessRight: ShareAccessRights.ReadOnly,
      shareLink: "",
      isLoadedShareData: false,
      showPanel: false,
      accessOptions: [],
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

    const rights =
      item.access !== ShareAccessRights.DenyAccess
        ? ShareAccessRights.DenyAccess
        : ShareAccessRights.ReadOnly;
    const newDataItems = JSON.parse(JSON.stringify(shareDataItems));
    console.log("newDataItems", newDataItems);

    newDataItems[0].access = rights;
    this.setState({
      shareDataItems: newDataItems,
    });
  };

  onSaveClick = () => {
    const {
      baseShareData,
      isNotifyUsers,
      message,
      shareDataItems,
    } = this.state;
    const { selection } = this.props;

    const folderIds = [];
    const fileIds = [];
    const share = [];

    let externalAccess = null;

    for (let item of shareDataItems) {
      const baseItem = baseShareData.find((x) => x.id === item.id);
      if (
        (baseItem &&
          baseItem.rights.rights !== item.rights.rights &&
          !item.shareLink) ||
        !baseItem
      ) {
        share.push({ shareTo: item.id, access: item.rights.accessNumber });
      }

      if (
        item.shareLink &&
        item.rights.accessNumber !== baseItem.rights.accessNumber
      ) {
        externalAccess = item.rights.accessNumber;
      }
    }

    for (let item of baseShareData) {
      const baseItem = shareDataItems.find((x) => x.id === item.id);
      if (!baseItem) {
        share.push({ shareTo: item.id, access: 0 });
      }
    }

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    setShareFiles(
      folderIds,
      fileIds,
      share,
      isNotifyUsers,
      message,
      externalAccess
    )
      .catch((err) => toastr.error(err))
      .finally(() => this.onClose());
  };

  onFullAccessClick = () => {
    this.setState({
      accessRight: ShareAccessRights.FullAccess,
    });
  };

  onReadOnlyClick = () => {
    this.setState({
      accessRight: ShareAccessRights.ReadOnly,
    });
  };

  onReviewClick = () => {
    this.setState({
      accessRight: ShareAccessRights.Review,
    });
  };

  onCommentClick = () => {
    this.setState({
      accessRight: ShareAccessRights.Comment,
    });
  };

  onFormFillingClick = () => {
    this.setState({
      accessRight: ShareAccessRights.FormFilling,
    });
  };

  onDenyAccessClick = () => {
    this.setState({
      accessRight: ShareAccessRights.DenyAccess,
    });
  };

  onFilterEditingClick = () => {
    this.setState({
      accessRight: ShareAccessRights.CustomFilter,
    });
  };

  onNotifyUsersChange = () =>
    this.setState({ isNotifyUsers: !this.state.isNotifyUsers });

  onShowUsersPanel = () =>
    this.setState({
      showAddUsersPanel: !this.state.showAddUsersPanel,
      showActionPanel: false,
    });

  onFullAccessItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);
    if (elem.access !== ShareAccessRights.FullAccess) {
      elem.access = ShareAccessRights.FullAccess;
      this.setState({ shareDataItems });
    }
  };
  onReadOnlyItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);
    if (elem.access !== ShareAccessRights.ReadOnly) {
      elem.access = ShareAccessRights.ReadOnly;
      this.setState({ shareDataItems });
    }
  };
  onReviewItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);
    if (elem.access !== ShareAccessRights.Review) {
      elem.access = ShareAccessRights.Review;
      this.setState({ shareDataItems });
    }
  };
  onCommentItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);
    if (elem.access !== ShareAccessRights.Comment) {
      elem.access = ShareAccessRights.Comment;
      this.setState({ shareDataItems });
    }
  };
  onFormFillingItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);
    if (elem.access !== ShareAccessRights.FormFilling) {
      elem.access = ShareAccessRights.FormFilling;
      this.setState({ shareDataItems });
    }
  };

  onFilterEditingItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);

    if (elem.access !== ShareAccessRights.CustomFilter) {
      elem.access = ShareAccessRights.CustomFilter;
      this.setState({ shareDataItems });
    }
  };
  onDenyAccessItemClick = (e) => {
    const id = e.currentTarget.dataset.id;
    const shareDataItems = this.state.shareDataItems;
    const elem = shareDataItems.find((x) => x.sharedTo.id === id);

    if (elem.access !== ShareAccessRights.DenyAccess) {
      elem.access = ShareAccessRights.DenyAccess;
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

  removeDuplicateShareData = (shareDataItems) => {
    let obj = {};
    return shareDataItems.filter((x) => {
      if (obj[x.id]) return false;
      obj[x.id] = true;
      return true;
    });
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
    const { getAccessOption, selection } = this.props;

    if (folderId.length !== 0 || fileId.length !== 0) {
      getShareUsers(folderId, fileId)
        .then((shareDataItems) => {
          const baseShareData = JSON.parse(JSON.stringify(shareDataItems));
          const accessOptions = getAccessOption(selection);

          this.setState({
            baseShareData,
            shareDataItems,
            accessOptions,
            showPanel: true,
          });
        })
        .catch((err) => {
          toastr.error(err);
        });
    }
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

  onChangeMessage = (e) => this.setState({ message: e.target.value });

  setShareDataItems = (shareDataItems) => this.setState({ shareDataItems });

  onClose = () =>
    this.props.setSharingPanelVisible(!this.props.sharingPanelVisible);

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
    } = this.state;
    if (showAddUsersPanel || showEmbeddingPanel || showAddGroupsPanel) return;
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
    const { t, isMyId, selection, groupsCaption } = this.props;
    const {
      showActionPanel,
      isNotifyUsers,
      shareDataItems,
      message,
      showAddUsersPanel,
      showAddGroupsPanel,
      showEmbeddingPanel,
      accessRight,
      shareLink,
      showPanel,
      accessOptions,
      externalAccessOptions,
    } = this.state;

    const visible = showPanel;
    const zIndex = 310;

    const advancedOptions = (
      <>
        {accessOptions.includes("FullAccess") && (
          <DropDownItem
            label="Full access"
            icon="AccessEditIcon"
            onClick={this.onFullAccessClick}
          />
        )}

        {accessOptions.includes("FilterEditing") && (
          <DropDownItem
            label="Custom filter"
            icon="CustomFilterIcon"
            onClick={this.onFilterEditingClick}
          />
        )}

        {accessOptions.includes("Review") && (
          <DropDownItem
            label="Review"
            icon="AccessReviewIcon"
            onClick={this.onReviewClick}
          />
        )}

        {accessOptions.includes("FormFilling") && (
          <DropDownItem
            label="Form filling"
            icon="AccessFormIcon"
            onClick={this.onFormFillingClick}
          />
        )}

        {accessOptions.includes("Comment") && (
          <DropDownItem
            label="Comment"
            icon="AccessCommentIcon"
            onClick={this.onCommentClick}
          />
        )}

        {accessOptions.includes("ReadOnly") && (
          <DropDownItem
            label="Read only"
            icon="EyeIcon"
            onClick={this.onReadOnlyClick}
          />
        )}

        {accessOptions.includes("DenyAccess") && (
          <DropDownItem
            label="Deny access"
            icon="AccessNoneIcon"
            onClick={this.onDenyAccessClick}
          />
        )}
      </>
    );

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop onClick={this.onClose} visible={visible} zIndex={zIndex} />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledHeaderContent>
              <Heading className="sharing_panel-header" size="medium" truncate>
                {t("SharingSettingsTitle")}
              </Heading>
              <div className="sharing_panel-icons-container">
                <div ref={this.ref} className="sharing_panel-drop-down-wrapper">
                  <IconButton
                    size="17"
                    iconName="PlusIcon"
                    className="sharing_panel-plus-icon"
                    onClick={this.onPlusClick}
                    color="A3A9AE"
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
                  key={index}
                  t={t}
                  selection={selection}
                  item={item}
                  index={index}
                  isMyId={isMyId}
                  accessOptions={accessOptions}
                  externalAccessOptions={externalAccessOptions}
                  onFullAccessClick={this.onFullAccessItemClick}
                  onReadOnlyClick={this.onReadOnlyItemClick}
                  onReviewClick={this.onReviewItemClick}
                  onCommentClick={this.onCommentItemClick}
                  onFormFillingClick={this.onFormFillingItemClick}
                  onFilterEditingClick={this.onFilterEditingItemClick}
                  onDenyAccessClick={this.onDenyAccessItemClick}
                  onRemoveUserClick={this.onRemoveUserItemClick}
                  onShowEmbeddingPanel={this.onShowEmbeddingPanel}
                  onToggleLink={this.onToggleLink}
                />
              ))}
              {isNotifyUsers && (
                <div className="sharing_panel-text-area">
                  <Textarea
                    placeholder={t("AddShareMessage")}
                    onChange={this.onChangeMessage}
                    value={message}
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
              />
              <Button
                className="sharing_panel-button"
                label={t("AddButton")}
                size="big"
                primary
                onClick={this.onSaveClick}
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
            accessRight={accessRight}
            groupsCaption={groupsCaption}
            advancedOptions={advancedOptions}
          />
        )}

        {showAddGroupsPanel && (
          <AddGroupsPanel
            onSharingPanelClose={this.onClose}
            onClose={this.onShowGroupsPanel}
            visible={showAddGroupsPanel}
            shareDataItems={shareDataItems}
            setShareDataItems={this.setShareDataItems}
            accessRight={accessRight}
            advancedOptions={advancedOptions}
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

const SharingPanelContainerTranslated = withTranslation()(
  SharingPanelComponent
);

const SharingPanel = (props) => (
  <SharingPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {
    getAccessOption: (selection) => getAccessOption(state, selection),
    getExternalAccessOption: (selectedItems) =>
      getExternalAccessOption(state, selectedItems),
    isMyId: getCurrentUserId(state),
    selection: getSelection(state),
    groupsCaption: getSettingsCustomNamesGroupsCaption(state),
    sharingPanelVisible: getSharePanelVisible(state),
  };
};

export default connect(mapStateToProps, { setSharingPanelVisible })(
  withRouter(SharingPanel)
);
