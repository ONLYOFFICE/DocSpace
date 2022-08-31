import React, { useCallback, useMemo } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobile, isMobileOnly } from "react-device-detect";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";

import Headline from "@docspace/common/components/Headline";
import Loaders from "@docspace/common/components/Loaders";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";

import DropDownItem from "@docspace/components/drop-down-item";
import ContextMenuButton from "@docspace/components/context-menu-button";
import {
  tablet,
  mobile,
  isTablet,
  isMobile as isMobileUtils,
  isDesktop,
} from "@docspace/components/utils/device";
import { Consumer } from "@docspace/components/utils/context";
import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";
import { Base } from "@docspace/components/themes";
import IconButton from "@docspace/components/icon-button";

import toastr from "client/toastr";

import withPeopleLoader from "SRC_DIR/HOCs/withPeopleLoader";

import config from "PACKAGE_FILE";

const StyledContainer = styled.div`
  width: 100%;
  height: 69px;

  @media ${tablet} {
    height: 61px;
  }

  ${isMobile &&
  css`
    height: 61px;
  `}

  @media ${mobile} {
    height: 53px;
  }

  ${isMobileOnly &&
  css`
    height: 53px;
  `}

  .group-button-menu-container {
    margin: 0 0 0 -20px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    width: calc(100% + 40px);
    height: 68px;

    @media ${tablet} {
      height: 60px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobile &&
    css`
      height: 60px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    `}

    @media ${mobile} {
      height: 52px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobileOnly &&
    css`
      height: 52px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    `}
  }

  .header-container {
    position: relative;

    width: 100%;
    height: 100%;

    display: grid;
    align-items: center;

    grid-template-columns: auto auto 1fr;

    @media ${tablet} {
      grid-template-columns: 1fr auto;
    }

    ${isMobile &&
    css`
      grid-template-columns: 1fr auto;
    `}

    .headline-header {
      line-height: 24px;

      @media ${tablet} {
        line-height: 28px;
      }

      ${isMobile &&
      css`
        line-height: 28px;
      `}

      @media ${mobile} {
        line-height: 24px;
      }

      ${isMobile &&
      css`
        line-height: 24px;
      `}
    }

    .action-button {
      margin-left: 16px;

      @media ${tablet} {
        display: none;
      }

      ${isMobile &&
      css`
        display: none;
      `}
    }
  }
`;

StyledContainer.defaultProps = { theme: Base };

const StyledInfoPanelToggleWrapper = styled.div`
  display: flex;

  margin-left: auto;

  @media ${tablet} {
    display: none;
  }

  ${isMobile &&
  css`
    display: none;
  `}

  align-items: center;
  justify-content: center;

  margin-bottom: 2px;

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
  }
`;

