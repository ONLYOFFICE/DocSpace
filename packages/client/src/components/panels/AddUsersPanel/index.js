import React, { useState, useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import IconButton from "@docspace/components/icon-button";
import { ShareAccessRights } from "@docspace/common/constants";
import Selector from "@docspace/components/selector";
import { withTranslation } from "react-i18next";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";
import toastr from "@docspace/components/toast/toastr";
import Filter from "@docspace/common/api/people/filter";

import { getMembersList } from "@docspace/common/api/people";
import { getUserRole } from "@docspace/common/utils";
import DefaultUserPhoto from "PUBLIC_DIR/images/default_user_photo_size_82-82.png";
import CatalogAccountsReactSvgUrl from "PUBLIC_DIR/images/catalog.accounts.react.svg?url";
import EmptyScreenPersonsSvgUrl from "PUBLIC_DIR/images/empty_screen_persons.svg?url";
import EmptyScreenPersonsSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_persons_dark.svg?url";

let timer = null;

const AddUsersPanel = ({
  isEncrypted,
  defaultAccess,
  onClose,
  onParentPanelClose,
  tempDataItems,
  setDataItems,
  t,
  visible,
  groupsCaption,
  accessOptions,
  isMultiSelect,
  theme,
  withoutBackground,
  withBlur,
  roomId,
}) => {
  const accessRight = defaultAccess
    ? defaultAccess
    : isEncrypted
    ? ShareAccessRights.FullAccess
    : ShareAccessRights.ReadOnly;

  const onBackClick = () => onClose();
  const getFilterWithOutDisabledUser = useCallback(
    () => Filter.getFilterWithOutDisabledUser(),
    []
  );

  const onKeyPress = (e) => {
    if (e.key === "Esc" || e.key === "Escape") onClose();
  };

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);

    return () => window.removeEventListener("keyup", onKeyPress);
  });

  const onClosePanels = () => {
    onClose();
    onParentPanelClose();
  };

  const onUsersSelect = (users, access) => {
    const items = [];

    for (let item of users) {
      const currentAccess =
        item.isOwner || item.isAdmin
          ? ShareAccessRights.RoomManager
          : access.access;

      const newItem = {
        access: currentAccess,
        email: item.email,
        id: item.id,
        displayName: item.label,
        avatar: item.avatar,
        isOwner: item.isOwner,
        isAdmin: item.isAdmin,
      };
      items.push(newItem);
    }

    if (users.length > items.length)
      toastr.warning("Some users are already in room");

    setDataItems(items);
    onClose();
  };

  const selectedAccess = accessOptions.filter(
    (access) => access.access === accessRight
  )[0];

  const [itemsList, setItemsList] = useState(null);
  const [searchValue, setSearchValue] = useState("");
  const [hasNextPage, setHasNextPage] = useState(true);
  const [isNextPageLoading, setIsNextPageLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const cleanTimer = () => {
    timer && clearTimeout(timer);
    timer = null;
  };

  useEffect(() => {
    loadNextPage(0);
  }, []);

  useEffect(() => {
    if (isLoading) {
      cleanTimer();
      timer = setTimeout(() => {
        setIsLoading(true);
      }, 100);
    } else {
      cleanTimer();
      setIsLoading(false);
    }

    return () => {
      cleanTimer();
    };
  }, [isLoading]);

  const onSearch = (value) => {
    setSearchValue(value);
    loadNextPage(0, value);
  };

  const onClearSearch = () => {
    setSearchValue("");
    loadNextPage(0, "");
  };

  const toListItem = (item) => {
    const {
      id,
      email,
      avatar,
      icon,
      displayName,
      hasAvatar,
      isOwner,
      isAdmin,
      isVisitor,
      isCollaborator,
    } = item;

    const role = getUserRole(item);

    const userAvatar = hasAvatar ? avatar : DefaultUserPhoto;

    return {
      id,
      email,
      avatar: userAvatar,
      icon,
      label: displayName || email,
      role,
      isOwner,
      isAdmin,
      isVisitor,
      isCollaborator,
    };
  };

  const loadNextPage = (startIndex, search = searchValue) => {
    const pageCount = 100;

    setIsNextPageLoading(true);

    if (startIndex === 0) {
      setIsLoading(true);
    }

    const currentFilter = getFilterWithOutDisabledUser();

    currentFilter.page = startIndex / pageCount;
    currentFilter.pageCount = pageCount;
    currentFilter.excludeShared = true;

    if (!!search.length) {
      currentFilter.search = search;
    }

    getMembersList(roomId, currentFilter)
      .then((response) => {
        let newItems = startIndex ? itemsList : [];
        let totalDifferent = startIndex ? response.total - total : 0;

        const items = response.items.map((item) => toListItem(item));

        newItems = [...newItems, ...items];

        const newTotal = response.total - totalDifferent;

        setHasNextPage(newItems.length < newTotal);
        setItemsList(newItems);
        setTotal(newTotal);

        setIsNextPageLoading(false);
        setIsLoading(false);
      })
      .catch((error) => console.log(error));
  };

  const emptyScreenImage = theme.isBase
    ? EmptyScreenPersonsSvgUrl
    : EmptyScreenPersonsSvgDarkUrl;

  return (
    <>
      <Backdrop
        onClick={onClosePanels}
        visible={visible}
        zIndex={310}
        isAside={true}
        withoutBackground={withoutBackground}
        withoutBlur={!withBlur}
      />
      <Aside
        className="header_aside-panel"
        visible={visible}
        onClose={onClosePanels}
        withoutBodyScroll
      >
        <Selector
          headerLabel={t("PeopleSelector:ListAccounts")}
          onBackClick={onBackClick}
          searchPlaceholder={t("Common:Search")}
          searchValue={searchValue}
          onSearch={onSearch}
          onClearSearch={onClearSearch}
          items={itemsList}
          isMultiSelect={isMultiSelect}
          acceptButtonLabel={t("Common:AddButton")}
          onAccept={onUsersSelect}
          withSelectAll={isMultiSelect}
          selectAllLabel={t("PeopleSelector:AllAccounts")}
          selectAllIcon={CatalogAccountsReactSvgUrl}
          withAccessRights={isMultiSelect}
          accessRights={accessOptions}
          selectedAccessRight={selectedAccess}
          withCancelButton={!isMultiSelect}
          cancelButtonLabel={t("Common:CancelButton")}
          onCancel={onClosePanels}
          emptyScreenImage={emptyScreenImage}
          emptyScreenHeader={t("PeopleSelector:EmptyHeader")}
          emptyScreenDescription={t("PeopleSelector:EmptyDescription")}
          searchEmptyScreenImage={emptyScreenImage}
          searchEmptyScreenHeader={t("People:NotFoundUsers")}
          searchEmptyScreenDescription={t("SearchEmptyDescription")}
          hasNextPage={hasNextPage}
          isNextPageLoading={isNextPageLoading}
          loadNextPage={loadNextPage}
          totalItems={total}
          isLoading={isLoading}
          searchLoader={<Loaders.SelectorSearchLoader />}
          rowLoader={
            <Loaders.SelectorRowLoader
              isMultiSelect={false}
              isContainer={isLoading}
              isUser={true}
            />
          }
        />
      </Aside>
    </>
  );
};

AddUsersPanel.propTypes = {
  visible: PropTypes.bool,
  onParentPanelClose: PropTypes.func,
  onClose: PropTypes.func,
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
  };
})(
  observer(
    withTranslation(["SharingPanel", "PeopleTranslations", "Common"])(
      withLoader(AddUsersPanel)(<Loaders.DialogAsideLoader isPanel />)
    )
  )
);
