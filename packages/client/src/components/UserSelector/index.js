import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";

import { getUserList } from "@docspace/common/api/people";
import Filter from "@docspace/common/api/people/filter";

import Selector from "@docspace/components/selector";

const defaultFilterData = {
  total: 0,
  page: 0,
  pageCount: 100,
  hasNextPage: true,
  isNextPageLoading: false,
  isFirstLoad: true,
};

const convertToItem = (user) => {
  const role = user.isOwner ? "owner" : user.isAdmin ? "admin" : "user";

  return {
    id: user.id,
    role,
    label: user.displayName,
    avatar: user.avatar,
  };
};

const UserSelector = ({
  id,
  className,
  style,
  t,
  isOpen,
  isMultiSelect,
  withSelectAll,
  withAccessRights,
  withCancelButton,
  onSearch,
  onClearSearch,
  onSelect,
  onSelectAll,
  onAccept,
  onCancel,
  onAccessRightsChange,
  selectedItems,
  accessRights,
  selectedAccessRight,
  excludeItems,
  userFilter,
}) => {
  const [filterData, setFilterData] = React.useState(defaultFilterData);

  const [searchValue, setSearchValue] = React.useState("");

  const [items, setItems] = React.useState([]);

  const fetchUserList = React.useCallback(
    (loadNextPage = false) => {
      const filter = userFilter ? userFilter : Filter.getDefault();

      filter.page = filterData.page;
      filter.pageCount = filterData.pageCount;

      if (searchValue) filter.search = searchValue;

      getUserList(filter, false).then((res) => {
        const { total, count } = res;

        const users = res.items.filter(
          (item) => !excludeItems.includes(item.id)
        );

        const items = users.map((user) => convertToItem(user));

        setItems((val) => (loadNextPage ? [...val, ...items] : [...items]));

        setFilterData((val) => ({
          ...val,
          total: res.total,
          hasNextPage: (val.page + 1) * res.total > val.pageCount,
        }));
      });
    },
    [
      userFilter,
      excludeItems,
      searchValue,
      filterData.page,
      filterData.pageCount,
    ]
  );

  const loadNextPage = React.useCallback((index) => {
    console.log(index);
  }, []);

  const onSearchAction = React.useCallback(
    (value) => {
      onSearch && onSearch(value);
      setSearchValue(value);
    },
    [onSearch]
  );

  const onClearSearchAction = React.useCallback(() => {
    onClearSearch && onClearSearch();
    setSearchValue("");
  }, [onClearSearch]);

  React.useEffect(() => {
    fetchUserList();
  }, [searchValue]);

  return (
    <div style={{ height: "900px" }}>
      <Selector
        id={id}
        className={className}
        style={style}
        headerLabel={t("AccountsList")}
        onBackClick={onCancel}
        searchPlaceholder={t("Common:Search")}
        searchValue={searchValue}
        onSearch={onSearchAction}
        onClearSearch={onClearSearchAction}
        items={items}
        totalItems={filterData.total}
        loadNextPage={loadNextPage}
        hasNextPage={filterData.hasNextPage}
        isNextPageLoading={filterData.isNextPageLoading}
      />
    </div>
  );
};

UserSelector.propTypes = {
  id: PropTypes.string,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,

  t: PropTypes.func,

  isOpen: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  withSelectAll: PropTypes.bool,
  withAccessRights: PropTypes.bool,
  withCancelButton: PropTypes.bool,

  onSearch: PropTypes.func,
  onClearSearch: PropTypes.func,
  onSelect: PropTypes.func,
  onSelectAll: PropTypes.func,
  onAccept: PropTypes.func,
  onCancel: PropTypes.func,
  onAccessRightsChange: PropTypes.func,

  selectedItems: PropTypes.array,

  accessRights: PropTypes.array,
  selectedAccessRight: PropTypes.object,

  excludeItems: PropTypes.array,

  userFilter: PropTypes.instanceOf(Filter),
};

UserSelector.defaultProps = {
  excludeItems: [],
};

const ExtendedUserSelector = inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withTranslation([
      "UserSelector",
      "PeopleSelector",
      "PeopleTranslations",
      "Common",
    ])(UserSelector)
  )
);

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <ExtendedUserSelector {...props} />
  </I18nextProvider>
);
