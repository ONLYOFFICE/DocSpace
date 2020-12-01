import { toUrlParams } from "../../utils";

const DEFAULT_PAGE = 0;
const DEFAULT_PAGE_COUNT = 25;
const DEFAULT_TOTAL = 0;
const DEFAULT_SORT_BY = "DateAndTime";
const DEFAULT_SORT_ORDER = "ascending";
const DEFAULT_VIEW = "row";
const DEFAULT_FILTER_TYPE = null;
const DEFAULT_SEARCH_TYPE = true; //withSubfolders
const DEFAULT_SEARCH = null;
const DEFAULT_FOLDER_PATH = [];
const DEFAULT_AUTHOR_TYPE = null;
const DEFAULT_SELECTED_ITEM = {};
const DEFAULT_FOLDER = "@my";

// TODO: add next params
// subjectGroup bool
// subjectID
// searchInContent bool

class FilesFilter {
  static getDefault(total = DEFAULT_TOTAL) {
    return new FilesFilter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, total);
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
    treeFolders = DEFAULT_FOLDER_PATH,
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
    this.treeFolders = treeFolders;
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

  toDto = () => {
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
      startIndex: this.getStartIndex(),
      page: page,
      sortby: sortBy,
      sortOrder: sortOrder,
      filterType: filterType,
      filterValue: (search ?? "").trim(),
      withSubfolders: isFilterSet,
      userIdOrGroupId,
    };

    return dtoFilter;
  };

  toUrlParams = () => {
    const dtoFilter = this.toDto();
    const str = toUrlParams(dtoFilter, true);
    return str;
  };

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
      this.treeFolders,
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
