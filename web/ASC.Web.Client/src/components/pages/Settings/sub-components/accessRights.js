import React, { Component } from "react";
//import PropTypes from "prop-types";
import { connect } from "react-redux";
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
import i18n from "../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";

import axios from "axios";

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
  width: 330px;
  height: 100px;
  margin-right: 130px;
  margin-bottom: 24px;

  padding: 8px;
  border: 1px solid grey;
`;

const ItemsContainer = styled.div`
  .portal_owner {
    /*margin-left: 20px;*/
  }
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
  margin-right: 180px;
  margin-bottom: 16px;
  width: 310px;
`;

const ProjectsBody = styled.div`
  .projects_margin {
    /*margin-left: 24px;*/
  }
`;

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
    axios
      .get("http://localhost:8092/api/2.0/people")
      .then(response => {
        const res = response.data.response;
        //console.log(res);

        const array = [];
        for (const item of res) {
          if (item.isAdmin) {
            array.push(item);
          }
        }
        this.setState({
          adminUsers: array,
          portalOwner: res.find(x => x.isOwner === true)
        });
        //console.log("adminUsers", this.state.adminUsers);
        console.log("portalOwner", this.state.portalOwner);
      })
      .catch(error => {
        //console.log(error);
      });
  }
  //componentDidUpdate(prevProps, prevState) {}
  //componentWillUnmount() {}

  onChange = e => {
    //console.log(e.target.value);
    //e.target.value = !e.target.value;
  };

  render() {
    const fakeData2 = this.state.adminUsers;
    //console.log("fakeData2", fakeData2);

    const { t } = this.props;
    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    return (
      <MainContainer>
        <HeaderContainer>
          <Text.Body fontSize={18}>{t("PortalOwner")}</Text.Body>
        </HeaderContainer>

        <BodyContainer>
          <AvatarContainer>
            <Avatar size="big" role="owner" />
          </AvatarContainer>
          <ItemsContainer>
            <Text.Body className="portal_owner" fontSize={12}>
              {t("AccessRightsOwnerCan")}:
            </Text.Body>
            <Text.Body fontSize={12}>
              {OwnerOpportunities.map((item, key) => (
                <li key={key}>{item};</li>
              ))}
            </Text.Body>
          </ItemsContainer>
        </BodyContainer>

        <ToggleContentContainer>
          <ToggleContent
            className="toggle_content"
            label={t("AdminSettings")}
            isOpen={true}
          >
            <RowContainer manualHeight="200px">
              {fakeData2.map(user => {
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
                        //href="/products/people/view/@self"
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
                    { value: "allUsers", label: t("AllUsers") },
                    {
                      value: "usersFromTheList",
                      label: t("UsersFromTheList")
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
                    { value: "allUsers", label: t("AllUsers") },
                    {
                      value: "usersFromTheList",
                      label: t("UsersFromTheList")
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
                    { value: "allUsers", label: t("AllUsers") },
                    {
                      value: "usersFromTheList",
                      label: t("UsersFromTheList")
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
                    { value: "allUsers", label: t("AllUsers") },
                    {
                      value: "usersFromTheList",
                      label: t("UsersFromTheList")
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
            <ProjectsContainer>
              <Text.Body fontSize={12}>
                {t("AccessRightsDisabledProduct", { module: "Sample" })}
              </Text.Body>
            </ProjectsContainer>
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
                    { value: "allUsers", label: t("AllUsers") },
                    {
                      value: "usersFromTheList",
                      label: t("UsersFromTheList")
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
  {}
)(AccessRights);
/*
  <RowContentHeader>
    <Text.Body>
      {t("AccessRightsFullAccess")}
      {t("DocumentsProduct")}
      {t("ProjectsProduct")}
      {t("CrmProduct")}
      {t("CommunityProduct")}
      {t("People")}
      {t("Sample")}
      {t("Mail")}
    </Text.Body>
  </RowContentHeader>
*/
