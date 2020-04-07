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
  ComboBox,
  Row,
  Text,
  Icons,
  utils,
  toastr
} from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { utils as commonUtils } from "asc-web-common";
import i18n from "./i18n";
import {
  getShareUsersAndGroups,
  setShareDataItems,
  setSharedFolders,
  setSharedFiles,
  setShareData,
} from "../../../store/files/actions";
import {
  StyledPanel,
  StyledContent,
  StyledFooter,
  StyledSharingHeaderContent,
  StyledSharingBody
} from "../StyledPanels";

const { changeLanguage } = commonUtils;

class SharingPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = {
      showActionPanel: false,
      isNotifyUsers: false,
      shareDataItems: props.shareDataItems,
      baseShareData: null
    };

    this.ref = React.createRef();
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onCloseActionPanel = e => {
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
    const sharingMessage = "message";

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    /*folderIds.length > 0 &&
      setSharedFolders(folderIds, shareTo, access, notify, sharingMessage);
    fileIds.length > 0 &&
      setSharedFiles(fileIds, shareTo, access, notify, sharingMessage);*/

    this.props.onClose();
  };

  onNotifyUsersChange = () =>
    this.setState({ isNotifyUsers: !this.state.isNotifyUsers });

  showAddUserPanel = () => {
    this.props.onShowUsersPanel();
  };

  showAddGroupPanel = () => {
    this.props.onShowGroupsPanel();
  };

  onFullAccessClick = item => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "FullAccess") {
      newUsers[elementIndex].rights = {
        icon: "AccessEditIcon",
        rights: "FullAccess",
        accessNumber: 1,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onReadOnlyClick = item => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "ReadOnly") {
      newUsers[elementIndex].rights = {
        icon: "EyeIcon",
        rights: "ReadOnly",
        accessNumber: 2,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onReviewClick = item => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "Review") {
      newUsers[elementIndex].rights = {
        icon: "AccessReviewIcon",
        rights: "Review",
        accessNumber: 999,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onCommentClick = item => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "Comment") {
      newUsers[elementIndex].rights = {
        icon: "AccessCommentIcon",
        rights: "Comment",
        accessNumber: 999,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onFormFillingClick = item => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "FormFilling") {
      newUsers[elementIndex].rights = {
        icon: "AccessFormIcon",
        rights: "FormFilling",
        accessNumber: 999,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };
  onDenyAccessClick = item => {
    const newUsers = this.state.shareDataItems;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "DenyAccess") {
      newUsers[elementIndex].rights = {
        icon: "AccessNoneIcon",
        rights: "DenyAccess",
        accessNumber: 999,
      };
      this.setState({ shareDataItems: newUsers });
    }
  };

  onRemoveUserClick = item => {
    const shareDataItems = this.state.shareDataItems.slice(0);

    const index = shareDataItems.findIndex(x => x.id === item.id);
    if (index !== -1) {
      shareDataItems.splice(index, 1);
      this.props.setShareDataItems(shareDataItems.slice(0));
    }
  };

  getItemAccess = item => {
    switch (item.access) {
      case 1:
        return {
          icon: "AccessEditIcon",
          rights: "FullAccess",
          accessNumber: 1,
          isOwner: item.isOwner,
        };
      case 2:
        return {
          icon: "EyeIcon",
          rights: "ReadOnly",
          accessNumber: 2,
          isOwner: false,
        };
      case 3: {
        //console.log("Share link", item.sharedTo.shareLink);
        return;
      }
      default:
        return;
    }
  };

  getShareDataItems = (res, shareDataItems, foldersIds, filesIds) => {
    const listOwners = [];
    const storeShareData = [];

    for (let array of res) {
      let foldersIndex = 0;
      let filesIndex = 0;
      let shareIndex = 0;

      if (foldersIds.length > foldersIndex) {
        storeShareData.push({
          folderId: foldersIds[foldersIndex],
          shareDataItems: []
        });
        foldersIndex++;
      } else {
        storeShareData.push({
          fileId: filesIds[filesIndex],
          shareDataItems: []
        });
        filesIndex++;
      }

      let arrayItems = [];
      for (let item of array) {
        const rights = this.getItemAccess(item);

        if (rights) {
          item.sharedTo = { ...item.sharedTo, ...{ rights } };
          shareDataItems.push(item.sharedTo);
          arrayItems.push(item.sharedTo);
        }
      }
      storeShareData[shareIndex].shareDataItems = arrayItems;
      arrayItems = [];
      shareIndex++;
    }

    for (let item of shareDataItems) {
      if (item.rights && item.rights.isOwner) {
        listOwners.push(item.id);
      }
    }

    this.props.setShareData([...storeShareData, ...this.props.shareData]);
    return [shareDataItems, listOwners];
  };

  removeDuplicateShareData = shareDataItems => {
    let obj = {};
    return shareDataItems.filter(x => {
      if (obj[x.id]) return false;
      obj[x.id] = true;
      return true;
    });
  };

  setDuplicateItemsRights = (shareDataItems, rights) => {
    const array = shareDataItems.slice(0);

    let i = 0;
    while (array.length !== 0) {
      const item = array[i];
      array.splice(i, 1);
      const duplicateItem = array.find(x => x.id === item.id);
      if (duplicateItem) {
        if (item.rights.rights !== duplicateItem.rights.rights) {
          const shareIndex = shareDataItems.findIndex(
            x => x.id === duplicateItem.id
          );
          shareDataItems[shareIndex].rights = rights;
        }
      }
    }
    return shareDataItems;
  };

  setOwnersRights = (listOwners, shareDataItems, rights) => {
    if (listOwners.length > 1) {
      while (listOwners.length !== 0) {
        const index = shareDataItems.findIndex(x => x.id === listOwners[0]);
        shareDataItems[index].rights = rights;
        listOwners.splice(0, 1);
      }
    }
    return shareDataItems;
  };

  getShareData() {
    const { selection, shareData } = this.props;

    const foldersIds = [];
    const filesIds = [];

    let shareDataItems = [];
    let listOwners = [];

    for (let item of selection) {
      if (item.fileExst) {
        const itemShareData = shareData.find(x => x.fileId === item.id);

        if (itemShareData) {
          for (let item of itemShareData.shareDataItems) {
            shareDataItems.push(item);
          }
        } else {
          filesIds.push(item.id);
        }
      } else {
        const itemShareData = shareData.find((x) => x.folderId === item.id);
        if (itemShareData) {
          for (let item of itemShareData.shareDataItems) {
            shareDataItems.push(item);
          }
        } else {
          foldersIds.push(item.id);
        }
      }
    }

    getShareUsersAndGroups(foldersIds, filesIds)
      .then(res => {
        //console.log("Response", res);

        const shareDataResult = this.getShareDataItems(
          res,
          shareDataItems,
          foldersIds,
          filesIds
        );
        shareDataItems = shareDataResult[0];
        listOwners = shareDataResult[1];
      })
      .then(() => {
        const rights = {
          icon: "CatalogQuestionIcon",
          rights: "Varies",
          isOwner: false,
        };

        shareDataItems = shareDataItems.filter(
          x => x !== undefined && x.length !== 0
        );
        shareDataItems = this.setDuplicateItemsRights(shareDataItems, rights);
        shareDataItems = this.removeDuplicateShareData(shareDataItems);
        shareDataItems = this.setOwnersRights(
          listOwners,
          shareDataItems,
          rights
        );

        const clearShareData = JSON.parse(JSON.stringify(shareDataItems));
        this.props.setShareDataItems(shareDataItems.slice(0));
        this.setState({ baseShareData: clearShareData });
      });
  }

  componentDidUpdate(prevProps, prevState) {
    const { selection, shareDataItems } = this.props;

    if (selection.length !== 0) {
      if (
        !utils.array.isArrayEqual(prevProps.selection, selection) ||
        selection.length !== prevProps.selection.length
      ) {
        this.getShareData(selection);
      }
    }

    if (
      !utils.array.isArrayEqual(this.state.shareDataItems, shareDataItems) ||
      this.state.shareDataItems.length !== shareDataItems.length
    ) {
      this.setState({ shareDataItems });
    }
  }

  onClose = () => {
    this.props.setShareDataItems(this.state.baseShareData.slice(0));
    this.props.onClose();
  };

  render() {
    const { visible, t, accessOptions, isMe } = this.props;
    const { showActionPanel, isNotifyUsers, shareDataItems } = this.state;

    const zIndex = 310;

    //console.log("Sharing panel render");

    return (
      <StyledPanel visible={visible}>
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
                      onClick={this.showAddUserPanel}
                    />
                    <DropDownItem
                      label={t("AddGroupsForSharingButton")}
                      onClick={this.showAddGroupPanel}
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
              {shareDataItems.map((item, index) => {
                const advancedOptions = (
                  <>
                    {accessOptions.includes("FullAccess") && (
                      <DropDownItem
                        label="Full access"
                        icon="AccessEditIcon"
                        onClick={this.onFullAccessClick.bind(this, item)}
                      />
                    )}

                    {accessOptions.includes("ReadOnly") && (
                      <DropDownItem
                        label="Read only"
                        icon="EyeIcon"
                        onClick={this.onReadOnlyClick.bind(this, item)}
                      />
                    )}

                    {accessOptions.includes("Review") && (
                      <DropDownItem
                        label="Review"
                        icon="AccessReviewIcon"
                        onClick={this.onReviewClick.bind(this, item)}
                      />
                    )}

                    {accessOptions.includes("Comment") && (
                      <DropDownItem
                        label="Comment"
                        icon="AccessCommentIcon"
                        onClick={this.onCommentClick.bind(this, item)}
                      />
                    )}

                    {accessOptions.includes("FormFilling") && (
                      <DropDownItem
                        label="Form filling"
                        icon="AccessFormIcon"
                        onClick={this.onFormFillingClick.bind(this, item)}
                      />
                    )}
                    {accessOptions.includes("DenyAccess") && (
                      <DropDownItem
                        label="Deny access"
                        icon="AccessNoneIcon"
                        onClick={this.onDenyAccessClick.bind(this, item)}
                      />
                    )}
                  </>
                );

                const embeddedComponent = (
                  <ComboBox
                    advancedOptions={advancedOptions}
                    options={[]}
                    selectedOption={{ key: 0 }}
                    size="content"
                    className="panel_combo-box"
                    scaled={false}
                    directionX="left"
                    //isDisabled={isDisabled}
                  >
                    {React.createElement(Icons[item.rights.icon], {
                      size: "medium",
                      className: "sharing-access-combo-box-icon"
                    })}
                  </ComboBox>
                );

                return (
                  <Row
                    key={index}
                    element={
                      item.rights.isOwner || item.id === isMe ? (
                        <Icons.AccessEditIcon
                          size="medium"
                          className="sharing_panel-owner-icon"
                        />
                      ) : (
                        embeddedComponent
                      )
                    }
                    contextButtonSpacerWidth="0px"
                  >
                    <>
                      <Text truncate>
                        {item.label
                          ? item.label
                          : item.name
                          ? item.name
                          : item.displayName}
                      </Text>
                      {item.rights.isOwner ? (
                        <Text
                          className="sharing_panel-remove-icon"
                          //color="#A3A9AE"
                        >
                          {t("Owner")}
                        </Text>
                      ) : item.id === isMe ? (
                        <Text
                          className="sharing_panel-remove-icon"
                          //color="#A3A9AE"
                        >
                          {t("AccessRightsFullAccess")}
                        </Text>
                      ) : (
                        <IconButton
                          iconName="RemoveIcon"
                          onClick={this.onRemoveUserClick.bind(this, item)}
                          className="sharing_panel-remove-icon"
                          size="medium"
                        />
                      )}
                    </>
                  </Row>
                );
              })}
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
      </StyledPanel>
    );
  }
}

SharingPanelComponent.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool,
  onShowUsersPanel: PropTypes.func,
  onShowGroupsPanel: PropTypes.func
};

const SharingPanelContainerTranslated = withTranslation()(
  SharingPanelComponent
);

const SharingPanel = props => (
  <SharingPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = state => {
  const { shareDataItems, shareData, selection } = state.files;

  return { shareDataItems, shareData, selection, isMe: state.auth.user.id };
};

export default connect(mapStateToProps, { setShareDataItems, setShareData })(
  withRouter(SharingPanel)
);
