import React from "react";
import PropTypes from "prop-types";
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
import { getShareUsers, setShareFiles } from "../../../store/files/actions";
import { getAccessOption, getSelection } from "../../../store/files/selectors";
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
      accessRight: {
        icon: "EyeIcon",
        rights: "ReadOnly",
        accessNumber: ShareAccessRights.ReadOnly,
      },
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

  //onKeyClick = () => console.log("onKeyClick");

  onSaveClick = () => {
    const {
      baseShareData,
      isNotifyUsers,
      message,
      shareDataItems,
    } = this.state;
    const { selectedItems, onClose } = this.props;

    const folderIds = [];
    const fileIds = [];

    const share = [];
    for (let item of shareDataItems) {
      const baseItem = baseShareData.find((x) => x.id === item.id);
      if (
        (baseItem && baseItem.rights.rights !== item.rights.rights) ||
        !baseItem
      ) {
        share.push({ shareTo: item.id, access: item.rights.accessNumber });
      }
    }

    for (let item of baseShareData) {
      const baseItem = shareDataItems.find((x) => x.id === item.id);
      if (!baseItem) {
        share.push({ shareTo: item.id, access: 0 });
      }
    }

    for (let item of selectedItems) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    setShareFiles(folderIds, fileIds, share, isNotifyUsers, message)
      .catch((err) => toastr.error(err))
      .finally(() => onClose());
  };

  onFullAccessClick = () => {
    this.setState({
      accessRight: {
        icon: "AccessEditIcon",
        rights: "FullAccess",
        accessNumber: ShareAccessRights.FullAccess,
        isOwner: false,
      },
    });
  };

  onReadOnlyClick = () => {
    this.setState({
      accessRight: {
        icon: "EyeIcon",
        rights: "ReadOnly",
        accessNumber: ShareAccessRights.ReadOnly,
        isOwner: false,
      },
    });
  };

  onReviewClick = () => {
    this.setState({
      accessRight: {
        icon: "AccessReviewIcon",
        rights: "Review",
        accessNumber: ShareAccessRights.Review,
        isOwner: false,
      },
    });
  };

  onCommentClick = () => {
    this.setState({
      accessRight: {
        icon: "AccessCommentIcon",
        rights: "Comment",
        accessNumber: ShareAccessRights.Comment,
        isOwner: false,
      },
    });
  };

  onFormFillingClick = () => {
    this.setState({
      accessRight: {
        icon: "AccessFormIcon",
        rights: "FormFilling",
        accessNumber: ShareAccessRights.FormFilling,
        isOwner: false,
      },
    });
  };

  onDenyAccessClick = () => {
    this.setState({
      accessRight: {
        icon: "AccessNoneIcon",
        rights: "DenyAccess",
        accessNumber: ShareAccessRights.DenyAccess,
        isOwner: false,
      },
    });
  };

  onNotifyUsersChange = () =>
    this.setState({ isNotifyUsers: !this.state.isNotifyUsers });

  onShowUsersPanel = () =>
    this.setState({
      showAddUsersPanel: !this.state.showAddUsersPanel,
      showActionPanel: false,
    });

  onFullAccessItemClick = (item) => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex((x) => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "FullAccess") {
      newUsers[elementIndex].rights = {
        icon: "AccessEditIcon",
        rights: "FullAccess",
        accessNumber: ShareAccessRights.FullAccess,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onReadOnlyItemClick = (item) => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex((x) => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "ReadOnly") {
      newUsers[elementIndex].rights = {
        icon: "EyeIcon",
        rights: "ReadOnly",
        accessNumber: ShareAccessRights.ReadOnly,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onReviewItemClick = (item) => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex((x) => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "Review") {
      newUsers[elementIndex].rights = {
        icon: "AccessReviewIcon",
        rights: "Review",
        accessNumber: ShareAccessRights.Review,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onCommentItemClick = (item) => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex((x) => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "Comment") {
      newUsers[elementIndex].rights = {
        icon: "AccessCommentIcon",
        rights: "Comment",
        accessNumber: ShareAccessRights.Comment,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onFormFillingItemClick = (item) => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex((x) => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "FormFilling") {
      newUsers[elementIndex].rights = {
        icon: "AccessFormIcon",
        rights: "FormFilling",
        accessNumber: ShareAccessRights.FormFilling,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onDenyAccessItemClick = (item) => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex((x) => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "DenyAccess") {
      newUsers[elementIndex].rights = {
        icon: "AccessNoneIcon",
        rights: "DenyAccess",
        accessNumber: ShareAccessRights.DenyAccess,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };

  onRemoveUserItemClick = (item) => {
    const shareDataItems = this.state.shareDataItems.slice(0);

    const index = shareDataItems.findIndex((x) => x.id === item.id);
    if (index !== -1) {
      shareDataItems.splice(index, 1);
      this.setState({ shareDataItems });
    }
  };

  getItemAccess = (item) => {
    const fullAccessRights = {
      icon: "AccessEditIcon",
      rights: "FullAccess",
      accessNumber: ShareAccessRights.FullAccess,
      isOwner: item.isOwner,
    };
    if (item.sharedTo.shareLink) {
      return fullAccessRights;
    }
    switch (item.access) {
      case 1:
        return fullAccessRights;
      case 2:
        return {
          icon: "EyeIcon",
          rights: "ReadOnly",
          accessNumber: ShareAccessRights.ReadOnly,
          isOwner: false,
        };
      case 3:
        return {
          icon: "AccessNoneIcon",
          rights: "DenyAccess",
          accessNumber: ShareAccessRights.DenyAccess,
          isOwner: false,
        };
      case 5:
        return {
          icon: "AccessReviewIcon",
          rights: "Review",
          accessNumber: ShareAccessRights.Review,
          isOwner: false,
        };
      case 6:
        return {
          icon: "AccessCommentIcon",
          rights: "Comment",
          accessNumber: ShareAccessRights.Comment,
          isOwner: false,
        };
      case 7:
        return {
          icon: "AccessFormIcon",
          rights: "FormFilling",
          accessNumber: ShareAccessRights.FormFilling,
          isOwner: false,
        };
      default:
        return;
    }
  };

  getShareDataItems = (items) => {
    let arrayItems = [];
    const newItems = [];
    let stash = [];

    for (let array of items) {
      for (let item of array) {
        const rights = this.getItemAccess(item);

        if (rights) {
          item.sharedTo = { ...item.sharedTo, ...{ rights } };
          arrayItems.push(item.sharedTo);
          stash.push(item.sharedTo);
        }
      }
      newItems.push(stash);
      stash = [];
    }

    stash = null;
    for (let item of arrayItems) {
      let length = newItems.length;
      if (!item.shareLink) {
        while (length !== 0) {
          if (newItems[length - 1].length !== 0) {
            stash = newItems[length - 1].find((x) => x.id === item.id);
            if (stash === this.props.isMyId) {
              const adminRights = {
                icon: "AccessEditIcon",
                rights: "FullAccess",
                accessNumber: ShareAccessRights.FullAccess,
                isOwner: item.isOwner,
              };
              item.rights = adminRights;
            } else if (
              !stash ||
              item.rights.rights !== stash.rights.rights ||
              item.rights.isOwner !== stash.rights.isOwner
            ) {
              const variesRights = {
                icon: "CatalogQuestionIcon",
                rights: "Varies",
                isOwner: false,
              };
              item.rights = variesRights;
            }
          }
          length--;
        }
      }
    }

    arrayItems = this.removeDuplicateShareData(arrayItems);
    const baseShareData = JSON.parse(JSON.stringify(arrayItems));

    const accessOptions = getAccessOption(this.props.selectedItems);

    return { baseShareData, shareDataItems: arrayItems, accessOptions };
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
    const { selectedItems } = this.props;
    const folderId = [];
    const fileId = [];

    for (let item of selectedItems) {
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
    let error = null;
    let shareData = {};

    if (folderId.length !== 0 || fileId.length !== 0) {
      getShareUsers(folderId, fileId)
        .then((res) => {
          shareData = this.getShareDataItems(res);
        })
        .catch((err) => {
          error = err;
          toastr.error(err);
        })
        .finally(
          () =>
            !error && this.setState({ ...shareData, ...{ showPanel: true } })
        );
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

  onClose = () => this.setState({ showPanel: false });

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
      this.props.onClose();
    }
  };

  componentDidUpdate(prevProps, prevState) {
    if (
      this.state.showPanel !== prevState.showPanel &&
      this.state.showPanel === false
    ) {
      this.props.onClose();
    }

    if (this.state.message === prevState.message) {
      this.scrollRef.current.view.focus();
    }
  }

  render() {
    //console.log("Sharing panel render");
    const { t, isMyId, selectedItems, groupsCaption } = this.props;
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

        {accessOptions.includes("ReadOnly") && (
          <DropDownItem
            label="Read only"
            icon="EyeIcon"
            onClick={this.onReadOnlyClick}
          />
        )}

        {accessOptions.includes("Review") && (
          <DropDownItem
            label="Review"
            icon="AccessReviewIcon"
            onClick={this.onReviewClick}
          />
        )}

        {accessOptions.includes("Comment") && (
          <DropDownItem
            label="Comment"
            icon="AccessCommentIcon"
            onClick={this.onCommentClick}
          />
        )}

        {accessOptions.includes("FormFilling") && (
          <DropDownItem
            label="Form filling"
            icon="AccessFormIcon"
            onClick={this.onFormFillingClick}
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

    const accessOptionsComboBox = (
      <ComboBox
        advancedOptions={advancedOptions}
        options={[]}
        selectedOption={{ key: 0, arrowIconColor: "#333333" }}
        size="content"
        className="panel_combo-box add-groups"
        scaled={false}
        directionX="right"
        //isDisabled={isDisabled}
      >
        {React.createElement(Icons[accessRight.icon], {
          size: "medium",
          //color: this.state.currentIconColor,
          //isfill: isFill
        })}
      </ComboBox>
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
            <StyledSharingBody ref={this.scrollRef} stype="mediumBlack">
              {shareDataItems.map((item, index) => (
                <SharingRow
                  key={index}
                  t={t}
                  selection={selectedItems}
                  item={item}
                  index={index}
                  isMyId={isMyId}
                  accessOptions={accessOptions}
                  onFullAccessClick={this.onFullAccessItemClick}
                  onReadOnlyClick={this.onReadOnlyItemClick}
                  onReviewClick={this.onReviewItemClick}
                  onCommentClick={this.onCommentItemClick}
                  onFormFillingClick={this.onFormFillingItemClick}
                  onDenyAccessClick={this.onDenyAccessItemClick}
                  onRemoveUserClick={this.onRemoveUserItemClick}
                  onShowEmbeddingPanel={this.onShowEmbeddingPanel}
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
            embeddedComponent={accessOptionsComboBox}
            accessRight={accessRight}
            shareDataItems={shareDataItems}
            setShareDataItems={this.setShareDataItems}
            groupsCaption={groupsCaption}
          />
        )}

        {showAddGroupsPanel && (
          <AddGroupsPanel
            onSharingPanelClose={this.onClose}
            onClose={this.onShowGroupsPanel}
            visible={showAddGroupsPanel}
            embeddedComponent={accessOptionsComboBox}
            accessRight={accessRight}
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

SharingPanelComponent.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool,
};

const SharingPanelContainerTranslated = withTranslation()(
  SharingPanelComponent
);

const SharingPanel = (props) => (
  <SharingPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {
    isMyId: getCurrentUserId(state),
    selectedItems: getSelection(state),
    groupsCaption: getSettingsCustomNamesGroupsCaption(state),
  };
};

export default connect(mapStateToProps)(withRouter(SharingPanel));
