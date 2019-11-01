import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import {
  changeAdmins,
  getListAdmins,
  getListUsers,
  getUserById
} from "../../../../../store/settings/actions";
import { filter } from "lodash";
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
  toastr
} from "asc-web-components";

const MainContainer = styled.div`
  padding: 16px 16px 16px 24px;
  width: 100%;
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
  height: 100px;
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
      advancedOptions: []
    };
  }

  componentDidMount() {
    const { getListAdmins, getListUsers, getUserById, ownerId } = this.props;

    getUserById(ownerId).catch(error => {
      toastr.error("accessRights getUserById", error);
      //console.log("accessRights getUserById", error);
    });

    getListUsers().catch(error => {
      toastr.error("accessRights getListAdmins", error);
      //console.log("accessRights getListAdmins", error);
    });

    getListAdmins().catch(error => {
      toastr.error("accessRights getListAdmins", error);
      //console.log("accessRights getListAdmins", error);
    });
  }

  onChangeAdmin = (userId, isAdmin) => {
    const { changeAdmins, productId } = this.props;

    changeAdmins(userId, productId, isAdmin).catch(error => {
      toastr.error("accessRights onChangeAdmin", error);
      //console.log("accessRights onChangeAdmin", error)
    });
  };

  onShowGroupSelector = () =>
    this.setState({
      showSelector: !this.state.showSelector,
      advancedOptions: this.props.advancedOptions
    });

  onSelect = selected => {
    selected.map(user => this.onChangeAdmin(user.key, true));
    this.onShowGroupSelector();
  };

  onSearchUsers = template =>
    this.setState({
      advancedOptions: this.filterUserSelectorOptions(
        this.props.advancedOptions,
        template
      )
    });

  filterUserSelectorOptions = (options, template) => {
    return options.filter(option => option.label.indexOf(template) > -1);
  };

  render() {
    const { t, owner, admins } = this.props;
    const { showSelector, advancedOptions } = this.state;
    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    return (
      <MainContainer>
        <HeaderContainer>
          <Text.Body fontSize={18}>{t("PortalOwner")}</Text.Body>
        </HeaderContainer>

        <BodyContainer>
          <AvatarContainer>
            <div className="avatar_wrapper">
              <Avatar
                size="big"
                role="owner"
                userName={owner.userName}
                source={owner.avatar}
              />
            </div>
            <div className="avatar_body">
              <Text.Body className="avatar_text" fontSize={16} isBold={true}>
                {owner.displayName}
              </Text.Body>
              <Text.Body className="avatar_text" fontSize={12}>
                {owner.department}
              </Text.Body>
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
            <div className="selector-button">
              <SelectorAddButton
                //isDisabled={isDisabled}
                //title={showGroupSelectorButtonTitle}
                onClick={this.onShowGroupSelector}
              />
            </div>

            <div className="advanced-selector">
              <AdvancedSelector
                displayType="dropdown"
                isOpen={showSelector}
                placeholder="placeholder"
                options={advancedOptions}
                //options={this.AdvancedSelectorFunction(users)}
                onSearchChanged={this.onSearchUsers}
                //groups={groups}
                isMultiSelect={true}
                buttonLabel="Add members"
                onSelect={this.onSelect}
                onCancel={this.onShowGroupSelector}
                onAddNewClick={() => console.log("onAddNewClick")}
                selectAllLabel="selectorSelectAllText"
              />
            </div>

            <div className="wrapper">
              <RowContainer manualHeight={`${admins.length * 50}px`}>
                {admins.map(user => {
                  const element = (
                    <Avatar
                      size="small"
                      role="admin"
                      userName={user.userName}
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
                          title={user.userName}
                          isBold={true}
                          fontSize={15}
                          color={nameColor}
                          href={user.profileUrl}
                        >
                          {user.userName}
                        </Link>

                        <div style={{ maxWidth: 120 }} />
                        <div style={{ marginLeft: "60px" }}>
                          <IconButton
                            size="16"
                            isDisabled={false}
                            onClick={this.onChangeAdmin.bind(
                              this,
                              user.id,
                              false
                            )}
                            iconName={"CatalogTrashIcon"}
                            isFill={true}
                            isClickable={false}
                          />
                        </div>
                      </RowContent>
                    </Row>
                  );
                })}
              </RowContainer>
            </div>
            {admins.length > 25 ? (
              <div className="wrapper">
                <Paging
                  previousLabel="Previous"
                  nextLabel="Next"
                  selectedPageItem={{ label: "1 of 1" }}
                  selectedCountItem={{ label: "25 per page" }}
                  previousAction={() => console.log("Prev")}
                  nextAction={() => console.log("Next")}
                  openDirection="bottom"
                  onSelectPage={a => console.log(a)}
                  onSelectCount={a => console.log(a)}
                />
              </div>
            ) : null}
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
                  {t("AccessRightsProductUsersCan", { category: t("People") })}
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

const filterOwner = (users, ownerId) =>
  filter(users, function(f) {
    return f.id !== ownerId;
  });
const filterAdminUsers = users => {
  return users.filter(user => user.listAdminModules === undefined);
};

const AdvancedSelectorFunction = users =>
  users.map(user => {
    return {
      key: user.id,
      label: user.displayName
    };
  });

function mapStateToProps(state) {
  const { ownerId } = state.auth.settings;
  const { admins, users } = state.settings;
  const arrayUsers = filterOwner(users, ownerId);
  const filterArrayUsers = filterAdminUsers(arrayUsers);

  return {
    users: filterArrayUsers,
    admins: filterOwner(admins, ownerId),
    productId: state.auth.modules[0].id,
    owner: state.settings.owner,
    ownerId,
    advancedOptions: AdvancedSelectorFunction(filterArrayUsers)
  };
}

AccessRights.defaultProps = {
  users: [],
  admins: [],
  productId: "",
  ownerId: "",
  owner: {}
};

AccessRights.propTypes = {
  users: PropTypes.arrayOf(PropTypes.object),
  admins: PropTypes.arrayOf(PropTypes.object),
  productId: PropTypes.string,
  ownerId: PropTypes.string,
  owner: PropTypes.object
};

export default connect(
  mapStateToProps,
  { getUserById, changeAdmins, getListAdmins, getListUsers }
)(withRouter(AccessRights));
