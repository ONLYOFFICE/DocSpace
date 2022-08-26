import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import Text from "@docspace/components/text";
import { inject } from "mobx-react";
import TextInput from "@docspace/components/text-input";
import StyledSaveCancelButtons from "@docspace/components/save-cancel-buttons";
import styled from "styled-components";
import Button from "@docspace/components/button";
import { Consumer } from "@docspace/components/utils/context";
import { Table } from "./TableView/TableView";
import HistoryRowContainer from "./RowView/HistoryRowContainer";

const MainContainer = styled.div`
  width: 100%;

  .history-content {
    max-width: 700px;
  }

  .save-cancel {
    padding: 0;
    position: static;
    display: block;
  }

  .login-subheader {
    font-size: 13px;
    color: #657077;
  }

  .latest-text {
    font-size: 13px;
    padding: 20px 0;
  }

  .storage-label {
    font-weight: 600;
  }

  .history-wrapper {
    margin-top: 16px;
    margin-bottom: 24px;
    .table-container_header {
      position: absolute;
    }
  }
`;

const StyledTextInput = styled(TextInput)`
  margin-top: 4px;
  margin-bottom: 24px;
`;

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
  } = props;

  const [loginLifeTime, setLoginLifeTime] = useState("180");

  const inputHandler = (e) => {
    setLoginLifeTime(e.target.value);
  };

  useEffect(() => {
    setDocumentTitle(t("LoginHistoryTitle"));

    getLoginHistory();
    getLifetimeAuditSettings();
  }, []);

  const setLifeTimeHandler = () => {
    const data = {
      settings: {
        loginHistoryLifeTime: loginLifeTime,
        auditTrailLifeTime: securityLifetime.auditTrailLifeTime,
      },
    };
    setLifetimeAuditSettings(data);
  };

  return (
    <MainContainer>
      <div className="history-content">
        <Text fontSize="13px" color="#657077">
          {t("LoginSubheaderTitle")}{" "}
        </Text>
        <Text className="latest-text">{t("LoginLatestText")} </Text>

        <label className="storage-label" htmlFor="storage-period">
          {t("StoragePeriod")}
        </label>
        <StyledTextInput
          onChange={inputHandler}
          value={loginLifeTime}
          size="big"
          id="storage-period"
          type="text"
        />
        <div>
          <StyledSaveCancelButtons
            onSaveClick={setLifeTimeHandler}
            saveButtonLabel={t("Common:SaveButton")}
            cancelButtonLabel={t("Common:CancelButton")}
            className="save-cancel"
            showReminder={true}
          />
        </div>
        <Text className="latest-text">{t("LoginDownloadText")}</Text>
      </div>

      <div className="history-wrapper">
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
      <Button
        primary
        isHovered
        label={t("DownloadReportBtn")}
        size="normal"
        onClick={() => getLoginHistoryReport()}
      />
    </MainContainer>
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

  return {
    getLoginHistory,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    securityLifetime,
    historyUsers: security.loginHistory.users,
    theme,
    viewAs,
    getLoginHistoryReport,
  };
})(withTranslation("Settings")(withRouter(LoginHistory)));
