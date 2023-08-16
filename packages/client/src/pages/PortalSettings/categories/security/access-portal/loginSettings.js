import { useState, useEffect } from "react";

import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import FieldContainer from "@docspace/components/field-container";
import toastr from "@docspace/components/toast/toastr";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
const LoginSettings = (props) => {
  const {
    t,
    numberAttempt,
    blockingTime,
    checkPeriod,
    setLoginSettings,
    getLoginSettings,
  } = props;

  console.log(
    "numberAttempt blockingTime checkPeriod",
    numberAttempt,
    blockingTime,
    checkPeriod
  );

  const defaultNumberAttempt = numberAttempt.toString();
  const defaultBlockingTime = blockingTime.toString();
  const defaultCheckPeriod = checkPeriod.toString();

  const [currentNumberAttempt, setCurrentNumberAttempt] =
    useState(defaultNumberAttempt);
  const [currentBlockingTime, setCurrentBlockingTime] =
    useState(defaultBlockingTime);
  const [currentCheckPeriod, setCurrentCheckPeriod] =
    useState(defaultCheckPeriod);

  const [showReminder, setShowReminder] = useState(false);

  const onChangeNumberAttempt = (e) => {
    setCurrentNumberAttempt(e.target.value);
    setShowReminder(true);
  };

  const onChangeBlockingTime = (e) => {
    setCurrentBlockingTime(e.target.value);
    setShowReminder(true);
  };

  const onChangeCheckPeriod = (e) => {
    setCurrentCheckPeriod(e.target.value);
    setShowReminder(true);
  };

  const onSaveClick = () => {
    const numberCurrentNumberAttempt = parseInt(currentNumberAttempt);
    const numberCurrentBlockingTime = parseInt(currentBlockingTime);
    const numberCurrentCheckPeriod = parseInt(currentCheckPeriod);

    setLoginSettings(
      numberCurrentNumberAttempt,
      numberCurrentBlockingTime,
      numberCurrentCheckPeriod
    )
      .then(() => {
        getLoginSettings();
      })
      .catch((error) => {
        console.log("error", error);
      });
  };

  console.log(
    "currentNumberAttempt currentBlockingTime currentCheckPeriod",
    currentNumberAttempt,
    currentBlockingTime,
    currentCheckPeriod
  );

  return (
    <div>
      <FieldContainer labelText={`Number of attempts:`} isVertical={true}>
        <TextInput
          tabIndex={5}
          // scale={true}
          value={currentNumberAttempt}
          onChange={onChangeNumberAttempt}
          // isDisabled={
          //   state.isLoadingGreetingSave || state.isLoadingGreetingRestore
          // }
          placeholder="Enter number"
        />
      </FieldContainer>

      <FieldContainer labelText={`Blocking time (sec):`} isVertical={true}>
        <TextInput
          tabIndex={5}
          // scale={true}
          value={currentBlockingTime}
          onChange={onChangeBlockingTime}
          // isDisabled={
          //   state.isLoadingGreetingSave || state.isLoadingGreetingRestore
          // }
          placeholder="Enter time"
        />
      </FieldContainer>

      <FieldContainer labelText={`Check period (sec):`} isVertical={true}>
        <TextInput
          tabIndex={5}
          // scale={true}
          value={currentCheckPeriod}
          onChange={onChangeCheckPeriod}
          // isDisabled={
          //   state.isLoadingGreetingSave || state.isLoadingGreetingRestore
          // }
          placeholder="Enter time"
          style={{ marginBottom: `24px` }}
        />

        <SaveCancelButtons
          className="save-cancel-buttons"
          onSaveClick={onSaveClick}
          // onCancelClick={onCancelClick}
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
    </div>
  );
};

export default inject(({ auth, setup }) => {
  const {
    numberAttempt,
    blockingTime,
    checkPeriod,
    setLoginSettings,
    getLoginSettings,
  } = auth.settingsStore;

  return {
    numberAttempt,
    blockingTime,
    checkPeriod,
    setLoginSettings,
    getLoginSettings,
  };
})(withTranslation(["Settings", "Common"])(observer(LoginSettings)));
