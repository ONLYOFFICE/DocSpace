import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import {
  changeAdmins,
  fetchPeople
} from "../../../../../store/settings/actions";
import {
  Text,
  Avatar,
  ToggleContent,
  Row,
  RowContent,
  RowContainer,
  Link,
  RadioButtonGroup,
  Paging,
  IconButton,
  AdvancedSelector,
  toastr,
  RequestLoader,
  FilterInput,
  Button,
  TabContainer
} from "asc-web-components";
import { getUserRole } from "../../../../../store/settings/selectors";

const MainContainer = styled.div`
  padding-bottom: 16px;
  width: 100%;

  .page_loader {
    position: fixed;
    left: 50%;
  }
`;

const ProjectsContainer = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;

  .display-block {
    display: block;
  }

  div label:not(:first-child) {
    margin: 0;
  }
`;

const RadioButtonContainer = styled.div`
  margin-right: 150px;
  margin-bottom: 16px;
  width: 310px;
`;

const HeaderContainer = styled.div`
  margin: 40px 0 16px 0;
`;

const BodyContainer = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;
  margin-bottom: 24px;
`;

const AvatarContainer = styled.div`
  display: flex;
  width: 330px;
  height: 120px;
  margin-right: 130px;
  margin-bottom: 24px;
  padding: 8px;
  border: 1px solid lightgrey;

  .avatar_wrapper {
    width: 100px;
    height: 100px;
  }

  .avatar_body {
    margin-left: 24px;
    max-width: 190px;
    word-wrap: break-word;
    overflow: hidden;
  }
`;

const ToggleContentContainer = styled.div`
  .toggle_content {
    margin-bottom: 24px;
  }

  .wrapper {
    margin-top: 16px;
  }

  .remove_icon {
    margin-left: 120px;
  }

  .button_style {
    margin-right: 16px;
  }

  .advanced-selector {
    position: relative;
  }

  .filter_container {
    margin-bottom: 50px;
    margin-top: 16px;
  }
`;

const ProjectsBody = styled.div`
  width: 280px;
`;

class PureAccessRights extends Component {
  constructor(props) {
    super(props);

    this.state = {
      showSelector: false,
      showFullAdminSelector: false,
      options: [],
      isLoading: false,
      selectedOptions: []
    };
  }

  componentDidMount() {
    const { fetchPeople } = this.props;
    this.onLoading(true);

    const newFilter = this.onAdminsFilter();

    fetchPeople(newFilter)
      .catch(error => {
        toastr.error(error);
      })
      .finally(() => this.onLoading(false));
  }

  onChangeAdmin = (userIds, isAdmin, productId) => {
    this.onLoading(true);
    const { changeAdmins } = this.props;
    const newFilter = this.onAdminsFilter();

    changeAdmins(userIds, productId, isAdmin, newFilter)
      .catch(error => {
        toastr.error("accessRights onChangeAdmin", error);
      })
      .finally(() => {
        this.onLoading(false);
      });
  };

  onShowGroupSelector = () =>
    this.setState({
      showSelector: !this.state.showSelector,
      options: this.props.options,
      selectedOptions: []
    });

  onShowFullAdminGroupSelector = () => {
    this.setState({
      showFullAdminSelector: !this.state.showFullAdminSelector,
      options: this.props.options, //?
      selectedOptions: [] //?
    });
  };

  onSelect = selected => {
    const { productId } = this.props;
    this.onChangeAdmin(
      selected.map(user => user.key),
      true,
      productId
    );
    this.onShowGroupSelector();
    this.setState({ selectedOptions: selected });
  };

  onSelectFullAdmin = selected => {
    this.onChangeAdmin(
      selected.map(user => user.key),
      true,
      "00000000-0000-0000-0000-000000000000"
    );
    this.onShowFullAdminGroupSelector();
    this.setState({ selectedOptions: selected });
  };

  onSearchUsers = template => {
    this.setState({
      options: this.filterUserSelectorOptions(this.props.options, template)
    });
  };

