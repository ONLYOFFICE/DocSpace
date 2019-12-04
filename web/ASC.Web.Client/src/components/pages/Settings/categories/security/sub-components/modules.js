import React, { Component } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import { Text, ToggleContent, RadioButtonGroup } from "asc-web-components";

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

class PureModulesSettings extends Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  componentDidMount() {}

  render() {
    const { t } = this.props;

    console.log("Modules render_");

    return (
      <ToggleContentContainer>
        <ToggleContent
          className="toggle_content"
          label={t("People")}
          isOpen={true}
        >
          <ProjectsContainer>
            <RadioButtonContainer>
              <Text>
                {t("AccessRightsAccessToProduct", {
                  product: t("People")
                })}
                :
              </Text>
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
              <Text className="projects_margin" fontSize={12}>
                {t("AccessRightsProductUsersCan", {
                  category: t("People")
                })}
              </Text>
              <Text fontSize={12}>
                <li>{t("ViewProfilesAndGroups")}</li>
              </Text>
            </ProjectsBody>
          </ProjectsContainer>
        </ToggleContent>
      </ToggleContentContainer>
    );
  }
}

const AccessRightsContainer = withTranslation()(PureModulesSettings);

const ModulesSettings = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <AccessRightsContainer {...props} />
    </I18nextProvider>
  );
};

export default connect(null, {})(withRouter(ModulesSettings));
