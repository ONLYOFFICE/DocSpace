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

const getEmployeeStatus = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-status";
    }),
    "key"
  );

  return employeeStatus ? +employeeStatus : null;
};

const getActivationStatus = (filterValues) => {
  const activationStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-email";
    }),
    "key"
  );

  return activationStatus ? +activationStatus : null;
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
  // TODO:
  const onFilter = (data) => {
    const employeeStatus = getEmployeeStatus(data);
    const activationStatus = getActivationStatus(data);
    const role = getRole(data);
    const group = getGroup(data);

    const newFilter = filter.clone();
    newFilter.page = 0;

    newFilter.employeeStatus = employeeStatus;
    newFilter.activationStatus = activationStatus;
    newFilter.role = role;
    newFilter.group = group;

    setIsLoading(true);
    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };
  // TODO:
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
  // TODO: change fetchPeople
  const onSearch = (data = "") => {
    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.search = data;

    setIsLoading(true);

    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };
  // TODO: change keys
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
        key: "1",
        group: "filter-status",
        label: t("Common:Active"),
      },
      {
        key: "3",
        group: "filter-status",
        label: t("PeopleTranslations:PendingTitle"),
      },
      {
        key: "2",
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
      },
      { key: "admin", group: "filter-type", label: t("Administrator") },
      {
        key: "manager",
        group: "filter-type",
        label: "TODO:Manager",
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
        label: "TODO: Role in room",
        isHeader: true,
      },
      { key: "1", group: "filter-role", label: "TODO: Room manager" },
      { key: "2", group: "filter-role", label: "TODO: Co-worker" },
      { key: "3", group: "filter-role", label: "TODO: Editor" },
      { key: "4", group: "filter-role", label: "TODO: Form filler" },
      { key: "5", group: "filter-role", label: "TODO: Reviewer" },
      { key: "6", group: "filter-role", label: "TODO: Commentator" },
      { key: "7", group: "filter-role", label: "TODO: Viewer" },
    ];

    const accountItems = [
      {
        key: "filter-account",
        group: "filter-account",
        label: "TODO: Account",
        isHeader: true,
      },
      { key: "paid", group: "filter-account", label: "TODO: Paid" },
      { key: "free", group: "filter-account", label: "TODO: Free" },
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

    filterOptions.push(statusItems);
    filterOptions.push(typeItems);
    filterOptions.push(roleItems);
    filterOptions.push(accountItems);
    filterOptions.push(roomItems);

    return filterOptions;
  };
  //TODO:
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
  //TODO:
  const getSelectedInputValue = React.useCallback(() => {
    return filter.search;
  }, [filter.search]);
  //TODO:
  const getSelectedSortData = React.useCallback(() => {
    return {
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };
  }, [filter.sortOrder, filter.sortBy]);
  //TODO:
  const getSelectedFilterData = async () => {
    const { guestCaption, userCaption, groupCaption } = customNames;
    const filterValues = [];

    if (filter.employeeStatus) {
      filterValues.push({
        key: `${filter.employeeStatus}`,
        label:
          `${filter.employeeStatus}` === "1"
            ? t("Common:Active")
            : t("PeopleTranslations:DisabledEmployeeStatus"),
        group: "filter-status",
      });
    }

    if (filter.activationStatus) {
      filterValues.push({
        key: `${filter.activationStatus}`,
        label:
          `${filter.activationStatus}` === "1"
            ? t("Common:Active")
            : t("PeopleTranslations:PendingTitle"),
        group: "filter-email",
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
  //TODO:
  const removeSelectedItem = ({ key, group }) => {
    const newFilter = filter.clone();
    newFilter.page = 0;

    if (group === "filter-status") {
      newFilter.employeeStatus = null;
    }

    if (group === "filter-type") {
      newFilter.role = null;
    }

    if (group === "filter-email") {
      newFilter.activationStatus = null;
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
