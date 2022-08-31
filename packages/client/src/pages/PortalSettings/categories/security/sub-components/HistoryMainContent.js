import React, { useEffect, useState } from "react";
import Text from "@docspace/components/text";

import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import styled from "styled-components";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import { hugeMobile, tablet } from "@docspace/components/utils/device";

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

  .download-button {
    width: auto;
    height: auto;
    font-size: 13px;
    line-height: 20px;
    padding-top: 5px;
    padding-bottom: 5px;

    @media ${tablet} {
      font-size: 14px;
      line-height: 16px;
      padding-top: 11px;
      padding-bottom: 11px;
    }

    @media ${hugeMobile} {
      width: 100%;
    }
  }
`;

const HistoryMainContent = (props) => {
  const {
    t,
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
  const [showReminder, setShowReminder] = useState(false);

  const lifeTimeHandler = (e) => {
    const reg = new RegExp(/^(\d){1,3}$/g);
    const condition = e.target.value === "";
    if ((e.target.value.match(reg) && e.target.value <= 180) || condition) {
      setLifeTime(e.target.value);
    }
  };

  const setLifeTimeSettings = async () => {
    if (loginHistory) {
      const data = {
        settings: {
          loginHistoryLifeTime: lifeTime,
          auditTrailLifeTime: securityLifetime.auditTrailLifeTime,
        },
      };
      try {
        await setLifetimeAuditSettings(data);
        setShowReminder(false);
        toastr.success(t("SuccessfullySaveSettingsMessage"));
      } catch (error) {
        console.error(error);
        toastr.error(error);
      }
    } else {
      const data = {
        settings: {
          loginHistoryLifeTime: securityLifetime.loginHistoryLifeTime,
          auditTrailLifeTime: lifeTime,
        },
      };

      try {
        await setLifetimeAuditSettings(data);
        setShowReminder(false);
        toastr.success(t("SuccessfullySaveSettingsMessage"));
      } catch (error) {
        console.error(error);
        toastr.error(error);
      }
    }
  };

  useEffect(() => {
    if (lifeTime === String(lifetime)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [lifeTime]);

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
        <SaveCancelButtons
          className="save-cancel"
          onSaveClick={setLifeTimeSettings}
          onCancelClick={() => setLifeTime(String(lifetime))}
          saveButtonLabel={saveButtonLabel}
          cancelButtonLabel={cancelButtonLabel}
          showReminder={showReminder}
          displaySettings={true}
          hasScroll={false}
        />
        <Text className="latest-text">{downloadText}</Text>
      </div>
      {content}
      <Button
        className="download-button"
        primary
        label={downloadReport}
        size="normal"
        minwidth="auto"
        onClick={() => getReport()}
      />
    </MainContainer>
  );
};

export default HistoryMainContent;
