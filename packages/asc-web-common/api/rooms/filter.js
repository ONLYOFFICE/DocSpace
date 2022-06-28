import { getObjectByLocation, toUrlParams } from "../../utils";
import queryString from "query-string";
import { RoomSearchArea } from "../../constants";

const PAGE = "page";
const PAGE_COUNT = "count";
const DEFAULT_PAGE = 0;
const DEFAULT_PAGE_COUNT = 25;
const DEFAULT_TOTAL = 0;

const FILTER_VALUE = "filterValue";
const DEFAULT_FILTER_VALUE = null;

const TYPES = "types";
const DEFAULT_TYPES = null;

const SUBJECT_ID = "subjectId";
const DEFAULT_SUBJECT_ID = null;

const SEARCH_IN_CONTENT = "searchInContent";
const DEFAULT_SEARCH_IN_CONTENT = null;

const SEARCH_TYPE = "withSubfolders";
const DEFAULT_SEARCH_TYPE = true;

const SEARCH_AREA = "searchArea";
const DEFAULT_SEARCH_AREA = RoomSearchArea.Active;

const TAGS = "tags";
const DEFAULT_TAGS = null;

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

    const types = (urlFilter[TYPES] && urlFilter[TYPES]) || defaultFilter.types;

    const subjectId =
      (urlFilter[SUBJECT_ID] && +urlFilter[SUBJECT_ID]) ||
      defaultFilter.subjectId;

    const searchInContent =
      (urlFilter[SEARCH_IN_CONTENT] && +urlFilter[SEARCH_IN_CONTENT]) ||
      defaultFilter.searchInContent;

    const withSubfolders =
      (urlFilter[SEARCH_TYPE] && urlFilter[SEARCH_TYPE]) ||
      defaultFilter.withSubfolders;

    const searchArea =
      (urlFilter[SEARCH_AREA] && urlFilter[SEARCH_AREA]) ||
      defaultFilter.searchArea;

    const tags = (urlFilter[TAGS] && urlFilter[TAGS]) || defaultFilter.tags;

    const newFilter = new RoomsFilter(
      page,
      pageCount,
      defaultFilter.total,
      filterValue,
      types,
      subjectId,
      searchInContent,
      withSubfolders,
      searchArea,
      tags
    );

    return newFilter;
  }

  constructor(
    page = DEFAULT_PAGE,
    pageCount = DEFAULT_PAGE_COUNT,
    total = DEFAULT_TOTAL,
    filterValue = DEFAULT_FILTER_VALUE,
    types = DEFAULT_TYPES,
    subjectId = DEFAULT_SUBJECT_ID,
    searchInContent = DEFAULT_SEARCH_IN_CONTENT,
    withSubfolders = DEFAULT_SEARCH_TYPE,
    searchArea = DEFAULT_SEARCH_AREA,
    tags = DEFAULT_TAGS
  ) {
    this.page = page;
    this.pageCount = pageCount;
    this.total = total;
    this.filterValue = filterValue;
    this.types = types;
    this.subjectId = subjectId;
    this.searchInContent = searchInContent;
    this.withSubfolders = withSubfolders;
    this.searchArea = searchArea;
    this.tags = tags;
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
      types,
      subjectId,
      searchInContent,
      withSubfolders,
      searchArea,
      tags,
    } = this;

    const dtoFilter = {
      count: pageCount,
      page: page,
      startIndex: this.getStartIndex(),
      filterValue: (filterValue ?? "").trim(),
      types: types,
      subjectId: subjectId,
      searchInContent: searchInContent,
      withSubfolders: withSubfolders,
      searchArea: searchArea,
      tags: tags,
    };

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  toUrlParams = () => {
    const {
      page,
      pageCount,
      filterValue,
      types,
      subjectId,
      searchInContent,
      withSubfolders,
      searchArea,
      tags,
    } = this;

    const dtoFilter = {};

    if (filterValue) {
      dtoFilter[FILTER_VALUE] = filterValue;
    }

    if (types) {
      dtoFilter[TYPES] = types;
    }

    if (subjectId) {
      dtoFilter[SUBJECT_ID] = subjectId;
    }

    if (searchInContent) {
      dtoFilter[SEARCH_IN_CONTENT] = searchInContent;
    }

    if (withSubfolders) {
      dtoFilter[SEARCH_TYPE] = withSubfolders;
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

    dtoFilter[PAGE] = page + 1;

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
      this.types,
      this.subjectId,
      this.searchInContent,
      this.withSubfolders,
      this.searchArea,
      this.tags
    );
  }

  equals(filter) {
    const equals =
      this.page === filter.page &&
      this.pageCount === filter.pageCount &&
      this.filterValue === filter.filterValue &&
      this.types === filter.types &&
      this.subjectId === filter.subjectId &&
      this.searchInContent === filter.searchInContent &&
      this.withSubfolders === filter.withSubfolders &&
      this.searchArea === filter.searchArea &&
      this.tags === filter.tags;

    return equals;
  }
}

export default RoomsFilter;
