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
  SelectorAddButton,
  IconButton,
  AdvancedSelector,
  toastr,
  RequestLoader
} from "asc-web-components";
import { getUserRole } from "../../../../../store/settings/selectors";

const MainContainer = styled.div`
  padding: 16px 16px 16px 24px;
  width: 100%;

  .page_loader {
    position: fixed;
    left: 48%;
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
  margin-bottom: 16px;
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

  .advanced-selector {
    position: relative;
  }

  .selector-button {
    max-width: 34px;
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
      options: [],
      isLoading: false,
      selectedOptions: []
    };
  }

  componentDidMount() {
    const { fetchPeople } = this.props;

    const newFilter = this.onFilter();

    fetchPeople(newFilter).catch(error => {
      toastr.error(error);
    });
  }

  onChangeAdmin = (userIds, isAdmin) => {
    this.onLoading(true);
    const { changeAdmins, productId } = this.props;
    const newFilter = this.onFilter();

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

  onSelect = selected => {
    this.onChangeAdmin(selected.map(user => user.key), true);
    this.onShowGroupSelector();
    this.setState({ selectedOptions: selected });
  };

  onSearchUsers = template => {
    this.onLoading(true);
    this.setState({
      options: this.filterUserSelectorOptions(this.props.options, template)
    });
    this.onLoading(false);
  };

  filterUserSelectorOptions = (options, template) =>
    options.filter(option => option.label.indexOf(template) > -1);

  onLoading = status => {
    this.setState({ isLoading: status });
  };

  onFilter = () => {
    const { filter } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.role = "admin";

    return newFilter;
  };

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

  onChangePage = pageItem => {
    const { filter, getListAdmins } = this.props;

    const newFilter = filter.clone();
    newFilter.page = pageItem.key;
    this.onLoading(true);
    getListAdmins(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onChangePageSize = pageItem => {
    const { filter, getListAdmins } = this.props;

    const newFilter = filter.clone();
    newFilter.pageCount = pageItem.key;
    newFilter.page = 0;
    this.onLoading(true);
    getListAdmins(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onPrevClick = e => {
    const { filter, getListAdmins } = this.props;

    if (!filter.hasPrev()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page--;
    this.onLoading(true);
    getListAdmins(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onNextClick = e => {
    const { filter, getListAdmins } = this.props;

    if (!filter.hasNext()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page++;
    this.onLoading(true);
    getListAdmins(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  render() {
    const { t, owner, admins, filter } = this.props;
    const { showSelector, options, selectedOptions, isLoading } = this.state;
    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    const countItems = [
      { key: 25, label: t("CountPerPage", { count: 25 }) },
      { key: 50, label: t("CountPerPage", { count: 50 }) },
      { key: 100, label: t("CountPerPage", { count: 100 }) }
    ];

    const pageItems = this.pageItems();

    const emptyPageSelection = {
      key: 0,
      label: t("PageOfTotalPage", { page: 1, totalPage: 1 })
    };

    const emptyCountSelection = {
      key: 0,
      label: t("CountPerPage", { count: 25 })
    };

    const selectedPageItem =
      pageItems.find(x => x.key === filter.page) || emptyPageSelection;

    const selectedCountItem =
      countItems.find(x => x.key === filter.pageCount) || emptyCountSelection;

    return (
      <MainContainer>
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
              <Text.Body className="avatar_text" fontSize={16} isBold={true}>
                {owner.displayName}
              </Text.Body>
              {owner.groups &&
                owner.groups.map(group => (
                  <Link fontSize={12} key={group.id} href={owner.profileUrl}>
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

        <ToggleContentContainer>
          <ToggleContent
            className="toggle_content"
            label={t("AdminSettings")}
            isOpen={true}
          >
            <SelectorAddButton
              className="selector-button"
              isDisabled={isLoading}
              //title={showGroupSelectorButtonTitle}
              onClick={this.onShowGroupSelector}
            />
            <div className="advanced-selector">
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
            <div className="wrapper">
              <RowContainer manualHeight={`${admins.length * 50}px`}>
                {admins.map(user => {
                  const element = (
                    <Avatar
                      size="small"
                      role={getUserRole(user)}
                      userName={user.displayName}
                      source={user.avatarSmall}
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
                        {!user.isOwner ? (
                          <div style={{ marginLeft: "60px" }}>
                            <IconButton
                              size="16"
                              isDisabled={isLoading}
                              onClick={this.onChangeAdmin.bind(
                                this,
                                [user.id],
                                false
                              )}
                              iconName={"CatalogTrashIcon"}
                              isFill={true}
                              isClickable={false}
                            />
                          </div>
                        ) : (
                          <div />
                        )}
                      </RowContent>
                    </Row>
                  );
                })}
              </RowContainer>
            </div>

              <div className="wrapper">
                <Paging
                  previousLabel={t("PreviousPage")}
                  nextLabel={t("NextPage")}
                  openDirection="top"
                  countItems={countItems}
                  pageItems={pageItems}
                  displayItems={false}
                  selectedPageItem={selectedPageItem}
                  selectedCountItem={selectedCountItem}
                  onSelectPage={this.onChangePage}
                  onSelectCount={this.onChangePageSize}
                  previousAction={this.onPrevClick}
                  nextAction={this.onNextClick}
                  disablePrevious={!filter.hasPrev()}
                  disableNext={!filter.hasNext()}
                />
              </div>

          </ToggleContent>

          <ToggleContent
            className="toggle_content"
            label={t("People")}
            isOpen={true}
          >
            <ProjectsContainer>
              <RadioButtonContainer>
                <Text.Body>
                  {t("AccessRightsAccessToProduct", { product: t("People") })}:
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
  const { admins, options, owner } = state.settings.accessRight;
  const { filter } = state.settings;

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

export default connect(
  mapStateToProps,
  { changeAdmins, fetchPeople }
)(withRouter(AccessRights));
