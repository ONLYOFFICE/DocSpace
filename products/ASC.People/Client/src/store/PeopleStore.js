import { action, makeObservable, observable } from "mobx";
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

  isLoading = false;

  constructor() {
    this.setGroupsStore(new GroupsStore(this));
    this.setUsersStore(new UsersStore(this));
    this.setTargetUserStore(new TargetUserStore(this));
    this.setSelectedGroupStore(new SelectedGroupStore(this));
    this.setEditingFormStore(new EditingFormStore(this));
    this.setFilterStore(new FilterStore(this));
    this.setSelectionStore(new SelectionStore(this));
    this.setHeaderMenuStore(new HeaderMenuStore(this));
    this.setAvatarEditorStore(new AvatarEditorStore(this));

    makeObservable(this, {
      isLoading: observable,
      setGroupsStore: action,
      setUsersStore: action,
      setTargetUserStore: action,
      setSelectedGroupStore: action,
      setEditingFormStore: action,
      setFilterStore: action,
      setSelectionStore: action,
      setHeaderMenuStore: action,
      setAvatarEditorStore: action,
      init: action,
    });
  }

  init = async () => {
    const re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    const match = window.location.pathname.match(re);

    if (match && match.length > 0) {
      const newFilter = getFilterByLocation(window.location);
      await this.usersStore.getUsersList(newFilter);
    }
    await this.groupsStore.getGroupList();
  };

  setGroupsStore = (store) => {
    this.groupsStore = store;
  };
  setUsersStore = (store) => {
    this.usersStore = store;
  };
  setTargetUserStore = (store) => {
    this.targetUserStore = store;
  };
  setSelectedGroupStore = (store) => {
    this.selectedGroupStore = store;
  };
  setEditingFormStore = (store) => {
    this.editingFormStore = store;
  };
  setFilterStore = (store) => {
    this.filterStore = store;
  };
  setSelectionStore = (store) => {
    this.selectionStore = store;
  };
  setHeaderMenuStore = (store) => {
    this.headerMenuStore = store;
  };
  setAvatarEditorStore = (store) => {
    this.avatarEditorStore = store;
  };
}

export default new PeopleStore();
