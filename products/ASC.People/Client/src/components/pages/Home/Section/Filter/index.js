import React, { useCallback } from "react";
import { connect } from "react-redux";
import { FilterInput } from "asc-web-components";
import { fetchPeople } from "../../../../../store/people/actions";
import find from "lodash/find";
import result from "lodash/result";
import { isAdmin } from "../../../../../store/auth/selectors";
import { useTranslation } from "react-i18next";
import { typeGuest, typeUser, department } from './../../../../../helpers/customNames';

const getSortData = ( t ) => {
  return [
    { key: "firstname", label: t('ByFirstNameSorting') },
    { key: "lastname", label: t('ByLastNameSorting') }
  ];
};

const getEmployeeStatus = filterValues => {
  const employeeStatus = result(
    find(filterValues, value => {
      return value.group === "filter-status";
    }),
    "key"
  );

  return employeeStatus ? +employeeStatus : null;
};

const getActivationStatus = filterValues => {
  const activationStatus = result(
    find(filterValues, value => {
      return value.group === "filter-email";
    }),
    "key"
  );

  return activationStatus ? +activationStatus : null;
};

const getRole = filterValues => {
  const employeeStatus = result(
    find(filterValues, value => {
      return value.group === "filter-type";
    }),
    "key"
  );

  return employeeStatus || null;
};

const getGroup = filterValues => {
  const groupId = result(
    find(filterValues, value => {
      return value.group === "filter-group";
    }),
    "key"
  );

  return groupId || null;
};

const SectionFilterContent = ({
  fetchPeople,
  filter,
  onLoading,
  user,
  groups
}) => {
  const { t } = useTranslation();
  const selectedFilterData = {
    filterValues: [],
    sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
    sortId: filter.sortBy
  };

  selectedFilterData.inputValue = filter.search;

  if (filter.employeeStatus) {
    selectedFilterData.filterValues.push({
      key: `${filter.employeeStatus}`,
      group: "filter-status"
    });
  }

  if (filter.activationStatus) {
    selectedFilterData.filterValues.push({
      key: `${filter.activationStatus}`,
      group: "filter-email"
    });
  }

  if (filter.role) {
    selectedFilterData.filterValues.push({
      key: filter.role,
      group: "filter-type"
    });
  }

  if (filter.group) {
    selectedFilterData.filterValues.push({
      key: filter.group,
      group: "filter-group"
    });
  }

  const getData = useCallback(() => {
    const options = !isAdmin(user)
      ? []
      : [
          {
            key: "filter-status",
            group: "filter-status",
            label: t("UserStatus"),
            isHeader: true
          },
          {
            key: "1",
            group: "filter-status",
            label: t("LblActive")
          },
          {
            key: "2",
            group: "filter-status",
            label: t("LblTerminated")
          }
        ];

    const groupOptions = groups.map(group => {
      return { key: group.id, inSubgroup: true, group: "filter-group", label: group.name };
    });

    const filterOptions = [
      ...options,
      {
        key: "filter-email",
        group: "filter-email",
        label: t("Email"),
        isHeader: true
      },
      { 
        key: "1", 
        group: "filter-email", 
        label: t("LblActive") 
      },
      {
        key: "2",
        group: "filter-email",
        label: t("LblPending")
      },
      {
        key: "filter-type",
        group: "filter-type",
        label: t("UserType"),
        isHeader: true
      },
      { key: "admin", group: "filter-type", label: t("Administrator")},
      { key: "user", group: "filter-type", label: t('CustomTypeUser', { typeUser })},
      { key: "guest", group: "filter-type", label: t('CustomTypeGuest', { typeGuest }) },
      {
        key: "filter-other",
        group: "filter-other",
        label: t("LblOther"),
        isHeader: true
      },
      { key: "filter-type-group", group: "filter-other", subgroup: 'filter-group', label: t('CustomDepartment', { department }), defaultSelectLabel: t("DefaultSelectLabel") },
      ...groupOptions
    ];

    //console.log("getData (filterOptions)", filterOptions);

    return filterOptions;

  }, [user, groups, t]);

  const onFilter = useCallback(
    data => {
    console.log(data);

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = data.sortId;
    newFilter.sortOrder =
      data.sortDirection === "desc" ? "descending" : "ascending";
    newFilter.employeeStatus = getEmployeeStatus(data.filterValues);
    newFilter.activationStatus = getActivationStatus(data.filterValues);
    newFilter.role = getRole(data.filterValues);
    newFilter.search = data.inputValue || null;
    newFilter.group = getGroup(data.filterValues);

    onLoading(true);
    fetchPeople(newFilter).finally(() => onLoading(false));
    },
    [onLoading, fetchPeople, filter]
  );
  return (
    <FilterInput
      getFilterData={getData}
      getSortData={getSortData.bind(this, t)}
      selectedFilterData={selectedFilterData}
      onFilter={onFilter}
      directionAscLabel={t("DirectionAscLabel")}
      directionDescLabel={t("DirectionDescLabel")}
    />
  );
};

function mapStateToProps(state) {
  return {
    user: state.auth.user,
    groups: state.people.groups,
    filter: state.people.filter
  };
}

export default connect(
  mapStateToProps,
  { fetchPeople }
)(SectionFilterContent);
