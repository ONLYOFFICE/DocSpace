import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import PageLayout from "@appserver/common/components/PageLayout";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import toastr from "studio/toastr";

const StyledForm = styled(Box)`
  margin: 63px auto auto 216px;
  width: 570px;
  display: flex;
  flex-direction: column;
  .app-code-text {
    margin-bottom: 14px;
  }
`;

const TfaAuthForm = (props) => {
  const { t, loginWithCode, location, history } = props;

  const [code, setCode] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const onSubmit = async () => {
    try {
      const { user, hash } = (location && location.state) || {};

      if (user && hash) {
        const url = await loginWithCode(user, hash, code);
        history.push(url || "/");
      } else {
        //TODO: call method to auth tfa with cookie
        throw "Not implemented auth tfa with cookie";
      }
    } catch (e) {
      setError(e);
      toastr.error(e);
    }
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter") onSubmit();
  };

  return (
    <StyledForm className="app-code-container">
      <Box className="app-code-description" marginProp="0 0 32px 0">
        <Text isBold fontSize="14px" className="app-code-text">
          {t("EnterAppCodeTitle")}
        </Text>
        <Text>{t("EnterAppCodeDescription")}</Text>
      </Box>
      <Box displayProp="flex">
        <Box className="app-code-input">
          <TextInput
            id="code"
            name="code"
            type="text"
            size="base"
            scale
            isAutoFocussed
            tabIndex={1}
            placeholder={t("EnterCodePlaceholder")}
            isDisabled={isLoading}
            onChange={(e) => setCode(e.target.value)}
            value={code}
          />
        </Box>
        <Box className="app-code-continue-btn" marginProp="0 0 0 8px">
          <Button
            primary
            size="medium"
            tabIndex={3}
            label={isLoading ? t("LoadingProcessing") : t("Continue")}
            isDisabled={!code.length || isLoading}
            isLoading={isLoading}
            onClick={onSubmit}
          />
        </Box>
      </Box>
    </StyledForm>
  );
};

const TfaAuthFormWrapper = (props) => {
  return (
    <PageLayout>
      <PageLayout.SectionBody>
        <TfaAuthForm {...props} />
      </PageLayout.SectionBody>
    </PageLayout>
  );
};

export default inject(({ auth }) => ({
  isLoaded: auth.isLoaded,
  loginWithCode: auth.loginWithCode,
}))(withRouter(withTranslation("Confirm")(observer(TfaAuthFormWrapper))));
