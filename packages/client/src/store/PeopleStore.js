import InfoReactSvgUrl from "PUBLIC_DIR/images/info.react.svg?url";
import EnableReactSvgUrl from "PUBLIC_DIR/images/enable.react.svg?url";
import DisableReactSvgUrl from "PUBLIC_DIR/images/disable.react.svg?url";
import ChangeToEmployeeReactSvgUrl from "PUBLIC_DIR/images/change.to.employee.react.svg?url";
import InviteAgainReactSvgUrl from "PUBLIC_DIR/images/invite.again.react.svg?url";
import DeleteReactSvgUrl from "PUBLIC_DIR/images/delete.react.svg?url";
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
import DialogStore from "./DialogStore";

import AccountsContextOptionsStore from "./AccountsContextOptionsStore";
import {
  isMobile,
  isTablet,
  isDesktop,
} from "@docspace/components/utils/device";
import { isMobileRDD } from "react-device-detect";

import toastr from "@docspace/components/toast/toastr";
import { EmployeeStatus, Events } from "@docspace/common/constants";
import Filter from "@docspace/common/api/people/filter";

class PeopleStore {
  contextOptionsStore = null;
  authStore = null;
  dialogsStore = null;
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
  setupStore = null;
  accessRightsStore = null;
  profileActionsStore = null;
  isInit = false;
  viewAs = isMobileRDD ? "row" : "table";
  isLoadedProfileSectionBody = false;

  constructor(authStore, setupStore, accessRightsStore, dialogsStore) {
    this.authStore = authStore;
    this.groupsStore = new GroupsStore(this);
    this.usersStore = new UsersStore(this, authStore);
    this.targetUserStore = new TargetUserStore(this);
    this.selectedGroupStore = new SelectedGroupStore(this);
    this.editingFormStore = new EditingFormStore(this);
    this.filterStore = new FilterStore();
    this.selectionStore = new SelectionStore(this);
    this.headerMenuStore = new HeaderMenuStore(this);
    this.avatarEditorStore = new AvatarEditorStore(this);
    this.inviteLinksStore = new InviteLinksStore(this);
    this.dialogStore = new DialogStore();

    this.setupStore = setupStore;
    this.accessRightsStore = accessRightsStore;
    this.dialogsStore = dialogsStore;

    this.contextOptionsStore = new AccountsContextOptionsStore(this);

    makeAutoObservable(this);
  }

  get isPeoplesAdmin() {
    return this.authStore.isAdmin;
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    //this.authStore.settingsStore.setModuleInfo(config.homepage, config.id);

    await this.authStore.settingsStore.getPortalPasswordSettings();
    await this.authStore.tfaStore.getTfaType();
  };

  reset = () => {
    this.isInit = false;
  };

  resetFilter = () => {
    const filter = Filter.getDefault();

    window.DocSpace.navigate(`accounts/filter?${filter.toUrlParams()}`);
  };

  onChangeType = (e) => {
    const action = e?.action ? e.action : e?.target?.dataset?.action;

    const { getUsersToMakeEmployees } = this.selectionStore;

    this.changeType(action, getUsersToMakeEmployees);
  };

  changeType = (type, users, successCallback, abortCallback) => {
    const { setDialogData } = this.dialogStore;
    const { getUserRole } = this.usersStore;
    const event = new Event(Events.CHANGE_USER_TYPE);

    let fromType =
      users.length === 1
        ? [users[0].role ? users[0].role : getUserRole(users[0])]
        : users.map((u) => (u.role ? u.role : getUserRole(u)));

    if (users.length > 1) {
      fromType = fromType.filter(
        (item, index) => fromType.indexOf(item) === index && item !== type
      );

      if (fromType.length === 0) fromType = [fromType[0]];
    }

    if (fromType.length === 1 && fromType[0] === type) return false;

    const userIDs = users
      .filter((u) => u.role !== type)
      .map((user) => {
        return user?.id ? user.id : user;
      });

    setDialogData({
      toType: type,
      fromType,
      userIDs,
      successCallback,
      abortCallback,
    });

    window.dispatchEvent(event);

    return true;
  };

