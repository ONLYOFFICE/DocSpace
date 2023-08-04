import { makeAutoObservable, runInAction } from "mobx";
import DefaultUserPhotoSize32PngUrl from "PUBLIC_DIR/images/default_user_photo_size_32-32.png";
import api from "@docspace/common/api";
import {
  EmployeeStatus,
  EmployeeType,
  EmployeeActivationStatus,
} from "@docspace/common/constants";
const { Filter } = api;

const fullAccessId = "00000000-0000-0000-0000-000000000000";

class UsersStore {
  peopleStore = null;
  authStore = null;

  users = [];
  providers = [];
  accountsIsIsLoading = false;
  operationRunning = false;

  constructor(peopleStore, authStore) {
    this.peopleStore = peopleStore;
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  getUsersList = async (filter, updateFilter = false) => {
    const filterData = filter ? filter.clone() : Filter.getDefault();

    if (!this.authStore.settingsStore.withPaging) {
      filterData.page = 0;
      filterData.pageCount = 100;
    }

    if (filterData.group && filterData.group === "root")
      filterData.group = undefined;

    const res = await api.people.getUserList(filterData);
    filterData.total = res.total;

    if (updateFilter) {
      this.peopleStore.filterStore.setFilterParams(filterData);
    }

    /*     this.peopleStore.filterStore.setFilterParams(filterData);
    this.peopleStore.selectedGroupStore.setSelectedGroup(
      filterData.group || "root"
    ); */

    this.setUsers(res.items);

    return Promise.resolve(res.items);
  };

  setUsers = (users) => {
    this.users = users;
  };

  setProviders = (providers) => {
    this.providers = providers;
  };

  setOperationRunning = (operationRunning) => {
    this.operationRunning = operationRunning;
  };

  employeeWrapperToMemberModel = (profile) => {
    const comment = profile.notes;
    const department = profile.groups
      ? profile.groups.map((group) => group.id)
      : [];
    const worksFrom = profile.workFrom;

    return { ...profile, comment, department, worksFrom };
  };

  createUser = async (user) => {
    const filter = this.peopleStore.filterStore.filter;
    const member = this.employeeWrapperToMemberModel(user);
    let result;
    const res = await api.people.createUser(member);
    result = res;

    await this.peopleStore.targetUserStore.getTargetUser(result.userName);
    await this.getUsersList(filter);
    return Promise.resolve(result);
  };

  removeUser = async (userId, filter) => {
    await api.people.deleteUsers(userId);
    await this.getUsersList(filter);
  };

  updateUserStatus = async (status, userIds) => {
    return api.people.updateUserStatus(status, userIds).then((users) => {
      if (users) {
        users.forEach((user) => {
          const userIndex = this.users.findIndex((x) => x.id === user.id);
          if (userIndex !== -1) this.users[userIndex] = user;
        });
      }

      return users;
    });
  };

  updateUserType = async (type, userIds, filter) => {
    let toType = 0;

    switch (type) {
      case "admin":
        toType = EmployeeType.Admin;
        break;
      case "user":
        toType = EmployeeType.Guest;
        break;
      case "collaborator":
        toType = EmployeeType.Collaborator;
        break;
      case "manager":
        toType = EmployeeType.User;
    }

    let users = null;

    try {
      users = await api.people.updateUserType(toType, userIds);
    } catch (e) {
      throw new Error(e);
    }

    await this.getUsersList(filter);

    return users;
  };

  updateProfileInUsers = async (updatedProfile) => {
    if (!this.users) {
      return this.getUsersList();
    }
    if (!updatedProfile) {
      updatedProfile = this.peopleStore.targetUserStore.targetUser;
    }
    const { userName } = updatedProfile;
    const oldProfileArr = this.users.filter((u) => u.userName === userName);
    const oldProfile = oldProfileArr[0];
    const newProfile = {};

    for (let key in oldProfile) {
      if (
        updatedProfile.hasOwnProperty(key) &&
        updatedProfile[key] !== oldProfile[key]
      ) {
        newProfile[key] = updatedProfile[key];
      } else {
        newProfile[key] = oldProfile[key];
      }
    }

    const updatedUsers = this.users.map((user) =>
      user.id === newProfile.id ? newProfile : user
    );

    this.setUsers(updatedUsers);
  };

  getStatusType = (user) => {
    if (
      user.status === EmployeeStatus.Active &&
      user.activationStatus === EmployeeActivationStatus.Activated
    ) {
      return "normal";
    } else if (
      user.status === EmployeeStatus.Active &&
      user.activationStatus === EmployeeActivationStatus.Pending
    ) {
      return "pending";
    } else if (user.status === EmployeeStatus.Disabled) {
      return "disabled";
    } else {
      return "unknown";
    }
  };

  getUserRole = (user) => {
    if (user.isOwner) return "owner";
    else if (user.isAdmin) return "admin";
    else if (user.isCollaborator) return "collaborator";
    else if (user.isVisitor) return "user";
    else return "manager";
  };

  getUserContextOptions = (isMySelf, statusType, userRole, status) => {
    const { isOwner, isAdmin, isVisitor, isCollaborator } =
      this.peopleStore.authStore.userStore.user;

    const options = [];

    switch (statusType) {
      case "normal":
      case "unknown":
        if (isMySelf) {
          options.push("profile");
        } else {
          options.push("details");
        }

        if (isAdmin || isOwner) {
          options.push("room-list");
        }

        if (isMySelf) {
          options.push("separator-1");

          options.push("change-name");
          options.push("change-email");
          options.push("change-password");

          if (isOwner) {
            options.push("separator-2");
            options.push("change-owner");
          }
        } else {
          if (
            isOwner ||
            (isAdmin &&
              (userRole === "user" ||
                userRole === "manager" ||
                userRole === "collaborator"))
          ) {
            options.push("separator-1");

            options.push("change-email");
            options.push("change-password");
            options.push("reset-auth");

            options.push("separator-2");
            options.push("disable");
          }
        }

        break;
      case "disabled":
        if (
          isOwner ||
          (isAdmin &&
            (userRole === "manager" ||
              userRole === "user" ||
              userRole === "collaborator"))
        ) {
          options.push("enable");

          options.push("details");

          options.push("separator-1");
          options.push("delete-user");
        } else {
          options.push("details");
        }

        break;

      case "pending":
        if (
          isOwner ||
          ((isAdmin || (!isVisitor && !isCollaborator)) &&
            userRole === "manager") ||
          userRole === "collaborator" ||
          userRole === "user"
        ) {
          if (isMySelf) {
            options.push("profile");
          } else {
            options.push("invite-again");
            options.push("details");
          }

          if (isAdmin || isOwner) {
            options.push("room-list");
          }

          if (
            isOwner ||
            (isAdmin &&
              (userRole === "manager" ||
                userRole === "user" ||
                userRole === "collaborator"))
          ) {
            options.push("separator-1");

            if (status === EmployeeStatus.Active) {
              options.push("disable");
            } else {
              options.push("enable");
            }
          }
        } else {
          if (isMySelf) {
            options.push("profile");
          } else {
            options.push("details");
          }

          if (isAdmin || isOwner) {
            options.push("room-list");
          }
        }

        break;
    }

    return options;
  };

  isUserSelected = (id) => {
    return this.peopleStore.selectionStore.selection.some((el) => el.id === id);
  };

  setAccountsIsIsLoading = (accountsIsIsLoading) => {
    this.accountsIsIsLoading = accountsIsIsLoading;
  };

  fetchMoreAccounts = async () => {
    if (!this.hasMoreAccounts || this.accountsIsIsLoading) return;
    // console.log("fetchMoreAccounts");

    this.setAccountsIsIsLoading(true);

    const { filter, setFilterParams } = this.peopleStore.filterStore;

    const newFilter = filter.clone();
    newFilter.page += 1;
    setFilterParams(newFilter);

    const res = await api.people.getUserList(newFilter);

    runInAction(() => {
      this.setUsers([...this.users, ...res.items]);
      this.setAccountsIsIsLoading(false);
    });
  };

  get hasMoreAccounts() {
    return this.peopleList.length < this.peopleStore.filterStore.filterTotal;
  }

  getUsersByQuery = async (query) => {
    const filter = Filter.getFilterWithOutDisabledUser();

    filter.search = query;
    filter.pageCount = 100;

    const res = await api.people.getUserList(filter);

    return res.items;
  };

  getPeopleListItem = (user) => {
    const {
      id,
      displayName,
      avatar,
      hasAvatar,
      email,
      isOwner,
      isAdmin: isAdministrator,
      isVisitor,
      isCollaborator,
      mobilePhone,
      userName,
      activationStatus,
      status,
      groups,
      title,
      firstName,
      lastName,
      isSSO,
    } = user;
    const statusType = this.getStatusType(user);
    const role = this.getUserRole(user);
    const isMySelf =
      this.peopleStore.authStore.userStore.user &&
      user.userName === this.peopleStore.authStore.userStore.user.userName;
    //const isViewerAdmin = this.peopleStore.authStore.isAdmin;

    const options = this.getUserContextOptions(
      isMySelf,
      statusType,
      role,
      status
    );

    const currentAvatar = hasAvatar ? avatar : DefaultUserPhotoSize32PngUrl;

    return {
      id,
      status,
      activationStatus,
      statusType,
      role,
      isOwner,
      isAdmin: isAdministrator,
      isCollaborator,
      isVisitor,
      displayName,
      avatar: currentAvatar,
      hasAvatar,
      email,
      userName,
      mobilePhone,
      options,
      groups,
      position: title,
      firstName,
      lastName,
      isSSO,
    };
  };

  get peopleList() {
    const list = this.users.map((user) => this.getPeopleListItem(user));

    return list;
  }

  inviteUsers = async (data) => {
    const result = await api.people.inviteUsers(data);

    return Promise.resolve(result);
  };
}

export default UsersStore;
