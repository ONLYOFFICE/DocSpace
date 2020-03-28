import React from "react";
import {
  Backdrop,
  Heading,
  Aside,
  IconButton,
  Checkbox,
  Button,
  DropDown,
  DropDownItem
} from "asc-web-components";
import styled from "styled-components";

const StyledSharingPanel = styled.div`
  .sharing_panel-aside {
    transform: translateX(${props => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: ${props => (props.scale ? "100%" : "320px")};
      transform: translateX(${props => (props.visible ? "0" : "320px")});
    }
  }
`;

const Content = styled.div`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  background-color: #fff;
  padding: 0 16px 16px;

  .sharing_panel-header {
    max-width: 500px;
    margin: 0;
    line-height: 56px;
    font-weight: 700;
  }
`;

const HeaderContent = styled.div`
  display: flex;
  align-items: center;
  border-bottom: 1px solid #dee2e6;

  .sharing_panel-icons-container {
    display: flex;
    margin-left: auto;

    .sharing_panel-drop-down-wrapper {
      position: relative;

      .sharing_panel-drop-down {
        padding: 8px 16px;
      }
      .sharing_panel-plus-icon {
        margin-right: 12px;
      }
    }
  }
`;

const Body = styled.div`
  position: relative;
  padding: 16px 0;
`;

const Footer = styled.div`
  display: flex;
  position: fixed;
  bottom: 16px;
  width: 94%;

  .sharing_panel-button {
    margin-left: auto;
  }

  @media (max-width: 550px) {
    width: 90%;
  }
`;

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

  render() {
    const checkboxNotifyUsersLabel = "Notify users";
    const addUserTranslationLabel = "Add user";
    const addGroupTranslationLabel = "Add group";
    const sharingHeaderText = "Sharing settings";

    const { onClose, selection, visible } = this.props;
    const { showActionPanel, isNotifyUsers } = this.state;

    const zIndex = 310;
    return (
      <StyledSharingPanel visible={visible}>
        <Backdrop onClick={onClose} visible={visible} zIndex={zIndex} />
        <Aside className="sharing_panel-aside" visible={visible}>
          <Content>
            <HeaderContent>
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
            </HeaderContent>
            <Body>Sharing content</Body>
            <Footer>
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
            </Footer>
          </Content>
        </Aside>
      </StyledSharingPanel>
    );
  }
}

export default SharingPanel;
