import {
  SEARCH_TYPE,
  FILTER_TYPE,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  VIEW_AS,
  PAGE,
  PAGE_COUNT,
  AUTHOR_TYPE,
  FOLDER,
} from "./constants";
import FilesFilter from "@appserver/common/src/api/files/filter";
import { getObjectByLocation } from "@appserver/common/src/utils";

export function getFilterByLocation(location) {
  const urlFilter = getObjectByLocation(location);

  if (!urlFilter) return null;

  const defaultFilter = FilesFilter.getDefault();

  const filterType =
    (urlFilter[FILTER_TYPE] && +urlFilter[FILTER_TYPE]) ||
    defaultFilter.filterType;
  const authorType =
    (urlFilter[AUTHOR_TYPE] &&
      urlFilter[AUTHOR_TYPE].includes("_") &&
      urlFilter[AUTHOR_TYPE]) ||
    defaultFilter.authorType;
  const withSubfolders =
    (urlFilter[SEARCH_TYPE] && urlFilter[SEARCH_TYPE]) ||
    defaultFilter.withSubfolders;
  const search = urlFilter[SEARCH] || defaultFilter.search;
  const sortBy = urlFilter[SORT_BY] || defaultFilter.sortBy;
  const viewAs = urlFilter[VIEW_AS] || defaultFilter.viewAs;
  const sortOrder = urlFilter[SORT_ORDER] || defaultFilter.sortOrder;
  const page = (urlFilter[PAGE] && +urlFilter[PAGE] - 1) || defaultFilter.page;
  const pageCount =
    (urlFilter[PAGE_COUNT] && +urlFilter[PAGE_COUNT]) ||
    defaultFilter.pageCount;
  const folder = urlFilter[FOLDER] || defaultFilter.folder;

  const newFilter = new FilesFilter(
    page,
    pageCount,
    defaultFilter.total,
    sortBy,
    sortOrder,
    viewAs,
    filterType,
    withSubfolders,
    search,
    authorType,
    defaultFilter.selectedItem,
    folder
  );

  return newFilter;
}
