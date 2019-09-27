import {
  EMPLOYEE_STATUS,
  ACTIVATION_STATUS,
  ROLE,
  GROUP,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  PAGE,
  PAGE_COUNT
} from "./constants";
import Filter from "../store/people/filter";

export function getObjectByLocation(location) {
  if (!location.search || !location.search.length) return null;

  const searchUrl = location.search.substring(1);
  const object = JSON.parse(
    '{"' +
      decodeURI(searchUrl)
        .replace(/"/g, '\\"')
        .replace(/&/g, '","')
        .replace(/=/g, '":"') +
      '"}'
  );

  return object;
}

export function getFilterByLocation(location) {
  const urlFilter = getObjectByLocation(location);

  if(!urlFilter) return null;

  const defaultFilter = Filter.getDefault();

  const employeeStatus =
    (urlFilter[EMPLOYEE_STATUS] && +urlFilter[EMPLOYEE_STATUS]) ||
    defaultFilter.employeeStatus;
  const activationStatus =
    (urlFilter[ACTIVATION_STATUS] && +urlFilter[ACTIVATION_STATUS]) ||
    defaultFilter.activationStatus;
  const role = urlFilter[ROLE] || defaultFilter.role;
  const group = urlFilter[GROUP] || defaultFilter.group;
  const search = urlFilter[SEARCH] || defaultFilter.search;
  const sortBy = urlFilter[SORT_BY] || defaultFilter.sortBy;
  const sortOrder = urlFilter[SORT_ORDER] || defaultFilter.sortOrder;
  const page = (urlFilter[PAGE] && +urlFilter[PAGE]) || defaultFilter.page;
  const pageCount =
    (urlFilter[PAGE_COUNT] && +urlFilter[PAGE_COUNT]) ||
    defaultFilter.pageCount;

  const newFilter = new Filter(
    page,
    pageCount,
    defaultFilter.total,
    sortBy,
    sortOrder,
    employeeStatus,
    activationStatus,
    role,
    search,
    group
  );

  return newFilter;
}
