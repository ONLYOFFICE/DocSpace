import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Section from "@docspace/common/components/Section";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import { suspendPortal } from "@docspace/common/api/portal";
import {
  StyledPage,
  StyledBody,
  StyledHeader,
  ButtonsWrapper,
} from "./StyledConfirm";

import withLoader from "../withLoader";

const DeactivatePortal = (props) => {
  const { t, greetingTitle, linkData, history } = props;

  const onDeleteClick = async () => {
    try {
      await suspendPortal(linkData.confirmHeader);
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
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>
        </StyledHeader>

        <Text className="subtitle">{t("PortalDeactivateTitle")}</Text>
        <ButtonsWrapper>
          <Button
            scale
            primary
            size="medium"
            label={t("Settings:Deactivate")}
            tabIndex={1}
            onClick={onDeleteClick}
          />
          <Button
            scale
            size="medium"
            label={t("Common:Cancel")}
            tabIndex={1}
            onClick={onCancelClick}
          />
        </ButtonsWrapper>
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
}))(
  withRouter(
    withTranslation(["Confirm", "Settings", "Common"])(
      withLoader(observer(DeactivatePortalWrapper))
    )
  )
);
