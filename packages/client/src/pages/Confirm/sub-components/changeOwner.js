import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { inject, observer } from "mobx-react";
import {
  StyledPage,
  StyledBody,
  ButtonsWrapper,
  StyledContent,
} from "./StyledConfirm";
import withLoader from "../withLoader";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const ChangeOwnerForm = (props) => {
  const { t, greetingTitle } = props;
  console.log(props.linkData);
  return (
    <StyledPage>
      <StyledContent>
        <StyledBody>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>

          <FormWrapper>
            <Text className="subtitle">
              {t("ConfirmOwnerPortalTitle", { newOwner: "NEW OWNER" })}
            </Text>
            <ButtonsWrapper>
              <Button
                primary
                scale
                size="medium"
                label={t("Common:SaveButton")}
                tabIndex={2}
                isDisabled={false}
                //onClick={this.onAcceptClick} // call toast with t("ConfirmOwnerPortalSuccessMessage")
              />
              <Button
                scale
                size="medium"
                label={t("Common:CancelButton")}
                tabIndex={2}
                isDisabled={false}
                //onClick={this.onCancelClick}
              />
            </ButtonsWrapper>
          </FormWrapper>
        </StyledBody>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  defaultPage: auth.settingsStore.defaultPage,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ChangeOwnerForm))
    )
  )
);
