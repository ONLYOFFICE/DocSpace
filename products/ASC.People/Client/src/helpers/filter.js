import { toUrlParams } from "../utils/converter";

class Filter {
  static getDefault(total = 0) {
    return new Filter(0, 25, total);
  }

  constructor(
    page = 0,
    pageCount = 25,
    total = 0,
    sortBy = "firstname",
    sortOrder = "ascending",
    employeeStatus = null,
    activationStatus = null
  ) {
    this.page = page;
    this.pageCount = pageCount;
    this.sortBy = sortBy;
    this.sortOrder = sortOrder;
    this.employeeStatus = employeeStatus;
    this.activationStatus = activationStatus;
    this.total = total;
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
      pageCount,
      sortBy,
      sortOrder,
      employeeStatus,
      activationStatus
    } = this;
    return {
      StartIndex: this.getStartIndex(),
      Count: pageCount,
      sortby: sortBy,
      sortorder: sortOrder,
      employeestatus: employeeStatus,
      activationstatus: activationStatus,
      //fields: "id,status,isAdmin,isOwner,isVisitor,activationStatus,userName,email,displayName,avatarSmall,listAdminModules,birthday,title,location,isLDAP,isSSO"
    };
  };

  toUrlParams = () => {
    const dtoFilter = this.toDto();
    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  clone() {
    return new Filter(
      this.page,
      this.pageCount,
      this.total,
      this.sortBy,
      this.sortOrder,
      this.employeeStatus,
      this.activationStatus
    );
  }
}

export default Filter;
