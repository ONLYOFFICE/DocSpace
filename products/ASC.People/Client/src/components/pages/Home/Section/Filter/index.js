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
    { key: "1", group: "filter-status", label: "Active" },
    { key: "2",  group: "filter-status", label: "Disabled" },
    {
      key: "filter-email",
      group: "filter-email",
      label: "Email",
      isHeader: true
    },
    { key: "1", group: "filter-email", label: "Active" },
    { key: "2", group: "filter-email", label: "Pending" },
    { 
      key: "filter-type", 
      group: "filter-type", 
      label: "Type", 
      isHeader: true 
    },
    {
      key: "admin",  group: "filter-type", label: "Administrator" },
    { key: "user", group: "filter-type", label: "User" },
    { key: "guest", group: "filter-type", label: "Guest" },
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
  return [{ key: "firstname", label: "Name" }, { key: "lastname", label: "Surname" }];
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

const SectionFilterContent = ({ fetchPeople, filter, onLoading }) => { 
  const selectedFilterData = {
    filterValue: [],
    sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
    sortId: filter.sortBy,
  };

  if(filter.search) {
    selectedFilterData.inputValue = filter.search;
  }

  if(filter.employeeStatus) {
    selectedFilterData.filterValue.push({ key: `${filter.employeeStatus}`, group: "filter-status" });
  }

  if(filter.activationStatus) {
    selectedFilterData.filterValue.push({ key: `${filter.activationStatus}`, group: "filter-email" });
  }

  if(filter.role) {
    selectedFilterData.filterValue.push({ key: filter.role, group: "filter-type" });
  }

  

  return (
  <FilterInput
    getFilterData={getData}
    getSortData={getSortData}
    selectedFilterData={selectedFilterData}
    onFilter={data => {
      console.log(data);

      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.sortBy = data.sortId;
      newFilter.sortOrder =
        data.sortDirection === "desc" ? "descending" : "ascending";
      newFilter.employeeStatus = getEmployeeStatus(data.filterValue);
      newFilter.activationStatus = getActivationStatus(data.filterValue);
      newFilter.role = getRole(data.filterValue);
      newFilter.search = data.inputValue || null;

      onLoading(true);
      fetchPeople(newFilter).finally(() => onLoading(false));
    }}
  />
)};

function mapStateToProps(state) {
  return {
    filter: state.people.filter
  };
}

export default connect(
  mapStateToProps,
  { fetchPeople }
)(SectionFilterContent);
