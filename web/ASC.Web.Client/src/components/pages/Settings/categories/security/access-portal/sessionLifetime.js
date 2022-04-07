import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import toastr from "@appserver/components/toast/toastr";
import Buttons from "../sub-components/buttons";
import { LearnMoreWrapper } from "../StyledSecurity";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";

const MainContainer = styled.div`
  width: 100%;

  .lifetime {
    margin-top: 16px;
    margin-bottom: 8px;
  }
`;

const SessionLifetime = (props) => {
  const { t } = props;
  const [type, setType] = useState(false);
  const [sessionLifetime, setSessionLifetime] = useState("0");
  const [showReminder, setShowReminder] = useState(false);

  const onSelectType = (e) => {
    setType(e.target.value === "enable" ? true : false);
  };

  const onChangeInput = (e) => {
    setSessionLifetime(e.target.value);
  };

  const onSaveClick = () => {};

  const onCancelClick = () => {};

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text>{t("SessionLifetimeHelper")}</Text>
      </LearnMoreWrapper>

      <RadioButtonGroup
        className="box"
        fontSize="13px"
        fontWeight="400"
        name="group"
        orientation="vertical"
        spacing="8px"
        options={[
          {
            label: t("Disabled"),
            value: "disabled",
          },
          {
            label: t("Common:Enable"),
            value: "enable",
          },
        ]}
        selected={type ? "enable" : "disabled"}
        onClick={onSelectType}
      />

      {type && (
        <>
          <Text className="lifetime" fontSize="15px" fontWeight="600">
            {t("Lifetime")}
          </Text>
          <TextInput
            isAutoFocussed={true}
            value={sessionLifetime}
            onChange={onChangeInput}
          />
        </>
      )}

      <Buttons
        t={t}
        showReminder={showReminder}
        onSaveClick={onSaveClick}
        onCancelClick={onCancelClick}
      />
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const { sessionLifetime, setSessionLifetimeSettings } = auth.settingsStore;

  return {
    sessionLifetime,
    setSessionLifetimeSettings,
  };
})(
  withTranslation(["Settings", "Common"])(withRouter(observer(SessionLifetime)))
);
