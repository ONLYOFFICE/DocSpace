import React, { useCallback } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobile, isMobileOnly } from "react-device-detect";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import Headline from "@docspace/common/components/Headline";
import Loaders from "@docspace/common/components/Loaders";
import DropDownItem from "@docspace/components/drop-down-item";
import ContextMenuButton from "@docspace/components/context-menu-button";
import {
  tablet,
  mobile,
  isTablet,
  isMobile as isMobileUtils,
  isDesktop,
} from "@docspace/components/utils/device";
import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";
import { Base } from "@docspace/components/themes";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";
import withPeopleLoader from "SRC_DIR/HOCs/withPeopleLoader";

const StyledContainer = styled.div`
  width: 100%;
  min-height: 33px;
  height: 69px;

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
        font-size: 21px;
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

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    margin-bottom: 1px;
  }
`;

const SectionHeaderContent = (props) => {
  const {
    t,
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    setSelected,
    getHeaderMenu,
    cbMenuItems,
    getCheckboxItemLabel,

    setInfoPanelIsVisible,
    isInfoPanelVisible,
    isOwner,
    isAdmin,
  } = props;

  //console.log("SectionHeaderContent render");

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

  const onSetInfoPanelVisible = () => {
    setInfoPanelIsVisible(true);
  };

  const getContextOptions = () => {
    return [
      isOwner && {
        id: "main-button_administrator",
        className: "main-button_drop-down",
        icon: "/static/images/person.admin.react.svg",
        label: t("Common:DocSpaceAdmin"),
        onClick: onInvite,
        "data-action": "administrator",
        key: "administrator",
      },
      {
        id: "main-button_manager",
        className: "main-button_drop-down",
        icon: "/static/images/person.manager.react.svg",
        label: t("Common:RoomAdmin"),
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
    <StyledContainer>
      {isHeaderVisible ? (
        <div className="group-button-menu-container">
          <TableGroupMenu
            checkboxOptions={getMenuItems()}
            onChange={onChange}
            isChecked={isHeaderChecked}
            isIndeterminate={isHeaderIndeterminate}
            headerMenu={headerMenu}
            isInfoPanelVisible={isInfoPanelVisible}
            toggleInfoPanel={onSetInfoPanelVisible}
            withoutInfoPanelToggler={false}
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
              {t("Common:Accounts")}
            </Headline>
            <ContextMenuButton
              className="action-button"
              directionX="left"
              title={t("Common:Actions")}
              iconName="images/plus.svg"
              size={15}
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
                      onClick={onSetInfoPanelVisible}
                    />
                  </div>
                )}
              </StyledInfoPanelToggleWrapper>
            )}
          </>
        </div>
      )}
    </StyledContainer>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const {
      setIsVisible: setInfoPanelIsVisible,
      isVisible: isInfoPanelVisible,
    } = auth.infoPanelStore;

    const { isOwner, isAdmin } = auth.userStore.user;

    const { selectionStore, headerMenuStore, getHeaderMenu } = peopleStore;

    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      cbMenuItems,
      getCheckboxItemLabel,
    } = headerMenuStore;

    const { setSelected } = selectionStore;

    return {
      setSelected,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      getHeaderMenu,
      cbMenuItems,
      getCheckboxItemLabel,
      setInfoPanelIsVisible,
      isInfoPanelVisible,
      isOwner,
      isAdmin,
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
