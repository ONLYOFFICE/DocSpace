import React from "react";
import find from "lodash/find";
import result from "lodash/result";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import FilterInput from "@appserver/common/components/FilterInput";
import Loaders from "@appserver/common/components/Loaders";
import { withLayoutSize } from "@appserver/common/utils";
import { isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";

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
  sectionWidth,
  tReady,
  viewAs,
  setIsLoading,
  fetchPeople,
  filter,
  groups,
  customNames,
  isAdmin,
}) => {
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

  const getData = () => {
    const { guestCaption, userCaption, groupCaption } = customNames;

    const options = !isAdmin
      ? []
      : [
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
            key: "2",
            group: "filter-status",
            label: t("Translations:DisabledEmployeeStatus"),
          },
        ];

    const filterOptions = [
      ...options,
      {
        key: "filter-type",
        group: "filter-type",
        label: t("Common:Type"),
        isHeader: true,
      },
      { key: "admin", group: "filter-type", label: t("Administrator") },
      {
        key: "user",
        group: "filter-type",
        label: userCaption,
      },
      {
        key: "guest",
        group: "filter-type",
        label: guestCaption,
      },
      {
        key: "filter-email",
        group: "filter-email",
        label: t("Common:Email"),
        isHeader: true,
      },
      {
        key: "1",
        group: "filter-email",
        label: t("Common:Active"),
      },
      {
        key: "2",
        group: "filter-email",
        label: t("Translations:PendingTitle"),
      },

      {
        key: "filter-other",
        group: "filter-other",
        label: t("Common:Department"),
        isHeader: true,
      },
      {
        key: "group",
        group: "filter-other",
        label: t("GroupSelector:AddDepartmentsButtonLabel"),
        isSelector: true,
      },
    ];

    return filterOptions;
  };

  const getSortData = () => {
    return [
      {
        key: "firstname",
        label: t("Common:ByFirstNameSorting"),
        default: true,
      },
      { key: "lastname", label: t("Common:ByLastNameSorting"), default: true },
    ];
  };

  const getSelectedFilterData = () => {
    const selectedFilterData = {
      filterValues: [],
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };

    selectedFilterData.inputValue = filter.search;

    if (filter.employeeStatus) {
      selectedFilterData.filterValues.push({
        key: `${filter.employeeStatus}`,
        group: "filter-status",
      });
    }

    if (filter.activationStatus) {
      selectedFilterData.filterValues.push({
        key: `${filter.activationStatus}`,
        group: "filter-email",
      });
    }

    if (filter.role) {
      selectedFilterData.filterValues.push({
        key: filter.role,
        group: "filter-type",
      });
    }

    if (filter.group) {
      const group = groups.find((group) => group.id === filter.group);

      if (group) {
        selectedFilterData.filterValues.push({
          key: filter.group,
          label: group.name,
          group: "filter-other",
        });
      }
    }

    return selectedFilterData;
  };

  return isLoaded && tReady ? (
    <FilterInput
      sectionWidth={sectionWidth}
      getFilterData={getData}
      getSortData={getSortData}
      getSelectedFilterData={getSelectedFilterData}
      onFilter={onFilter}
      onSort={onSort}
      onSearch={onSearch}
      contextMenuHeader={t("Common:AddFilter")}
      placeholder={t("Common:Search")}
      isMobile={isMobileOnly}
      viewAs={viewAs}
      viewSelectorVisible={false}
      style={{ marginBottom: "2px" }}
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
        withTranslation(["Home", "Common", "Translations", "GroupSelector"])(
          SectionFilterContent
        )
      )
    )
  )
);
