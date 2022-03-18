import React from "react";
import { withRouter } from "react-router";
import styled from "styled-components";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import Section from "@appserver/common/components/Section";
import { deleteSelf } from "@appserver/common/api/people"; //TODO: Move inside UserStore
import withLoader from "../withLoader";

const ProfileRemoveContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;

  .start-basis {
    align-items: flex-start;
  }

  .confirm-row {
    margin: 23px 0 0;
  }

  .break-word {
    word-break: break-word;
  }
`;

class ProfileRemove extends React.PureComponent {
  constructor() {
    super();

    this.state = {
      isProfileDeleted: false,
    };
  }

  onDeleteProfile = () => {
    this.setState({ isLoading: true }, function () {
      const { linkData, logout } = this.props;
      deleteSelf(linkData.confirmHeader)
        .then((res) => {
          this.setState({
            isLoading: false,
            isProfileDeleted: true,
          });
          console.log("success delete", res);
          return logout();
        })
        .catch((e) => {
          this.setState({ isLoading: false });
          console.log("error delete", e);
        });
    });
  };

  render() {
    console.log("profileRemove render");
    const { t, greetingTitle, theme } = this.props;
    const { isProfileDeleted } = this.state;
    return (
      <ProfileRemoveContainer>
        <div className="start-basis">
          <div className="confirm-row full-width break-word">
            <a href="/login">
              <img src="images/dark_general.png" alt="Logo" />
            </a>
            <Text
              as="p"
              fontSize="24px"
              color={theme.studio.confirm.change.titleColor}
            >
              {greetingTitle}
            </Text>
          </div>

          {!isProfileDeleted ? (
            <>
              <Text className="confirm-row" as="p" fontSize="18px">
                {t("DeleteProfileConfirmation")}
              </Text>
              <Text className="confirm-row" as="p" fontSize="16px">
                {t("DeleteProfileConfirmationInfo")}
              </Text>

              <Button
                className="confirm-row"
                primary
                size="normal"
                label={t("DeleteProfileBtn")}
                tabIndex={1}
                isLoading={this.state.isLoading}
                onClick={this.onDeleteProfile}
              />
            </>
          ) : (
            <>
              <Text className="confirm-row" as="p" fontSize="18px">
                {t("DeleteProfileSuccessMessage")}
              </Text>
              <Text className="confirm-row" as="p" fontSize="16px">
                {t("DeleteProfileSuccessMessageInfo")}
              </Text>
            </>
          )}
        </div>
      </ProfileRemoveContainer>
    );
  }
}

ProfileRemove.propTypes = {
  location: PropTypes.object.isRequired,
};
const ProfileRemoveForm = (props) => (
  <Section>
    <Section.SectionBody>
      <ProfileRemove {...props} />
    </Section.SectionBody>
  </Section>
);

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  theme: auth.settingsStore.theme,
  logout: auth.logout,
}))(
  withRouter(
    withTranslation("Confirm")(withLoader(observer(ProfileRemoveForm)))
  )
);
