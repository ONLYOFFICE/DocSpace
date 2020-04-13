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
  utils,
  toastr,
  Textarea,
  ComboBox,
  Icons,
} from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { utils as commonUtils, constants, api } from "asc-web-common";
import i18n from "./i18n";
import {
  getShareUsersAndGroups,
  setShareDataItems,
  setSharedFolders,
  setSharedFiles,
  setShareData,
} from "../../../store/files/actions";
import {
  StyledSharingPanel,
  StyledContent,
  StyledFooter,
  StyledSharingHeaderContent,
  StyledSharingBody,
} from "../StyledPanels";
import { AddUsersPanel, AddGroupsPanel, EmbeddingPanel } from "../index";
import SharingRow from "./SharingRow";

const { changeLanguage } = commonUtils;
const { ShareAccessRights } = constants;
const { files } = api;

class SharingPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = {
      showActionPanel: false,
      isNotifyUsers: false,
      shareDataItems: props.shareDataItems,
      baseShareData: null,
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
    };

    this.ref = React.createRef();
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onCloseActionPanel = (e) => {
    if (this.ref.current.contains(e.target)) return;
    this.setState({ showActionPanel: !this.state.showActionPanel });
  };

  onKeyClick = () => console.log("onKeyClick");

  onSaveClick = () => {
    toastr.success("onSaveClick");

    const { baseShareData, isNotifyUsers } = this.state;
    const { shareDataItems, selection } = this.props;

    const folderIds = [];
    const fileIds = [];

    const shareTo = [];
    const access = [];
    for (let item of shareDataItems) {
      const baseItem = baseShareData.find((x) => x.id === item.id);
      if (
        (baseItem && baseItem.rights.rights !== item.rights.rights) ||
        !baseItem
      ) {
        shareTo.push(item.id);
        access.push(item.rights.accessNumber);
      }
    }

    const notify = isNotifyUsers;

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    /*folderIds.length > 0 &&
      setSharedFolders(folderIds, shareTo, access, notify, message);
    fileIds.length > 0 &&
      setSharedFiles(fileIds, shareTo, access, notify, message);*/

    this.onClose();
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
    this.setState({ showAddUsersPanel: !this.state.showAddUsersPanel });

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
      this.props.setShareDataItems(shareDataItems.slice(0));
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

  getShareDataItems = (items, shareDataItems, foldersIds, filesIds) => {
    const storeShareData = [];
    const arrayItems = [];
    for (let item of items) {
      const rights = this.getItemAccess(item);

      if (rights) {
        item.sharedTo = { ...item.sharedTo, ...{ rights } };
        arrayItems.push(item.sharedTo);
      }
    }
    if (foldersIds) {
      storeShareData.push({
        folderId: foldersIds[0],
        shareDataItems: arrayItems,
      });
    } else {
      storeShareData.push({ fileId: filesIds[0], shareDataItems: arrayItems });
    }
    this.props.setShareData([...storeShareData, ...this.props.shareData]);

    let listOfArrays = [...shareDataItems.slice(0), ...[arrayItems.slice(0)]];

    listOfArrays = listOfArrays.filter((x) => x.length !== 0);

    let allItems = [];
    for (let array of listOfArrays) {
      for (let item of array) {
        allItems.push(item);
      }
    }

    allItems = this.removeDuplicateShareData(allItems);
    allItems = JSON.parse(JSON.stringify(allItems));

    let stash = 0;
    for (let item of allItems) {
      let length = listOfArrays.length;
      if (!item.shareLink) {
        while (length !== 0) {
          if (listOfArrays[length - 1].length !== 0) {
            stash = listOfArrays[length - 1].find((x) => x.id === item.id);
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

    this.setShareDataItemsFunction(allItems);
  };

  removeDuplicateShareData = (shareDataItems) => {
    let obj = {};
    return shareDataItems.filter((x) => {
      if (obj[x.id]) return false;
      obj[x.id] = true;
      return true;
    });
  };

  setShareDataItemsFunction = (shareDataItems) => {
    shareDataItems = shareDataItems.filter(
      (x) => x !== undefined && x.length !== 0
    );

    const clearShareData = JSON.parse(JSON.stringify(shareDataItems));
    this.props.setShareDataItems(shareDataItems.slice(0));
    this.setState({ baseShareData: clearShareData });
  };

  getData = () => {
    const { selection, shareData } = this.props;
    const shareDataItems = [];
    const folderId = [];
    const fileId = [];

    for (let item of selection) {
      if (item.fileExst) {
        const itemShareData = shareData.find((x) => x.fileId === item.id);

        if (itemShareData) {
          shareDataItems.push(itemShareData.shareDataItems);
        } else {
          fileId.push(item.id);
        }
      } else {
        const itemShareData = shareData.find((x) => x.folderId === item.id);
        if (itemShareData) {
          shareDataItems.push(itemShareData.shareDataItems);
        } else {
          folderId.push(item.id);
        }
      }
    }

    return [shareDataItems, folderId, fileId];
  };

  getShareData() {
    const returnValue = this.getData();
    let shareDataItems = returnValue[0];
    const folderId = returnValue[1];
    const fileId = returnValue[2];

    if (folderId.length !== 0) {
      files
        .getShareFolders(folderId)
        .then((res) => {
          shareDataItems = this.getShareDataItems(
            res,
            shareDataItems,
            folderId,
            null
          );
        })
        .catch((err) => {
          const newShareDataItem = [
            { folderId: folderId[0], shareDataItems: [] },
          ];
          this.props.setShareData([
            ...newShareDataItem,
            ...this.props.shareData,
          ]);
          console.log("getShareFolders", err);
          return;
        });
    } else if (fileId.length !== 0) {
      files
        .getShareFiles(fileId)
        .then((res) => {
          shareDataItems = this.getShareDataItems(
            res,
            shareDataItems,
            null,
            fileId
          );
        })
        .catch((err) => {
          const newShareDataItem = [{ fileId: fileId[0], shareDataItems: [] }];
          this.props.setShareData([
            ...newShareDataItem,
            ...this.props.shareData,
          ]);
          console.log("getShareFiles", err);
          return;
        });
    } else {
      shareDataItems = this.getShareDataItems([], shareDataItems, null, fileId);
    }
  }

  onClose = () => {
    this.props.setShareDataItems(this.state.baseShareData.slice(0));
    this.setState({ message: "", isNotifyUsers: false });
    this.props.onClose();
  };

  onShowEmbeddingPanel = (link) =>
    this.setState({
      showEmbeddingPanel: !this.state.showEmbeddingPanel,
      shareLink: link,
    });

  onShowGroupsPanel = () =>
    this.setState({ showAddGroupsPanel: !this.state.showAddGroupsPanel });

  onChangeMessage = (e) => this.setState({ message: e.target.value });

  componentDidUpdate(prevProps, prevState) {
    const { selection, shareDataItems } = this.props;

    if (selection.length !== 0) {
      if (
        !utils.array.isArrayEqual(prevProps.selection, selection) ||
        selection.length !== prevProps.selection.length
      ) {
        this.getShareData();
      }
    }

    if (
      !utils.array.isArrayEqual(this.state.shareDataItems, shareDataItems) ||
      this.state.shareDataItems.length !== shareDataItems.length
    ) {
      this.setState({ shareDataItems });
    }
  }

  render() {
    //console.log("Sharing panel render");
    const { visible, t, accessOptions, isMyId, selection } = this.props;
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
    } = this.state;

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
        selectedOption={{ key: 0 }}
        size="content"
        className="panel_combo-box"
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
      <StyledSharingPanel visible={visible}>
        <Backdrop onClick={this.onClose} visible={visible} zIndex={zIndex} />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledSharingHeaderContent>
              <Heading className="sharing_panel-header" size="medium" truncate>
                {t("SharingSettingsTitle")}
              </Heading>
              <div className="sharing_panel-icons-container">
                <div ref={this.ref} className="sharing_panel-drop-down-wrapper">
                  <IconButton
                    size="16"
                    iconName="PlusIcon"
                    className="sharing_panel-plus-icon"
                    onClick={this.onPlusClick}
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

                <IconButton
                  size="16"
                  iconName="KeyIcon"
                  onClick={this.onKeyClick}
                />
              </div>
            </StyledSharingHeaderContent>
            <StyledSharingBody>
              {shareDataItems.map((item, index) => (
                <SharingRow
                  key={index}
                  t={t}
                  selection={selection}
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

        <AddUsersPanel
          onSharingPanelClose={this.onClose}
          onClose={this.onShowUsersPanel}
          visible={showAddUsersPanel}
          embeddedComponent={accessOptionsComboBox}
          setShareDataItems={setShareDataItems}
          accessRight={accessRight}
          shareDataItems={shareDataItems}
        />

        <AddGroupsPanel
          onSharingPanelClose={this.onClose}
          onClose={this.onShowGroupsPanel}
          visible={showAddGroupsPanel}
          embeddedComponent={accessOptionsComboBox}
          setShareDataItems={setShareDataItems}
          accessRight={accessRight}
          shareDataItems={shareDataItems}
        />

        <EmbeddingPanel
          visible={showEmbeddingPanel}
          onSharingPanelClose={this.onClose}
          onClose={this.onShowEmbeddingPanel}
          embeddingLink={shareLink}
        />
      </StyledSharingPanel>
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
  const { shareDataItems, shareData, selection } = state.files;

  return { shareDataItems, shareData, selection, isMyId: state.auth.user.id };
};

export default connect(mapStateToProps, { setShareDataItems, setShareData })(
  withRouter(SharingPanel)
);
