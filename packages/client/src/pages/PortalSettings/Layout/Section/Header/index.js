import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Headline from "@docspace/common/components/Headline";
import IconButton from "@docspace/components/icon-button";
import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";
import DropDownItem from "@docspace/components/drop-down-item";
import LoaderSectionHeader from "../loaderSectionHeader";
import { tablet } from "@docspace/components/utils/device";
import withLoading from "SRC_DIR/HOCs/withLoading";

import {
  getKeyByLink,
  settingsTree,
  getTKeyByKey,
  checkPropertyByLink,
} from "../../../utils";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import { isMobile, isTablet, isMobileOnly } from "react-device-detect";

const HeaderContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  max-width: calc(100vw - 32px);

  .action-wrapper {
    flex-grow: 1;

    .action-button {
      margin-left: auto;
    }
  }

  .arrow-button {
    margin-right: 12px;

    @media ${tablet} {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  ${isTablet &&
  css`
    h1 {
      line-height: 61px;
      font-size: 21px;
    }
  `};

  @media (min-width: 600px) and (max-width: 1024px) {
    h1 {
      line-height: 61px;
      font-size: 21px;
    }
  }

  @media (min-width: 1024px) {
    h1 {
      font-size: 18px;
      line-height: 59px !important;
      padding-bottom: 6px;
    }
  }
`;

const StyledContainer = styled.div`
  .group-button-menu-container {
    ${(props) =>
      props.viewAs === "table"
        ? css`
            margin: 0px -20px;
            width: calc(100% + 40px);
          `
        : css`
            margin: 0px -20px;
            width: calc(100% + 40px);
          `}

    @media ${tablet} {
      margin: 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobile &&
    css`
      margin: 0 -16px;
      width: calc(100% + 32px);
    `}

    ${isMobileOnly &&
    css`
      margin: 0 -16px;
      width: calc(100% + 32px);
    `}
  }
`;

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);

    const { match, location } = props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;

    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split("/");

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const currKey = key.length > 3 ? key : key[0];
    const header = getTKeyByKey(currKey, settingsTree);
    const isCategory = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isCategory"
    );
    const isHeader = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isHeader"
    );
    this.state = {
      header,
      isCategoryOrHeader: isCategory || isHeader,
      showSelector: false,
      isHeaderVisible: false,
    };
  }

  componentDidUpdate() {
    const { isLoaded, tReady, setIsLoadedSectionHeader } = this.props;

    const isLoadedSetting = isLoaded && tReady;

    if (isLoadedSetting) setIsLoadedSectionHeader(isLoadedSetting);

    const arrayOfParams = this.getArrayOfParams();

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const currKey = key.length > 3 ? key : key[0];
    const header = getTKeyByKey(currKey, settingsTree);
    const isCategory = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isCategory"
    );
    const isHeader = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isHeader"
    );
    const isCategoryOrHeader = isCategory || isHeader;

    header !== this.state.header && this.setState({ header });
    isCategoryOrHeader !== this.state.isCategoryOrHeader &&
      this.setState({ isCategoryOrHeader });
  }

  onBackToParent = () => {
    let newArrayOfParams = this.getArrayOfParams();
    newArrayOfParams.splice(-1, 1);
    const newPath = "/portal-settings/" + newArrayOfParams.join("/");
    this.props.history.push(combineUrl(AppServerConfig.proxyURL, newPath));
  };

  getArrayOfParams = () => {
    const { match, location } = this.props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;
    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split("/").filter((param) => {
      return param !== "filter";
    });
    return arrayOfParams;
  };

  addUsers = (items) => {
    const { addUsers } = this.props;
    if (!addUsers) return;
    addUsers(items);
  };

  onToggleSelector = (isOpen = !this.props.selectorIsOpen) => {
    const { toggleSelector } = this.props;
    toggleSelector(isOpen);
  };

  onClose = () => {
    const { deselectUser } = this.props;
    deselectUser();
  };

  onCheck = (checked) => {
    const { setSelected } = this.props;
    setSelected(checked ? "all" : "close");
  };

  onSelectAll = () => {
    const { setSelected } = this.props;
    setSelected("all");
  };

  removeAdmins = () => {
    const { removeAdmins } = this.props;
    if (!removeAdmins) return;
    removeAdmins();
  };

  render() {
    const {
      t,
      tReady,
      addUsers,
      isHeaderIndeterminate,
      isHeaderChecked,
      isHeaderVisible,
      selection,
    } = this.props;
    const { header, isCategoryOrHeader } = this.state;
    const arrayOfParams = this.getArrayOfParams();

    const menuItems = (
      <>
        <DropDownItem
          key="all"
          label={t("Common:SelectAll")}
          data-index={1}
          onClick={this.onSelectAll}
        />
      </>
    );

    const headerMenu = [
      {
        label: t("Common:Delete"),
        disabled: !selection || !selection.length > 0,
        onClick: this.removeAdmins,
        iconUrl: "/static/images/delete.react.svg",
      },
    ];

    const showLoader = !tReady;

    return (
      <StyledContainer isHeaderVisible={isHeaderVisible}>
        {isHeaderVisible ? (
          <div className="group-button-menu-container">
            <TableGroupMenu
              checkboxOptions={menuItems}
              onChange={this.onCheck}
              isChecked={isHeaderChecked}
              isIndeterminate={isHeaderIndeterminate}
              headerMenu={headerMenu}
            />
          </div>
        ) : showLoader ? (
          <LoaderSectionHeader />
        ) : (
          <HeaderContainer>
            {!isCategoryOrHeader && arrayOfParams[0] && (
              <IconButton
                iconName="/static/images/arrow.path.react.svg"
                size="17"
                isFill={true}
                onClick={this.onBackToParent}
                className="arrow-button"
              />
            )}
            <Headline type="content" truncate={true}>
              {t(header)}
            </Headline>
            {addUsers && (
              <div className="action-wrapper">
                <IconButton
                  iconName="/static/images/actions.header.touch.react.svg"
                  size="17"
                  isFill={true}
                  onClick={this.onToggleSelector}
                  className="action-button"
                />
              </div>
            )}
          </HeaderContainer>
        )}
      </StyledContainer>
    );
  }
}

export default inject(({ auth, setup, common }) => {
  const { customNames } = auth.settingsStore;
  const { addUsers, removeAdmins } = setup.headerAction;
  const { toggleSelector } = setup;
  const {
    selected,
    setSelected,
    isHeaderIndeterminate,
    isHeaderChecked,
    isHeaderVisible,
    deselectUser,
    selectAll,
    selection,
  } = setup.selectionStore;
  const { admins, selectorIsOpen } = setup.security.accessRight;
  const { isLoaded, setIsLoadedSectionHeader } = common;
  return {
    addUsers,
    removeAdmins,
    groupsCaption: customNames.groupsCaption,
    selected,
    setSelected,
    admins,
    isHeaderIndeterminate,
    isHeaderChecked,
    isHeaderVisible,
    deselectUser,
    selectAll,
    toggleSelector,
    selectorIsOpen,
    selection,
    isLoaded,
    setIsLoadedSectionHeader,
  };
})(
  withLoading(
    withRouter(
      withTranslation(["Settings", "Common"])(observer(SectionHeaderContent))
    )
  )
);
