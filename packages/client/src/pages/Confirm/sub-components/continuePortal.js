import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Section from "@docspace/common/components/Section";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import { continuePortal } from "@docspace/common/api/portal";
import {
  StyledPage,
  StyledBody,
  StyledHeader,
  ButtonsWrapper,
} from "./StyledConfirm";

import withLoader from "../withLoader";

import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const ContinuePortal = (props) => {
  const { t, greetingTitle, linkData, history } = props;

  const onRestoreClick = async () => {
    try {
      await continuePortal(linkData.confirmHeader);
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
          <Text className="subtitle">{t("PortalContinueTitle")}</Text>
          <ButtonsWrapper>
            <Button
              primary
              scale
              size="medium"
              label={t("Reactivate")}
              tabIndex={1}
              onClick={onRestoreClick}
            />
            <Button
              scale
              size="medium"
              label={t("Common:Cancel")}
              tabIndex={1}
              onClick={onCancelClick}
            />
          </ButtonsWrapper>
        </FormWrapper>
      </StyledBody>
    </StyledPage>
  );
};

const ContinuePortalWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <ContinuePortal {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  theme: auth.settingsStore.theme,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ContinuePortalWrapper))
    )
  )
);
