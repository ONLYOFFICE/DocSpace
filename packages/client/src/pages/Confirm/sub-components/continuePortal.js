import React, { useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import { continuePortal } from "@docspace/common/api/portal";
import {
  StyledPage,
  StyledBody,
  StyledContent,
  ButtonsWrapper,
} from "./StyledConfirm";

import withLoader from "../withLoader";

import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const ContinuePortal = (props) => {
  const { t, greetingTitle, linkData, history } = props;
  const [isReactivate, setIsReactivate] = useState(false);

  const onRestoreClick = async () => {
    try {
      await continuePortal(linkData.confirmHeader);
      setIsReactivate(true);
      setTimeout(() => (location.href = "/"), 10000);
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
            {isReactivate ? (
              <Trans t={t} i18nKey="SuccessReactivate" ns="Confirm">
                Your account has been successfully reactivated. In 10 seconds
                you will be redirected to the
                <Link isHovered href="/">
                  portal
                </Link>
              </Trans>
            ) : (
              <>
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
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(withLoader(observer(ContinuePortal)))
  )
);
