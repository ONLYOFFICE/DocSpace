import React from "react";
import PropTypes from "prop-types";
import { Backdrop, Heading, Aside, IconButton } from "asc-web-components";
import { PeopleSelector, utils } from "asc-web-common";
import { withTranslation } from "react-i18next";
import {
  StyledAddUsersPanelPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "AddUsersPanel",
  localesPath: "panels/AddUsersPanel",
});

const { changeLanguage } = utils;

class AddUsersPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = {
      showActionPanel: false,
    };

    this.scrollRef = React.createRef();
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  onPeopleSelect = (users) => {
    const {
      accessRight,
      shareDataItems,
      setShareDataItems,
      onClose,
    } = this.props;
    const items = shareDataItems;
    for (let item of users) {
      if (item.key) {
        item.id = item.key;
        delete item.key;
      }
      const currentItem = shareDataItems.find((x) => x.id === item.id);
      if (!currentItem) {
        item.rights = accessRight;
        items.push(item);
      }
    }

    setShareDataItems(items);
    onClose();
  };

  componentDidMount() {
    const scroll = this.scrollRef.current.getElementsByClassName("scroll-body");
    setTimeout(() => scroll[1] && scroll[1].focus(), 2000);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPress);
  }

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.props.onClose();
    }
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
    const { visible, embeddedComponent, t, groupsCaption } = this.props;

    const zIndex = 310;

    //console.log("AddUsersPanel render");
    return (
      <StyledAddUsersPanelPanel visible={visible}>
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
                color="#A3A9AE"
              />
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                {t("LinkText")}
              </Heading>
              {/*<IconButton
                size="16"
                iconName="PlusIcon"
                className="header_aside-panel-plus-icon"
                onClick={() => console.log("onPlusClick")}
              />*/}
            </StyledHeaderContent>

            <StyledBody ref={this.scrollRef}>
              <PeopleSelector
                displayType="aside"
                withoutAside
                isOpen={visible}
                isMultiSelect
                onSelect={this.onPeopleSelect}
                embeddedComponent={embeddedComponent}
                groupsCaption={groupsCaption}
                showCounter={true}
                //onCancel={onClose}
              />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledAddUsersPanelPanel>
    );
  }
}

AddUsersPanelComponent.propTypes = {
  visible: PropTypes.bool,
  onSharingPanelClose: PropTypes.func,
  onClose: PropTypes.func,
};

const AddUsersPanelContainerTranslated = withTranslation()(
  AddUsersPanelComponent
);

const AddUsersPanel = (props) => (
  <AddUsersPanelContainerTranslated i18n={i18n} {...props} />
);

export default AddUsersPanel;