const SectionHeaderContent = (props) => {
  const {
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,

    t,

    isLoaded,
    isTabletView,
    setSelected,

    getHeaderMenu,

    showText,
    cbMenuItems,
    getCheckboxItemLabel,

    setInfoPanelVisible,
    isInfoPanelVisible,
  } = props;

  //console.log("SectionHeaderContent render", props.isTabletView);

  const onChange = (checked) => {
    setSelected(checked ? "all" : "none");
  };

  const onSelect = useCallback(
    (e) => {
      const key = e.currentTarget.dataset.key;
      setSelected(key);
    },
    [setSelected]
  );

  const onSelectorSelect = useCallback(
    (item) => {
      setSelected(item.key);
    },
    [onSelect]
  );

  const getMenuItems = () => {
    const checkboxOptions = (
      <>
        {cbMenuItems.map((key) => {
          const label = getCheckboxItemLabel(t, key);
          return (
            <DropDownItem
              key={key}
              label={label}
              data-key={key}
              onClick={onSelect}
            />
          );
        })}
      </>
    );

    return checkboxOptions;
  };

  const headerMenu = getHeaderMenu(t);

  const onInvite = React.useCallback((e) => {
    const type = e.target.dataset.action;
    toastr.warning("Work in progress " + type);
    console.log("invite ", type);
  }, []);

  const onInviteAgain = React.useCallback(() => {
    toastr.warning("Work in progress (invite again)");
    console.log("invite again");
  }, []);

  const getContextOptions = () => {
    return [
      {
        id: "main-button_administrator",
        className: "main-button_drop-down",
        icon: "/static/images/person.admin.react.svg",
        label: t("Administrator"),
        onClick: onInvite,
        "data-action": "administrator",
        key: "administrator",
      },
      {
        id: "main-button_manager",
        className: "main-button_drop-down",
        icon: "/static/images/person.manager.react.svg",
        label: t("Manager"),
        onClick: onInvite,
        "data-action": "manager",
        key: "manager",
      },
      {
        id: "main-button_user",
        className: "main-button_drop-down",
        icon: "/static/images/person.user.react.svg",
        label: t("Common:User"),
        onClick: onInvite,
        "data-action": "user",
        key: "user",
      },
      {
        key: "separator",
        isSeparator: true,
      },
      {
        id: "main-button_invite-again",
        className: "main-button_drop-down",
        icon: "/static/images/invite.again.react.svg",
        label: t("LblInviteAgain"),
        onClick: onInviteAgain,
        "data-action": "invite-again",
        key: "invite-again",
      },
    ];
  };

  return (
    <Consumer>
      {(context) => (
        <StyledContainer
          isHeaderVisible={isHeaderVisible}
          isLoaded={isLoaded}
          width={context.sectionWidth}
          isTabletView={isTabletView}
          showText={showText}
        >
          {isHeaderVisible ? (
            <div className="group-button-menu-container">
              <TableGroupMenu
                checkboxOptions={getMenuItems()}
                onChange={onChange}
                isChecked={isHeaderChecked}
                isIndeterminate={isHeaderIndeterminate}
                headerMenu={headerMenu}
                withoutInfoPanelToggler={true}
              />
            </div>
          ) : (
            <div className="header-container">
              <>
                <Headline
                  className="headline-header"
                  type="content"
                  truncate={true}
                >
                  {t("Accounts")}
                </Headline>
                <ContextMenuButton
                  className="action-button"
                  directionX="left"
                  title={t("Common:Actions")}
                  iconName="images/plus.svg"
                  size={17}
                  getData={getContextOptions}
                  isDisabled={false}
                />
                {!isInfoPanelVisible && (
                  <StyledInfoPanelToggleWrapper>
                    {!(
                      isTablet() ||
                      isMobile ||
                      isMobileUtils() ||
                      !isDesktop()
                    ) && (
                      <div className="info-panel-toggle-bg">
                        <IconButton
                          className="info-panel-toggle"
                          iconName="images/panel.react.svg"
                          size="16"
                          isFill={true}
                          onClick={setInfoPanelVisible}
                        />
                      </div>
                    )}
                  </StyledInfoPanelToggleWrapper>
                )}
              </>
            </div>
          )}
        </StyledContainer>
      )}
    </Consumer>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { settingsStore, isLoaded, isAdmin } = auth;
    const { customNames, isTabletView, showText } = settingsStore;
    const {
      setVisible: setInfoPanelVisible,
      isVisible: isInfoPanelVisible,
    } = auth.infoPanelStore;

    const {
      resetFilter,
      usersStore,
      selectionStore,
      headerMenuStore,
      groupsStore,
      selectedGroupStore,
      getHeaderMenu,
      dialogStore,
    } = peopleStore;
    const { getUsersList, removeUser, updateUserStatus } = usersStore;
    const {
      setSelected,
      selectByStatus,
      clearSelection,
      selectAll,
    } = selectionStore;

    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      cbMenuItems,
      getCheckboxItemLabel,
    } = headerMenuStore;
    const { deleteGroup } = groupsStore;
    const { group } = selectedGroupStore;
    const { setInvitationDialogVisible } = dialogStore;

    return {
      resetFilter,
      customNames,
      homepage: config.homepage,
      isLoaded,
      isAdmin,
      fetchPeople: getUsersList,
      setSelected,
      selectByStatus,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      clearSelection,
      selectAll,
      deleteGroup,
      removeUser,
      updateUserStatus,
      group,
      isTabletView,
      getHeaderMenu,
      setInvitationDialogVisible,
      showText,
      cbMenuItems,
      getCheckboxItemLabel,
      setInfoPanelVisible,
      isInfoPanelVisible,
    };
  })(
    withTranslation([
      "People",
      "Common",
      "PeopleTranslations",
      "Files",
      "ChangeUserTypeDialog",
    ])(
      withPeopleLoader(observer(SectionHeaderContent))(
        <Loaders.SectionHeader />
      )
    )
  )
);
