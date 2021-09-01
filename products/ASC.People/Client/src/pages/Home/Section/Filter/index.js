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
      return value.group === "filter-group";
    }),
    "key"
  );

  return groupId || null;
};

class SectionFilterContent extends React.Component {
  onFilter = (data) => {
    const { setIsLoading, fetchPeople, filter } = this.props;

    const employeeStatus = getEmployeeStatus(data.filterValues);
    const activationStatus = getActivationStatus(data.filterValues);
    const role = getRole(data.filterValues);
    const group = getGroup(data.filterValues);
    const search = data.inputValue || "";
    const sortBy = data.sortId;
    const sortOrder =
      data.sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;
    newFilter.employeeStatus = employeeStatus;
    newFilter.activationStatus = activationStatus;
    newFilter.role = role;
    newFilter.search = search;
    newFilter.group = group;

    setIsLoading(true);
    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };

  getData = () => {
    const { groups, t, customNames, isAdmin } = this.props;
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

    const groupOptions = groups.map((group) => {
      return {
        key: group.id,
        inSubgroup: true,
        group: "filter-group",
        label: group.name,
      };
    });

    const filterOptions = [
      ...options,
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
        key: "filter-other",
        group: "filter-other",
        label: t("LblOther"),
        isHeader: true,
      },
      {
        key: "filter-type-group",
        group: "filter-other",
        subgroup: "filter-group",
        label: groupCaption,
        defaultSelectLabel: t("Common:Select"),
      },
      ...groupOptions,
    ];

    //console.log("getData (filterOptions)", filterOptions);

    return filterOptions;
  };

  getSortData = () => {
    const { t } = this.props;

    return [
      {
        key: "firstname",
        label: t("Common:ByFirstNameSorting"),
        default: true,
      },
      { key: "lastname", label: t("Common:ByLastNameSorting"), default: true },
    ];
  };

  getSelectedFilterData = () => {
    const { filter } = this.props;

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
      selectedFilterData.filterValues.push({
        key: filter.group,
        group: "filter-group",
      });
    }

    return selectedFilterData;
  };

  render() {
    const selectedFilterData = this.getSelectedFilterData();
    const { t, isLoaded, sectionWidth, tReady, viewAs } = this.props;

    return isLoaded && tReady ? (
      <FilterInput
        sectionWidth={sectionWidth}
        getFilterData={this.getData}
        getSortData={this.getSortData}
        selectedFilterData={selectedFilterData}
        onFilter={this.onFilter}
        directionAscLabel={t("Common:DirectionAscLabel")}
        directionDescLabel={t("Common:DirectionDescLabel")}
        placeholder={t("Common:Search")}
        contextMenuHeader={t("Common:AddFilter")}
        isMobile={isMobileOnly}
        viewAs={viewAs}
        viewSelectorVisible={false}
      />
    ) : (
      <Loaders.Filter />
    );
  }
}

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
        withTranslation(["Home", "Common", "Translations"])(
          SectionFilterContent
        )
      )
    )
  )
);
