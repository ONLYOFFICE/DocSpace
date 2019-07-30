import { toUrlParams } from "../utils/converter";

class Filter {
  static getDefault(total = 0) {
    return new Filter(0, 25, total);
  }

  static nextPage(filter) {
    const {
      page,
      pageCount,
      total,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    } = filter;

    let nextPage = page + 1;

    const nextFilter = new Filter(
      nextPage,
      pageCount,
      total,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    );

    return nextFilter;
  }

  static prevPage(filter) {
    const {
      page,
      pageCount,
      total,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    } = filter;

    let prevPage = page - 1;

    const nextFilter = new Filter(
      prevPage,
      pageCount,
      total,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    );

    return nextFilter;
  }

  constructor(
    page = 0,
    pageCount = 25,
    total = 0,
    sortby = "firstname",
    sortorder = "ascending",
    employeestatus = null,
    activationstatus = null
  ) {
    this.page = page;
    this.startIndex = page * pageCount;
    this.pageCount = pageCount;
    this.sortby = sortby;
    this.sortorder = sortorder;
    this.employeestatus = employeestatus;
    this.activationstatus = activationstatus;
    this.total = total;
    this.hasNext = total - this.startIndex > pageCount;
    this.hasPrev = page > 0;
  }

  toDto = () => {
    const {
      startIndex,
      pageCount,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    } = this;
    return {
      StartIndex: startIndex,
      Count: pageCount,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    };
  };

  toUrlParams = () => {
    return toUrlParams(this.toDto(), true);
  };
}

export default Filter;
