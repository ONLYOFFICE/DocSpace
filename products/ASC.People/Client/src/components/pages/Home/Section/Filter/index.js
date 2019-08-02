import React from "react";
import { connect } from "react-redux";
import { FilterInput } from "asc-web-components";
import { fetchPeople } from "../../../../../store/people/actions";
import find from "lodash/find";
import result from "lodash/result";

const getData = () => {
  return [
    {
      key: "filter-status",
      group: "filter-status",
      label: "Status",
      isHeader: true
    },
    { key: "filter-status-active", group: "filter-status", label: "Active" },
    { key: "filter-status-disabled",  group: "filter-status", label: "Disabled" },
    {
      key: "filter-email",
      group: "filter-email",
      label: "Email",
      isHeader: true
    },
    { key: "filter-email-active", group: "filter-email", label: "Active" },
    { key: "filter-email-pending", group: "filter-email", label: "Pending" },
    { 
      key: "filter-type", 
      group: "filter-type", 
      label: "Type", 
      isHeader: true 
    },
    {
      key: "filter-type-admin",  group: "filter-type", label: "Administrator" },
    { key: "filter-type-user", group: "filter-type", label: "User" },
    { key: "filter-type-guest", group: "filter-type", label: "Guest" },
    {
      key: "filter-group",
      group: "filter-group",
      label: "Group",
      isHeader: true
    },
    { key: "filter-type-group", group: "filter-group", label: "Group" }
  ];
};

const getSortData = () => {
  return [{ id: "name", label: "Name" }, { id: "surname", label: "Surname" }];
};

const getEmployeeStatus = filterValues => {
  const employeeStatus = result(
    find(filterValues, value => {
      return value.key === "filter-status";
    }),
    "value"
  );

  if (employeeStatus === "filter-status-active") {
    return 1;
  } else if (employeeStatus === "filter-status-disabled") {
    return 2;
  }

  return null;
};

const getEmailStatus = filterValues => {
  const employeeStatus = result(
    find(filterValues, value => {
      return value.key === "filter-type";
    }),
    "value"
  );

  if (employeeStatus === "filter-email-active") {
    return 1;
  } else if (employeeStatus === "filter-email-pending") {
    return 2;
  }

  return null;
};

const getRole = filterValues => {
  const employeeStatus = result(
    find(filterValues, value => {
      return value.key === "filter-type";
    }),
    "value"
  );

  if (employeeStatus === "filter-type-admin") {
    return "admin";
  } if (employeeStatus === "filter-type-user") {
    return "user";
  } else if (employeeStatus === "filter-type-guest") {
    return "guest";
  }

  return null;
};

const SectionFilterContent = ({ fetchPeople, filter, onLoading }) => (
  <FilterInput
    getFilterData={getData}
    getSortData={getSortData}
    onFilter={data => {
      console.log(data);

      const newFilter = filter.clone();
      newFilter.sortBy = data.sortId === "name" ? "firstname" : "lastname";
      newFilter.sortOrder =
        data.sortDirection === "desc" ? "descending" : "ascending";
      newFilter.employeeStatus = getEmployeeStatus(data.filterValue);
      newFilter.activationStatus = getEmailStatus(data.filterValue);
      newFilter.role = getRole(data.filterValue);
      newFilter.search = data.inputValue || null;

      onLoading(true);
      fetchPeople(newFilter).finally(() => onLoading(false));
    }}
  />
);

function mapStateToProps(state) {
  return {
    filter: state.people.filter
  };
}

export default connect(
  mapStateToProps,
  { fetchPeople }
)(SectionFilterContent);
