import React, { useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Section from "@docspace/common/components/Section";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import { suspendPortal } from "@docspace/common/api/portal";
import {
  StyledPage,
  StyledBody,
  StyledHeader,
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
      <StyledBody>
        <StyledHeader>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>
        </StyledHeader>

        <FormWrapper>
          {isDeactivate ? (
            <Trans t={t} i18nKey="SuccessDeactivate" ns="Confirm">
              Your account has been successfully deactivated. In 10 seconds you
              will be redirected to the
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
                  label={t("Common:Cancel")}
                  tabIndex={1}
                  onClick={onCancelClick}
                />
              </ButtonsWrapper>
            </>
          )}
        </FormWrapper>
      </StyledBody>
    </StyledPage>
  );
};

const DeactivatePortalWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <DeactivatePortal {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  theme: auth.settingsStore.theme,
  companyInfoSettingsData: auth.settingsStore.companyInfoSettingsData,
}))(
  withRouter(
    withTranslation(["Confirm", "Settings", "Common"])(
      withLoader(observer(DeactivatePortalWrapper))
    )
  )
);
