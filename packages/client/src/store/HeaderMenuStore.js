import { computed, makeObservable } from "mobx";
import { getUserStatus } from "SRC_DIR/helpers/people-helpers";

class HeaderMenuStore {
  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      isHeaderVisible: computed,
      isHeaderIndeterminate: computed,
      isHeaderChecked: computed,
    });
  }

  get cbMenuItems() {
    const { users } = this.peopleStore.usersStore;

    let cbMenu = ["all"];

    for (let user of users) {
      const status = getUserStatus(user);
      switch (status) {
        case "active":
          cbMenu.push(status);
          break;
        case "pending":
          cbMenu.push(status);
          break;
        case "disabled":
          cbMenu.push(status);
          break;
      }
    }

    cbMenu = cbMenu.filter((item, index) => cbMenu.indexOf(item) === index);

    return cbMenu;
  }

  getCheckboxItemLabel = (t, item) => {
    switch (item) {
      case "active":
        return t("Common:Active");
      case "pending":
        return t("PeopleTranslations:PendingTitle");
      case "disabled":
        return t("PeopleTranslations:DisabledEmployeeStatus");
      case "all":
        return t("Files:All");
    }
  };

  get isHeaderVisible() {
    const { selection } = this.peopleStore.selectionStore;
    return selection.length > 0;
  }

  get isHeaderIndeterminate() {
    const { selection } = this.peopleStore.selectionStore;
    const { users } = this.peopleStore.usersStore;

    return (
      this.isHeaderVisible &&
      !!selection.length &&
      selection.length < users.length
    );
  }

  get isHeaderChecked() {
    const { selection } = this.peopleStore.selectionStore;
    const { users } = this.peopleStore.usersStore;

    return this.isHeaderVisible && selection.length === users.length;
  }
}

export default HeaderMenuStore;
