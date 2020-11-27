import React, { Component } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/src/components/text";
import ToggleContent from "@appserver/components/src/components/toggle-content";
import RadioButtonGroup from "@appserver/components/src/components/radio-button-group";

import { createI18N } from "../../../../../../helpers/i18n";

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings",
});

const ProjectsContainer = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;
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
                  product: t("People"),
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
                      users: t("Employees"),
                    }),
                  },
                  {
                    value: "usersFromTheList",
                    label: t("AccessRightsUsersFromList", {
                      users: t("Employees"),
                    }),
                  },
                ]}
                orientation="vertical"
                spacing="10px"
              />
            </RadioButtonContainer>
            <ProjectsBody>
              <Text className="projects_margin" fontSize="12px">
                {t("AccessRightsProductUsersCan", {
                  category: t("People"),
                })}
              </Text>
              <Text fontSize="12px">
                <li>{t("ProductUserOpportunities")}</li>
              </Text>
            </ProjectsBody>
          </ProjectsContainer>
        </ToggleContent>
      </ToggleContentContainer>
    );
  }
}

const AccessRightsContainer = withTranslation()(PureModulesSettings);

const ModulesSettings = (props) => (
  <I18nextProvider i18n={i18n}>
    <AccessRightsContainer {...props} />
  </I18nextProvider>
);

export default connect(null, {})(withRouter(ModulesSettings));
