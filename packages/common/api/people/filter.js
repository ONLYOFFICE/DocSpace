import { getObjectByLocation, toUrlParams } from "../../utils";

const DEFAULT_PAGE = 0;
const DEFAULT_PAGE_COUNT = 25;
const DEFAULT_TOTAL = 0;
const DEFAULT_SORT_BY = "firstname";
const DEFAULT_SORT_ORDER = "ascending";
const DEFAULT_EMPLOYEE_STATUS = null;
const DEFAULT_ACTIVATION_STATUS = null;
const DEFAULT_ROLE = null;
const DEFAULT_SEARCH = "";
const DEFAULT_GROUP = null;
const DEFAULT_PAYMENTS = null;
const DEFAULT_ACCOUNT_LOGIN_TYPE = null;

const ACTIVE_EMPLOYEE_STATUS = 1;

const EMPLOYEE_STATUS = "employeestatus";
const ACTIVATION_STATUS = "activationstatus";
const ROLE = "employeeType";
const GROUP = "group";
const SEARCH = "search";
const SORT_BY = "sortby";
const SORT_ORDER = "sortorder";
const PAGE = "page";
const PAGE_COUNT = "pagecount";
const PAYMENTS = "payments";
const ACCOUNT_LOGIN_TYPE = "accountLoginType";

class Filter {
  static getDefault(total = DEFAULT_TOTAL) {
    return new Filter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, total);
  }

  static getFilterWithOutDisabledUser() {
    return new Filter(
      DEFAULT_PAGE,
      DEFAULT_PAGE_COUNT,
      DEFAULT_TOTAL,
      DEFAULT_SORT_BY,
      DEFAULT_SORT_ORDER,
      ACTIVE_EMPLOYEE_STATUS,
      DEFAULT_ACTIVATION_STATUS,
      DEFAULT_ROLE,
      DEFAULT_SEARCH,
      DEFAULT_GROUP,
      DEFAULT_PAYMENTS,
      DEFAULT_ACCOUNT_LOGIN_TYPE
    );
  }
  static getFilter(location) {
    if (!location) return this.getDefault();

    const urlFilter = getObjectByLocation(location);

    if (!urlFilter) return null;

    const defaultFilter = Filter.getDefault();

    const employeeStatus =
      (urlFilter[EMPLOYEE_STATUS] && +urlFilter[EMPLOYEE_STATUS]) ||
      defaultFilter.employeeStatus;
    const activationStatus =
      (urlFilter[ACTIVATION_STATUS] && +urlFilter[ACTIVATION_STATUS]) ||
      defaultFilter.activationStatus;
    const role = urlFilter[ROLE] || defaultFilter.role;
    const group = urlFilter[GROUP] || defaultFilter.group;
    const search = urlFilter[SEARCH] || defaultFilter.search;
    const sortBy = urlFilter[SORT_BY] || defaultFilter.sortBy;
    const sortOrder = urlFilter[SORT_ORDER] || defaultFilter.sortOrder;
    const page =
      (urlFilter[PAGE] && +urlFilter[PAGE] - 1) || defaultFilter.page;
    const pageCount =
      (urlFilter[PAGE_COUNT] && +urlFilter[PAGE_COUNT]) ||
      defaultFilter.pageCount;
    const payments = urlFilter[PAYMENTS] || defaultFilter.payments;
    const accountLoginType =
      urlFilter[ACCOUNT_LOGIN_TYPE] || defaultFilter.accountLoginType;

    const newFilter = new Filter(
      page,
      pageCount,
      defaultFilter.total,
      sortBy,
      sortOrder,
      employeeStatus,
      activationStatus,
      role,
      search,
      group,
      payments,
      accountLoginType
    );

    return newFilter;
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
    group = DEFAULT_GROUP,
    payments = DEFAULT_PAYMENTS,
    accountLoginType = DEFAULT_ACCOUNT_LOGIN_TYPE
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
    this.payments = payments;
    this.accountLoginType = accountLoginType;
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

  toApiUrlParams = (fields = undefined) => {
    const {
      pageCount,
      sortBy,
      sortOrder,
      employeeStatus,
      activationStatus,
      role,
      search,
      group,
      payments,
      accountLoginType,
    } = this;

    let dtoFilter = {
      StartIndex: this.getStartIndex(),
      Count: pageCount,
      sortby: sortBy,
      sortorder: sortOrder,
      employeestatus: employeeStatus,
      employeetype: role,
      activationstatus: activationStatus,
      filtervalue: (search ?? "").trim(),
      groupId: group,
      fields: fields,
      payments,
      accountLoginType,
    };

    const str = toUrlParams(dtoFilter, true);
    return str;
  };

  toUrlParams = () => {
    const {
      pageCount,
      sortBy,
      sortOrder,
      employeeStatus,
      activationStatus,
      role,
      search,
      group,
      page,
      payments,
      accountLoginType,
    } = this;

    const dtoFilter = {};

    if (employeeStatus) {
      dtoFilter[EMPLOYEE_STATUS] = employeeStatus;
    }

    if (activationStatus) {
      dtoFilter[ACTIVATION_STATUS] = activationStatus;
    }

    if (role) {
      dtoFilter[ROLE] = role;
    }

    if (group) {
      dtoFilter[GROUP] = group;
    }

    if (search) {
      dtoFilter[SEARCH] = search.trim();
    }

    if (pageCount !== DEFAULT_PAGE_COUNT) {
      dtoFilter[PAGE_COUNT] = pageCount;
    }

    dtoFilter[PAGE] = page + 1;
    dtoFilter[SORT_BY] = sortBy;
    dtoFilter[SORT_ORDER] = sortOrder;
    dtoFilter[PAYMENTS] = payments;
    dtoFilter[ACCOUNT_LOGIN_TYPE] = accountLoginType;

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
          this.group,
          this.payments,
          this.accountLoginType
        );
  }

  reset(idGroup) {
    if (idGroup) {
      return new Filter(
        0,
        this.pageCount,
        this.total,
        this.sortBy,
        this.sortOrder,
        null,
        null,
        null,
        "",
        idGroup,
        null,
        null
      );
    } else {
      this.clone(true);
    }
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
      this.pageCount === filter.pageCount &&
      this.payments === filter.payments &&
      this.accountLoginType === filter.accountLoginType;

    return equals;
  }
}

export default Filter;
