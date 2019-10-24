import React, { Component } from "react";
//import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import { getAdminUsers } from "../../../../../store/people/actions";
import {
  Text,
  Avatar,
  ToggleContent,
  Row,
  RowContent,
  RowContainer,
  RadioButtonGroup,
  //Icons,
  Link,
  Checkbox
  //toastr,
} from "asc-web-components";

const MainContainer = styled.div`
  padding: 16px 16px 16px 24px;
  width: 100%;
`;

const HeaderContainer = styled.div`
  margin-bottom: 16px;
`;

const BodyContainer = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;
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

const ProjectsBody = styled.div`
  width: 280px;
`;

/*const AdministratorsHead = styled.div`
  display: flex;
  margin-right: 70px;

  .category {
    margin-left: 5px;
    flex-basis: 12.5%;  (1/elementsCount*100%)
    text-align: center;
  }
`;*/

class PureAccessRights extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isChecked: this.props.isChecked,
      adminUsers: [],
      portalOwner: null
    };
  }

  componentDidMount() {
    const { getAdminUsers } = this.props;

    getAdminUsers()
      .then(res => {
        console.log("getAdminUsers response", res);
        this.setState({adminUsers: res, portalOwner: res.find(x=> x.isOwner)})
      })
      .catch(error => {
        console.log(error);
      });
  }

  //componentDidUpdate(prevProps, prevState) {}
  //componentWillUnmount() {}

  onChange = e => {
    //console.log(e.target.value);
    //e.target.value = !e.target.value;
  };

  render() {
    const { t } = this.props;
    const { adminUsers, portalOwner } = this.state;
    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    return (
      <MainContainer>
        <HeaderContainer>
          <Text.Body fontSize={18}>{t("PortalOwner")}</Text.Body>
        </HeaderContainer>

        <BodyContainer>
          <AvatarContainer>
            <Avatar size="big" role="owner" />
            {portalOwner ? (
              <div style={{ marginLeft: "24px" }}>
                <Text.Body className="avatar_text" fontSize={16} isBold={true}>
                  {portalOwner.displayName}
                </Text.Body>
                <Text.Body className="avatar_text" fontSize={12}>
                  {portalOwner.department}
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
            <RowContainer manualHeight="200px">
              {adminUsers.map(user => {
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
                    checked={false}
                    data={user}
                    element={element}
                    //contextOptions={user.contextOptions}
                  >
                    <RowContent disableSideInfo={true}>
                      <Link
                        containerWidth="120px"
                        type="page"
                        title={user.userName}
                        isBold={true}
                        fontSize={15}
                        color={nameColor}
                        //href="/products/people/profile.aspx?user=administrator"
                        href={user.profileUrl}
                      >
                        {user.userName}
                      </Link>

                      <div
                        /*containerWidth='120px'*/ style={{ maxWidth: 120 }}
                      ></div>

                      <Checkbox
                        isChecked={false}
                        onChange={this.onChange}
                        id={`fullAccess_${user.id}`}
                      />
                      <Checkbox
                        isChecked={false}
                        onChange={this.onChange}
                        id={`people_${user.id}`}
                      />
                    </RowContent>
                  </Row>
                );
              })}
            </RowContainer>
          </ToggleContent>

          <ToggleContent
            className="toggle_content"
            label={t("ProjectsProduct")}
            isOpen={true}
          >
            <ProjectsContainer>
              <RadioButtonContainer>
                <Text.Body>
                  {t("AccessRightsAccessToProduct", {
                    product: t("ProjectsProduct")
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
                    category: t("ProjectsProduct")
                  })}
                </Text.Body>
                <Text.Body fontSize={12}>
                  <li>{t("ProjectsUserCapabilityView")}</li>
                  <li>{t("ProjectsUserCapabilityCreate")}</li>
                  <li>{t("ProjectsUserCapabilityTrack")}</li>
                  <li>{t("ProjectsUserCapabilityForm")}</li>
                </Text.Body>
              </ProjectsBody>
            </ProjectsContainer>
          </ToggleContent>

          <ToggleContent
            className="toggle_content"
            label={t("CrmProduct")}
            isOpen={true}
          >
            <ProjectsContainer>
              <RadioButtonContainer>
                <Text.Body>
                  {t("AccessRightsAccessToProduct", {
                    product: t("CrmProduct")
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
                    category: t("CrmProduct")
                  })}
                </Text.Body>
                <Text.Body fontSize={12}>
                  <li>{t("CRMUserCapability")}</li>
                  <li>{t("CRMUserCapabilityEdit")}</li>
                </Text.Body>
              </ProjectsBody>
            </ProjectsContainer>
          </ToggleContent>

          <ToggleContent
            className="toggle_content"
            label={t("CommunityProduct")}
            isOpen={true}
          >
            <ProjectsContainer>
              <RadioButtonContainer>
                <Text.Body>
                  {t("AccessRightsAccessToProduct", {
                    product: t("CommunityProduct")
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
                    category: t("CommunityProduct")
                  })}
                </Text.Body>
                <Text.Body fontSize={12}>
                  <li>{t("CommunityUserCapability")}</li>
                </Text.Body>
              </ProjectsBody>
            </ProjectsContainer>
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

          <ToggleContent
            className="toggle_content"
            label={t("Sample")}
            isOpen={true}
          >
            <Text.Body fontSize={12}>
              {t("AccessRightsDisabledProduct", { module: "Sample" })}
            </Text.Body>
          </ToggleContent>

          <ToggleContent
            className="toggle_content"
            label={t("Mail")}
            isOpen={true}
          >
            <ProjectsContainer>
              <RadioButtonContainer>
                <Text.Body>
                  {t("AccessRightsAccessToProduct", { product: t("Mail") })}:
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
                  {t("AccessRightsProductUsersCan", { category: t("Mail") })}
                </Text.Body>
                <Text.Body fontSize={12}>
                  <li>{t("ManageOwnMailAccounts")}</li>
                  <li>{t("ManageOwnMailSettings")}</li>
                  <li>{t("ManageTheTagsAndAddressBook")}</li>
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

PureAccessRights.defaultProps = {
  isChecked: false
};

const ProfileContainer = withTranslation()(PureAccessRights);

const AccessRights = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <ProfileContainer {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  console.log("state", state);
  return {
    //adminUsers: state.auth.isValidConfirmLink
  };
}

export default connect(
  mapStateToProps,
  { getAdminUsers }
)(withRouter(withTranslation()(AccessRights)));
/*
  <AdministratorsHead>
    <div style={{width: 250}}></div>
    <Text.Body className="category">
      {t("AccessRightsFullAccess")}
    </Text.Body>
    <Text.Body className="category">{t("DocumentsProduct")}</Text.Body>
    <Text.Body className="category">{t("ProjectsProduct")}</Text.Body>
    <Text.Body className="category">{t("CrmProduct")}</Text.Body>
    <Text.Body className="category">{t("CommunityProduct")}</Text.Body>
    <Text.Body className="category">{t("People")}</Text.Body>
    <Text.Body className="category">{t("Sample")}</Text.Body>
    <Text.Body className="category">{t("Mail")}</Text.Body>
  </AdministratorsHead>
*/
/*
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`fullAccess_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`documents_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`projects_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`crm_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`community_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`people_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`sample_${user.id}`}
  />
  <Checkbox
    isChecked={false}
    onChange={this.onChange}
    id={`mail_${user.id}`}
  />
*/
