import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";

import Selector from "@docspace/components/selector";
import Filter from "@docspace/common/api/people/filter";

import UserTooltip from "./sub-components/UserTooltip";

import { getUserList } from "@docspace/common/api/people";
import Loaders from "@docspace/common/components/Loaders";

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
}) => {
  const [itemsList, setItemsList] = useState(items);
  const [searchValue, setSearchValue] = useState("");
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(0);
  const [hasNextPage, setHasNextPage] = useState(true);
  const [isNextPageLoading, setIsNextPageLoading] = useState(false);
  const [isFirstLoad, setIsFirstLoad] = useState(true);

  useEffect(() => {
    setIsFirstLoad(true);
    loadNextPage(0);
  }, []);

  const toListItem = (item) => {
    return {
      id: item.id,
      avatar: item.avatar,
      icon: item.icon,
      label: item.displayName,
    };
  };

  const loadNextPage = (startIndex, search) => {
    const pageCount = 100;

    setIsNextPageLoading(true);

    const filter = filter || Filter.getDefault();
    filter.page = startIndex / pageCount;
    filter.pageCount = pageCount;

    if (search) {
      filter.search = search;
    }

    getUserList(filter)
      .then((response) => {
        let newItems = startIndex ? itemsList : [];

        const items = response.items.map((item) => toListItem(item));

        newItems = [...newItems, ...items];

        setHasNextPage(newItems.length < response.total);
        setItemsList(newItems);
        setTotal(response.total);

        setIsNextPageLoading(false);
        isFirstLoad && setIsFirstLoad(false);
      })
      .catch((error) => console.log(error));
  };

  const onSearch = (value) => {
    setSearchValue(value);
    loadNextPage(0, value);
  };

  const setDefaultTranslations = () => {
    headerLabel = headerLabel || t("ListAccounts");
    searchPlaceholder = searchPlaceholder || t("Common:Search");
    acceptButtonLabel = acceptButtonLabel || t("PeopleTranslations:AddMembers");
    cancelButtonLabel = cancelButtonLabel || t("Common:CancelButton");
    selectAllLabel = selectAllLabel || t("AllAccounts");
    selectAllIcon =
      selectAllIcon || "/static/images/catalog.accounts.react.svg";
    emptyScreenImage =
      emptyScreenImage || "/static/images/empty_screen_persons.png";
    emptyScreenHeader = emptyScreenHeader || t("EmptyHeader");
    emptyScreenDescription = emptyScreenDescription || t("EmptyDescription");
    searchEmptyScreenImage =
      searchEmptyScreenImage || "/static/images/empty_screen_persons.png";
    searchEmptyScreenHeader = searchEmptyScreenHeader || t("SearchEmptyHeader");
    searchEmptyScreenDescription =
      searchEmptyScreenDescription || t("SearchEmptyDescription");
  };

  setDefaultTranslations();

  return (
    <Selector
      id={id}
      className={className}
      style={style}
      headerLabel={headerLabel}
      onBackClick={onBackClick}
      searchPlaceholder={searchPlaceholder}
      searchValue={searchValue}
      onSearch={onSearch}
      items={itemsList}
      isMultiSelect={isMultiSelect}
      selectedItems={selectedItems}
      acceptButtonLabel={acceptButtonLabel}
      onAccept={onAccept}
      withSelectAll={withSelectAll}
      selectAllLabel={selectAllLabel}
      selectAllIcon={selectAllIcon}
      withAccessRights={withAccessRights}
      accessRights={accessRights}
      selectedAccessRight={selectedAccessRight}
      withCancelButton={withCancelButton}
      cancelButtonLabel={cancelButtonLabel}
      onCancel={onCancel}
      emptyScreenImage={emptyScreenImage}
      emptyScreenHeader={emptyScreenHeader}
      emptyScreenDescription={emptyScreenDescription}
      searchEmptyScreenImage={searchEmptyScreenImage}
      searchEmptyScreenHeader={searchEmptyScreenHeader}
      searchEmptyScreenDescription={searchEmptyScreenDescription}
      hasNextPage={hasNextPage}
      isNextPageLoading={isNextPageLoading}
      loadNextPage={loadNextPage}
      totalItems={total}
      isLoading={isFirstLoad}
      searchLoader={<Loaders.SelectorSearchLoader />}
      rowLoader={
        <Loaders.SelectorRowLoader
          isMultiSelect={false}
          isContainer={isFirstLoad}
        />
      }
    />
  );
};

PeopleSelector.propTypes = {};

PeopleSelector.defaultProps = {};

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
