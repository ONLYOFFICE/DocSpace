import React, { useState, useEffect, useCallback } from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";
import Checkbox from "@docspace/components/checkbox";
import toastr from "@docspace/components/toast/toastr";
import LoaderAdditionalResources from "../sub-components/loaderAdditionalResources";
import isEqual from "lodash/isEqual";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";

const StyledComponent = styled.div`
  margin-top: 40px;

  .branding-checkbox {
    display: flex;
    flex-direction: column;
    gap: 18px;
    margin-bottom: 24px;
  }

  .additional-header {
    padding-bottom: 2px;
  }

  .additional-description {
    padding-bottom: 18px;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }

  .checkbox {
    width: max-content;
    margin-right: 9px;
  }
`;

const AdditionalResources = (props) => {
  const {
    t,
    tReady,
    isSettingPaid,
    getAdditionalResources,
    setAdditionalResources,
    restoreAdditionalResources,
    additionalResourcesData,
    additionalResourcesIsDefault,
    setIsLoadedAdditionalResources,
    isLoadedAdditionalResources,
  } = props;

  const [additionalSettings, setAdditionalSettings] = useState({});
  const [hasChange, setHasChange] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const { feedbackAndSupportEnabled, videoGuidesEnabled, helpCenterEnabled } =
    additionalSettings;

  const getSettings = () => {
    const additionalSettings = getFromSessionStorage("additionalSettings");

    const defaultData = {
      feedbackAndSupportEnabled:
        additionalResourcesData.feedbackAndSupportEnabled,
      videoGuidesEnabled: additionalResourcesData.videoGuidesEnabled,
      helpCenterEnabled: additionalResourcesData.helpCenterEnabled,
    };

    saveToSessionStorage("defaultAdditionalSettings", defaultData);

    if (additionalSettings) {
      setAdditionalSettings({
        feedbackAndSupportEnabled: additionalSettings.feedbackAndSupportEnabled,
        videoGuidesEnabled: additionalSettings.videoGuidesEnabled,
        helpCenterEnabled: additionalSettings.helpCenterEnabled,
      });
    } else {
      setAdditionalSettings(defaultData);
    }
  };

  useEffect(() => {
    getSettings();
  }, [isLoading]);

  useEffect(() => {
    const defaultAdditionalSettings = getFromSessionStorage(
      "defaultAdditionalSettings"
    );
    const newSettings = {
      feedbackAndSupportEnabled: additionalSettings.feedbackAndSupportEnabled,
      videoGuidesEnabled: additionalSettings.videoGuidesEnabled,
      helpCenterEnabled: additionalSettings.helpCenterEnabled,
    };
    saveToSessionStorage("additionalSettings", newSettings);

    if (isEqual(defaultAdditionalSettings, newSettings)) {
      setHasChange(false);
    } else {
      setHasChange(true);
    }
  }, [additionalSettings, additionalResourcesData]);

  useEffect(() => {
    if (!(additionalResourcesData && tReady)) return;

    setIsLoadedAdditionalResources(true);
  }, [additionalResourcesData, tReady]);

  const onSave = useCallback(async () => {
    setIsLoading(true);

    await setAdditionalResources(
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
      helpCenterEnabled
    )
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
      })
      .catch((error) => {
        toastr.error(error);
      });

    await getAdditionalResources();

    const data = {
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
      helpCenterEnabled,
    };

    saveToSessionStorage("additionalSettings", data);
    saveToSessionStorage("defaultAdditionalSettings", data);
    setIsLoading(false);
  }, [
    setIsLoading,
    setAdditionalResources,
    getAdditionalResources,
    additionalSettings,
  ]);

  const onRestore = useCallback(async () => {
    setIsLoading(true);

    await restoreAdditionalResources()
      .then((res) => {
        setAdditionalSettings(res);
        saveToSessionStorage("additionalSettings", res);
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
      })
      .catch((error) => {
        toastr.error(error);
      });

    await getAdditionalResources();

    setIsLoading(false);
  }, [setIsLoading, restoreAdditionalResources, getAdditionalResources]);

  const onChangeFeedback = () => {
    setAdditionalSettings({
      ...additionalSettings,
      feedbackAndSupportEnabled: !feedbackAndSupportEnabled,
    });
    saveToSessionStorage("additionalSettings", {
      ...additionalSettings,
      feedbackAndSupportEnabled: !feedbackAndSupportEnabled,
    });
  };

  const onChangeVideoGuides = () => {
    setAdditionalSettings({
      ...additionalSettings,
      videoGuidesEnabled: !videoGuidesEnabled,
    });
    saveToSessionStorage("additionalSettings", {
      ...additionalSettings,
      videoGuidesEnabled: !videoGuidesEnabled,
    });
  };

  const onChangeHelpCenter = () => {
    setAdditionalSettings({
      ...additionalSettings,
      helpCenterEnabled: !helpCenterEnabled,
    });
    saveToSessionStorage("additionalSettings", {
      ...additionalSettings,
      helpCenterEnabled: !helpCenterEnabled,
    });
  };

  if (!isLoadedAdditionalResources) return <LoaderAdditionalResources />;

  return (
    <>
      <StyledComponent>
        <div className="header">
          <div className="additional-header settings_unavailable">
            {t("Settings:AdditionalResources")}
          </div>
        </div>
        <div className="settings_unavailable additional-description">
          {t("Settings:AdditionalResourcesDescription")}
        </div>
        <div className="branding-checkbox">
          <Checkbox
            tabIndex={12}
            className="show-feedback-support checkbox"
            isDisabled={!isSettingPaid}
            label={t("ShowFeedbackAndSupport")}
            isChecked={feedbackAndSupportEnabled}
            onChange={onChangeFeedback}
          />

          <Checkbox
            tabIndex={13}
            className="show-video-guides checkbox"
            isDisabled={!isSettingPaid}
            label={t("ShowVideoGuides")}
            isChecked={videoGuidesEnabled}
            onChange={onChangeVideoGuides}
          />
          <Checkbox
            tabIndex={14}
            className="show-help-center checkbox"
            isDisabled={!isSettingPaid}
            label={t("ShowHelpCenter")}
            isChecked={helpCenterEnabled}
            onChange={onChangeHelpCenter}
          />
        </div>
        <SaveCancelButtons
          tabIndex={15}
          onSaveClick={onSave}
          onCancelClick={onRestore}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          displaySettings={true}
          reminderTest={t("YouHaveUnsavedChanges")}
          showReminder={(isSettingPaid && hasChange) || isLoading}
          disableRestoreToDefault={additionalResourcesIsDefault || isLoading}
          additionalClassSaveButton="additional-resources-save"
          additionalClassCancelButton="additional-resources-cancel"
        />
      </StyledComponent>
    </>
  );
};

export default inject(({ auth, common }) => {
  const { settingsStore } = auth;

  const { setIsLoadedAdditionalResources, isLoadedAdditionalResources } =
    common;

  const {
    getAdditionalResources,
    setAdditionalResources,
    restoreAdditionalResources,
    additionalResourcesData,
    additionalResourcesIsDefault,
  } = settingsStore;

  return {
    getAdditionalResources,
    setAdditionalResources,
    restoreAdditionalResources,
    additionalResourcesData,
    additionalResourcesIsDefault,
    setIsLoadedAdditionalResources,
    isLoadedAdditionalResources,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(AdditionalResources))
  )
);
