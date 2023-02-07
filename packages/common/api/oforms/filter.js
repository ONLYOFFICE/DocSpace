import { getObjectByLocation, toUrlParams } from "../../utils";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 150;
const DEFAULT_TOTAL = 0;

const PAGE = "pagination[page]";
const PAGE_SIZE = "pagination[pageSize]";

class OformsFilter {
  static getDefault(total = DEFAULT_TOTAL) {
    return new OformsFilter(DEFAULT_PAGE, DEFAULT_PAGE_SIZE, total);
  }

  static getFilter(location) {
    if (!location) return this.getDefault();

    const urlFilter = getObjectByLocation(location);

    if (!urlFilter) return null;

    const defaultFilter = OformsFilter.getDefault();

    const page =
      (urlFilter[PAGE] && +urlFilter[PAGE] - 1) || defaultFilter.page;
    const pageSize =
      (urlFilter[PAGE_SIZE] && +urlFilter[PAGE_SIZE]) || defaultFilter.pageSize;

    const newFilter = new OformsFilter(page, pageSize, defaultFilter.total);

    return newFilter;
  }

  constructor(
    page = DEFAULT_PAGE,
    pageSize = DEFAULT_PAGE_SIZE,
    total = DEFAULT_TOTAL
  ) {
    this.page = page;
    this.pageSize = pageSize;
    this.total = total;
  }

  getStartIndex = () => {
    return this.page * this.pageSize;
  };

  toUrlParams = () => {
    const { pageSize, page } = this;

    const dtoFilter = {};

    if (pageSize !== PAGE_SIZE) {
      dtoFilter[PAGE_SIZE] = pageSize;
    }

    dtoFilter[PAGE] = page;

    const str = toUrlParams(dtoFilter, true);

    return str;
  };

  toApiUrlParams = () => {
    const { pageSize } = this;

    let dtoFilter = {
      StartIndex: this.getStartIndex(),
      Count: pageSize,
    };

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  clone() {
    return new OformsFilter(this.page, this.pageSize, this.total);
  }
}

export default OformsFilter;
