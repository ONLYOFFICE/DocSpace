import React from "react";
import PropTypes from "prop-types";
import { Backdrop, Heading, Aside, IconButton } from "asc-web-components";
import { GroupSelector, utils } from "asc-web-common";
import { withTranslation } from "react-i18next";
import {
  StyledAddGroupsPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "AddGroupsPanel",
  localesPath: "panels/AddGroupsPanel",
});

const { changeLanguage } = utils;

class AddGroupsPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = { showActionPanel: false };
    this.scrollRef = React.createRef();
  }

  onPlusClick = () =>
    this.setState({ showActionPanel: !this.state.showActionPanel });

  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  onSelectGroups = (groups) => {
    const {
      accessRight,
      shareDataItems,
      setShareDataItems,
      onClose,
    } = this.props;
    const items = shareDataItems;

    for (let item of groups) {
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

  onPLusClick = () => {
    console.log("onPlusClick");
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
    const { visible, embeddedComponent, t } = this.props;

    const zIndex = 310;

    //console.log("AddGroupsPanel render");
    return (
      <StyledAddGroupsPanel visible={visible}>
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
                color="A3A9AE"
              />
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                {t("AddGroupsForSharingButton")}
              </Heading>
              {/*<IconButton
                size="16"
                iconName="PlusIcon"
                className="header_aside-panel-plus-icon"
                onClick={this.onPLusClick}
              />*/}
            </StyledHeaderContent>

            <StyledBody ref={this.scrollRef}>
              <GroupSelector
                isOpen={visible}
                isMultiSelect
                displayType="aside"
                withoutAside
                onSelect={this.onSelectGroups}
                embeddedComponent={embeddedComponent}
                showCounter={true}
              />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledAddGroupsPanel>
    );
  }
}

AddGroupsPanelComponent.propTypes = {
  visible: PropTypes.bool,
  onSharingPanelClose: PropTypes.func,
  onClose: PropTypes.func,
};

const AddGroupsPanelContainerTranslated = withTranslation()(
  AddGroupsPanelComponent
);

const AddGroupsPanel = (props) => (
  <AddGroupsPanelContainerTranslated i18n={i18n} {...props} />
);

export default AddGroupsPanel;
