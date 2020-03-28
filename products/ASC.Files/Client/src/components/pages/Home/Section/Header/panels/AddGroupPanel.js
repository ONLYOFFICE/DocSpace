import React from "react";
import {
  Backdrop,
  Heading,
  Aside,
  IconButton,
  toastr
} from "asc-web-components";
import { GroupSelector } from "asc-web-common";
import styled from "styled-components";

const StyledAddUserPanel = styled.div`
  .add_users_panel-aside {
    transform: translateX(${props => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
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

  .add_users_panel-header {
    max-width: 500px;
    margin: 0 0 0 16px;
    line-height: 56px;
    font-weight: 700;
  }

  .add_users_panel-plus-icon {
    margin-left: auto;
  }
`;

const HeaderContent = styled.div`
  display: flex;
  align-items: center;
`;

const Body = styled.div`
  .selector-wrapper {
    position: fixed;
    height: 94%;

    .column-options {
      padding: 0 0 16px 0;
      width: 470px;

      @media (max-width: 550px) {
        width: 320px;
        padding: 0 28px 16px 0;
      }

      .body-options {
        padding-top: 16px;
      }
    }
    .footer {
      @media (max-width: 550px) {
        padding: 16px 28px 16px 0;
      }
    }
  }
`;

class AddGroupPanel extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showActionPanel: false,
      isNotifyUsers: false,
      currentIconName: "EyeIcon"
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

  onAddUsers = () => {
    console.log("onAddUsers");
    this.props.onClose();
  };

  onNotifyUsersChange = () =>
    this.setState({ isNotifyUsers: !this.state.isNotifyUsers });

  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  onSelectGroups = items => {
    this.props.onClose();
    toastr.success("onSelectGroups", "Task in progress");
    console.log("onSelect", items);
  };

  render() {
    const { selection, visible, me, advancedOptions } = this.props;
    const { currentIconName } = this.state;

    const headerText = "Add group";

    const zIndex = 310;
    return (
      <StyledAddUserPanel visible={visible}>
        <Backdrop
          onClick={this.onClosePanels}
          visible={visible}
          zIndex={zIndex}
        />
        <Aside className="add_users_panel-aside">
          <Content>
            <HeaderContent>
              <IconButton
                size="16"
                iconName="ArrowPathIcon"
                onClick={this.onArrowClick}
              />
              <Heading
                className="add_users_panel-header"
                size="medium"
                truncate
              >
                {headerText}
              </Heading>
              <IconButton
                size="16"
                iconName="PlusIcon"
                className="add_users_panel-plus-icon"
                onClick={() => console.log("onPlusClick")}
              />
            </HeaderContent>

            <Body>
              <GroupSelector
                isOpen={visible}
                isMultiSelect
                displayType="aside"
                withoutAside
                comboBoxOptions={advancedOptions}
                onSelect={this.onSelectGroups}
                //onCancel={onCloseGroupSelector}
              />
            </Body>
          </Content>
        </Aside>
      </StyledAddUserPanel>
    );
  }
}

export default AddGroupPanel;
