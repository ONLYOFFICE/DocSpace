import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import {
  getListUsers,
  getListAdmins,
  changeAdmins,
  getUserById
} from "../../../../../store/people/actions";
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
  SearchInput,
  SelectorAddButton,
  IconButton
  //toastr,
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
`;

const ToggleContentContainer = styled.div`
  .toggle_content {
    margin-bottom: 24px;
  }

  .wrapper {
    margin-top: 16px;
  }

  .icon {
    width: 25px;
  }
`;

const ProjectsBody = styled.div`
  width: 280px;
`;

class PureAccessRights extends Component {
  constructor(props) {
    super(props);

    this.state = {
      searchValue: ""
    };
  }

  componentDidMount() {
    const { getListUsers, getListAdmins, getUserById, ownerId } = this.props;

    getUserById(ownerId)
      .then(res => {
        /*console.log("getUserById", res)*/
      })
      .catch(res => {
        /*console.log("getUserById", res)*/
      });

    getListUsers()
      .then(res => {
        //console.log("getUsers response", res);
      })
      .catch(error => {
        console.log(error);
      });

    getListAdmins()
      .then(res => {
        //console.log("getUsers response", res);
      })
      .catch(error => {
        console.log(error);
      });
  }

  //componentDidUpdate(prevProps, prevState) {}
  //componentWillUnmount() {}

  onSearchChange = value => {
    this.setState({
      searchValue: value
    });
  };

  onChangeAdmin = (user, isAdmin) => {
    const userId = user.id;
    const { changeAdmins, productId } = this.props;

    changeAdmins(userId, productId, isAdmin)
      .then(res => {
        console.log("Delete admin response", res);
      })
      .catch(error => {
        console.log("onDeleteAdminUser", error);
      });
  };

  onShowAdvancedSelector = () => console.log("Add new user");

  render() {
    const { t } = this.props;
    const { users, owner, admins } = this.props;
    const { searchValue } = this.state;
    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    return (
      <MainContainer>
        <HeaderContainer>
          <Text.Body fontSize={18}>{t("PortalOwner")}</Text.Body>
        </HeaderContainer>

        <BodyContainer>
          <AvatarContainer>
            <Avatar size="big" role="owner" />
            {owner ? (
              <div style={{ marginLeft: "24px" }}>
                <Text.Body className="avatar_text" fontSize={16} isBold={true}>
                  {owner.displayName}
                </Text.Body>
                <Text.Body className="avatar_text" fontSize={12}>
                  {owner.department}
                </Text.Body>
              </div>
            ) : null}
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
            <SearchInput
              className="wrapper"
              id="member-search"
              //isDisabled={inLoading}
              scale={true}
              placeholder="Search"
              value={searchValue}
              onChange={this.onSearchChange}
            />
            <div className="wrapper">
              {console.log("manualHeight", admins)}
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
                            size="25"
                            isDisabled={false}
                            onClick={this.onChangeAdmin.bind(this, user, false)}
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
            <SelectorAddButton onClick={this.onShowAdvancedSelector} />
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

PureAccessRights.propTypes = {};

PureAccessRights.defaultProps = {};

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
  return {
    users: state.settings.users,
    admins: state.settings.admins,
    productId: state.auth.modules[0].id,
    ownerId: state.auth.settings.ownerId,
    owner: state.settings.owner
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
  { getListUsers, getListAdmins, changeAdmins, getUserById }
)(withRouter(AccessRights));
