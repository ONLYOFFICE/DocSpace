import React, { useState, useEffect, useRef } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";

import Selector from "@docspace/components/selector";
import Filter from "@docspace/common/api/people/filter";

import { getUserList } from "@docspace/common/api/people";
import Loaders from "@docspace/common/components/Loaders";
import { getUserRole } from "@docspace/common/utils";

let timer = null;

const PeopleSelector = ({
  acceptButtonLabel,
  accessRights,
  cancelButtonLabel,
  className,
  emptyScreenDescription,
  emptyScreenHeader,
  emptyScreenImage,
  headerLabel,
  id,
  isMultiSelect,
  items,
  onAccept,
  onAccessRightsChange,
  onBackClick,
  onCancel,
  onSelect,
  onSelectAll,
  searchEmptyScreenDescription,
  searchEmptyScreenHeader,
  searchEmptyScreenImage,
  searchPlaceholder,
  selectAllIcon,
  selectAllLabel,
  selectedAccessRight,
  selectedItems,
  style,
  t,
  withAccessRights,
  withCancelButton,
  withSelectAll,
  filter,
  excludeItems,
}) => {
  const [itemsList, setItemsList] = useState(items);
  const [searchValue, setSearchValue] = useState("");
  const [total, setTotal] = useState(0);
  const [hasNextPage, setHasNextPage] = useState(true);
  const [isNextPageLoading, setIsNextPageLoading] = useState(false);
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

  const toListItem = (item) => {
    const { id, email, avatar, icon, displayName } = item;

    const role = getUserRole(item);

    return {
      id,
      email,
      avatar,
      icon,
      label: displayName || email,
      role,
    };
  };

  const loadNextPage = (startIndex, search = searchValue) => {
    const pageCount = 100;

    setIsNextPageLoading(true);
    setIsLoading(true);

    const currentFilter =
      typeof filter === "function" ? filter() : filter ?? Filter.getDefault();

    currentFilter.page = startIndex / pageCount;
    currentFilter.pageCount = pageCount;

    if (!!search.length) {
      currentFilter.search = search;
    }

    getUserList(currentFilter)
      .then((response) => {
        let newItems = startIndex ? itemsList : [];
        let totalDifferent = startIndex ? response.total - total : 0;

        const items = response.items
          .filter((item) => {
            if (excludeItems.includes(item.id)) {
              totalDifferent++;
              return false;
            } else {
              return true;
            }
          })
          .map((item) => toListItem(item));

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

  const onSearch = (value) => {
    setSearchValue(value);
    loadNextPage(0, value);
  };

  const onClearSearch = () => {
    setSearchValue("");
    loadNextPage(0, "");
  };

  return (
    <Selector
      id={id}
      className={className}
      style={style}
      headerLabel={headerLabel || t("ListAccounts")}
      onBackClick={onBackClick}
      searchPlaceholder={searchPlaceholder || t("Common:Search")}
      searchValue={searchValue}
      onSearch={onSearch}
      onClearSearch={onClearSearch}
      items={itemsList}
      isMultiSelect={isMultiSelect}
      selectedItems={selectedItems}
      acceptButtonLabel={
        acceptButtonLabel || t("PeopleTranslations:AddMembers")
      }
      onAccept={onAccept}
      withSelectAll={withSelectAll}
      selectAllLabel={selectAllLabel || t("AllAccounts")}
      selectAllIcon={selectAllIcon}
      withAccessRights={withAccessRights}
      accessRights={accessRights}
      selectedAccessRight={selectedAccessRight}
      withCancelButton={withCancelButton}
      cancelButtonLabel={cancelButtonLabel || t("Common:CancelButton")}
      onCancel={onCancel}
      emptyScreenImage={emptyScreenImage}
      emptyScreenHeader={emptyScreenHeader || t("EmptyHeader")}
      emptyScreenDescription={emptyScreenDescription || t("EmptyDescription")}
      searchEmptyScreenImage={searchEmptyScreenImage}
      searchEmptyScreenHeader={searchEmptyScreenHeader || t("NotFoundUsers")}
      searchEmptyScreenDescription={
        searchEmptyScreenDescription || t("SearchEmptyDescription")
      }
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
  );
};

PeopleSelector.propTypes = { excludeItems: PropTypes.array };

PeopleSelector.defaultProps = {
  excludeItems: [],
  selectAllIcon: "/static/images/catalog.accounts.react.svg",
  emptyScreenImage: "images/empty_screen_persons.svg",
  searchEmptyScreenImage: "images/empty_screen_persons.svg",
};

const ExtendedPeopleSelector = inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withTranslation(["PeopleSelector", "PeopleTranslations", "Common"])(
      PeopleSelector
    )
  )
);

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <ExtendedPeopleSelector {...props} />
  </I18nextProvider>
);
