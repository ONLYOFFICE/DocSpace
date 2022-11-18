import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { isMobileOnly } from "react-device-detect";
import find from "lodash/find";
import result from "lodash/result";

import FilterInput from "@docspace/common/components/FilterInput";
import Loaders from "@docspace/common/components/Loaders";
import { withLayoutSize } from "@docspace/common/utils";

import withPeopleLoader from "SRC_DIR/HOCs/withPeopleLoader";

const getStatus = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-status";
    }),
    "key"
  );

  return employeeStatus ? +employeeStatus : null;
};

const getRole = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-type";
    }),
    "key"
  );

  return employeeStatus || null;
};

const getGroup = (filterValues) => {
  const groupId = result(
    find(filterValues, (value) => {
      return value.group === "filter-other";
    }),
    "key"
  );

  return groupId || null;
};

const SectionFilterContent = ({
  t,
  isLoaded,
  tReady,
  viewAs,
  setIsLoading,
  fetchPeople,
  filter,
  groups,
  customNames,
}) => {
  const [selectedFilterValues, setSelectedFilterValues] = React.useState(null);

  //TODO: add new options from filter after update backend and fix manager from role
  const onFilter = (data) => {
    const status = getStatus(data);

    const role = getRole(data);
    const group = getGroup(data);

    const newFilter = filter.clone();

    if (status === 3) {
      newFilter.employeeStatus = 2;
      newFilter.activationStatus = null;
    } else {
      newFilter.employeeStatus = null;
      newFilter.activationStatus = status;
    }

    newFilter.page = 0;

    newFilter.role = role;

    newFilter.group = group;

    setIsLoading(true);
    fetchPeople(newFilter, true).finally(() => setIsLoading(false));
  };

  const onSort = (sortId, sortDirection) => {
    const sortBy = sortId;
    const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;

    setIsLoading(true);

    fetchPeople(newFilter, true).finally(() => setIsLoading(false));
  };

  const onSearch = (data = "") => {
    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.search = data;

    setIsLoading(true);

    fetchPeople(newFilter, true).finally(() => setIsLoading(false));
  };

  // TODO: change translation keys
  const getData = async () => {
    const { userCaption } = customNames;

    const statusItems = [
      {
        id: "filter_status-user",
        key: "filter-status",
        group: "filter-status",
        label: t("UserStatus"),
        isHeader: true,
      },
      {
        id: "filter_status-active",
        key: 1,
        group: "filter-status",
        label: t("Common:Active"),
      },
      {
        id: "filter_status-pending",
        key: 2,
        group: "filter-status",
        label: t("PeopleTranslations:PendingTitle"),
      },
      {
        id: "filter_status-disabled",
        key: 3,
        group: "filter-status",
        label: t("PeopleTranslations:DisabledEmployeeStatus"),
      },
    ];

    const typeItems = [
      {
        key: "filter-type",
        group: "filter-type",
        label: t("Common:Type"),
        isHeader: true,
        isLast: true,
      },
      {
        id: "filter_type-doc-space-admin",
        key: "admin",
        group: "filter-type",
        label: t("Common:DocSpaceAdmin"),
      },
      {
        id: "filter_type-room-admin",
        key: "manager",
        group: "filter-type",
        label: t("Common:RoomAdmin"),
      },
      {
        id: "filter_type-user",
        key: "user",
        group: "filter-type",
        label: userCaption,
      },
    ];

    const roleItems = [
      {
        key: "filter-role",
        group: "filter-role",
        label: "Role in room",
        isHeader: true,
      },
      { key: "1", group: "filter-role", label: "Room manager" },
      { key: "2", group: "filter-role", label: "Co-worker" },
      { key: "3", group: "filter-role", label: "Editor" },
      { key: "4", group: "filter-role", label: "Form filler" },
      { key: "5", group: "filter-role", label: "Reviewer" },
      { key: "6", group: "filter-role", label: "Commentator" },
      { key: "7", group: "filter-role", label: "Viewer" },
    ];

    const accountItems = [
      {
        key: "filter-account",
        group: "filter-account",
        label: "Account",
        isHeader: true,
      },
      { key: "paid", group: "filter-account", label: "Paid" },
      { key: "free", group: "filter-account", label: "Free" },
    ];

    const roomItems = [
      {
        key: "filter-status",
        group: "filter-status",
        label: t("UserStatus"),
        isHeader: true,
      },
      {
        key: "1",
        group: "filter-status",
        label: t("Common:Active"),
        isSelector: true,
        selectorType: "room",
      },
    ];

    const filterOptions = [];

    filterOptions.push(...statusItems);
    filterOptions.push(...typeItems);
    // filterOptions.push(...roleItems);
    // filterOptions.push(...accountItems);
    // filterOptions.push(...roomItems);

    return filterOptions;
  };

  const getSortData = React.useCallback(() => {
    return [
      {
        id: "sory-by_first-name",
        key: "firstname",
        label: t("Common:ByFirstNameSorting"),
        default: true,
      },
      {
        id: "sory-by_last-name",
        key: "lastname",
        label: t("Common:ByLastNameSorting"),
        default: true,
      },
    ];
  }, [t]);

  const getSelectedInputValue = React.useCallback(() => {
    return filter.search;
  }, [filter.search]);

  const getSelectedSortData = React.useCallback(() => {
    return {
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };
  }, [filter.sortOrder, filter.sortBy]);

  //TODO: add new options from filter after update backend
  const getSelectedFilterData = React.useCallback(async () => {
    const { guestCaption, userCaption, groupCaption } = customNames;
    const filterValues = [];

    if (filter.employeeStatus || filter.activationStatus) {
      const key = filter.employeeStatus === 2 ? 3 : filter.activationStatus;
      let label = "";

      switch (key) {
        case 1:
          label = t("Common:Active");
          break;
        case 2:
          label = t("PeopleTranslations:PendingTitle");
          break;
        case 3:
          label = t("PeopleTranslations:DisabledEmployeeStatus");
          break;
      }

      filterValues.push({
        key,
        label,
        group: "filter-status",
      });
    }

    if (filter.role) {
      let label = null;

      switch (filter.role) {
        case "admin":
          label = t("Common:DocSpaceAdmin");
          break;
        case "manager":
          label = t("Common:RoomAdmin");
          break;
        case "user":
          label = userCaption;
          break;
        default:
          label = "";
      }

      filterValues.push({
        key: filter.role,
        label: label,
        group: "filter-type",
      });
    }

    if (filter.group) {
      const group = groups.find((group) => group.id === filter.group);

      if (group) {
        filterValues.push({
          key: filter.group,
          label: group.name,
          group: "filter-other",
        });
      }
    }

    const currentFilterValues = [];

    setSelectedFilterValues((value) => {
      if (!value) {
        currentFilterValues.push(...filterValues);
        return filterValues.map((f) => ({ ...f }));
      }

      const items = value.map((v) => {
        const item = filterValues.find((f) => f.group === v.group);

        if (item) {
          if (item.isMultiSelect) {
            let isEqual = true;

            item.key.forEach((k) => {
              if (!v.key.includes(k)) {
                isEqual = false;
              }
            });

            if (isEqual) return item;

            return false;
          } else {
            if (item.key === v.key) return item;
            return false;
          }
        } else {
          return false;
        }
      });

      const newItems = filterValues.filter(
        (v) => !items.find((i) => i.group === v.group)
      );

      items.push(...newItems);

      currentFilterValues.push(...items.filter((i) => i));

      return items.filter((i) => i);
    });

    return currentFilterValues;
  }, [
    filter.employeeStatus,
    filter.activationStatus,
    filter.role,
    filter.group,
    t,
    customNames,
  ]);

  //TODO: add new options from filter after update backend
  const removeSelectedItem = ({ key, group }) => {
    const newFilter = filter.clone();
    newFilter.page = 0;

    if (group === "filter-status") {
      newFilter.employeeStatus = null;
      newFilter.activationStatus = null;
    }

    if (group === "filter-type") {
      newFilter.role = null;
    }

    if (group === "filter-other") {
      newFilter.group = null;
    }

    setIsLoading(true);
    fetchPeople(newFilter, true).finally(() => setIsLoading(false));
  };

  const clearAll = () => {
    setIsLoading(true);
    fetchPeople(null, true).finally(() => setIsLoading(false));
  };

  return isLoaded && tReady ? (
    <FilterInput
      t={t}
      onFilter={onFilter}
      getFilterData={getData}
      getSelectedFilterData={getSelectedFilterData}
      onSort={onSort}
      getSortData={getSortData}
      getSelectedSortData={getSelectedSortData}
      onSearch={onSearch}
      getSelectedInputValue={getSelectedInputValue}
      filterHeader={t("Common:AdvancedFilter")}
      contextMenuHeader={t("Common:AddFilter")}
      placeholder={t("Common:Search")}
      isMobile={isMobileOnly}
      viewAs={viewAs}
      viewSelectorVisible={false}
      removeSelectedItem={removeSelectedItem}
      isAccounts={true}
      clearAll={clearAll}
    />
  ) : (
    <Loaders.Filter />
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const {
      loadingStore,
      filterStore,
      usersStore,
      groupsStore,
      viewAs,
    } = peopleStore;
    const { settingsStore, userStore, isLoaded, isAdmin } = auth;
    const { customNames } = settingsStore;
    const { user } = userStore;
    const { groups } = groupsStore;
    const { getUsersList: fetchPeople } = usersStore;
    const { filter } = filterStore;
    const { setIsLoading } = loadingStore;

    return {
      customNames,
      isLoaded,
      isAdmin,
      user,
      groups,
      fetchPeople,
      filter,
      setIsLoading,
      viewAs,
    };
  })(
    observer(
      withLayoutSize(
        withTranslation([
          "People",
          "Common",
          "PeopleTranslations",
          "GroupSelector",
        ])(withPeopleLoader(SectionFilterContent)(<Loaders.Filter />))
      )
    )
  )
);
