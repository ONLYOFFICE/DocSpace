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
  utils
} from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { utils as commonUtils } from "asc-web-common";
import i18n from "./i18n";
import {
  getShareUsersAndGroups,
  setShareData
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
      shareData: props.shareData,
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
    console.log("onSaveClick");
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
    const newUsers = this.state.shareData;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "FullAccess") {
      newUsers[elementIndex].rights = {
        icon: "AccessEditIcon",
        rights: "FullAccess"
      };
      this.setState({ shareData: newUsers });
    }
  };
  onReadOnlyClick = item => {
    const newUsers = this.state.shareData;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "ReadOnly") {
      newUsers[elementIndex].rights = { icon: "EyeIcon", rights: "ReadOnly" };
      this.setState({ shareData: newUsers });
    }
  };
  onReviewClick = item => {
    const newUsers = this.state.shareData;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "Review") {
      newUsers[elementIndex].rights = {
        icon: "AccessReviewIcon",
        rights: "Review"
      };
      this.setState({ shareData: newUsers });
    }
  };
  onCommentClick = item => {
    const newUsers = this.state.shareData;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "Comment") {
      newUsers[elementIndex].rights = {
        icon: "AccessCommentIcon",
        rights: "Comment"
      };
      this.setState({ shareData: newUsers });
    }
  };
  onFormFillingClick = item => {
    const newUsers = this.state.shareData;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "FormFilling") {
      newUsers[elementIndex].rights = {
        icon: "AccessFormIcon",
        rights: "FormFilling"
      };
      this.setState({ shareData: newUsers });
    }
  };
  onDenyAccessClick = item => {
    const newUsers = this.state.shareData;
    const elementIndex = newUsers.findIndex(x => x.id === item.id);
    if (newUsers[elementIndex].rights.rights !== "DenyAccess") {
      newUsers[elementIndex].rights = {
        icon: "AccessNoneIcon",
        rights: "DenyAccess"
      };
      this.setState({ shareData: newUsers });
    }
  };

  onRemoveUserClick = item => {
    const shareData = this.state.shareData.slice(0);

    const index = shareData.findIndex(x => x.id === item.id);
    if (index !== -1) {
      shareData.splice(index, 1);
      this.props.setShareData(shareData.slice(0));
    }
  };

  getItemAccess = item => {
    switch (item.access) {
      case 1:
        return {
          icon: "AccessEditIcon",
          rights: "FullAccess",
          isOwner: item.isOwner
        };
      case 2:
        return {
          icon: "EyeIcon",
          rights: "ReadOnly",
          isOwner: false
        };
      case 3: {
        //console.log("Share link", item.sharedTo.shareLink);
        return;
      }
      default:
        return;
    }
  };

  getShareDataItems = arrayItems => {
    const listOwners = [];
    const shareData = [];
    for (let array of arrayItems) {
      for (let item of array) {
        if (item.isOwner) {
          listOwners.push(item.sharedTo.id);
        }
        const rights = this.getItemAccess(item);

        if (rights) {
          item.sharedTo = { ...item.sharedTo, ...{ rights } };
          shareData.push(item.sharedTo);
        }
      }
    }
    return [shareData, listOwners];
  };

  removeDuplicateShareData = shareData => {
    let obj = {};
    return shareData.filter(x => {
      if (obj[x.id]) return false;
      obj[x.id] = true;
      return true;
    });
  };

  setDuplicateItemsRights = (shareData, rights) => {
    const array = shareData.slice(0);

    let i = 0;
    while (array.length !== 0) {
      const item = array[i];
      array.splice(i, 1);
      const duplicateItem = array.find(x => x.id === item.id);
      if (duplicateItem) {
        if (item.rights.rights !== duplicateItem.rights.rights) {
          const shareIndex = shareData.findIndex(
            x => x.id === duplicateItem.id
          );
          shareData[shareIndex].rights = rights;
        }
      }
    }
    return shareData;
  };

  setOwnersRights = (listOwners, shareData, rights) => {
    if (listOwners.length > 1) {
      while (listOwners.length !== 0) {
        const index = shareData.findIndex(x => x.id === listOwners[0]);
        shareData[index].rights = rights;
        listOwners.splice(0, 1);
      }
    }
    return shareData;
  };

  getShareData() {
    const foldersIds = [];
    const filesIds = [];

    for (let item of this.props.selection) {
      if (item.fileExst) {
        filesIds.push(item.id);
      } else {
        foldersIds.push(item.id);
      }
    }
    let shareData = [];
    let listOwners = [];
    getShareUsersAndGroups(foldersIds, filesIds)
      .then(res => {
        //console.log("Response", res);

        shareData = this.getShareDataItems(res)[0];
        listOwners = this.getShareDataItems(res)[1];
      })
      .then(() => {
        const rights = {
          icon: "CatalogQuestionIcon",
          rights: "Varies",
          isOwner: false
        };

        shareData = this.setDuplicateItemsRights(shareData, rights);
        shareData = this.removeDuplicateShareData(shareData);
        shareData = this.setOwnersRights(listOwners, shareData, rights);

        this.props.setShareData(shareData.slice(0));
        //this.setState({ shareData: shareData.slice(0) });
        this.setState({ baseShareData: shareData.slice(0) });
      }).catch(() => console.log("HELLO"));
  }

  componentDidUpdate(prevProps, prevState) {
    const { selection, shareData } = this.props;

    if (selection.length !== 0) {
      if (
        !utils.array.isArrayEqual(prevProps.selection, selection) ||
        selection.length !== prevProps.selection.length
      ) {
        this.getShareData(selection);
      }
    }

    if (
      !utils.array.isArrayEqual(this.state.shareData, shareData) ||
      this.state.shareData.length !== shareData.length
    ) {
      this.setState({ shareData });
    }
  }

  onClose = () => {
    this.props.setShareData(this.state.baseShareData.slice(0));
    this.props.onClose();
  };

  render() {
    const { visible, t, accessOptions, isMe } = this.props;
    const { showActionPanel, isNotifyUsers, shareData } = this.state;

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
              {shareData.map((item, index) => {
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
  const { shareData, selection } = state.files;

  return { shareData, selection, isMe: state.auth.user.id };
};

export default connect(mapStateToProps, { setShareData })(
  withRouter(SharingPanel)
);
