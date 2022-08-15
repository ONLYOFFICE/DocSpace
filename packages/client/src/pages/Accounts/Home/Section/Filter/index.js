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

  const getData = async () => {
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
            label: t("PeopleTranslations:DisabledEmployeeStatus"),
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
        label: t("PeopleTranslations:PendingTitle"),
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

  const getSelectedInputValue = React.useCallback(() => {
    return filter.search;
  }, [filter.search]);

  const getSelectedSortData = React.useCallback(() => {
    return {
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };
  }, [filter.sortOrder, filter.sortBy]);

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
