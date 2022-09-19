import { getObjectByLocation, toUrlParams } from "../../utils";
import { RoomSearchArea } from "../../constants";

const PAGE = "page";
const PAGE_COUNT = "count";
const DEFAULT_PAGE = 0;
const DEFAULT_PAGE_COUNT = 25;
const DEFAULT_TOTAL = 0;

const FILTER_VALUE = "filterValue";
const DEFAULT_FILTER_VALUE = null;

const TYPE = "type";
const DEFAULT_TYPE = null;

const SUBJECT_ID = "subjectId";
const DEFAULT_SUBJECT_ID = null;

const SEARCH_IN_CONTENT = "searchInContent";
const DEFAULT_SEARCH_IN_CONTENT = null;

const SEARCH_TYPE = "withSubfolders";
const DEFAULT_SEARCH_TYPE = null;

const SEARCH_AREA = "searchArea";
const DEFAULT_SEARCH_AREA = RoomSearchArea.Active;

const TAGS = "tags";
const DEFAULT_TAGS = null;

const SORT_BY = "sortby";
const DEFAULT_SORT_BY = "DateAndTime";

const SORT_ORDER = "sortorder";
const DEFAULT_SORT_ORDER = "descending";

const EXCLUDE_SUBJECT = "excludeSubject";
const DEFAULT_EXCLUDE_SUBJECT = false;

const WITHOUT_TAGS = "withoutTags";
const DEFAULT_WITHOUT_TAGS = false;

