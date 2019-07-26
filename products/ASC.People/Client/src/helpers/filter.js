import { objectToUrlQuery } from "../utils/converter";

class Filter {
  static getDefault() {
    return new Filter(0, 25);
  }

  constructor(
    page = 0,
    pageCount = 25,
    sortby = "firstname",
    sortorder = "ascending",
    employeestatus = null,
    activationstatus = null
  ) {
    this.StartIndex = (page > 0 ? page - 1 : page) * pageCount;
    this.Count = pageCount;
    this.sortby = sortby;
    this.sortorder = sortorder;
    this.employeestatus = employeestatus;
    this.activationstatus = activationstatus;
  }

  toJSON = () => {
    let {
      StartIndex,
      Count,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    } = this;
    return {
      StartIndex,
      Count,
      sortby,
      sortorder,
      employeestatus,
      activationstatus
    };
  };

  toUrlParams = () => {
    return objectToUrlQuery(this.toJSON(), true);
  };
}

export default Filter;
