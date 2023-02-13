import React, { useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import { suspendPortal } from "@docspace/common/api/portal";
import {
  StyledPage,
  StyledBody,
  StyledContent,
  ButtonsWrapper,
} from "./StyledConfirm";

import withLoader from "../withLoader";

import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const DeactivatePortal = (props) => {
  const {
    t,
    greetingTitle,
    linkData,
    history,
    companyInfoSettingsData,
  } = props;
  const [isDeactivate, setIsDeactivate] = useState(false);

  const url = companyInfoSettingsData?.site
    ? companyInfoSettingsData.site
    : "https://onlyoffice.com";

  const onDeactivateClick = async () => {
    try {
      await suspendPortal(linkData.confirmHeader);
      setIsDeactivate(true);
      setTimeout(() => (location.href = url), 10000);
    } catch (e) {
      toastr.error(e);
    }
  };

  const onCancelClick = () => {
    history.push("/");
  };

  return (
    <StyledPage>
      <StyledContent>
        <StyledBody>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>

          <FormWrapper>
            {isDeactivate ? (
              <Trans t={t} i18nKey="SuccessDeactivate" ns="Confirm">
                Your account has been successfully deactivated. In 10 seconds
                you will be redirected to the
                <Link isHovered href={url}>
                  site
                </Link>
              </Trans>
            ) : (
              <>
                <Text className="subtitle">{t("PortalDeactivateTitle")}</Text>
                <ButtonsWrapper>
                  <Button
                    scale
                    primary
                    size="medium"
                    label={t("Settings:Deactivate")}
                    tabIndex={1}
                    onClick={onDeactivateClick}
                  />
                  <Button
                    scale
                    size="medium"
                    label={t("Common:CancelButton")}
                    tabIndex={1}
                    onClick={onCancelClick}
                  />
                </ButtonsWrapper>
              </>
            )}
          </FormWrapper>
        </StyledBody>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  theme: auth.settingsStore.theme,
  companyInfoSettingsData: auth.settingsStore.companyInfoSettingsData,
}))(
  withRouter(
    withTranslation(["Confirm", "Settings", "Common"])(
      withLoader(observer(DeactivatePortal))
    )
  )
);
