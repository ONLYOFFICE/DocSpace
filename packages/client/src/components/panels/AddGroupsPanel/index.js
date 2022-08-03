import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import IconButton from "@docspace/components/icon-button";
import { ShareAccessRights } from "@docspace/common/constants";
import GroupSelector from "@docspace/client/src/components/GroupSelector";
import { withTranslation } from "react-i18next";
import {
  StyledAddGroupsPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import AccessComboBox from "../SharingPanel/AccessComboBox";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

class AddGroupsPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showActionPanel: false,
      accessRight: ShareAccessRights.ReadOnly,
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

  onSelectGroups = (groups) => {
    const { shareDataItems, setShareDataItems, onClose } = this.props;
    const items = shareDataItems;

    for (let item of groups) {
      if (item.key) {
        item.id = item.key;
      }
      const currentItem = shareDataItems.find((x) => x.sharedTo.id === item.id);
      if (!currentItem) {
        const newItem = {
          access: this.state.accessRight,
          isLocked: false,
          isOwner: false,
          sharedTo: item,
        };
        items.push(newItem);
      }
    }

    setShareDataItems(items);
    onClose();
  };

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.props.onClose();
    }
  };

  onAccessChange = (e) => {
    const accessRight = +e.currentTarget.dataset.access;
    this.setState({ accessRight });
  };

  //onPLusClick = () => console.log("onPlusClick");

  componentDidMount() {
    const scroll = this.scrollRef.current.getElementsByClassName("scroll-body");
    setTimeout(() => scroll[1] && scroll[1].focus(), 2000);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPress);
  }
  // shouldComponentUpdate(nextProps, nextState) {
  //   const { showActionPanel, accessRight } = this.state;
  //   const { visible } = this.props;

  //   if (accessRight !== nextState.accessRight) {
  //     return true;
  //   }

  //   if (showActionPanel !== nextState.showActionPanel) {
  //     return true;
  //   }

  //   if (visible !== nextProps.visible) {
  //     return true;
  //   }

  //   return false;
  // }

  render() {
    const { t, visible, accessOptions, theme, shareDataItems } = this.props;
    const { accessRight } = this.state;

    const selectedOptions = [];
    shareDataItems.forEach((item) => {
      const { sharedTo } = item;
      if (item?.isGroup) {
        selectedOptions.push({ id: sharedTo.id, key: sharedTo.id });
      }
    });

    const zIndex = 310;

    //console.log("AddGroupsPanel render");
    return (
      <StyledAddGroupsPanel visible={visible}>
        <Backdrop
          onClick={this.onClosePanels}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside
          className="header_aside-panel"
          visible={visible}
          onClose={this.onClosePanels}
        >
          <StyledContent>
            <StyledBody ref={this.scrollRef}>
              <GroupSelector
                className="groupSelector"
                isOpen={visible}
                isMultiSelect
                withoutAside
                onSelect={this.onSelectGroups}
                embeddedComponent={
                  <AccessComboBox
                    t={t}
                    access={accessRight}
                    directionX="right"
                    directionY="top"
                    onAccessChange={this.onAccessChange}
                    accessOptions={accessOptions}
                    arrowIconColor={theme.filesPanels.addGroups.arrowColor}
                    isEmbedded={true}
                  />
                }
                headerLabel={t("AddGroupsForSharingButton")}
                onArrowClick={this.onArrowClick}
                showCounter
                selectedOptions={selectedOptions}
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

export default inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withTranslation("SharingPanel")(
      withLoader(AddGroupsPanelComponent)(<Loaders.DialogAsideLoader isPanel />)
    )
  )
);
