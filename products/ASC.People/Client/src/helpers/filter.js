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
    activationStatus = null,
    role = null,
    search = null
  ) {
    this.page = page;
    this.pageCount = pageCount;
    this.sortBy = sortBy;
    this.sortOrder = sortOrder;
    this.employeeStatus = employeeStatus;
    this.activationStatus = activationStatus;
    this.role = role;
    this.search = search;
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
      activationStatus,
      role,
      search
    } = this;

    let dtoFilter = {
      StartIndex: this.getStartIndex(),
      Count: pageCount,
      sortby: sortBy,
      sortorder: sortOrder,
      employeestatus: employeeStatus,
      activationstatus: activationStatus,
      filtervalue: search
      //fields: "id,status,isAdmin,isOwner,isVisitor,activationStatus,userName,email,displayName,avatarSmall,listAdminModules,birthday,title,location,isLDAP,isSSO"
    };

    switch (role) {
      case "admin":
        dtoFilter.isadministrator = true;
        break;
      case "user":
        dtoFilter.employeeType = 1;
        break;
      case "guest":
        dtoFilter.employeeType = 2;
        break;
      default:
        break;
    }

    return dtoFilter;
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
      this.activationStatus,
      this.role,
      this.search,
      this.total
    );
  }
}

export default Filter;
