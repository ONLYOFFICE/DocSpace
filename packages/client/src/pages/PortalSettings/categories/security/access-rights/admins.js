import React, { Component } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import styled from "styled-components";
import HelpButton from "@docspace/components/help-button";
import api from "@docspace/common/api";
const { Filter } = api;

import ToggleButton from "@docspace/components/toggle-button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Avatar from "@docspace/components/avatar";
import Row from "@docspace/components/row";
import RowContainer from "@docspace/components/row-container";
import Link from "@docspace/components/link";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";
import SearchInput from "@docspace/components/search-input";
import RequestLoader from "@docspace/components/request-loader";
import Loaders from "@docspace/common/components/Loaders";
import EmptyScreenContainer from "@docspace/components/empty-screen-container";
import PeopleSelector from "@docspace/client/src/components/PeopleSelector";

import { inject, observer } from "mobx-react";

import { getUserRole } from "@docspace/client/src/helpers/people-helpers";

import isEmpty from "lodash/isEmpty";

import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@docspace/common/constants";

import { tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

const getUserStatus = (user) => {
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

const StyledModalBody = styled.div`
  display: flex;
  flex-direction: column;

  .avatar {
    margin-right: 16px;
    min-width: 48px;
  }

  .toggle-btn {
    position: relative;
  }

  .user-info {
    display: flex;
    align-items: center;
    margin-bottom: 32px;

    .user-info-wrapper {
      flex: 1 1 auto;
      max-width: 100%;
      min-width: 0;
    }
  }

  .full-access-wrapper {
    display: flex;
    justify-content: space-between;
  }

  .help-button-wrapper {
    display: flex;
    align-items: center;

    &.modules {
      margin-top: 12px;
    }

    .place-left {
      margin-left: 0;
    }
  }

  p {
    margin-right: 8px;
  }

  .modules-settings {
    margin-top: 12px;
  }

  .setting-wrapper {
    display: flex;
    justify-content: space-between;
    margin-top: 12px;
  }

  .listOfFullAccess {
    padding-left: 10px;
    position: relative;
    margin-left: 8px;
    text-transform: lowercase;

    &:before {
      content: "";
      width: 3px;
      height: 3px;
      background-color: ${(props) =>
        props.theme.client.settings.security.admins.backgroundColor};
      border-radius: 2px;
      position: absolute;
      left: 0;
      top: 0;
      bottom: 0;
      margin: auto;
    }
  }
`;

StyledModalBody.defaultProps = { theme: Base };

const ToggleContentContainer = styled.div`
  .buttons_container {
    display: flex;
    @media (max-width: 1024px) {
      display: block;
    }
  }
  .toggle_content {
    margin-bottom: 24px;
  }

  .wrapper {
    margin-top: 8px;
  }

  .remove_icon {
    margin-left: 70px;
    @media (max-width: 576px) {
      margin-left: 0px;
    }
  }

  .people-admin_container {
    margin-right: 16px;
    position: relative;

    @media (max-width: 1024px) {
      margin-bottom: 8px;
    }
  }

  .full-admin_container {
    position: relative;
  }

  *,
  *::before,
  *::after {
    box-sizing: border-box;
  }
  .nameAndStatus {
    display: flex;
    align-items: center;
    height: 100%;

    .statusIcon {
      margin-left: 5px;
    }
  }

  .rowMainContainer {
    height: auto;
  }

  .userRole {
    text-transform: capitalize;
    font-size: 12px;
    color: ${(props) => props.theme.client.settings.security.admins.roleColor};
  }

  .styled-element {
    cursor: pointer; //TODO:  temporary solution. Layouts for desktops will be different
  }

  .row_content {
    justify-content: space-between;
    align-items: center;
    cursor: pointer; //TODO:  temporary solution. Layouts for desktops will be different

    .userData {
      flex: 1 1 auto;
      max-width: 100%;
      min-width: 0;
    }
  }

  .iconsWrapper {
    display: flex;
  }

  .actionIconsWrapper {
    display: flex;
    align-items: center;

    .fullAccessWrapper {
      display: flex;
      align-items: center;
      padding: 0px 5px;
      background-color: ${(props) =>
        props.theme.client.settings.security.admins.backgroundColorWrapper};
      border-radius: 9px;
    }

    .iconWrapper {
      display: inline-block;
      margin-right: 15px;

      &:last-child {
        margin-right: 0;
      }
    }
  }

  @media ${tablet} {
    .row_content {
      flex-direction: row;
      align-items: center;
      height: 48px;

      .userData {
        max-width: 100%;
      }
    }

    .actionIconsWrapper {
      .fullAccessWrapper {
        margin-right: 0;
      }

      .iconWrapper {
        margin-right: 12px;
      }
    }

    .wrapper {
      #rowContainer {
        .styled-element {
          margin-right: 8px;
        }
      }
    }
  }
`;

ToggleContentContainer.defaultProps = { theme: Base };

const fullAccessId = "00000000-0000-0000-0000-000000000000";

class PortalAdmins extends Component {
  constructor(props) {
    super(props);

    this.state = {
      showFullAdminSelector: false,
      isLoading: false,
      showLoader: true,
      selectedOptions: [],
      searchValue: "",
      selectedUser: null,
      request: {
        pending: false,
        activeRequestId: null,
      },
    };
  }

  async componentDidMount() {
    const {
      admins,
      setAddUsers,
      setRemoveAdmins,
      updateListAdmins,
      setFilterParams,
      history,
    } = this.props;

    const { location } = history;
    const { pathname } = location;

    let filter = {};

    if (pathname.indexOf("/admins/filter") > -1) {
      filter = Filter.getFilter(location);
      filter.page += 1;
    }

    const currentFilter =
      Object.keys(filter).length > 0 ? filter : this.props.filter;

    setAddUsers(this.addUsers);
    setRemoveAdmins(this.removeAdmins);

    if (isEmpty(admins, true)) {
      this.setIsLoading(true);
      try {
        await updateListAdmins(currentFilter, true);
      } catch (error) {
        toastr.error(error);
      }
      this.setIsLoading(false);
    } else {
      setFilterParams(currentFilter);
    }
  }

  setIsLoading = (isLoading) => {
    this.setState({
      isLoading,
    });
  };

  componentWillUnmount() {
    const { setAddUsers, setRemoveAdmins, setSelected } = this.props;
    setAddUsers("");
    setRemoveAdmins("");
    setSelected("none");
  }

  onAdminsFilter = () => {
    const { filter } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.role = "admin";

    return newFilter;
  };

  onCancelSelector = () => {
    const { toggleSelector } = this.props;

    toggleSelector(false);
  };

  onSelect = (items) => {
    const { toggleSelector } = this.props;

    toggleSelector(false);
    this.addUsers(items);
  };

  findAdminById = (admin) => {
    if (admin.id === this.id) return true;
  };

  addUsers = (users) => {
    const { t, admins, changeAdmins, updateListAdmins, filter } = this.props;

    if (!users && users.length === 0) return;

    const userIds = [];

    users.forEach((user) => {
      let isNewAdmin = admins.every((admin) => {
        return admin.id !== user.key;
      });

      if (user.isAdmin) isNewAdmin = false;

      if (isNewAdmin) userIds.push(user.key);
    });

    if (!userIds.length > 0) return;

    changeAdmins(userIds, fullAccessId, true).then(async () => {
      try {
        await updateListAdmins(filter, true);
        toastr.success(t("AdministratorsAddedSuccessfully"));
      } catch (e) {
        console.log(e);
      }
    });
  };

  removeAdmins = () => {
    const {
      selection,
      setSelected,
      t,
      changeAdmins,
      updateListAdmins,
      filter,
    } = this.props;

    if (!selection && selection.length === 0) return;
    const userIds = selection.map((user) => {
      return user.id;
    });

    changeAdmins(userIds, fullAccessId, false).then(async () => {
      await updateListAdmins(filter, true);
      setSelected("none");
      toastr.success(t("AdministratorsRemovedSuccessfully"));
    });
  };

  filterNewAdmins = (admins, newAdmins) => {
    admins.forEach((admin) => {
      for (let t = 0; t < newAdmins.length; t++) {
        if (admin.id === newAdmins[t].id) {
          newAdmins.splice(t, 1);
          break;
        }
      }
    });
  };

  fullAccessTooltip = () => {
    const { t } = this.props;
    return (
      <>
        <Text>{t("FullAccessTooltip")}</Text>
        <br />
        <Text className="listOfFullAccess">{t("ChangeOwner")}</Text>
        <Text className="listOfFullAccess">
          {t("DeactivateOrDeletePortal")}
        </Text>
      </>
    );
  };

  modulesTooltip = () => {
    const { t } = this.props;
    return (
      <div>
        <Text>{t("DocumentsAdministratorsCan")}</Text>
        <br />
        <Text>{t("PeopleAdministratorsCan")}</Text>
      </div>
    );
  };

  createNewListAdminModules = (isAdmin, listAdminModules, moduleName) => {
    let newListAdminModules = listAdminModules ? listAdminModules.slice() : [];

    if (!isAdmin) {
      newListAdminModules.push(moduleName);
    } else {
      newListAdminModules = listAdminModules.filter((module) => {
        return module !== moduleName;
      });
    }
    return newListAdminModules;
  };

  onContentRowSelect = (checked, user) => {
    const { selectUser, deselectUser } = this.props;

    if (checked) {
      selectUser(user);
    } else {
      deselectUser(user);
    }
  };

  onSearchChange = (value) => {
    if (this.state.searchValue === value) return false;

    this.setState({
      searchValue: value,
    });
  };

  onRowClick = (user) => {
    this.setState({
      selectedUser: user,
    });
    this.openModal();
  };

  closeModal = () => {
    this.setState({
      modalIsVisible: false,
    });
  };

  openModal = () => {
    this.setState({
      modalIsVisible: true,
    });
  };

  onFullAccessClick = (access) => {
    const { selectedUser, request } = this.state;
    const { changeAdmins, admins, setAdmins, modules } = this.props;

    if (request.pending) return;
    this.setActiveRequestId(fullAccessId);
    setTimeout(() => {
      if (!this.state.request.activeRequestId) return;
      this.setRequestPending();
    }, 100);

    changeAdmins([selectedUser.id], fullAccessId, access)
      .then(() => {
        let updatedAdmin = {};
        const updatedAdmins = admins.map((admin) => {
          if (admin.id === selectedUser.id) {
            admin.isAdmin = access;
            if (access) {
              admin.listAdminModules = modules.map((module) => {
                return module.appName;
              });
            } else {
              delete admin.listAdminModules;
            }

            updatedAdmin = admin;
          }
          return admin;
        });
        this.setState({
          selectedUser: updatedAdmin,
        });
        setAdmins(updatedAdmins);
      })
      .catch((e) => {
        console.log(e);
      })
      .finally(() => {
        this.clearRequest();
      });
  };

  setActiveRequestId = (id) => {
    this.setState({
      request: Object.assign(this.state.request, { activeRequestId: id }),
    });
  };

  setRequestPending = () => {
    this.setState({
      request: Object.assign(this.state.request, {
        pending: true,
      }),
    });
  };

  clearRequest = () => {
    this.setState({
      request: {
        pending: false,
        activeRequestId: null,
      },
    });
  };

  toggleIsLoading = (id) => {
    const { request } = this.state;
    if (request.activeRequestId === id && request.pending) return true;
  };

  onModuleToggle = (module, access) => {
    const { selectedUser, request } = this.state;
    const { changeAdmins, admins, setAdmins, modules } = this.props;

    if (request.pending) return;
    this.setActiveRequestId(module.id);
    setTimeout(() => {
      if (!this.state.request.activeRequestId) return;
      this.setRequestPending();
    }, 100);

    changeAdmins([selectedUser.id], module.id, access)
      .then(() => {
        let updatedAdmin = {};
        const updatedAdmins = admins.map((admin) => {
          if (admin.id === selectedUser.id) {
            updatedAdmin = { ...admin };
            if (!updatedAdmin.listAdminModules) {
              updatedAdmin.listAdminModules = [module.appName];
            } else if (!access) {
              const moduleIndex = updatedAdmin.listAdminModules.findIndex(
                (adminModule) => {
                  return module.appName === adminModule;
                }
              );

              updatedAdmin.listAdminModules.splice(moduleIndex, 1);
            } else if (access) {
              const newModuleList = getNewModulesList(
                module,
                updatedAdmin.listAdminModules,
                modules
              );

              updatedAdmin.listAdminModules = newModuleList;
            }
            return updatedAdmin;
          }
          return admin;
        });

        this.setState({
          selectedUser: updatedAdmin,
        });
        setAdmins(updatedAdmins);
      })
      .catch((e) => {
        console.log(e);
      })
      .finally(() => {
        this.clearRequest();
      });
  };

  isModuleAdmin = (user, moduleName) => {
    let isModuleAdmin = false;

    if (!user.listAdminModules) return false;

    for (let i = 0; i < user.listAdminModules.length; i++) {
      if (user.listAdminModules[i] === moduleName) {
        isModuleAdmin = true;
        break;
      }
    }

    return isModuleAdmin;
  };

  getFilteredAdmins = (admins, searchValue) => {
    const filteredAdmins = admins.filter((admin) => {
      if (
        admin.displayName.toLowerCase().indexOf(searchValue.toLowerCase()) !==
        -1
      )
        return true;
      return false;
    });

    return filteredAdmins;
  };

  onToggleSelector = (isOpen = !this.props.selectorIsOpen) => {
    const { toggleSelector } = this.props;
    toggleSelector(isOpen);
  };

  render() {
    const {
      t,
      admins,
      isUserSelected,
      selectorIsOpen,
      groupsCaption,
      modules,
      theme,
    } = this.props;
    const {
      isLoading,
      showLoader,
      searchValue,
      modalIsVisible,
      selectedUser,
    } = this.state;

    const filteredAdmins = searchValue
      ? this.getFilteredAdmins(admins, searchValue)
      : admins;

    const selectedAdmins = admins.map((admin) => {
      const groups = admin?.groups?.map((group) => group.id);

      return {
        label: admin.displayName,
        avatarUrl: admin.avatar,
        groups,
        id: admin.id,
        key: admin.id,
      };
    });

    const fullAccessIsLoading = this.toggleIsLoading(fullAccessId);

    return (
      <>
        {isLoading ? (
          <Loaders.Rows isRectangle={false} />
        ) : (
          <>
            <ToggleContentContainer>
              <SearchInput
                className="filter_container"
                placeholder={t("Common:Search")}
                onChange={this.onSearchChange}
                onClearSearch={this.onSearchChange}
                value={searchValue}
              />
              {selectorIsOpen && (
                <PeopleSelector
                  isMultiSelect={true}
                  displayType="aside"
                  isOpen={!!selectorIsOpen}
                  onSelect={this.onSelect}
                  groupsCaption={groupsCaption}
                  onCancel={this.onCancelSelector}
                  headerLabel={t("AddAdmins")}
                  onArrowClick={this.onCancelSelector}
                  showCounter
                  selectedOptions={selectedAdmins}
                />
              )}
              {selectedUser && (
                <ModalDialog
                  visible={modalIsVisible}
                  zIndex={310}
                  onClose={this.closeModal}
                >
                  <ModalDialog.Header>{t("AccessSettings")}</ModalDialog.Header>
                  <ModalDialog.Body>
                    <StyledModalBody>
                      <div className="user-info">
                        <Avatar
                          size="medium"
                          role={getUserRole(selectedUser)}
                          userName={selectedUser.displayName}
                          source={selectedUser.avatar}
                          className="avatar"
                        />
                        <div className="user-info-wrapper">
                          <Text
                            color={theme.client.settings.security.admins.color}
                            fontWeight={600}
                            fontSize="19px"
                            truncate={true}
                          >
                            {selectedUser.displayName}
                          </Text>
                          {selectedUser.department && (
                            <Text
                              color={
                                theme.client.settings.security.admins
                                  .departmentColor
                              }
                              fontWeight={400}
                              fontSize="13px"
                              truncate={true}
                            >
                              {selectedUser.department}
                            </Text>
                          )}
                        </div>
                      </div>
                      <div>
                        <div className="full-access-wrapper">
                          <div className="help-button-wrapper">
                            <Text as="p" fontWeight={600} fontSize="15px">
                              {t("Common:FullAccess")}
                            </Text>
                            <HelpButton
                              displayType="dropdown"
                              place="top"
                              offsetRight={0}
                              tooltipContent={this.fullAccessTooltip()}
                              tooltipColor={
                                theme.client.settings.security.admins
                                  .tooltipColor
                              }
                            />
                          </div>
                          <ToggleButton
                            className="toggle-btn"
                            isChecked={
                              fullAccessIsLoading
                                ? !selectedUser.isAdmin
                                : selectedUser.isAdmin
                            }
                            onChange={() =>
                              this.onFullAccessClick(!selectedUser.isAdmin)
                            }
                            isLoading={fullAccessIsLoading}
                            isDisabled={false}
                          />
                        </div>
                        {modules && modules.length > 0 && (
                          <>
                            <div className="help-button-wrapper modules">
                              <Text as="p" fontWeight={600} fontSize="15px">
                                {t("AdminInModules")}
                              </Text>
                              <HelpButton
                                displayType="dropdown"
                                place="bottom"
                                offsetRight={0}
                                tooltipContent={this.modulesTooltip()}
                                tooltipColor={
                                  theme.client.settings.security.admins.color
                                }
                              />
                            </div>

                            <div className="modules-settings">
                              <div className="module-settings">
                                {modules.map((module) => {
                                  const isModuleAdmin = this.isModuleAdmin(
                                    selectedUser,
                                    module.appName
                                  );

                                  const toggleIsLoading = this.toggleIsLoading(
                                    module.id
                                  );

                                  return (
                                    <div
                                      key={module.appName}
                                      className="setting-wrapper"
                                    >
                                      <Text fontWeight={400} fontSize="13px">
                                        {module.title}
                                      </Text>
                                      <ToggleButton
                                        className="toggle-btn"
                                        isChecked={
                                          toggleIsLoading
                                            ? !isModuleAdmin
                                            : isModuleAdmin
                                        }
                                        isLoading={toggleIsLoading}
                                        onChange={() =>
                                          this.onModuleToggle(
                                            module,
                                            !isModuleAdmin
                                          )
                                        }
                                        isDisabled={selectedUser.isAdmin}
                                      />
                                    </div>
                                  );
                                })}
                              </div>
                            </div>
                          </>
                        )}
                      </div>
                    </StyledModalBody>
                  </ModalDialog.Body>
                </ModalDialog>
              )}

              {filteredAdmins.length > 0 ? (
                <>
                  <div className="wrapper">
                    <RowContainer useReactWindow={false}>
                      {filteredAdmins.map((user) => {
                        const userRole = getUserRole(user);

                        if (userRole === "owner") return;
                        const element = (
                          <Avatar
                            size="min"
                            role={userRole}
                            userName={user.displayName}
                            source={user.avatar}
                          />
                        );

                        const nameColor =
                          getUserStatus(user) === "pending"
                            ? theme.client.settings.security.admins
                                .pendingNameColor
                            : theme.client.settings.security.admins.nameColor;

                        const checked = isUserSelected(user.id);

                        return (
                          <Row
                            key={user.id}
                            status={user.status}
                            onSelect={this.onContentRowSelect}
                            data={user}
                            element={element}
                            checkbox={true}
                            checked={checked}
                            contextButtonSpacerWidth={"0px"}
                            onRowClick={() => this.onRowClick(user)}
                          >
                            <>
                              <div className="userData">
                                <div className="nameAndStatus">
                                  <Text
                                    fontSize="15px"
                                    fontWeight="600"
                                    color={nameColor}
                                    truncate={true}
                                  >
                                    {user.displayName}
                                  </Text>
                                </div>
                              </div>
                              <div className="actionIconsWrapper">
                                {user.isAdmin ? (
                                  <div className="fullAccessWrapper">
                                    <Text
                                      truncate={true}
                                      color={
                                        theme.client.settings.security.admins
                                          .textColor
                                      }
                                      fontSize="9px"
                                      fontWeight={600}
                                    >
                                      {t("Common:FullAccess")}
                                    </Text>
                                  </div>
                                ) : user.listAdminModules ? (
                                  <div className="iconsWrapper">
                                    {user.listAdminModules.map((moduleName) => {
                                      const { modules } = this.props;
                                      const module = modules.find((module) => {
                                        return module.appName === moduleName;
                                      });

                                      return (
                                        <div
                                          key={`key-${moduleName}`}
                                          className="iconWrapper"
                                        >
                                          <IconButton
                                            iconName={module.iconUrl}
                                            size={14}
                                            color={
                                              theme.client.settings.security
                                                .admins.iconColor
                                            }
                                            isfill={true}
                                            isClickable={true} // TODO: temporary solution for desktop. Layouts for desktops will be different
                                          />
                                        </div>
                                      );
                                    })}
                                  </div>
                                ) : (
                                  <></>
                                )}
                              </div>
                            </>
                          </Row>
                        );
                      })}
                    </RowContainer>
                  </div>
                </>
              ) : searchValue.length > 0 ? (
                <EmptyScreenContainer
                  imageSrc="products/people/images/empty_screen_filter.png"
                  imageAlt="Empty Screen Filter image"
                  headerText={t("Common:NotFoundTitle")}
                  descriptionText={t("NotFoundDescription")}
                  buttons={
                    <>
                      <Link
                        type="action"
                        isHovered={true}
                        onClick={this.onSearchChange.bind(this, "")}
                      >
                        {t("Common:ClearButton")}
                      </Link>
                    </>
                  }
                />
              ) : (
                <EmptyScreenContainer
                  imageSrc="images/people_logolarge.png"
                  imageAlt="Empty Screen Admins image"
                  headerText={t("NoAdmins")}
                  descriptionText={t("NoAdminsDescription")}
                  buttons={
                    <>
                      <Link
                        type="action"
                        isHovered={true}
                        onClick={this.onToggleSelector}
                      >
                        {t("AddAdmins")}
                      </Link>
                    </>
                  }
                />
              )}
            </ToggleContentContainer>
          </>
        )}
      </>
    );
  }
}

PortalAdmins.defaultProps = {
  admins: [],
  productId: "",
  owner: {},
};

PortalAdmins.propTypes = {
  admins: PropTypes.arrayOf(PropTypes.object),
  productId: PropTypes.string,
  owner: PropTypes.object,
  setAddUsers: PropTypes.func.isRequired,
  updateListAdmins: PropTypes.func.isRequired,
};

export default inject(({ auth, setup }) => {
  const { admins, owner, filter, selectorIsOpen } = setup.security.accessRight;
  const { user: me } = auth.userStore;

  const {
    setAddUsers,
    setRemoveAdmins,
    toggleSelector,
    setAdmins,
    getUsersByIds,
  } = setup;
  const {
    selectUser,
    deselectUser,
    selection,
    isUserSelected,
    setSelected,
  } = setup.selectionStore;

  return {
    theme: auth.settingsStore.theme,
    groupsCaption: auth.settingsStore.customNames.groupsCaption,
    changeAdmins: setup.changeAdmins,
    fetchPeople: setup.fetchPeople,
    updateListAdmins: setup.updateListAdmins,
    setFilterParams: setup.setFilterParams,
    modules,
    admins,
    productId: modules[0].id,
    owner,
    filter,
    me,
    setAddUsers,
    setRemoveAdmins,
    selectUser,
    deselectUser,
    selection,
    isUserSelected,
    setSelected,
    selectorIsOpen,
    toggleSelector,
    setAdmins,
    getUsersByIds,
  };
})(withTranslation(["Settings", "Common"])(withRouter(observer(PortalAdmins))));