class RoomsFilter {
  static getDefault(total = DEFAULT_TOTAL) {
    return new RoomsFilter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, total);
  }

  static getFilter(location) {
    if (!location) return this.getDefault();

    const urlFilter = getObjectByLocation(location);

    if (!urlFilter) return null;

    const defaultFilter = RoomsFilter.getDefault();

    const page =
      (urlFilter[PAGE] && +urlFilter[PAGE] - 1) || defaultFilter.page;

    const pageCount =
      (urlFilter[PAGE_COUNT] && +urlFilter[PAGE_COUNT]) ||
      defaultFilter.pageCount;

    const filterValue =
      (urlFilter[FILTER_VALUE] && +urlFilter[FILTER_VALUE]) ||
      defaultFilter.filterValue;

    const type = (urlFilter[TYPE] && urlFilter[TYPE]) || defaultFilter.type;

    const subjectId =
      (urlFilter[SUBJECT_ID] && urlFilter[SUBJECT_ID]) ||
      defaultFilter.subjectId;

    //TODO: remove it if search with subfolders and in content will be available
    // const searchInContent = urlFilter[SEARCH_IN_CONTENT]
    //   ? urlFilter[SEARCH_IN_CONTENT] === "true"
    //   : defaultFilter.searchInContent;

    // const withSubfolders = urlFilter[SEARCH_TYPE]
    //   ? urlFilter[SEARCH_TYPE] === "true"
    //   : defaultFilter.withSubfolders;

    const searchInContent = false;
    const withSubfolders = false;

    const searchArea =
      (urlFilter[SEARCH_AREA] && urlFilter[SEARCH_AREA]) ||
      defaultFilter.searchArea;

    const tags =
      (urlFilter[TAGS] && [...urlFilter[TAGS]]) || defaultFilter.tags;

    const sortBy = urlFilter[SORT_BY] || defaultFilter.sortBy;

    const sortOrder = urlFilter[SORT_ORDER] || defaultFilter.sortOrder;

    const excludeSubject =
      urlFilter[EXCLUDE_SUBJECT] || defaultFilter.excludeSubject;

    const withoutTags = urlFilter[WITHOUT_TAGS] || defaultFilter.withoutTags;

    const newFilter = new RoomsFilter(
      page,
      pageCount,
      defaultFilter.total,
      filterValue,
      type,
      subjectId,
      searchInContent,
      withSubfolders,
      searchArea,
      tags,
      sortBy,
      sortOrder,
      excludeSubject,
      withoutTags
    );

    return newFilter;
  }

  constructor(
    page = DEFAULT_PAGE,
    pageCount = DEFAULT_PAGE_COUNT,
    total = DEFAULT_TOTAL,
    filterValue = DEFAULT_FILTER_VALUE,
    type = DEFAULT_TYPE,
    subjectId = DEFAULT_SUBJECT_ID,
    searchInContent = DEFAULT_SEARCH_IN_CONTENT,
    withSubfolders = DEFAULT_SEARCH_TYPE,
    searchArea = DEFAULT_SEARCH_AREA,
    tags = DEFAULT_TAGS,
    sortBy = DEFAULT_SORT_BY,
    sortOrder = DEFAULT_SORT_ORDER,
    excludeSubject = DEFAULT_EXCLUDE_SUBJECT,
    withoutTags = DEFAULT_WITHOUT_TAGS
  ) {
    this.page = page;
    this.pageCount = pageCount;
    this.total = total;
    this.filterValue = filterValue;
    this.type = type;
    this.subjectId = subjectId;
    this.searchInContent = searchInContent;
    this.withSubfolders = withSubfolders;
    this.searchArea = searchArea;
    this.tags = tags;
    this.sortBy = sortBy;
    this.sortOrder = sortOrder;
    this.excludeSubject = excludeSubject;
    this.withoutTags = withoutTags;
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
      page,
      pageCount,
      filterValue,
      type,
      subjectId,
      searchInContent,
      withSubfolders,
      searchArea,
      tags,
      sortBy,
      sortOrder,
      excludeSubject,
      withoutTags,
    } = this;

    const dtoFilter = {
      count: pageCount,
      page: page,
      startIndex: this.getStartIndex(),
      filterValue: (filterValue ?? "").trim(),
      type: type,
      subjectId: subjectId,
      searchInContent: searchInContent,
      withSubfolders: withSubfolders,
      searchArea: searchArea,
      tags: tags,
      sortBy: sortBy,
      sortOrder: sortOrder,
      excludeSubject: excludeSubject,
      withoutTags: withoutTags,
    };

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  toUrlParams = () => {
    const {
      page,
      pageCount,
      filterValue,
      type,
      subjectId,
      searchInContent,
      withSubfolders,
      searchArea,
      tags,
      sortBy,
      sortOrder,
      excludeSubject,
      withoutTags,
    } = this;

    const dtoFilter = {};

    if (filterValue) {
      dtoFilter[FILTER_VALUE] = filterValue;
    }

    if (type) {
      dtoFilter[TYPE] = type;
    }

    if (subjectId) {
      dtoFilter[SUBJECT_ID] = subjectId;
    }

    if (searchInContent) {
      dtoFilter[SEARCH_IN_CONTENT] = searchInContent;
    }

    if (searchArea) {
      dtoFilter[SEARCH_AREA] = searchArea;
    }

    if (tags) {
      dtoFilter[TAGS] = tags;
    }

    if (pageCount !== DEFAULT_PAGE_COUNT) {
      dtoFilter[PAGE_COUNT] = pageCount;
    }

    if (excludeSubject) {
      dtoFilter[EXCLUDE_SUBJECT] = excludeSubject;
    }

    if (withoutTags) {
      dtoFilter[WITHOUT_TAGS] = withoutTags;
    }

    dtoFilter[PAGE] = page + 1;
    dtoFilter[SORT_BY] = sortBy;
    dtoFilter[SORT_ORDER] = sortOrder;
    dtoFilter[SEARCH_TYPE] = withSubfolders;

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  getLastPage() {
    return Math.ceil(this.total / this.pageCount) - 1;
  }

  clone() {
    return new RoomsFilter(
      this.page,
      this.pageCount,
      this.total,
      this.filterValue,
      this.type,
      this.subjectId,
      this.searchInContent,
      this.withSubfolders,
      this.searchArea,
      this.tags,
      this.sortBy,
      this.sortOrder,
      this.excludeSubject,
      this.withoutTags
    );
  }

  equals(filter) {
    const typeEqual =
      this.type.length === filter.type.length &&
      this.type.forEach((type) => filter.type.includes(type));

    const tagsEqual =
      this.tags.length === filter.tags.length &&
      this.tags.forEach((tag) => filter.tags.includes(tag));

    const equals =
      this.page === filter.page &&
      this.pageCount === filter.pageCount &&
      this.filterValue === filter.filterValue &&
      typeEqual &&
      this.subjectId === filter.subjectId &&
      this.searchInContent === filter.searchInContent &&
      this.withSubfolders === filter.withSubfolders &&
      this.searchArea === filter.searchArea &&
      tagsEqual &&
      this.sortBy === filter.sortBy &&
      this.sortOrder === filter.sortOrder &&
      this.excludeSubject === filter.excludeSubject &&
      this.withoutTags === filter.withoutTags;

    return equals;
  }
}

export default RoomsFilter;
