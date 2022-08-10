import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Section from "@docspace/common/components/Section";
import { inject, observer } from "mobx-react";
import {
  StyledPage,
  StyledBody,
  StyledHeader,
  ButtonsWrapper,
} from "./StyledConfirm";
import withLoader from "../withLoader";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const ChangeOwnerForm = (props) => {
  const { t, greetingTitle } = props;
  console.log(props.linkData);
  return (
    <StyledPage>
      <StyledBody>
        <StyledHeader>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>

          <Text className="subtitle">
            {t("ConfirmOwnerPortalTitle", { newOwner: "NEW OWNER" })}
          </Text>
        </StyledHeader>

        <FormWrapper>
          <ButtonsWrapper>
            <Button
              className="button"
              primary
              size="normal"
              label={t("Common:SaveButton")}
              tabIndex={2}
              isDisabled={false}
              //onClick={this.onAcceptClick} // call toast with t("ConfirmOwnerPortalSuccessMessage")
            />
            <Button
              className="button"
              size="normal"
              label={t("Common:CancelButton")}
              tabIndex={2}
              isDisabled={false}
              //onClick={this.onCancelClick}
            />
          </ButtonsWrapper>
        </FormWrapper>
      </StyledBody>
    </StyledPage>
  );
};

const ChangeOwnerFormWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <ChangeOwnerForm {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  defaultPage: auth.settingsStore.defaultPage,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ChangeOwnerFormWrapper))
    )
  )
);
