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
import {
  EmployeeType,
  EmployeeStatus,
  PaymentsType,
  AccountLoginType,
} from "@docspace/common/constants";
import { SSO_LABEL } from "SRC_DIR/helpers/constants";

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

const getPayments = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-account";
    }),
    "key"
  );

  return employeeStatus || null;
};

const getAccountLoginType = (filterValues) => {
  const accountLoginType = result(
    find(filterValues, (value) => {
      return value.group === "filter-login-type";
    }),
    "key"
  );

  return accountLoginType || null;
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
  standalone,
}) => {
  const [selectedFilterValues, setSelectedFilterValues] = React.useState(null);

  const onFilter = (data) => {
    const status = getStatus(data);

    const role = getRole(data);
    const group = getGroup(data);
    const payments = getPayments(data);
    const accountLoginType = getAccountLoginType(data);

    const newFilter = filter.clone();

    if (status === 3) {
      newFilter.employeeStatus = EmployeeStatus.Disabled;
      newFilter.activationStatus = null;
    } else if (status === 2) {
      console.log(status);
      newFilter.employeeStatus = EmployeeStatus.Active;
      newFilter.activationStatus = status;
    } else {
      newFilter.employeeStatus = null;
      newFilter.activationStatus = status;
    }

    newFilter.page = 0;

    newFilter.role = role;

    newFilter.group = group;

    newFilter.payments = payments;

    newFilter.accountLoginType = accountLoginType;

    //console.log(newFilter);

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

  const getData = async () => {
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
      },
      {
        id: "filter_type-docspace-admin",
        key: EmployeeType.Admin,
        group: "filter-type",
        label: t("Common:DocSpaceAdmin"),
      },
      {
        id: "filter_type-room-admin",
        key: EmployeeType.User,
        group: "filter-type",
        label: t("Common:RoomAdmin"),
      },
      {
        id: "filter_type-room-admin",
        key: EmployeeType.Collaborator,
        group: "filter-type",
        label: t("Common:PowerUser"),
      },
      {
        id: "filter_type-user",
        key: EmployeeType.Guest,
        group: "filter-type",
        label: t("Common:User"),
      },
    ];

    // const roleItems = [
    //   {
    //     key: "filter-role",
    //     group: "filter-role",
    //     label: "Role in room",
    //     isHeader: true,
    //   },
    //   { key: "1", group: "filter-role", label: "Room manager" },
    //   { key: "2", group: "filter-role", label: "Co-worker" },
    //   { key: "3", group: "filter-role", label: "Editor" },
    //   { key: "4", group: "filter-role", label: "Form filler" },
    //   { key: "5", group: "filter-role", label: "Reviewer" },
    //   { key: "6", group: "filter-role", label: "Commentator" },
    //   { key: "7", group: "filter-role", label: "Viewer" },
    // ];

    const accountItems = [
      {
        key: "filter-account",
        group: "filter-account",
        label: t("ConnectDialog:Account"),
        isHeader: true,
        isLast: false,
      },
      {
        key: PaymentsType.Paid,
        group: "filter-account",
        label: t("Common:Paid"),
      },
      {
        key: PaymentsType.Free,
        group: "filter-account",
        label: t("SmartBanner:Price"),
      },
    ];

    // const roomItems = [
    //   {
    //     key: "filter-status",
    //     group: "filter-status",
    //     label: t("UserStatus"),
    //     isHeader: true,
    //   },
    //   {
    //     key: "1",
    //     group: "filter-status",
    //     label: t("Common:Active"),
    //     isSelector: true,
    //     selectorType: "room",
    //   },
    // ];

    const accountLoginTypeItems = [
      {
        key: "filter-login-type",
        group: "filter-login-type",
        label: t("PeopleTranslations:AccountLoginType"),
        isHeader: true,
        isLast: true,
      },
      {
        key: AccountLoginType.SSO,
        group: "filter-login-type",
        label: SSO_LABEL,
      },
      //TODO: uncomment after ldap be ready
      /*{
        key: AccountLoginType.LDAP,
        group: "filter-login-type",
        label: t("PeopleTranslations:LDAPLbl"),
      },*/
      {
        key: AccountLoginType.STANDART,
        group: "filter-login-type",
        label: t("PeopleTranslations:StandardLogin"),
      },
    ];

    const filterOptions = [];

    filterOptions.push(...statusItems);
    filterOptions.push(...typeItems);
    // filterOptions.push(...roleItems);
    if (!standalone) filterOptions.push(...accountItems);
    // filterOptions.push(...roomItems);
    filterOptions.push(...accountLoginTypeItems);

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
      {
        id: "sory-by_type",
        key: "type",
        label: t("Common:Type"),
        default: true,
      },
      {
        id: "sory-by_email",
        key: "email",
        label: t("Common:Email"),
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

      switch (+filter.role) {
        case EmployeeType.Admin:
          label = t("Common:DocSpaceAdmin");
          break;
        case EmployeeType.User:
          label = t("Common:RoomAdmin");
          break;
        case EmployeeType.Collaborator:
          label = t("Common:PowerUser");
          break;
        case EmployeeType.Guest:
          label = t("Common:User");
          break;
        default:
          label = "";
      }

      filterValues.push({
        key: +filter.role,
        label: label,
        group: "filter-type",
      });
    }

    if (filter?.payments?.toString()) {
      filterValues.push({
        key: filter.payments.toString(),
        label:
          PaymentsType.Paid === filter.payments.toString()
            ? t("Common:Paid")
            : t("SmartBanner:Price"),
        group: "filter-account",
      });
    }

    if (filter?.accountLoginType?.toString()) {
      const label =
        AccountLoginType.SSO === filter.accountLoginType.toString()
          ? SSO_LABEL
          : AccountLoginType.LDAP === filter.accountLoginType.toString()
          ? t("PeopleTranslations:LDAPLbl")
          : t("PeopleTranslations:StandardLogin");
      filterValues.push({
        key: filter.accountLoginType.toString(),
        label: label,
        group: "filter-login-type",
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
    filter.payments,
    filter.group,
    filter.accountLoginType,
    t,
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

    if (group === "filter-account") {
      newFilter.payments = null;
    }

    if (group === "filter-login-type") {
      newFilter.accountLoginType = null;
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
    const { loadingStore, filterStore, usersStore, groupsStore, viewAs } =
      peopleStore;
    const { userStore, isLoaded, isAdmin, settingsStore } = auth;
    const { standalone } = settingsStore;
    const { user } = userStore;
    const { groups } = groupsStore;
    const { getUsersList: fetchPeople } = usersStore;
    const { filter } = filterStore;
    const { setIsLoading } = loadingStore;

    return {
      isLoaded,
      isAdmin,
      user,
      groups,
      fetchPeople,
      filter,
      setIsLoading,
      viewAs,
      standalone,
    };
  })(
    observer(
      withLayoutSize(
        withTranslation([
          "People",
          "Common",
          "PeopleTranslations",
          "ConnectDialog",
          "SmartBanner",
        ])(withPeopleLoader(SectionFilterContent)(<Loaders.Filter />))
      )
    )
  )
);
