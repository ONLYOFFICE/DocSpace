import { makeAutoObservable } from "mobx";
import GroupsStore from "./GroupsStore";
import UsersStore from "./UsersStore";
import TargetUserStore from "./TargetUserStore";
import SelectedGroupStore from "./SelectedGroupStore";
import EditingFormStore from "./EditingFormStore";
import FilterStore from "./FilterStore";
import SelectionStore from "./SelectionPeopleStore";
import HeaderMenuStore from "./HeaderMenuStore";
import AvatarEditorStore from "./AvatarEditorStore";
import InviteLinksStore from "./InviteLinksStore";
import store from "client/store";
import DialogStore from "./DialogStore";
import LoadingStore from "./LoadingStore";
import AccountsContextOptionsStore from "./AccountsContextOptionsStore";
import { isMobile } from "react-device-detect";
import toastr from "client/toastr";
const { auth: authStore } = store;

const fullAccessId = "00000000-0000-0000-0000-000000000000";

class PeopleStore {
  contextOptionsStore = null;
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
  loadingStore = null;
  infoPanelStore = null;
  setupStore = null;
  isInit = false;
  viewAs = isMobile ? "row" : "table";

  constructor(infoPanelStore, setupStore) {
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
    this.loadingStore = new LoadingStore();
    this.infoPanelStore = infoPanelStore;
    this.setupStore = setupStore;

    this.contextOptionsStore = new AccountsContextOptionsStore(
      authStore,
      this.dialogStore,
      this.targetUserStore,
      this.usersStore,
      this.selectionStore,
      this.infoPanelStore
    );

    makeAutoObservable(this);
  }

  get isPeoplesAdmin() {
    return authStore.isAdmin;
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    //authStore.settingsStore.setModuleInfo(config.homepage, config.id);

    await authStore.settingsStore.getPortalPasswordSettings();

    this.loadingStore.setIsLoaded(true);
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

  onChangeType = (e) => {
    const action = e.target.dataset.action;

    const { getUsersToMakeEmployeesIds } = this.selectionStore;

    this.changeType(action, getUsersToMakeEmployeesIds);
  };

  changeType = (type, users, t, needClearSelection = true) => {
    const { changeAdmins } = this.setupStore;
    const { getUsersList } = this.usersStore;
    const { filter } = this.filterStore;
    const { clearSelection } = this.selectionStore;

    const userIDs = users.map((user) => {
      return user?.id ? user.id : user;
    });

    if (type === "admin") {
      changeAdmins(userIDs, fullAccessId, true).then((res) => {
        getUsersList(filter);
        needClearSelection && clearSelection();
        toastr.success(t("Settings:AdministratorsAddedSuccessfully"));
      });
    }

    if (type === "user") {
      changeAdmins(userIDs, fullAccessId, false).then((res) => {
        getUsersList(filter);
        needClearSelection && clearSelection();
        toastr.success(t("Settings:AdministratorsRemovedSuccessfully"));
      });
    }
  };

  getHeaderMenu = (t) => {
    const {
      hasUsersToMakeEmployees,
      hasUsersToActivate,
      hasUsersToDisable,
      hasUsersToInvite,
      hasUsersToRemove,
      getUsersToRemoveIds,
      selection,
    } = this.selectionStore;
    const {
      setActiveDialogVisible,
      setDisableDialogVisible,
      setSendInviteDialogVisible,
      setDeleteDialogVisible,
    } = this.dialogStore;

    const { isAdmin, isOwner } = authStore.userStore.user;

    const { setVisible, isVisible } = this.infoPanelStore;

    const options = [];

    const adminOption = {
      id: "group-menu_administrator",
      className: "group-menu_drop-down",
      label: t("Administrator"),
      title: t("Administrator"),
      onClick: this.onChangeType,
      "data-action": "admin",
      key: "administrator",
    };
    const managerOption = {
      id: "group-menu_manager",
      className: "group-menu_drop-down",
      label: t("Manager"),
      title: t("Manager"),
      onClick: this.onChangeType,
      "data-action": "manager",
      key: "manager",
    };
    const userOption = {
      id: "group-menu_user",
      className: "group-menu_drop-down",
      label: t("Common:User"),
      title: t("Common:User"),
      onClick: this.onChangeType,
      "data-action": "user",
      key: "user",
    };

    isOwner && options.push(adminOption);

    isAdmin && options.push(managerOption);

    options.push(userOption);

    const headerMenu = [
      {
        label: t("ChangeUserTypeDialog:ChangeUserTypeButton"),
        disabled: (isAdmin || isOwner) && !hasUsersToMakeEmployees,
        iconUrl: "/static/images/change.to.employee.react.svg",
        withDropDown: true,
        options: options,
      },
      {
        label: t("PeopleTranslations:Details"),
        disabled: isVisible,
        onClick: setVisible,
        iconUrl: "images/info.react.svg",
      },
      {
        label: t("Common:Invite"),
        disabled: !hasUsersToInvite,
        onClick: () => setSendInviteDialogVisible(true),
        iconUrl: "/static/images/invite.again.react.svg",
      },
      {
        label: t("Common:Enable"),
        disabled: !hasUsersToActivate,
        onClick: () => setActiveDialogVisible(true),
        iconUrl: "images/enable.react.svg",
      },
      {
        label: t("PeopleTranslations:DisableUserButton"),
        disabled: !hasUsersToDisable,
        onClick: () => setDisableDialogVisible(true),
        iconUrl: "images/disable.react.svg",
      },
      {
        label: t("Common:Delete"),
        disabled: !hasUsersToRemove,
        onClick: () => setDeleteDialogVisible(true),
        iconUrl: "/static/images/delete.react.svg",
      },
    ];

    return headerMenu;
  };

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
  };
}

export default PeopleStore;
