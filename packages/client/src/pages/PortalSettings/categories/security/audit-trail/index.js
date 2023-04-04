import React, { useEffect } from "react";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { inject } from "mobx-react";
import { Consumer } from "@docspace/components/utils/context";
import { Table } from "./TableView/TableView";
import AuditRowContainer from "./RowView/AuditRowContainer";
import HistoryMainContent from "../sub-components/HistoryMainContent";

const AuditTrail = (props) => {
  const {
    t,
    getAuditTrail,
    auditTrailUsers,
    theme,
    viewAs,
    setLifetimeAuditSettings,
    getLifetimeAuditSettings,
    getAuditTrailReport,
    securityLifetime,
    isAuditAvailable,
    isLoadingDownloadReport,
  } = props;

  useEffect(() => {
    setDocumentTitle(t("AuditTrailNav"));

    if (isAuditAvailable) {
      getAuditTrail();
    }

    getLifetimeAuditSettings();
  }, []);

  const getContent = () => {
    return (
      <div className="content-wrapper">
        <Consumer>
          {(context) =>
            viewAs === "table" ? (
              <>
                <Table
                  theme={theme}
                  auditTrailUsers={auditTrailUsers}
                  sectionWidth={context.sectionWidth}
                  isSettingNotPaid={!isAuditAvailable}
                />
              </>
            ) : (
              <>
                <AuditRowContainer
                  sectionWidth={context.sectionWidth}
                  isSettingNotPaid={!isAuditAvailable}
                />
              </>
            )
          }
        </Consumer>
      </div>
    );
  };
  return (
    <>
      {securityLifetime && securityLifetime.auditTrailLifeTime && (
        <HistoryMainContent
          t={t}
          subHeader={t("AuditSubheader")}
          latestText={t("LoginLatestText")}
          storagePeriod={t("StoragePeriod")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Common:CancelButton")}
          downloadText={t("DownloadStatisticsText")}
          securityLifetime={securityLifetime}
          lifetime={securityLifetime.auditTrailLifeTime}
          setLifetimeAuditSettings={setLifetimeAuditSettings}
          content={getContent()}
          downloadReport={t("DownloadReportBtnText")}
          downloadReportDescription={t("DownloadReportDescription")}
          getReport={getAuditTrailReport}
          isSettingNotPaid={!isAuditAvailable}
          isLoadingDownloadReport={isLoadingDownloadReport}
        />
      )}
    </>
  );
};

export default inject(({ setup, auth }) => {
  const {
    getAuditTrail,
    security,
    viewAs,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    getAuditTrailReport,
    securityLifetime,
    isLoadingDownloadReport,
  } = setup;
  const { settingsStore, currentQuotaStore } = auth;
  const { theme } = settingsStore;
  const { isAuditAvailable } = currentQuotaStore;
  return {
    getAuditTrail,
    auditTrailUsers: security.auditTrail.users,
    theme,
    viewAs,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    getAuditTrailReport,
    securityLifetime,
    isAuditAvailable,
    isLoadingDownloadReport,
  };
})(withTranslation("Settings")(AuditTrail));
