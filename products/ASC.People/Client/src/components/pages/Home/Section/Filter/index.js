import React from "react";
import { connect } from "react-redux";
import { FilterInput } from "asc-web-components";
import { fetchPeople } from "../../../../../store/people/actions";
import find from "lodash/find";
import result from "lodash/result";
import { withTranslation } from "react-i18next";
import {
  typeGuest,
  typeUser,
  department
} from "./../../../../../helpers/customNames";
import { withRouter } from "react-router";
import { getFilterByLocation } from "../../../../../helpers/converters";
import { store } from 'asc-web-common';
const { isAdmin } = store.auth.selectors;

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

class SectionFilterContent extends React.Component {
  componentDidMount() {
    const { location, filter, onLoading, fetchPeople } = this.props;

    const newFilter = getFilterByLocation(location);

    if(!newFilter || newFilter.equals(filter)) return;

    onLoading(true);
    fetchPeople(newFilter).finally(() => onLoading(false));
  }

  onFilter = data => {
    const { onLoading, fetchPeople, filter } = this.props;

    const employeeStatus = getEmployeeStatus(data.filterValues);
    const activationStatus = getActivationStatus(data.filterValues);
    const role = getRole(data.filterValues);
    const group = getGroup(data.filterValues);
    const search = data.inputValue || null;
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

    onLoading(true);
    fetchPeople(newFilter).finally(() => onLoading(false));
  };

  getData = () => {
    const { user, groups, t } = this.props;

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
      return {
        key: group.id,
        inSubgroup: true,
        group: "filter-group",
        label: group.name
      };
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
      { key: "admin", group: "filter-type", label: t("Administrator") },
      {
        key: "user",
        group: "filter-type",
        label: t("CustomTypeUser", { typeUser })
      },
      {
        key: "guest",
        group: "filter-type",
        label: t("CustomTypeGuest", { typeGuest })
      },
      {
        key: "filter-other",
        group: "filter-other",
        label: t("LblOther"),
        isHeader: true
      },
      {
        key: "filter-type-group",
        group: "filter-other",
        subgroup: "filter-group",
        label: t("CustomDepartment", { department }),
        defaultSelectLabel: t("DefaultSelectLabel")
      },
      ...groupOptions
    ];

    //console.log("getData (filterOptions)", filterOptions);

    return filterOptions;
  };

  getSortData = () => {
    const { t } = this.props;

    return [
      { key: "firstname", label: t("ByFirstNameSorting"), default: true },
      { key: "lastname", label: t("ByLastNameSorting"), default: true }
    ];
  };

  getSelectedFilterData = () => {
    const { filter } = this.props;
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

    return selectedFilterData;
  };

  render() {
    const selectedFilterData = this.getSelectedFilterData();
    const { t } = this.props;
    return (
      <FilterInput
        getFilterData={this.getData}
        getSortData={this.getSortData}
        selectedFilterData={selectedFilterData}
        onFilter={this.onFilter}
        directionAscLabel={t("DirectionAscLabel")}
        directionDescLabel={t("DirectionDescLabel")}
        placeholder={t("FilterPlaceholder")}
      />
    );
  }
}

function mapStateToProps(state) {
  return {
    user: state.auth.user,
    groups: state.people.groups,
    filter: state.people.filter,
    settings: state.auth.settings
  };
}

export default connect(
  mapStateToProps,
  { fetchPeople }
)(withRouter(withTranslation()(SectionFilterContent)));
