import React from "react";
import PropTypes from "prop-types";
import { Backdrop, Heading, Aside, IconButton } from "asc-web-components";
import { PeopleSelector } from "asc-web-common";
import {
  StyledPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody
} from "./StyledPanels";

class AddUsersPanel extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showActionPanel: false
    };
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onKeyClick = () => console.log("onKeyClick");

  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  onPeopleSelect = users => {
    const items = [];
    for (let item of users) {
      item.id = item.key;
      delete item.key;
      item.rights = this.props.accessRight;
      items.push(item);
    }

    this.props.onSetSelectedUsers(items);
    this.props.onClose();
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

    const headerText = "Add users";
    const zIndex = 310;

    //console.log("AddUsersPanel render");
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
              <PeopleSelector
                displayType="aside"
                withoutAside
                isOpen={visible}
                isMultiSelect
                onSelect={this.onPeopleSelect}
                embeddedComponent={embeddedComponent}

                //onCancel={this.onCancelSelector}
                //groupsCaption={groupsCaption}
                //defaultOptionLabel={t("MeLabel")}
              />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledPanel>
    );
  }
}

AddUsersPanel.propTypes = {
  visible: PropTypes.bool,
  onSharingPanelClose: PropTypes.func,
  onClose: PropTypes.func,
  onSetSelectedUsers: PropTypes.func
};

export default AddUsersPanel;
