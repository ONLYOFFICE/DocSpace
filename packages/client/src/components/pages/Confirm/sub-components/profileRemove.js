import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Section from "@docspace/common/components/Section";
import { inject, observer } from "mobx-react";
import { deleteSelf } from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";
import { StyledPage, StyledBody, StyledHeader } from "./StyledConfirm";
import withLoader from "../withLoader";

const ProfileRemoveForm = (props) => {
  const { t, greetingTitle, linkData, logout } = props;
  const [isProfileDeleted, setIsProfileDeleted] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const onDeleteProfile = () => {
    setIsLoading(true);

    deleteSelf(linkData.confirmHeader)
      .then((res) => {
        setIsLoading(false);
        setIsProfileDeleted(true);
        return logout(false);
      })
      .catch((e) => {
        setIsLoading(false);
        toastr.error(e);
      });
  };

  if (isProfileDeleted) {
    return (
      <StyledPage>
        <StyledBody>
          <StyledHeader>
            <Text fontSize="23px" fontWeight="700" className="title">
              {t("DeleteProfileSuccessMessage")}
            </Text>
            <Text fontSize="16px" fontWeight="600" className="confirm-subtitle">
              {t("DeleteProfileSuccessMessageInfo")}
            </Text>
          </StyledHeader>
        </StyledBody>
      </StyledPage>
    );
  }

  return (
    <StyledPage>
      <StyledBody>
        <StyledHeader>
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>
          <Text fontSize="16px" fontWeight="600" className="confirm-subtitle">
            {t("DeleteProfileConfirmation")}
          </Text>
          <Text className="info-delete">
            {t("DeleteProfileConfirmationInfo")}
          </Text>
        </StyledHeader>

        <Button
          className="confirm-button"
          primary
          size="normal"
          label={t("DeleteProfileBtn")}
          tabIndex={1}
          isDisabled={isLoading}
          onClick={onDeleteProfile}
        />
      </StyledBody>
    </StyledPage>
  );
};

const ProfileRemoveFormWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <ProfileRemoveForm {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  theme: auth.settingsStore.theme,
  logout: auth.logout,
}))(
  withRouter(
    withTranslation("Confirm")(withLoader(observer(ProfileRemoveFormWrapper)))
  )
);
