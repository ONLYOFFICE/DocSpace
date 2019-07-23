import React from "react";
import { FilterInput } from "asc-web-components";

const getData = () => {
  return [
    {
      key: "filter-status",
      group: "filter-status",
      label: "Status",
      isHeader: true
    },
    { key: "filter-status-active", group: "filter-status", label: "Active" },
    {
      key: "filter-status-disabled",
      group: "filter-status",
      label: "Disabled"
    },
    { key: "filter-status-pending", group: "filter-status", label: "Pending" },
    { key: "filter-type", group: "filter-type", label: "Type", isHeader: true },
    {
      key: "filter-type-administrator",
      group: "filter-type",
      label: "Administrator"
    },
    { key: "filter-type-user", group: "filter-type", label: "User" },
    { key: "filter-type-guest", group: "filter-type", label: "Guest" },
    {
      key: "filter-group",
      group: "filter-group",
      label: "Group",
      isHeader: true
    },
    { key: "filter-type-guest", group: "filter-group", label: "Group" }
  ];
};

const getSortData = () => {
  return [{ id: "name", label: "Name" }, { id: "surname", label: "Surname" }];
};

const SectionFilterContent = () => (
  <FilterInput
    getFilterData={getData}
    getSortData={getSortData}
    onFilter={result => {
      console.log(result);
    }}
  />
);

export default SectionFilterContent;
