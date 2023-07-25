import React, { useState } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { I18nextProvider, useTranslation } from "react-i18next";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useParams } from "react-router-dom";
import Link from "@docspace/components/link";
import { ZendeskAPI } from "@docspace/common/components/Zendesk";

import { ReportDialog } from "SRC_DIR/components/dialogs";
import DocspaceLogo from "SRC_DIR/DocspaceLogo";

import i18n from "./i18n";

const StyledWrapper = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  margin: 0 16px;

  .logo {
    margin-bottom: 28px;
  }

  .link {
    margin-top: 24px;
  }

  #customized-text {
    max-width: 480px;
    text-align: center;
  }
`;

const Error520 = ({ errorLog, currentColorScheme, FirebaseHelper }) => {
  const { t } = useTranslation(["Common"]);
  const { error } = useParams();

  const [reportDialogVisible, setReportDialogVisible] = useState(false);

  const showDialog = () => {
    setReportDialogVisible(true);
  };

  const closeDialog = () => {
    setReportDialogVisible(false);
  };

  const onReloadClick = () => {
    window.location.reload();
  };

  ZendeskAPI("webWidget", "show");

  if (!FirebaseHelper.isEnabledDB)
    return <ErrorContainer headerText={t("SomethingWentWrong")} />;

  return (
    <StyledWrapper>
      <DocspaceLogo className="logo" />
      <ErrorContainer
        isPrimaryButton={false}
        headerText={t("SomethingWentWrong")}
        customizedBodyText={t("SomethingWentWrongDescription")}
        buttonText={t("SendReport")}
        onClickButton={showDialog}
      />
      <Link
        className="link"
        type="action"
        isHovered
        fontWeight={600}
        onClick={onReloadClick}
        color={currentColorScheme?.main?.accent}
      >
        {t("ReloadPage")}
      </Link>
      <ReportDialog
        visible={reportDialogVisible}
        onClose={closeDialog}
        error={errorLog}
      />
    </StyledWrapper>
  );
};

const Error520Wrapper = inject(({ auth }) => {
  const { currentColorScheme, firebaseHelper } = auth.settingsStore;

  return {
    currentColorScheme,
    FirebaseHelper: firebaseHelper,
  };
})(observer(Error520));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <Error520Wrapper {...props} />
  </I18nextProvider>
);
