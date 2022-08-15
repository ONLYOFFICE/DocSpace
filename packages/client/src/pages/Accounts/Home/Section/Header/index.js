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
    group,
    isAdmin,
    t,
    history,
    customNames,
    homepage,
    // deleteGroup,
    isLoaded,
    isTabletView,
    setSelected,
    // resetFilter,
    getHeaderMenu,
    setInvitationDialogVisible,
    showText,
  } = props;

  const {
    userCaption,
    guestCaption,
    // groupCaption,
    groupsCaption,
  } = customNames;

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

  // const onEditGroup = useCallback(
  //   () =>
  //     history.push(
  //       combineUrl(
  //         AppServerConfig.proxyURL,
  //         homepage,
  //         `/group/edit/${group.id}`
  //       )
  //     ),
  //   [history, homepage, group]
  // );

  // const onDeleteGroup = useCallback(() => {
  //   deleteGroup(group.id)
  //     .then(() => toastr.success(t("SuccessfullyRemovedGroup")))
  //     .then(() => resetFilter());
  // }, [deleteGroup, group, t]);

  // const getContextOptionsGroup = useCallback(() => {
  //   return [
  //     {
  //       key: "edit-group",
  //       label: t("Common:EditButton"),
  //       onClick: onEditGroup,
  //     },
  //     {
  //       key: "delete-group",
  //       label: t("Common:Delete"),
  //       onClick: onDeleteGroup,
  //     },
  //   ];
  // }, [t, onEditGroup, onDeleteGroup]);

  const goToEmployeeCreate = useCallback(() => {
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/accounts/create/user")
    );
  }, [history, homepage]);

  // const goToGuestCreate = useCallback(() => {
  //   history.push(
  //     combineUrl(AppServerConfig.proxyURL, homepage, "/accounts/create/guest")
  //   );
  // }, [history, homepage]);

  // const goToGroupCreate = useCallback(() => {
  //   history.push(
  //     combineUrl(AppServerConfig.proxyURL, homepage, "/accounts/group/create")
  //   );
  // }, [history, homepage]);

  const onInvitationDialogClick = () => setInvitationDialogVisible(true);

  const getContextOptionsPlus = useCallback(() => {
    return [
      {
        key: "new-employee",
        label: userCaption,
        onClick: goToEmployeeCreate,
      },
      // {
      //   key: "new-guest",
      //   label: guestCaption,
      //   onClick: goToGuestCreate,
      // },
      // {
      //   key: "new-group",
      //   label: groupCaption,
      //   onClick: goToGroupCreate,
      // },
      { key: "separator", isSeparator: true },
      {
        key: "make-invitation-link",
        label: t("MakeInvitationLink"),
        onClick: onInvitationDialogClick,
      } /* ,
      {
        key: "send-invitation",
        label: t("PeopleTranslations:SendInviteAgain"),
        onClick: onSentInviteAgain
      } */,
    ];
  }, [
    userCaption,
    guestCaption,
    // groupCaption,
    t,
    goToEmployeeCreate,
    // goToGuestCreate,
    // goToGroupCreate,
    /* , onSentInviteAgain */
    ,
  ]);

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
              {group ? (
                <>
                  <Headline
                    className="headline-header"
                    type="content"
                    truncate={true}
                  >
                    {t("Accounts")}
                  </Headline>
                  {/* {isAdmin && (
                    <ContextMenuButton
                      className="action-button"
                      directionX="right"
                      title={t("Common:Actions")}
                      iconName="/static/images/vertical-dots.react.svg"
                      size={17}
                      getData={getContextOptionsGroup}
                      isDisabled={false}
                    />
                  )} */}
                </>
              ) : (
                <>
                  <Headline
                    className="headline-header"
                    truncate={true}
                    type="content"
                  >
                    {groupsCaption}
                  </Headline>
                  {isAdmin && (
                    <>
                      <ContextMenuButton
                        className="action-button"
                        directionX="right"
                        title={t("Common:Actions")}
                        iconName="/static/images/actions.header.touch.react.svg"
                        size={17}
                        getData={getContextOptionsPlus}
                        isDisabled={false}
                      />
                    </>
                  )}
                </>
              )}
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
