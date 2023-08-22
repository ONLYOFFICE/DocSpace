import { action, makeObservable, observable } from "mobx";
import Filter from "@docspace/common/api/people/filter";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";

class FilterStore {
  filter = Filter.getDefault();

  constructor() {
    makeObservable(this, {
      filter: observable,
      setFilterParams: action,
      setFilterUrl: action,
      resetFilter: action,
      setFilter: action,
    });
  }

  setFilterUrl = (filter) => {
    const urlFilter = filter.toUrlParams();
    window.history.replaceState(
      "",
      "",
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/accounts/filter?${urlFilter}`
      )
    );
  };

  setFilterParams = (data) => {
    this.setFilterUrl(data);
    this.setFilter(data);
  };

  resetFilter = () => {
    this.setFilter(Filter.getDefault());
  };

  setFilter = (filter) => {
    this.filter = filter;
  };

  get filterTotal() {
    return this.filter.total;
  }

  get isFiltered() {
    return (
      this.filter.activationStatus ||
      this.filter.employeeStatus ||
      this.filter.payments ||
      this.filter.search ||
      this.filter.role ||
      this.filter.accountLoginType
    );
  }
}

export default FilterStore;
