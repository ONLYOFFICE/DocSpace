import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import toastr from "@appserver/components/toast/toastr";
import SectionLoader from "../sub-components/section-loader";
import { getLanguage } from "@appserver/common/utils";
import { isMobile } from "react-device-detect";

const MainContainer = styled.div`
  width: 100%;

  .page-subtitle {
    margin-bottom: 10px;
  }

  .box {
    margin-top: 20px;
    margin-bottom: 24px;
  }
`;

const ButtonsWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 8px;
  align-items: center;

  @media (max-width: 375px) {
    position: absolute;
    bottom: 16px;
    width: calc(100vw - 32px);

    .button {
      height: 40px;
      width: 100%;
    }

    .reminder {
      position: absolute;
      bottom: 48px;
    }
  }
`;

const TwoFactorAuth = (props) => {
  const { t } = props;
  const [type, setType] = useState("none");
  const [currentState, setCurrentState] = useState("");
  const [smsDisabled, setSmsDisabled] = useState(false);
  const [appDisabled, setAppDisabled] = useState(false);
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = async () => {
    const { getTfaType, getTfaSettings } = props;
    const type = await getTfaType();
    setType(type);
    setCurrentState(type);

    const settings = await getTfaSettings();
    setSmsDisabled(settings[0].avaliable);
    setAppDisabled(settings[1].avaliable);
  };

  useEffect(() => {
    getSettings();
    setIsLoading(true);
  }, []);

  const onSelectTfaType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
      setShowReminder(true);
    }
    if (e.target.value === currentState) {
      setShowReminder(false);
    }
  };

  const onSaveClick = () => {
    const { t, setTfaSettings, getTfaConfirmLink, history } = props;

    setTfaSettings(type).then((res) => {
      toastr.success(t("SuccessfullySaveSettingsMessage"));
      if (type !== "none") {
        getTfaConfirmLink(res).then((link) =>
          history.push(link.replace(window.location.origin, ""))
        );
      }
      setType(type);
      setShowReminder(false);
    });
  };

  const onCancelClick = () => {
    setShowReminder(false);
    setType(currentState);
  };

  const lng = getLanguage(localStorage.getItem("language") || "en");
  if (!isLoading) return <SectionLoader />;
  return (
    <MainContainer>
      {isMobile && (
        <>
          <Text className="page-subtitle">{t("TwoFactorAuthHelper")}</Text>
          <Link
            className="learn-more"
            target="_blank"
            href={`https://helpcenter.onlyoffice.com/${lng}/administration/two-factor-authentication.aspx`}
          >
            {t("Common:LearnMore")}
          </Link>
        </>
      )}

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
            value: "none",
          },
          {
            label: t("BySms"),
            value: "sms",
            disabled: !smsDisabled,
          },
          {
            label: t("ByApp"),
            value: "app",
            disabled: !appDisabled,
          },
        ]}
        selected={type}
        onClick={onSelectTfaType}
      />

      <ButtonsWrapper>
        <Button
          label={t("Common:SaveButton")}
          size="small"
          primary={true}
          className="button"
          onClick={onSaveClick}
          isDisabled={!showReminder}
        />
        <Button
          label={t("Common:CancelButton")}
          size="small"
          className="button"
          onClick={onCancelClick}
          isDisabled={!showReminder}
        />
        {showReminder && (
          <Text
            color="#A3A9AE"
            fontSize="12px"
            fontWeight="600"
            className="reminder"
          >
            {t("YouHaveUnsavedChanges")}
          </Text>
        )}
      </ButtonsWrapper>
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const { organizationName } = auth.settingsStore;
  const {
    getTfaType,
    getTfaSettings,
    setTfaSettings,
    getTfaConfirmLink,
  } = auth.tfaStore;

  return {
    organizationName,
    getTfaType,
    getTfaSettings,
    setTfaSettings,
    getTfaConfirmLink,
  };
})(
  withTranslation(["Settings", "Common"])(withRouter(observer(TwoFactorAuth)))
);
