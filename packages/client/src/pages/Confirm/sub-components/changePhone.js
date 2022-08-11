import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import Section from "@docspace/common/components/Section";
import { inject, observer } from "mobx-react";
import { StyledPage, StyledBody, StyledHeader } from "./StyledConfirm";
import withLoader from "../withLoader";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const ChangePhoneForm = (props) => {
  const { t, greetingTitle } = props;
  const [currentNumber, setCurrentNumber] = useState("+00000000000");

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
          <div className="subtitle">
            <Text fontSize="16px" fontWeight="600" className="phone-title">
              {t("EnterPhone")}
            </Text>
            <Text>
              {t("CurrentNumber")}: {currentNumber}
            </Text>
            <Text>{t("PhoneSubtitle")}</Text>
          </div>

          <TextInput
            className="phone-input"
            id="phone"
            name="phone"
            type="phone"
            size="large"
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
            hasError={false}
            guide={false}
          />

          <Button
            primary
            scale
            size="medium"
            label={t("GetCode")}
            tabIndex={2}
            isDisabled={false}
          />
        </FormWrapper>
      </StyledBody>
    </StyledPage>
  );
};

const ChangePhoneFormWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <ChangePhoneForm {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
}))(
  withRouter(
    withTranslation("Confirm")(withLoader(observer(ChangePhoneFormWrapper)))
  )
);
