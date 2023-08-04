import React, { useEffect } from "react";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { inject } from "mobx-react";
import styled from "styled-components";
import { Consumer } from "@docspace/components/utils/context";
import { Table } from "./TableView/TableView";
import HistoryRowContainer from "./RowView/HistoryRowContainer";
import HistoryMainContent from "../sub-components/HistoryMainContent";

const LoginHistory = (props) => {
  const {
    t,
    getLoginHistory,
    historyUsers,
    theme,
    viewAs,
    getLoginHistoryReport,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    securityLifetime,
    isAuditAvailable,
  } = props;

  useEffect(() => {
    setDocumentTitle(t("LoginHistoryTitle"));

    if (isAuditAvailable) {
      getLoginHistory();
    }

    getLifetimeAuditSettings();
  }, []);

  const getContent = () => {
    return (
      <>
        <div className="content-wrapper">
          <Consumer>
            {(context) =>
              viewAs === "table" ? (
                <>
                  <Table
                    theme={theme}
                    historyUsers={historyUsers}
                    sectionWidth={context.sectionWidth}
                  />
                </>
              ) : (
                <>
                  <HistoryRowContainer sectionWidth={context.sectionWidth} />
                </>
              )
            }
          </Consumer>
        </div>
      </>
    );
  };

  return (
    <>
      {securityLifetime && securityLifetime.loginHistoryLifeTime && (
        <HistoryMainContent
          t={t}
          loginHistory={true}
          subHeader={t("LoginSubheaderTitle")}
          latestText={t("LoginLatestText")}
          storagePeriod={t("StoragePeriod")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Common:CancelButton")}
          downloadText={t("DownloadStatisticsText")}
          lifetime={securityLifetime.loginHistoryLifeTime}
          securityLifetime={securityLifetime}
          setLifetimeAuditSettings={setLifetimeAuditSettings}
          content={getContent()}
          downloadReport={t("DownloadReportBtnText")}
          downloadReportDescription={t("DownloadReportDescription")}
          getReport={getLoginHistoryReport}
          isSettingNotPaid={!isAuditAvailable}
        />
      )}
    </>
  );
};

export default inject(({ setup, auth }) => {
  const {
    getLoginHistory,
    security,
    viewAs,
    getLoginHistoryReport,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    securityLifetime,
  } = setup;
  const { theme } = auth.settingsStore;

  const { isAuditAvailable } = auth.currentQuotaStore;

  return {
    getLoginHistory,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    securityLifetime,
    historyUsers: security.loginHistory.users,
    theme,
    viewAs,
    getLoginHistoryReport,
    isAuditAvailable,
  };
})(withTranslation("Settings")(LoginHistory));
