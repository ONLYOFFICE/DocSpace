import { useState, useEffect } from "react";
import { isMobile } from "react-device-detect";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import {
  LearnMoreWrapper,
  StyledBruteForceProtection,
} from "../StyledSecurity";
import isEqual from "lodash/isEqual";
import FieldContainer from "@docspace/components/field-container";
import toastr from "@docspace/components/toast/toastr";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import Text from "@docspace/components/text";
import { size } from "@docspace/components/utils/device";
import { useNavigate, useLocation } from "react-router-dom";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import BruteForceProtectionLoader from "../sub-components/loaders/brute-force-protection-loader";
import Link from "@docspace/components/link";

const BruteForceProtection = (props) => {
  const {
    t,
    numberAttempt,
    blockingTime,
    checkPeriod,
    setBruteForceProtection,
    getBruteForceProtection,
    initSettings,
    isInit,
    bruteForceProtectionUrl,
  } = props;

  const defaultNumberAttempt = numberAttempt?.toString();
  const defaultBlockingTime = blockingTime?.toString();
  const defaultCheckPeriod = checkPeriod?.toString();

  const [currentNumberAttempt, setCurrentNumberAttempt] =
    useState(defaultNumberAttempt);

  const [currentBlockingTime, setCurrentBlockingTime] =
    useState(defaultBlockingTime);
  const [currentCheckPeriod, setCurrentCheckPeriod] =
    useState(defaultCheckPeriod);

  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingSave, setIsLoadingSave] = useState(false);

  const [hasErrorNumberAttempt, setHasErrorNumberAttempt] = useState(false);
  const [hasErrorCheckPeriod, setHasErrorCheckPeriod] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (currentNumberAttempt == null || currentCheckPeriod == null) return;

    if (parseInt(currentNumberAttempt) === 0 || !currentNumberAttempt?.trim())
      setHasErrorNumberAttempt(true);
    else if (hasErrorNumberAttempt) setHasErrorNumberAttempt(false);

    if (parseInt(currentCheckPeriod) === 0 || !currentCheckPeriod?.trim())
      setHasErrorCheckPeriod(true);
    else if (hasErrorCheckPeriod) setHasErrorCheckPeriod(false);
  }, [
    currentNumberAttempt,
    currentCheckPeriod,
    hasErrorNumberAttempt,
    hasErrorCheckPeriod,
  ]);

  useEffect(() => {
    if (!isInit) return;
    getSettings();
  }, [isLoading]);

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);

    if (!isInit) initSettings().then(() => setIsLoading(true));
    else setIsLoading(true);

    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  useEffect(() => {
    if (!isLoading) return;

    const defaultSettings = getFromSessionStorage(
      "defaultBruteForceProtection"
    );

    let finishBlockingTime = currentBlockingTime;

    if (
      finishBlockingTime === "" ||
      finishBlockingTime.replace(/^0+/, "") === ""
    )
      finishBlockingTime = "0";
    else finishBlockingTime = finishBlockingTime.replace(/^0+/, "");

    const newSettings = {
      numberAttempt: currentNumberAttempt.replace(/^0+/, ""),
      blockingTime: finishBlockingTime,
      checkPeriod: currentCheckPeriod.replace(/^0+/, ""),
    };

    saveToSessionStorage("currentBruteForceProtection", newSettings);

    if (isEqual(defaultSettings, newSettings)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [currentNumberAttempt, currentBlockingTime, currentCheckPeriod]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      location.pathname.includes("brute-force-protection") &&
      navigate("/portal-settings/security/access-portal");
  };

  const getSettings = () => {
    const currentSettings = getFromSessionStorage(
      "currentBruteForceProtection"
    );

    let finishBlockingTime = defaultBlockingTime;

    if (
      finishBlockingTime === "" ||
      finishBlockingTime.replace(/^0+/, "") === ""
    )
      finishBlockingTime = "0";
    else finishBlockingTime = finishBlockingTime.replace(/^0+/, "");

    const defaultData = {
      numberAttempt: defaultNumberAttempt.replace(/^0+/, ""),
      blockingTime: finishBlockingTime,
      checkPeriod: defaultCheckPeriod.replace(/^0+/, ""),
    };
    saveToSessionStorage("defaultBruteForceProtection", defaultData);

    if (currentSettings) {
      setCurrentNumberAttempt(currentSettings.numberAttempt);
      setCurrentBlockingTime(currentSettings.blockingTime);
      setCurrentCheckPeriod(currentSettings.checkPeriod);
    } else {
      setCurrentNumberAttempt(defaultNumberAttempt);
      setCurrentBlockingTime(defaultBlockingTime);
      setCurrentCheckPeriod(defaultCheckPeriod);
    }
  };

  const onValidation = (inputValue) => {
    const isPositiveOrZeroNumber =
      Math.sign(inputValue) === 1 || Math.sign(inputValue) === 0;

    if (
      !isPositiveOrZeroNumber ||
      inputValue.indexOf(".") !== -1 ||
      inputValue.length > 4
    )
      return false;

    return true;
  };

  const onChangeNumberAttempt = (e) => {
    const inputValue = e.target.value;

    onValidation(inputValue) &&
      setCurrentNumberAttempt(inputValue.trim()) &&
      setShowReminder(true);
  };

  const onChangeBlockingTime = (e) => {
    const inputValue = e.target.value;

    onValidation(inputValue) && setCurrentBlockingTime(inputValue.trim());
  };

  const onChangeCheckPeriod = (e) => {
    const inputValue = e.target.value;

    onValidation(inputValue) &&
      setCurrentCheckPeriod(inputValue.trim()) &&
      setShowReminder(true);
  };

  const onSaveClick = () => {
    if (hasErrorNumberAttempt || hasErrorCheckPeriod) return;
    setIsLoadingSave(true);

    const numberCurrentNumberAttempt = parseInt(currentNumberAttempt);
    const numberCurrentBlockingTime = parseInt(currentBlockingTime);
    const numberCurrentCheckPeriod = parseInt(currentCheckPeriod);

    let finishBlockingTime = numberCurrentBlockingTime;
    if (currentBlockingTime === "") finishBlockingTime = 0;

    setBruteForceProtection(
      numberCurrentNumberAttempt,
      finishBlockingTime,
      numberCurrentCheckPeriod
    )
      .then(() => {
        saveToSessionStorage("defaultBruteForceProtection", {
          numberAttempt: currentNumberAttempt.replace(/^0+/, ""),
          blockingTime: finishBlockingTime.toString(),
          checkPeriod: currentCheckPeriod.replace(/^0+/, ""),
        });

        getBruteForceProtection();
        setShowReminder(false);
        setIsLoadingSave(false);
        toastr.success(t("SuccessfullySaveSettingsMessage"));
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage(
      "defaultBruteForceProtection"
    );
    setCurrentNumberAttempt(defaultSettings.numberAttempt);
    setCurrentBlockingTime(defaultSettings.blockingTime);
    setCurrentCheckPeriod(defaultSettings.checkPeriod);
    setShowReminder(false);
  };

  if (
    (isMobile && !isInit && !isLoading) ||
    (isMobile && currentNumberAttempt == null)
  ) {
    return <BruteForceProtectionLoader />;
  }

  return (
    <StyledBruteForceProtection>
      <div className="description">
        <Text className="page-subtitle">
          {t("BruteForceProtectionDescription")}
        </Text>

        <Link
          className="link"
          fontSize="13px"
          target="_blank"
          isHovered
          href={bruteForceProtectionUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

      <FieldContainer
        className="input-container"
        labelText={t("NumberOfAttempts")}
        isVertical={true}
      >
        <TextInput
          className="brute-force-protection-input"
          tabIndex={1}
          value={currentNumberAttempt}
          onChange={onChangeNumberAttempt}
          isDisabled={isLoadingSave}
          placeholder={t("EnterNumber")}
          hasError={hasErrorNumberAttempt}
        />
        {hasErrorNumberAttempt && (
          <div className="errorText">
            {t("ErrorMessageBruteForceProtection")}
          </div>
        )}
      </FieldContainer>

      <FieldContainer
        className="input-container"
        labelText={t("BlockingTime")}
        isVertical={true}
      >
        <TextInput
          className="brute-force-protection-input"
          tabIndex={2}
          value={currentBlockingTime}
          onChange={onChangeBlockingTime}
          isDisabled={isLoadingSave}
          placeholder={t("EnterTime")}
        />
      </FieldContainer>

      <FieldContainer
        className="input-container"
        labelText={t("CheckPeriod")}
        isVertical={true}
      >
        <TextInput
          className="brute-force-protection-input"
          tabIndex={3}
          value={currentCheckPeriod}
          onChange={onChangeCheckPeriod}
          isDisabled={isLoadingSave}
          placeholder={t("EnterTime")}
          hasError={hasErrorCheckPeriod}
        />
        {hasErrorCheckPeriod && (
          <div className="errorText">
            {t("ErrorMessageBruteForceProtection")}
          </div>
        )}

        <SaveCancelButtons
          className="save-cancel-buttons"
          tabIndex={4}
          onSaveClick={onSaveClick}
          onCancelClick={onCancelClick}
          showReminder={showReminder}
          reminderTest={t("YouHaveUnsavedChanges")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Common:CancelButton")}
          displaySettings={true}
          hasScroll={false}
          additionalClassSaveButton="session-lifetime-save"
          additionalClassCancelButton="session-lifetime-cancel"
        />
      </FieldContainer>
    </StyledBruteForceProtection>
  );
};

export default inject(({ auth, setup }) => {
  const {
    numberAttempt,
    blockingTime,
    checkPeriod,
    setBruteForceProtection,
    getBruteForceProtection,
    bruteForceProtectionUrl,
  } = auth.settingsStore;

  const { initSettings, isInit } = setup;

  return {
    numberAttempt,
    blockingTime,
    checkPeriod,
    setBruteForceProtection,
    getBruteForceProtection,
    initSettings,
    isInit,
    bruteForceProtectionUrl,
  };
})(withTranslation(["Settings", "Common"])(observer(BruteForceProtection)));
