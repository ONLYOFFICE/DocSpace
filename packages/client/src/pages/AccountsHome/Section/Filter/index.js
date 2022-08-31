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
    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };

  const onSort = (sortId, sortDirection) => {
    const sortBy = sortId;
    const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;

    setIsLoading(true);

    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };

  const onSearch = (data = "") => {
    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.search = data;

    setIsLoading(true);

    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };

  // TODO: change translation keys
  const getData = async () => {
    const { userCaption } = customNames;

    const statusItems = [
      {
        key: "filter-status",
        group: "filter-status",
        label: t("UserStatus"),
        isHeader: true,
      },
      {
        key: 1,
        group: "filter-status",
        label: t("Common:Active"),
      },
      {
        key: 2,
        group: "filter-status",
        label: t("PeopleTranslations:PendingTitle"),
      },
      {
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
      { key: "admin", group: "filter-type", label: t("Administrator") },
      {
        key: "manager",
        group: "filter-type",
        label: "Manager",
      },
      {
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
        key: "firstname",
        label: t("Common:ByFirstNameSorting"),
        default: true,
      },
      { key: "lastname", label: t("Common:ByLastNameSorting"), default: true },
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
  const getSelectedFilterData = async () => {
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
          label = t("Administrator");
          break;
        case "user":
          label = userCaption;
          break;
        case "guest":
          label = guestCaption;
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

    return filterValues;
  };

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
    fetchPeople(newFilter).finally(() => setIsLoading(false));
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
      filterHeader={t("Common:Filter")}
      contextMenuHeader={t("Common:AddFilter")}
      placeholder={t("Common:Search")}
      isMobile={isMobileOnly}
      viewAs={viewAs}
      viewSelectorVisible={false}
      removeSelectedItem={removeSelectedItem}
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
