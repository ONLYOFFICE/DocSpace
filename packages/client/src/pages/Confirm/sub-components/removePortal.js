import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";

import { deletePortal } from "@docspace/common/api/portal";
import {
  StyledPage,
  StyledBody,
  StyledContent,
  ButtonsWrapper,
} from "./StyledConfirm";

import withLoader from "../withLoader";

import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const RemovePortal = (props) => {
  const {
    t,
    greetingTitle,
    linkData,
    history,
    companyInfoSettingsData,
  } = props;
  const [isRemoved, setIsRemoved] = useState(false);

  const url = companyInfoSettingsData?.site
    ? companyInfoSettingsData.site
    : "https://onlyoffice.com";

  const onDeleteClick = async () => {
    try {
      await deletePortal(linkData.confirmHeader);
      setIsRemoved(true);
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
            {isRemoved ? (
              <Text>
                <Trans t={t} i18nKey="SuccessRemoved" ns="Confirm">
                  Your account has been successfully removed. In 10 seconds you
                  will be redirected to the
                  <Link isHovered href={url}>
                    site
                  </Link>
                </Trans>
              </Text>
            ) : (
              <>
                <Text className="subtitle">{t("PortalRemoveTitle")}</Text>
                <ButtonsWrapper>
                  <Button
                    primary
                    scale
                    size="medium"
                    label={t("Common:Delete")}
                    tabIndex={1}
                    onClick={onDeleteClick}
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
    withTranslation(["Confirm", "Common"])(withLoader(observer(RemovePortal)))
  )
);
