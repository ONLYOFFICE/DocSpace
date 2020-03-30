import React from "react";
import PropTypes from "prop-types";
import {
  Backdrop,
  Heading,
  Aside,
  IconButton,
  toastr
} from "asc-web-components";
import { GroupSelector } from "asc-web-common";
import { getUsersOfGroups } from "../../../../../../store/files/actions";
import {
  StyledPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody
} from "./StyledPanels";

class AddGroupPanel extends React.Component {
  constructor(props) {
    super(props);

    this.state = { showActionPanel: false };
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onKeyClick = () => console.log("onKeyClick");

  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  onSelectGroups = groups => {
    const groupIds = [];
    for (let item of groups) {
      groupIds.push(item.key);
    }

    getUsersOfGroups(groupIds)
      .then(res => {
        this.props.onSetSelectedUsers(res);
        this.props.onClose();
      })
      .catch(err => toastr.error(err));
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { showActionPanel } = this.state;
    const { visible, accessRight } = this.props;

    if (accessRight && accessRight.rights !== nextProps.accessRight.rights) {
      return true;
    }

    if (showActionPanel !== nextState.showActionPanel) {
      return true;
    }

    if (visible !== nextProps.visible) {
      return true;
    }

    return false;
  }

  render() {
    const { visible, embeddedComponent } = this.props;

    const headerText = "Add group";
    const zIndex = 310;

    //console.log("AddGroupPanel render");
    return (
      <StyledPanel visible={visible}>
        <Backdrop
          onClick={this.onClosePanels}
          visible={visible}
          zIndex={zIndex}
        />
        <Aside className="header_aside-panel">
          <StyledContent>
            <StyledHeaderContent>
              <IconButton
                size="16"
                iconName="ArrowPathIcon"
                onClick={this.onArrowClick}
              />
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                {headerText}
              </Heading>
              <IconButton
                size="16"
                iconName="PlusIcon"
                className="header_aside-panel-plus-icon"
                onClick={() => console.log("onPlusClick")}
              />
            </StyledHeaderContent>

            <StyledBody>
              <GroupSelector
                isOpen={visible}
                isMultiSelect
                displayType="aside"
                withoutAside
                onSelect={this.onSelectGroups}
                embeddedComponent={embeddedComponent}
                //onCancel={onCloseGroupSelector}
              />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledPanel>
    );
  }
}

AddGroupPanel.propTypes = {
  visible: PropTypes.bool,
  onSharingPanelClose: PropTypes.func,
  onClose: PropTypes.func,
  onSetSelectedUsers: PropTypes.func
};

export default AddGroupPanel;
