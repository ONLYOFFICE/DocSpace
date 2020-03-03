import {
  SEARCH_TYPE,
  AUTHOR_TYPE,
  FILE_TYPE,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  PAGE,
  PAGE_COUNT
} from "./constants";
import { api, utils } from "asc-web-common";
const { FilesFilter } = api;
const { getObjectByLocation } = utils;

export function getFilterByLocation(location) {
  const urlFilter = getObjectByLocation(location);

  if(!urlFilter) return null;

  const defaultFilter = FilesFilter.getDefault();

  const fileType =
    (urlFilter[FILE_TYPE] && +urlFilter[FILE_TYPE]) ||
    defaultFilter.fileType;
  const authorType =
    (urlFilter[AUTHOR_TYPE] && +urlFilter[AUTHOR_TYPE]) ||
    defaultFilter.authorType;
  const searchType = (urlFilter[SEARCH_TYPE] && +urlFilter[SEARCH_TYPE]) ||
  defaultFilter.searchType;
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
    fileType,
    authorType,
    searchType,
    search
  );

  return newFilter;
}
