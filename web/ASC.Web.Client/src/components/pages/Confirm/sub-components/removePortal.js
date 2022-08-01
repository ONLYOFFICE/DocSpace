import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Section from "@appserver/common/components/Section";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import toastr from "@appserver/components/toast/toastr";
import { deletePortal } from "@appserver/common/api/portal";
import { StyledPage, StyledBody, StyledHeader } from "./StyledConfirm";

import withLoader from "../withLoader";

const RemovePortal = (props) => {
  const { t, greetingTitle, linkData } = props;

  const onDeleteClick = async () => {
    try {
      await deletePortal(linkData.confirmHeader);
    } catch (e) {
      toastr.error(e);
    }
  };

  return (
    <StyledPage>
      <StyledBody>
        <StyledHeader>
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>
        </StyledHeader>

        <Text>{t("PortalRemoveTitle")}</Text>
        <Button
          className="confirm-button"
          primary
          size="medium"
          label={t("Common:Delete")}
          tabIndex={1}
          onClick={onDeleteClick}
        />
      </StyledBody>
    </StyledPage>
  );
};

const RemovePortalWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <RemovePortal {...props} />
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
      withLoader(observer(RemovePortalWrapper))
    )
  )
);
