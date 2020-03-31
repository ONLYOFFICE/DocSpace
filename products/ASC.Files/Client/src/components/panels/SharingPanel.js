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
import {
  StyledPanel,
  StyledContent,
  StyledFooter,
  StyledSharingHeaderContent,
  StyledSharingBody
} from "./StyledPanels";

class SharingPanel extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showActionPanel: false,
      isNotifyUsers: false
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
    console.log("ITEM", item);
  };
  onReadOnlyClick = item => {
    console.log("ITEM", item);
  };
  onReviewClick = item => {
    console.log("ITEM", item);
  };
  onCommentClick = item => {
    console.log("ITEM", item);
  };
  onFormFillingClick = item => {
    console.log("ITEM", item);
  };
  onDenyAccessClick = item => {
    console.log("ITEM", item);
  };

  /*shouldComponentUpdate(nextProps, nextState) {
    const { showActionPanel, isNotifyUsers } = this.state;
    const { visible, users, accessRight } = this.props;

    if (accessRight && accessRight.rights !== nextProps.accessRight.rights) {
      return true;
    }

    if (
      !utils.array.isArrayEqual(nextProps.users, users) ||
      users.length !== nextProps.users.length
    ) {
      return true;
    }

    if (showActionPanel !== nextState.showActionPanel) {
      return true;
    }

    if (isNotifyUsers !== nextState.isNotifyUsers) {
      return true;
    }

    if (visible !== nextProps.visible) {
      return true;
    }

    return false;
  }*/

  render() {
    const checkboxNotifyUsersLabel = "Notify users";
    const addUserTranslationLabel = "Add user";
    const addGroupTranslationLabel = "Add group";
    const sharingHeaderText = "Sharing settings";

    const { onClose, visible, users, onRemoveUserClick } = this.props;
    const { showActionPanel, isNotifyUsers } = this.state;

    const zIndex = 310;

    return (
      <StyledPanel visible={visible}>
        <Backdrop onClick={onClose} visible={visible} zIndex={zIndex} />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledSharingHeaderContent>
              <Heading className="sharing_panel-header" size="medium" truncate>
                {sharingHeaderText}
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
                      label={addUserTranslationLabel}
                      onClick={this.showAddUserPanel}
                    />
                    <DropDownItem
                      label={addGroupTranslationLabel}
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
              {users.map((item, index) => {
                const advancedOptions = (
                  <>
                    <DropDownItem
                      label="Full access"
                      icon="AccessEditIcon"
                      onClick={this.onFullAccessClick.bind(this, item)}
                    />
                    <DropDownItem
                      label="Read only"
                      icon="EyeIcon"
                      onClick={this.onReadOnlyClick.bind(this, item)}
                    />
                    <DropDownItem
                      label="Review"
                      icon="AccessReviewIcon"
                      onClick={this.onReviewClick.bind(this, item)}
                    />
                    <DropDownItem
                      label="Comment"
                      icon="AccessCommentIcon"
                      onClick={this.onCommentClick.bind(this, item)}
                    />
                    <DropDownItem
                      label="Form filling"
                      icon="AccessFormIcon"
                      onClick={this.onFormFillingClick.bind(this, item)}
                    />
                    <DropDownItem
                      label="Deny access"
                      icon="AccessNoneIcon"
                      onClick={this.onDenyAccessClick.bind(this, item)}
                    />
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
                    element={embeddedComponent}
                    contextButtonSpacerWidth="0px"
                  >
                    <Text truncate>
                      {item.label
                        ? item.label
                        : item.name
                        ? item.name
                        : item.displayName}
                    </Text>
                    {index === 0 ? (
                      <Text
                        className="sharing_panel-remove-icon"
                        //color="#A3A9AE"
                      >
                        Owner
                      </Text>
                    ) : (
                      <IconButton
                        iconName="RemoveIcon"
                        onClick={onRemoveUserClick.bind(this, item)}
                        className="sharing_panel-remove-icon"
                        size="medium"
                      />
                    )}
                  </Row>
                );
              })}
            </StyledSharingBody>
            <StyledFooter>
              <Checkbox
                isChecked={isNotifyUsers}
                label={checkboxNotifyUsersLabel}
                onChange={this.onNotifyUsersChange}
              />
              <Button
                className="sharing_panel-button"
                label="Save"
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

SharingPanel.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool,
  onShowUsersPanel: PropTypes.func,
  onShowGroupsPanel: PropTypes.func,
  onRemoveUserClick: PropTypes.func,
  users: PropTypes.array
};

export default SharingPanel;
