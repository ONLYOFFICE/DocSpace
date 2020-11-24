import { toUrlParams } from "../../utils";

const DEFAULT_PAGE = 0;
const DEFAULT_PAGE_COUNT = 25;
const DEFAULT_TOTAL = 0;
const DEFAULT_SORT_BY = "firstname";
const DEFAULT_SORT_ORDER = "ascending";
const DEFAULT_EMPLOYEE_STATUS = null;
const DEFAULT_ACTIVATION_STATUS = null;
const DEFAULT_ROLE = null;
const DEFAULT_SEARCH = null;
const DEFAULT_GROUP = null;

class Filter {
  static getDefault(total = DEFAULT_TOTAL) {
    return new Filter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, total);
  }

  constructor(
    page = DEFAULT_PAGE,
    pageCount = DEFAULT_PAGE_COUNT,
    total = DEFAULT_TOTAL,
    sortBy = DEFAULT_SORT_BY,
    sortOrder = DEFAULT_SORT_ORDER,
    employeeStatus = DEFAULT_EMPLOYEE_STATUS,
    activationStatus = DEFAULT_ACTIVATION_STATUS,
    role = DEFAULT_ROLE,
    search = DEFAULT_SEARCH,
    group = DEFAULT_GROUP
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
    this.group = group;
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
      search,
      group,
    } = this;

    let dtoFilter = {
      StartIndex: this.getStartIndex(),
      Count: pageCount,
      sortby: sortBy,
      sortorder: sortOrder,
      employeestatus: employeeStatus,
      activationstatus: activationStatus,
      filtervalue: (search ?? "").trim(),
      groupId: group,
      fields:
        "id,status,isAdmin,isOwner,isVisitor,activationStatus,userName,email,mobilePhone,displayName,avatar,listAdminModules,birthday,title,location,isLDAP,isSSO,groups",
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

  clone(onlySorting) {
    return onlySorting
      ? new Filter(
          DEFAULT_PAGE,
          DEFAULT_PAGE_COUNT,
          DEFAULT_TOTAL,
          this.sortBy,
          this.sortOrder
        )
      : new Filter(
          this.page,
          this.pageCount,
          this.total,
          this.sortBy,
          this.sortOrder,
          this.employeeStatus,
          this.activationStatus,
          this.role,
          this.search,
          this.group
        );
  }

  equals(filter) {
    const equals =
      this.employeeStatus === filter.employeeStatus &&
      this.activationStatus === filter.activationStatus &&
      this.role === filter.role &&
      this.group === filter.group &&
      this.search === filter.search &&
      this.sortBy === filter.sortBy &&
      this.sortOrder === filter.sortOrder &&
      this.page === filter.page &&
      this.pageCount === filter.pageCount;

    return equals;
  }
}

export default Filter;
