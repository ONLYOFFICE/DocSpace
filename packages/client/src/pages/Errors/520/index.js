import React, { useState } from "react";
import styled from "styled-components";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";

import { ReportDialog } from "SRC_DIR/components/dialogs";
import DocspaceLogo from "SRC_DIR/DocspaceLogo";

const StyledWrapper = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 28px;
  margin: 0 16px;
`;

const Error520 = ({ errorLog }) => {
  const { t } = useTranslation(["Common"]);

  const [reportDialogVisible, setReportDialogVisible] = useState(false);

  const showDialog = () => {
    setReportDialogVisible(true);
  };

  const closeDialog = () => {
    setReportDialogVisible(false);
  };

  return (
    <StyledWrapper>
      <DocspaceLogo />
      <ErrorContainer
        isPrimaryButton={false}
        headerText={t("SomethingWentWrong")}
        customizedBodyText={t("SomethingWentWrongDescription")}
        buttonText={t("SendReport")}
        onClickButton={showDialog}
      />
      <ReportDialog
        visible={reportDialogVisible}
        onClose={closeDialog}
        error={errorLog}
      />
    </StyledWrapper>
  );
};

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <Error520 {...props} />
  </I18nextProvider>
);
