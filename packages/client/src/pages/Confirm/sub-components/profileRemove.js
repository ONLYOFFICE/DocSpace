import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Section from "@docspace/common/components/Section";
import { inject, observer } from "mobx-react";
import { deleteSelf } from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";
import { StyledPage, StyledBody, StyledContent } from "./StyledConfirm";
import withLoader from "../withLoader";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

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
        return logout();
      })
      .catch((e) => {
        setIsLoading(false);
        toastr.error(e);
      });
  };

  if (isProfileDeleted) {
    return (
      <StyledPage>
        <StyledContent>
          <StyledBody>
            <DocspaceLogo className="docspace-logo" />
            <Text fontSize="23px" fontWeight="700" className="title">
              {t("DeleteProfileSuccessMessage")}
            </Text>
            <Text fontSize="16px" fontWeight="600" className="confirm-subtitle">
              {t("DeleteProfileSuccessMessageInfo")}
            </Text>
          </StyledBody>
        </StyledContent>
      </StyledPage>
    );
  }

  return (
    <StyledPage>
      <StyledContent>
        <StyledBody>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>

          <FormWrapper>
            <div className="subtitle">
              <Text
                fontSize="16px"
                fontWeight="600"
                className="delete-profile-confirm"
              >
                {t("DeleteProfileConfirmation")}
              </Text>
              <Text>{t("DeleteProfileConfirmationInfo")}</Text>
            </div>

            <Button
              primary
              scale
              size="medium"
              label={t("DeleteProfileBtn")}
              tabIndex={1}
              isDisabled={isLoading}
              onClick={onDeleteProfile}
            />
          </FormWrapper>
        </StyledBody>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  theme: auth.settingsStore.theme,
  logout: auth.logout,
}))(
  withRouter(
    withTranslation("Confirm")(withLoader(observer(ProfileRemoveForm)))
  )
);
