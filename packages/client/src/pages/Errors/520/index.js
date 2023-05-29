import React, { useState } from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";

import { ReportDialog } from "SRC_DIR/components/dialogs";

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
    <>
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
    </>
  );
};

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <Error520 {...props} />
  </I18nextProvider>
);
