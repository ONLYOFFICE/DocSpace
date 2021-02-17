import { action, makeObservable, observable } from "mobx";
//import { getFilterByLocation } from "../helpers/converters";
import { api, history } from "asc-web-common";
import config from "../../package.json";
import {
  EMPLOYEE_STATUS,
  ACTIVATION_STATUS,
  ROLE,
  GROUP,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  PAGE,
  PAGE_COUNT,
} from "../helpers/constants";

const { Filter } = api;

class FilterStore {
  filter = Filter.getDefault();

  constructor() {
    makeObservable(this, {
      filter: observable,
      setFilterParams: action,
      setFilterUrl: action,
      resetFilter: action,
      setFilter: action,
    });
  }

  setFilterUrl = (filter) => {
    const defaultFilter = Filter.getDefault();
    const params = [];

    if (filter.employeeStatus) {
      params.push(`${EMPLOYEE_STATUS}=${filter.employeeStatus}`);
    }

    if (filter.activationStatus) {
      params.push(`${ACTIVATION_STATUS}=${filter.activationStatus}`);
    }

    if (filter.role) {
      params.push(`${ROLE}=${filter.role}`);
    }

    if (filter.group) {
      params.push(`${GROUP}=${filter.group}`);
    }

    if (filter.search) {
      params.push(`${SEARCH}=${filter.search.trim()}`);
    }

    if (filter.pageCount !== defaultFilter.pageCount) {
      params.push(`${PAGE_COUNT}=${filter.pageCount}`);
    }

    params.push(`${PAGE}=${filter.page + 1}`);
    params.push(`${SORT_BY}=${filter.sortBy}`);
    params.push(`${SORT_ORDER}=${filter.sortOrder}`);

    //const isProfileView = history.location.pathname.includes('/people/view') || history.location.pathname.includes('/people/edit');
    //if (params.length > 0 && !isProfileView) {
    history.push(`${config.homepage}/filter?${params.join("&")}`);
    //}
  };

  setFilterParams = (data) => {
    this.setFilterUrl(data);
    this.setFilter(data);
  };

  resetFilter = () => {
    this.setFilter(Filter.getDefault());
  };

  setFilter = (filter) => {
    this.filter = filter;
  };
}

export default FilterStore;
