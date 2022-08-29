import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import Text from "@docspace/components/text";
import { inject } from "mobx-react";
import styled from "styled-components";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import { Consumer } from "@docspace/components/utils/context";
import StyledSaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { Table } from "./TableView/TableView";
import AuditRowContainer from "./RowView/AuditRowContainer";

const MainContainer = styled.div`
  width: 100%;

  .audit-content {
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

  .audit-wrapper {
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
  } = props;

  const [value, setValue] = useState("180");

  const inputHandler = (e) => {
    setValue(e.target.value);
  };

  useEffect(() => {
    setDocumentTitle(t("AuditTrailNav"));

    getAuditTrail();

    getLifetimeAuditSettings();
  }, []);
  return (
    <MainContainer>
      <div className="audit-content">
        <Text fontSize="13px" color="#657077">
          {t("AuditSubheader")}
        </Text>
        <Text className="latest-text">{t("LoginLatestText")} </Text>
        <label className="storage-label" htmlFor="storage-period">
          {t("StoragePeriod")}
        </label>
        <StyledTextInput
          onChange={inputHandler}
          value={value}
          size="big"
          id="storage-period"
          type="text"
        />
        <div>
          <StyledSaveCancelButtons
            saveButtonLabel={t("Common:SaveButton")}
            cancelButtonLabel={t("Common:CancelButton")}
            className="save-cancel"
            showReminder={true}
          />
        </div>
        <Text className="latest-text">{t("AuditDownloadText")}</Text>
      </div>
      <div className="audit-wrapper">
        <Consumer>
          {(context) =>
            viewAs === "table" ? (
              <>
                <Table
                  theme={theme}
                  auditTrailUsers={auditTrailUsers}
                  sectionWidth={context.sectionWidth}
                />
              </>
            ) : (
              <>
                <AuditRowContainer sectionWidth={context.sectionWidth} />
              </>
            )
          }
        </Consumer>
      </div>
      <Button
        primary
        isHovered
        label="DownloadReport"
        size="normal"
        onClick={() => getAuditTrailReport()}
      />
    </MainContainer>
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
  } = setup;
  const { theme } = auth.settingsStore;

  return {
    getAuditTrail,
    auditTrailUsers: security.auditTrail.users,
    theme,
    viewAs,
    getLifetimeAuditSettings,
    setLifetimeAuditSettings,
    getAuditTrailReport,
  };
})(withTranslation("Settings")(withRouter(AuditTrail)));
