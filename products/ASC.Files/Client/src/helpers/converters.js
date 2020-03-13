import {
  SEARCH_TYPE,
  FILTER_TYPE,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  PAGE,
  PAGE_COUNT,
  AUTHOR_TYPE
} from "./constants";
import { api, utils } from "asc-web-common";
const { FilesFilter } = api;
const { getObjectByLocation } = utils;

export function getFilterByLocation(location) {
  const urlFilter = getObjectByLocation(location);

  if(!urlFilter) return null;

  const defaultFilter = FilesFilter.getDefault();

  const filterType =
    (urlFilter[FILTER_TYPE] && +urlFilter[FILTER_TYPE]) ||
    defaultFilter.filterType;
  const authorType = (urlFilter[AUTHOR_TYPE] && urlFilter[AUTHOR_TYPE].includes('_') && urlFilter[AUTHOR_TYPE]) || defaultFilter.authorType;
  const withSubfolders = (urlFilter[SEARCH_TYPE] && urlFilter[SEARCH_TYPE]) ||
  defaultFilter.withSubfolders;
  const search = urlFilter[SEARCH] || defaultFilter.search;
  const sortBy = urlFilter[SORT_BY] || defaultFilter.sortBy;
  const sortOrder = urlFilter[SORT_ORDER] || defaultFilter.sortOrder;
  const page = (urlFilter[PAGE] && (+urlFilter[PAGE]-1)) || defaultFilter.page;
  const pageCount =
    (urlFilter[PAGE_COUNT] && +urlFilter[PAGE_COUNT]) ||
    defaultFilter.pageCount;

  const newFilter = new FilesFilter(
    page,
    pageCount,
    defaultFilter.total,
    sortBy,
    sortOrder,
    filterType,
    withSubfolders,
    search,
    authorType
  );

  return newFilter;
}