  onChangeStatus = (status) => {
    const users = [];

    if (status === EmployeeStatus.Active) {
      const { getUsersToActivate } = this.selectionStore;

      users.push(...getUsersToActivate);
    } else {
      const { getUsersToDisable } = this.selectionStore;

      users.push(...getUsersToDisable);
    }

    this.changeStatus(status, users);
  };

  changeStatus = (status, users) => {
    const { setChangeUserStatusDialogVisible, setDialogData } =
      this.dialogStore;

    const userIDs = users.map((user) => {
      return user?.id ? user.id : user;
    });

    setDialogData({ status, userIDs });

    setChangeUserStatusDialogVisible(true);
  };

  onOpenInfoPanel = () => {
    const { setIsVisible } = this.authStore.infoPanelStore;
    setIsVisible(true);
  };

  getHeaderMenu = (t) => {
    const {
      hasUsersToMakeEmployees,
      hasUsersToActivate,
      hasUsersToDisable,
      hasUsersToInvite,
      hasUsersToRemove,
      hasFreeUsers,
    } = this.selectionStore;
    const { setSendInviteDialogVisible, setDeleteDialogVisible } =
      this.dialogStore;

    const { isOwner } = this.authStore.userStore.user;

    const { isVisible } = this.authStore.infoPanelStore;

    const options = [];

    const adminOption = {
      id: "menu_change-user_administrator",
      className: "group-menu_drop-down",
      label: t("Common:DocSpaceAdmin"),
      title: t("Common:DocSpaceAdmin"),
      onClick: (e) => this.onChangeType(e),
      "data-action": "admin",
      key: "administrator",
    };
    const managerOption = {
      id: "menu_change-user_manager",
      className: "group-menu_drop-down",
      label: t("Common:RoomAdmin"),
      title: t("Common:RoomAdmin"),
      onClick: (e) => this.onChangeType(e),
      "data-action": "manager",
      key: "manager",
    };
    const userOption = {
      id: "menu_change-user_user",
      className: "group-menu_drop-down",
      label: t("Common:User"),
      title: t("Common:User"),
      onClick: (e) => this.onChangeType(e),
      "data-action": "user",
      key: "user",
    };

    isOwner && options.push(adminOption);

    options.push(managerOption);

    hasFreeUsers && options.push(userOption);

    const headerMenu = [
      {
        id: "menu-change-type",
        key: "change-user",
        label: t("ChangeUserTypeDialog:ChangeUserTypeButton"),
        disabled: !hasUsersToMakeEmployees,
        iconUrl: ChangeToEmployeeReactSvgUrl,
        withDropDown: true,
        options: options,
      },
      {
        id: "menu-info",
        key: "info",
        label: t("Common:Info"),
        disabled:
          isVisible ||
          !(isTablet() || isMobile() || isMobileRDD || !isDesktop()),
        onClick: (item) => this.onOpenInfoPanel(item),
        iconUrl: InfoReactSvgUrl,
      },
      {
        id: "menu-invite",
        key: "invite",
        label: t("Common:Invite"),
        disabled: !hasUsersToInvite,
        onClick: () => setSendInviteDialogVisible(true),
        iconUrl: InviteAgainReactSvgUrl,
      },
      {
        id: "menu-enable",
        key: "enable",
        label: t("Common:Enable"),
        disabled: !hasUsersToActivate,
        onClick: () => this.onChangeStatus(EmployeeStatus.Active),
        iconUrl: EnableReactSvgUrl,
      },
      {
        id: "menu-disable",
        key: "disable",
        label: t("PeopleTranslations:DisableUserButton"),
        disabled: !hasUsersToDisable,
        onClick: () => this.onChangeStatus(EmployeeStatus.Disabled),
        iconUrl: DisableReactSvgUrl,
      },
      {
        id: "menu-delete",
        key: "delete",
        label: t("Common:Delete"),
        disabled: !hasUsersToRemove,
        onClick: () => setDeleteDialogVisible(true),
        iconUrl: DeleteReactSvgUrl,
      },
    ];

    return headerMenu;
  };

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
  };

  setIsLoadedProfileSectionBody = (isLoadedProfileSectionBody) => {
    this.isLoadedProfileSectionBody = isLoadedProfileSectionBody;
  };
}

export default PeopleStore;
