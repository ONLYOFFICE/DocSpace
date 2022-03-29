import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import FieldContainer from "@appserver/components/field-container";
import Text from "@appserver/components/text";
import Section from "@appserver/common/components/Section";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import toastr from "studio/toastr";
import withLoader from "../withLoader";
import { mobile, tablet } from "@appserver/components/utils/device";

const StyledForm = styled(Box)`
  margin: 63px auto auto 216px;
  width: 570px;
  display: flex;
  flex-direction: column;

  @media ${tablet} {
    margin: 120px auto;
    width: 480px;
  }
  @media ${mobile} {
    margin: 72px 16px auto 8px;
    width: 311px;
  }

  .app-code-wrapper {
    @media ${tablet} {
      flex-direction: column;
    }
  }

  .app-code-continue-btn {
    @media ${tablet} {
      margin: 32px 0 0 0;
    }
  }

  .app-code-text {
    margin-bottom: 14px;
  }
`;

const TfaAuthForm = withLoader((props) => {
  const { t, loginWithCode, loginWithCodeAndCookie, location, history } = props;

  const [code, setCode] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const onSubmit = async () => {
    try {
      const { user, hash } = (location && location.state) || {};

      setIsLoading(true);

      if (user && hash) {
        const url = await loginWithCode(user, hash, code);
        history.push(url || "/");
      } else {
        const url = await loginWithCodeAndCookie(code);
        history.push(url || "/");
      }
    } catch (e) {
      setError(e);
      toastr.error(e);
    }
    setIsLoading(false);
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter" || target.code === "NumpadEnter") onSubmit();
  };

  const width = window.innerWidth;

  return (
    <StyledForm className="app-code-container">
      <Box className="app-code-description" marginProp="0 0 32px 0">
        <Text isBold fontSize="14px" className="app-code-text">
          {t("EnterAppCodeTitle")}
        </Text>
        <Text>{t("EnterAppCodeDescription")}</Text>
      </Box>
      <Box displayProp="flex" className="app-code-wrapper">
        <Box className="app-code-input">
          <FieldContainer
            labelVisible={false}
            hasError={error ? true : false}
            errorMessage={error}
          >
            <TextInput
              id="code"
              name="code"
              type="text"
              size={width <= 1024 ? "large" : "base"}
              scale
              isAutoFocussed
              tabIndex={1}
              placeholder={t("EnterCodePlaceholder")}
              isDisabled={isLoading}
              maxLength={6}
              onChange={(e) => {
                setCode(e.target.value);
                setError("");
              }}
              value={code}
              hasError={error ? true : false}
              onKeyDown={onKeyPress}
            />
          </FieldContainer>
        </Box>
        <Box className="app-code-continue-btn" marginProp="0 0 0 8px">
          <Button
            scale
            primary
            size={width <= 1024 ? "medium" : "normal"}
            tabIndex={3}
            label={
              isLoading
                ? t("Common:LoadingProcessing")
                : t("Common:ContinueButton")
            }
            isDisabled={!code.length || isLoading}
            isLoading={isLoading}
            onClick={onSubmit}
          />
        </Box>
      </Box>
    </StyledForm>
  );
});

const TfaAuthFormWrapper = (props) => {
  const { setIsLoaded, setIsLoading } = props;

  useEffect(async () => {
    setIsLoaded(true);
    setIsLoading(false);
  }, []);

  return (
    <Section>
      <Section.SectionBody>
        <TfaAuthForm {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth, confirm }) => ({
  setIsLoaded: confirm.setIsLoaded,
  setIsLoading: confirm.setIsLoading,
  loginWithCode: auth.loginWithCode,
  loginWithCodeAndCookie: auth.tfaStore.loginWithCodeAndCookie,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(observer(TfaAuthFormWrapper))
  )
);
