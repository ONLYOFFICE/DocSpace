import { getObjectByLocation, toUrlParams } from "../../utils";
import queryString from "query-string";

const DEFAULT_PAGE = 0;
const DEFAULT_PAGE_COUNT = 100;
const DEFAULT_TOTAL = 0;
const DEFAULT_SORT_BY = "DateAndTime";
const DEFAULT_SORT_ORDER = "descending";
const DEFAULT_VIEW = "row";
const DEFAULT_FILTER_TYPE = null;
const DEFAULT_SEARCH_TYPE = true; //withSubfolders
const DEFAULT_SEARCH = null;
const DEFAULT_AUTHOR_TYPE = null;
const DEFAULT_SELECTED_ITEM = {};
const DEFAULT_FOLDER = "@my";

const SEARCH_TYPE = "withSubfolders";
const AUTHOR_TYPE = "authorType";
const FILTER_TYPE = "filterType";
const SEARCH = "search";
const SORT_BY = "sortby";
const SORT_ORDER = "sortorder";
const VIEW_AS = "viewas";
const PAGE = "page";
const PAGE_COUNT = "count";
const FOLDER = "folder";
const PREVIEW = "preview";

// TODO: add next params
// subjectGroup bool
// subjectID
// searchInContent bool

class FilesFilter {
  static getDefault(total = DEFAULT_TOTAL) {
    return new FilesFilter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, total);
  }

  static getFilter(location) {
    if (!location) return this.getDefault();

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
    const page =
      (urlFilter[PAGE] && +urlFilter[PAGE] - 1) || defaultFilter.page;
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

  constructor(
    page = DEFAULT_PAGE,
    pageCount = DEFAULT_PAGE_COUNT,
    total = DEFAULT_TOTAL,
    sortBy = DEFAULT_SORT_BY,
    sortOrder = DEFAULT_SORT_ORDER,
    viewAs = DEFAULT_VIEW,
    filterType = DEFAULT_FILTER_TYPE,
    withSubfolders = DEFAULT_SEARCH_TYPE,
    search = DEFAULT_SEARCH,
    authorType = DEFAULT_AUTHOR_TYPE,
    selectedItem = DEFAULT_SELECTED_ITEM,
    folder = DEFAULT_FOLDER
  ) {
    this.page = page;
    this.pageCount = pageCount;
    this.sortBy = sortBy;
    this.sortOrder = sortOrder;
    this.viewAs = viewAs;
    this.filterType = filterType;
    this.withSubfolders = withSubfolders;
    this.search = search;
    this.total = total;
    this.authorType = authorType;
    this.selectedItem = selectedItem;
    this.folder = folder;
  }

  getStartIndex = () => {
    return this.page * this.pageCount;
  };

  hasNext = () => {
    return this.total - this.getStartIndex() > this.pageCount;
  };

  hasPrev = () => {
    return this.page > 0;
  };

  toApiUrlParams = () => {
    const {
      authorType,
      filterType,
      page,
      pageCount,
      search,
      sortBy,
      sortOrder,
      withSubfolders,
      startIndex,
    } = this;

    const isFilterSet =
      filterType || (search ?? "").trim() || authorType
        ? withSubfolders
        : false;
    const userIdOrGroupId =
      authorType && authorType.includes("_")
        ? authorType.slice(authorType.indexOf("_") + 1)
        : null;

    const dtoFilter = {
      count: pageCount,
      startIndex: startIndex ? startIndex : this.getStartIndex(),
      page: page,
      sortby: sortBy,
      sortOrder: sortOrder,
      filterType: filterType,
      filterValue: (search ?? "").trim(),
      withSubfolders: isFilterSet,
      userIdOrGroupId,
    };

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  toUrlParams = () => {
    const {
      authorType,
      filterType,
      folder,
      page,
      pageCount,
      search,
      sortBy,
      sortOrder,
      withSubfolders,
    } = this;

    const dtoFilter = {};

    const URLParams = queryString.parse(window.location.href);

    if (filterType) {
      dtoFilter[FILTER_TYPE] = filterType;
    }

    if (withSubfolders) {
      dtoFilter[SEARCH_TYPE] = withSubfolders;
    }

    if (search) {
      dtoFilter[SEARCH] = search.trim();
    }
    if (authorType) {
      dtoFilter[AUTHOR_TYPE] = authorType;
    }
    if (folder) {
      dtoFilter[FOLDER] = folder;
    }

    if (pageCount !== DEFAULT_PAGE_COUNT) {
      dtoFilter[PAGE_COUNT] = pageCount;
    }

    if (URLParams.preview) {
      dtoFilter[PREVIEW] = URLParams.preview;
    }

    dtoFilter[PAGE] = page + 1;
    dtoFilter[SORT_BY] = sortBy;
    dtoFilter[SORT_ORDER] = sortOrder;

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  getLastPage() {
    return Math.ceil(this.total / this.pageCount) - 1;
  }

  clone() {
    return new FilesFilter(
      this.page,
      this.pageCount,
      this.total,
      this.sortBy,
      this.sortOrder,
      this.viewAs,
      this.filterType,
      this.withSubfolders,
      this.search,
      this.authorType,
      this.selectedItem,
      this.folder
    );
  }

  equals(filter) {
    const equals =
      this.filterType === filter.filterType &&
      this.authorType === filter.authorType &&
      this.withSubfolders === filter.withSubfolders &&
      this.search === filter.search &&
      this.sortBy === filter.sortBy &&
      this.sortOrder === filter.sortOrder &&
      this.viewAs === filter.viewAs &&
      this.page === filter.page &&
      this.selectedItem.key === filter.selectedItem.key &&
      this.folder === filter.folder &&
      this.pageCount === filter.pageCount;

    return equals;
  }
}

export default FilesFilter;
