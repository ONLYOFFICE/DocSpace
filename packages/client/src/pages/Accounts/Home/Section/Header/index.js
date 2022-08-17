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
import { tablet, mobile } from "@docspace/components/utils/device";
import { Consumer } from "@docspace/components/utils/context";
import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";

// import toastr from "client/toastr";

import withPeopleLoader from "SRC_DIR/HOCs/withPeopleLoader";

import config from "PACKAGE_FILE";

const StyledContainer = styled.div`
  .group-button-menu-container {
    margin: 0 -20px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    width: calc(100vw - 256px);

    @media ${tablet} {
      width: ${(props) =>
        props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"};

      margin: 0 -16px;
    }

    ${isMobile &&
    css`
      width: ${(props) =>
        props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"};

      margin: 0 -16px;
    `}

    @media ${mobile} {
      width: 100vw;

      margin: 0 -16px;
    }

    ${isMobileOnly &&
    css`
      width: 100vw !important;

      margin: 0 -16px;
    `}
  }

  .header-container {
    position: relative;
    ${(props) =>
      props.isLoaded &&
      css`
        display: grid;
        grid-template-columns: auto auto 1fr;

        @media ${tablet} {
          grid-template-columns: 1fr auto;
        }
      `}

    margin-bottom: 3px;
    align-items: center;
    max-width: calc(100vw - 32px);

    .action-button {
      margin-left: 16px;

      @media ${tablet} {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 16px 8px 16px;
          margin-right: -16px;
        }
      }
    }

    .headline-header {
      @media ${tablet} {
        padding: 4px 0;
      }
    }
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

  let menuItems = useMemo(
    () => (
      <>
        <DropDownItem
          key="active"
          label={t("Common:Active")}
          data-key={"active"}
          onClick={onSelect}
        />
        <DropDownItem
          key="disabled"
          label={t("PeopleTranslations:DisabledEmployeeStatus")}
          data-key={"disabled"}
          onClick={onSelect}
        />
        <DropDownItem
          key="invited"
          label={t("LblInvited")}
          data-key={"invited"}
          onClick={onSelect}
        />
      </>
    ),
    [t, onSelectorSelect]
  );

  const headerMenu = getHeaderMenu(t);

  const onInvite = React.useCallback((e) => {
    const type = e.target.dataset.action;
    console.log("invite ", type);
  }, []);

  const onInviteAgain = React.useCallback(() => {
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
                checkboxOptions={menuItems}
                onChange={onChange}
                isChecked={isHeaderChecked}
                isIndeterminate={isHeaderIndeterminate}
                headerMenu={headerMenu}
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
                  directionX="right"
                  title={t("Common:Actions")}
                  iconName="images/plus.svg"
                  size={17}
                  getData={getContextOptions}
                  isDisabled={false}
                />
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
    };
  })(
    withTranslation(["People", "Common", "PeopleTranslations"])(
      withPeopleLoader(observer(SectionHeaderContent))(
        <Loaders.SectionHeader />
      )
    )
  )
);
