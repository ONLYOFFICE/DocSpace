import { action, computed, makeObservable, observable } from "mobx";
import GroupsStore from "./GroupsStore";
import UsersStore from "./UsersStore";
import { getFilterByLocation } from "../helpers/converters";
import config from "../../package.json";
import TargetUserStore from "./TargetUserStore";
import SelectedGroupStore from "./SelectedGroupStore";
import EditingFormStore from "./EditingFormStore";
import FilterStore from "./FilterStore";
import SelectionStore from "./SelectionStore";
import HeaderMenuStore from "./HeaderMenuStore";
import AvatarEditorStore from "./AvatarEditorStore";
import InviteLinksStore from "./InviteLinksStore";
import store from "studio/store";
import DialogStore from "./DialogStore";
const { auth: authStore } = store;

class PeopleStore {
  groupsStore = null;
  usersStore = null;
  targetUserStore = null;
  selectedGroupStore = null;
  editingFormStore = null;
  filterStore = null;
  selectionStore = null;
  headerMenuStore = null;
  avatarEditorStore = null;
  inviteLinksStore = null;
  dialogStore = null;

  isLoading = false;
  isLoaded = false;

  constructor() {
    this.groupsStore = new GroupsStore(this);
    this.usersStore = new UsersStore(this);
    this.targetUserStore = new TargetUserStore(this);
    this.selectedGroupStore = new SelectedGroupStore(this);
    this.editingFormStore = new EditingFormStore(this);
    this.filterStore = new FilterStore();
    this.selectionStore = new SelectionStore(this);
    this.headerMenuStore = new HeaderMenuStore(this);
    this.avatarEditorStore = new AvatarEditorStore(this);
    this.inviteLinksStore = new InviteLinksStore(this);
    this.dialogStore = new DialogStore();

    makeObservable(this, {
      isLoading: observable,
      isLoaded: observable,
      setIsLoading: action,
      setIsLoaded: action,
      init: action,
      isPeoplesAdmin: computed,
      resetFilter: action,
    });
  }

  get isPeoplesAdmin() {
    return (
      authStore.userStore.user &&
      authStore.userStore.user.listAdminModules.includes("people")
    );
  }

  init = async () => {
    const re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    const match = window.location.pathname.match(re);

    authStore.settingsStore.setModuleInfo(
      config.homepage,
      "f4d98afd-d336-4332-8778-3c6945c81ea0"
    );

    if (match && match.length > 0) {
      const newFilter = getFilterByLocation(window.location);
      await this.usersStore.getUsersList(newFilter);
    }

    await this.groupsStore.getGroupList();
    await authStore.settingsStore.getPortalPasswordSettings();

    this.setIsLoaded(true);
  };

  setIsLoading = (loading) => {
    this.isLoading = loading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  resetFilter = (withoutGroup = false) => {
    const { filter } = this.filterStore;
    const { getUsersList } = this.usersStore;
    let newFilter;

    if (withoutGroup) {
      const { group } = filter;
      newFilter = filter.reset(group);
    } else {
      newFilter = filter.clone(true);
    }

    return getUsersList(newFilter);
  };
}

export default PeopleStore;
