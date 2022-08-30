import React, { useEffect, useState } from "react";
import Text from "@docspace/components/text";

import TextInput from "@docspace/components/text-input";
import StyledSaveCancelButtons from "@docspace/components/save-cancel-buttons";
import styled from "styled-components";
import Button from "@docspace/components/button";

import { hugeMobile } from "@docspace/components/utils/device";

const StyledTextInput = styled(TextInput)`
  margin-top: 4px;
  margin-bottom: 24px;
  width: 350px;

  @media ${hugeMobile} {
    width: 100%;
  }
`;

const MainContainer = styled.div`
  width: 100%;

  .main-wrapper {
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

  .content-wrapper {
    margin-top: 16px;
    margin-bottom: 24px;
    .table-container_header {
      position: absolute;
      z-index: 1 !important;
    }
  }
`;

const HistoryMainContent = (props) => {
  const {
    loginHistory,
    latestText,
    subHeader,
    storagePeriod,
    lifetime,
    saveButtonLabel,
    cancelButtonLabel,
    downloadText,
    setLifetimeAuditSettings,
    securityLifetime,
    content,
    downloadReport,
    getReport,
  } = props;

  const [lifeTime, setLifeTime] = useState(String(lifetime) || "180");

  const lifeTimeHandler = (e) => {
    const reg = new RegExp(/^(\d){1,3}$/g);
    const condition = e.target.value === "";
    if ((e.target.value.match(reg) && e.target.value <= 180) || condition) {
      setLifeTime(e.target.value);
    }
  };

  const setLifeTimeSettings = () => {
    if (loginHistory) {
      const data = {
        settings: {
          loginHistoryLifeTime: lifeTime,
          auditTrailLifeTime: securityLifetime.auditTrailLifeTime,
        },
      };
      return setLifetimeAuditSettings(data);
    }
    const data = {
      settings: {
        loginHistoryLifeTime: securityLifetime.loginHistoryLifeTime,
        auditTrailLifeTime: lifeTime,
      },
    };

    return setLifetimeAuditSettings(data);
  };

  return (
    <MainContainer>
      <div className="main-wrapper">
        <Text fontSize="13px" color="#657077">
          {subHeader}
        </Text>
        <Text className="latest-text">{latestText} </Text>

        <label className="storage-label" htmlFor="storage-period">
          {storagePeriod}
        </label>
        <StyledTextInput
          onChange={lifeTimeHandler}
          value={lifeTime}
          size="base"
          id="storage-period"
          type="text"
        />
        <div>
          <StyledSaveCancelButtons
            onSaveClick={setLifeTimeSettings}
            onCancelClick={() => setLifeTime(lifetime)}
            saveButtonLabel={saveButtonLabel}
            cancelButtonLabel={cancelButtonLabel}
            className="save-cancel"
            showReminder={true}
          />
        </div>
        <Text className="latest-text">{downloadText}</Text>
      </div>
      {content}
      <Button
        primary
        isHovered
        label={downloadReport}
        size="normal"
        onClick={() => getReport()}
      />
    </MainContainer>
  );
};

export default HistoryMainContent;
