import PlusSvgUrl from "PUBLIC_DIR/images/plus.svg?url";
import PanelReactSvgUrl from "PUBLIC_DIR/images/panel.react.svg?url";
import PersonAdminReactSvgUrl from "PUBLIC_DIR/images/person.admin.react.svg?url";
import PersonManagerReactSvgUrl from "PUBLIC_DIR/images/person.manager.react.svg?url";
import PersonUserReactSvgUrl from "PUBLIC_DIR/images/person.user.react.svg?url";
import InviteAgainReactSvgUrl from "PUBLIC_DIR/images/invite.again.react.svg?url";
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
import { EmployeeType } from "@docspace/common/constants";
import { resendInvitesAgain } from "@docspace/common/api/people";

const StyledContainer = styled.div`
  width: 100%;
  min-height: 33px;

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
      height: 60px;
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
    getMenuItemId,
    setInfoPanelIsVisible,
    isInfoPanelVisible,
    isOwner,

    setInvitePanelOptions,
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
          const id = getMenuItemId(key);
          return (
            <DropDownItem
              id={id}
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
    const type = e.target.dataset.type;

    setInvitePanelOptions({
      visible: true,
      roomId: -1,
      hideSelector: true,
      defaultAccess: type,
    });
  }, []);

  const onInviteAgain = React.useCallback(() => {
    resendInvitesAgain()
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessSentMultipleInvitatios"))
      )
      .catch((err) => toastr.error(err));
  }, [resendInvitesAgain]);

  const onSetInfoPanelVisible = () => {
    setInfoPanelIsVisible(true);
  };

  const getContextOptions = () => {
    return [
      isOwner && {
        id: "accounts-add_administrator",
        className: "main-button_drop-down",
        icon: PersonAdminReactSvgUrl,
        label: t("Common:DocSpaceAdmin"),
        onClick: onInvite,
        "data-type": EmployeeType.Admin,
        key: "administrator",
      },
      {
        id: "accounts-add_manager",
        className: "main-button_drop-down",
        icon: PersonManagerReactSvgUrl,
        label: t("Common:RoomAdmin"),
        onClick: onInvite,
        "data-type": EmployeeType.User,
        key: "manager",
      },
      {
        id: "accounts-add_user",
        className: "main-button_drop-down",
        icon: PersonUserReactSvgUrl,
        label: t("Common:User"),
        onClick: onInvite,
        "data-type": EmployeeType.Guest,
        key: "user",
      },
      {
        key: "separator",
        isSeparator: true,
      },
      {
        id: "accounts-add_invite-again",
        className: "main-button_drop-down",
        icon: InviteAgainReactSvgUrl,
        label: t("LblInviteAgain"),
        onClick: onInviteAgain,
        "data-action": "invite-again",
        key: "invite-again",
      },
    ];
  };

  const isEmptyHeader = headerMenu.some((x) => !x.disabled);

  return (
    <StyledContainer>
      {isHeaderVisible && isEmptyHeader ? (
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
              id="header_add-button"
              className="action-button"
              directionX="left"
              title={t("Common:Actions")}
              iconName={PlusSvgUrl}
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
                      id="info-panel-toggle--open"
                      className="info-panel-toggle"
                      iconName={PanelReactSvgUrl}
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
  inject(({ auth, peopleStore, dialogsStore }) => {
    const {
      setIsVisible: setInfoPanelIsVisible,
      isVisible: isInfoPanelVisible,
    } = auth.infoPanelStore;

    const { setInvitePanelOptions } = dialogsStore;

    const { isOwner, isAdmin } = auth.userStore.user;

    const { selectionStore, headerMenuStore, getHeaderMenu } = peopleStore;

    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      cbMenuItems,
      getMenuItemId,
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
      getMenuItemId,
      getCheckboxItemLabel,
      setInfoPanelIsVisible,
      isInfoPanelVisible,
      isOwner,
      isAdmin,
      setInvitePanelOptions,
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