  onChangePage = pageItem => {
    const { filter, fetchPeople } = this.props;

    const newFilter = filter.clone();
    newFilter.page = pageItem.key;
    this.onLoading(true);
    fetchPeople(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onChangePageSize = pageItem => {
    const { filter, fetchPeople } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.pageCount = pageItem.key;
    this.onLoading(true);
    fetchPeople(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onPrevClick = e => {
    const { filter, fetchPeople } = this.props;

    if (!filter.hasPrev()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page--;
    this.onLoading(true);
    fetchPeople(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onNextClick = e => {
    const { filter, fetchPeople } = this.props;

    if (!filter.hasNext()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page++;
    this.onLoading(true);
    fetchPeople(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onLoading = status => {
    this.setState({ isLoading: status });
  };

  onAdminsFilter = () => {
    const { filter } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.role = "admin";

    return newFilter;
  };

  onFilter = data => {
    const { filter, fetchPeople } = this.props;

    const search = data.inputValue || null;
    const sortBy = data.sortId;
    const sortOrder =
      data.sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();

    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;
    newFilter.page = 0;
    newFilter.role = "admin";
    newFilter.search = search;
    this.onLoading(true);
    fetchPeople(newFilter)
      .catch(res => console.log(res))
      .finally(this.onLoading(false));
  };

  onChangeOwner = () => {
    toastr.warning("onChangeOwner");
  };

  onSelectPage = page => {
    //console.log("onSelectPage", page.key);
    /*
    const { history } = this.props;
    switch (page) {
      case 0:
        history.push("/owner");
        break;
      case 1:
        history.push("/admins");
        break;
      case 2:
        history.push("/modules");
        break;
      default:
        break;
    }
    */
  };

  filterUserSelectorOptions = (options, template) =>
    options.filter(option => option.label.indexOf(template) > -1);

  pageItems = () => {
    const { t, filter } = this.props;
    if (filter.total < filter.pageCount) return [];
    const totalPages = Math.ceil(filter.total / filter.pageCount);
    return [...Array(totalPages).keys()].map(item => {
      return {
        key: item,
        label: t("PageOfTotalPage", { page: item + 1, totalPage: totalPages })
      };
    });
  };

  countItems = () => [
    { key: 25, label: this.props.t("CountPerPage", { count: 25 }) },
    { key: 50, label: this.props.t("CountPerPage", { count: 50 }) },
    { key: 100, label: this.props.t("CountPerPage", { count: 100 }) }
  ];

  selectedPageItem = () => {
    const { filter, t } = this.props;
    const pageItems = this.pageItems();

    const emptyPageSelection = {
      key: 0,
      label: t("PageOfTotalPage", { page: 1, totalPage: 1 })
    };

    return pageItems.find(x => x.key === filter.page) || emptyPageSelection;
  };

  selectedCountItem = () => {
    const { filter, t } = this.props;

    const emptyCountSelection = {
      key: 0,
      label: t("CountPerPage", { count: 25 })
    };

    const countItems = this.countItems();

    return (
      countItems.find(x => x.key === filter.pageCount) || emptyCountSelection
    );
  };

  getSortData = () => {
    const { t } = this.props;

    return [
      { key: "firstname", label: t("ByFirstNameSorting") },
      { key: "lastname", label: t("ByLastNameSorting") }
    ];
  };

  render() {
    const { t, owner, admins, filter } = this.props;
    const {
      showSelector,
      options,
      selectedOptions,
      isLoading,
      showFullAdminSelector
    } = this.state;

    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");
    const countElements = filter.total;

    //console.log("accessRight render");

    return (
      <MainContainer>
        <TabContainer
          /*selectedItem={2}*/ isDisabled={isLoading}
          onSelect={this.onSelectPage}
        >
          {[
            {
              key: "0",
              title: "Owner settings",
              content: (
                <>
                  <HeaderContainer>
                    <Text.Body fontSize={18}>{t("PortalOwner")}</Text.Body>
                  </HeaderContainer>

                  <BodyContainer>
                    <AvatarContainer>
                      <Avatar
                        className="avatar_wrapper"
                        size="big"
                        role="owner"
                        userName={owner.userName}
                        source={owner.avatar}
                      />
                      <div className="avatar_body">
                        <Text.Body
                          className="avatar_text"
                          fontSize={16}
                          isBold={true}
                        >
                          {owner.displayName}
                        </Text.Body>
                        {owner.groups &&
                          owner.groups.map(group => (
                            <Link
                              fontSize={12}
                              key={group.id}
                              href={owner.profileUrl}
                            >
                              {group.name}
                            </Link>
                          ))}
                      </div>
                    </AvatarContainer>
                    <ProjectsBody>
                      <Text.Body className="portal_owner" fontSize={12}>
                        {t("AccessRightsOwnerCan")}:
                      </Text.Body>
                      <Text.Body fontSize={12}>
                        {OwnerOpportunities.map((item, key) => (
                          <li key={key}>{item};</li>
                        ))}
                      </Text.Body>
                    </ProjectsBody>
                  </BodyContainer>
                  <Button
                    size="medium"
                    primary={true}
                    label="Change portal owner"
                    isDisabled={isLoading}
                    onClick={this.onChangeOwner}
                  />
                </>
              )
            },
            {
              key: "1",
              title: "Admins settings",
              content: (
                <ToggleContentContainer>
                  <div style={{ display: "flex", width: "fit-content" }}>
                    <Button
                      className="button_style"
                      size="medium"
                      primary={true}
                      label="Set people admin"
                      isDisabled={isLoading}
                      onClick={this.onShowGroupSelector}
                    />
                    <div style={{ right: 180 }} className="advanced-selector">
                      <AdvancedSelector
                        displayType="dropdown"
                        isOpen={showSelector}
                        placeholder="placeholder"
                        options={options}
                        onSearchChanged={this.onSearchUsers}
                        //groups={groups}
                        isMultiSelect={true}
                        buttonLabel="Add members"
                        onSelect={this.onSelect}
                        onCancel={this.onShowGroupSelector}
                        onAddNewClick={() => console.log("onAddNewClick")}
                        selectAllLabel="selectorSelectAllText"
                        selectedOptions={selectedOptions}
                      />
                    </div>

                    <Button
                      size="medium"
                      primary={true}
                      label="Set portal admin"
                      isDisabled={isLoading}
                      onClick={this.onShowFullAdminGroupSelector}
                    />
                    <div style={{ right: 160 }} className="advanced-selector">
                      <AdvancedSelector
                        displayType="dropdown"
                        isOpen={showFullAdminSelector}
                        placeholder="placeholder"
                        options={options}
                        onSearchChanged={this.onSearchUsers}
                        //groups={groups}
                        isMultiSelect={true}
                        buttonLabel="Add members"
                        onSelect={this.onSelectFullAdmin}
                        onCancel={this.onShowFullAdminGroupSelector}
                        onAddNewClick={() => console.log("onAddNewClick")}
                        selectAllLabel="selectorSelectAllText"
                        selectedOptions={selectedOptions}
                      />
                    </div>
                  </div>

                  {countElements > 25 ? (
                    <FilterInput
                      className="filter_container"
                      getFilterData={() => []}
                      getSortData={this.getSortData}
                      onFilter={this.onFilter}
                    />
                  ) : null}

                  <div className="wrapper">
                    <RowContainer manualHeight={`${admins.length * 50}px`}>
                      {admins.map(user => {
                        const element = (
                          <Avatar
                            size="small"
                            role={getUserRole(user)}
                            userName={user.displayName}
                            source={user.avatar}
                          />
                        );
                        const nameColor =
                          user.status === "pending" ? "#A3A9AE" : "#333333";

                        return (
                          <Row
                            key={user.id}
                            status={user.status}
                            data={user}
                            element={element}
                          >
                            <RowContent disableSideInfo={true}>
                              <Link
                                containerWidth="120px"
                                type="page"
                                title={user.displayName}
                                isBold={true}
                                fontSize={15}
                                color={nameColor}
                                href={user.profileUrl}
                              >
                                {user.displayName}
                              </Link>
                              <div style={{ maxWidth: 120 }} />

                              <Text.Body>
                                {user.isAdmin
                                  ? "Full access"
                                  : "People module admin"}
                              </Text.Body>

                              {!user.isOwner ? (
                                <IconButton
                                  className="remove_icon"
                                  size="16"
                                  isDisabled={isLoading}
                                  onClick={this.onChangeAdmin.bind(
                                    this,
                                    [user.id],
                                    false,
                                    "00000000-0000-0000-0000-000000000000"
                                  )}
                                  iconName={"CatalogTrashIcon"}
                                  isFill={true}
                                  isClickable={false}
                                />
                              ) : (
                                <div />
                              )}
                            </RowContent>
                          </Row>
                        );
                      })}
                    </RowContainer>
                  </div>

                  {countElements > 25 ? (
                    <div className="wrapper">
                      <Paging
                        previousLabel={t("PreviousPage")}
                        nextLabel={t("NextPage")}
                        openDirection="top"
                        countItems={this.countItems()}
                        pageItems={this.pageItems()}
                        displayItems={false}
                        selectedPageItem={this.selectedPageItem()}
                        selectedCountItem={this.selectedCountItem()}
                        onSelectPage={this.onChangePage}
                        onSelectCount={this.onChangePageSize}
                        previousAction={this.onPrevClick}
                        nextAction={this.onNextClick}
                        disablePrevious={!filter.hasPrev()}
                        disableNext={!filter.hasNext()}
                      />
                    </div>
                  ) : null}
                </ToggleContentContainer>
              )
            },
            {
              key: "2",
              title: "Portals settings",
              content: (
                <ToggleContentContainer>
                  <ToggleContent
                    className="toggle_content"
                    label={t("People")}
                    isOpen={true}
                  >
                    <ProjectsContainer>
                      <RadioButtonContainer>
                        <Text.Body>
                          {t("AccessRightsAccessToProduct", {
                            product: t("People")
                          })}
                          :
                        </Text.Body>
                        <RadioButtonGroup
                          name="selectGroup"
                          selected="allUsers"
                          options={[
                            {
                              value: "allUsers",
                              label: t("AccessRightsAllUsers", {
                                users: t("Employees")
                              })
                            },
                            {
                              value: "usersFromTheList",
                              label: t("AccessRightsUsersFromList", {
                                users: t("Employees")
                              })
                            }
                          ]}
                          className="display-block"
                        />
                      </RadioButtonContainer>
                      <ProjectsBody>
                        <Text.Body className="projects_margin" fontSize={12}>
                          {t("AccessRightsProductUsersCan", {
                            category: t("People")
                          })}
                        </Text.Body>
                        <Text.Body fontSize={12}>
                          <li>{t("ViewProfilesAndGroups")}</li>
                        </Text.Body>
                      </ProjectsBody>
                    </ProjectsContainer>
                  </ToggleContent>
                </ToggleContentContainer>
              )
            }
          ]}
        </TabContainer>
        <RequestLoader
          visible={isLoading}
          zIndex={256}
          loaderSize={16}
          loaderColor={"#999"}
          label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
          fontSize={12}
          fontColor={"#999"}
          className="page_loader"
        />
      </MainContainer>
    );
  }
}

const AccessRightsContainer = withTranslation()(PureAccessRights);

const AccessRights = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <AccessRightsContainer {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  const {
    admins,
    options,
    owner,
    filter
  } = state.settings.security.accessRight;

  return {
    admins,
    productId: state.auth.modules[0].id,
    owner,
    options,
    filter
  };
}

AccessRights.defaultProps = {
  admins: [],
  productId: "",
  owner: {},
  options: []
};

AccessRights.propTypes = {
  admins: PropTypes.arrayOf(PropTypes.object),
  productId: PropTypes.string,
  owner: PropTypes.object,
  options: PropTypes.arrayOf(PropTypes.object)
};

export default connect(mapStateToProps, { changeAdmins, fetchPeople })(
  withRouter(AccessRights)
);
