import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";
import Checkbox from "@docspace/components/checkbox";
import toastr from "@docspace/components/toast/toastr";
import LoaderAdditionalResources from "../sub-components/loaderAdditionalResources";
import isEqual from "lodash/isEqual";

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

  const [feedbackAndSupportEnabled, setShowFeedback] = useState(
    additionalResourcesData?.feedbackAndSupportEnabled
  );

  const [videoGuidesEnabled, setShowVideoGuides] = useState(
    additionalResourcesData?.videoGuidesEnabled
  );

  const [helpCenterEnabled, setShowHelpCenter] = useState(
    additionalResourcesData?.helpCenterEnabled
  );

  const [hasChange, setHasChange] = useState(false);
  const [disableButtons, setDisableButtons] = useState(false);

  useEffect(() => {
    setShowFeedback(additionalResourcesData?.feedbackAndSupportEnabled);
    setShowVideoGuides(additionalResourcesData?.videoGuidesEnabled);
    setShowHelpCenter(additionalResourcesData?.helpCenterEnabled);
  }, [additionalResourcesData]);

  useEffect(() => {
    const settings = {
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
      helpCenterEnabled,
    };

    const dataAdditionalResources = {
      feedbackAndSupportEnabled:
        additionalResourcesData.feedbackAndSupportEnabled,
      videoGuidesEnabled: additionalResourcesData.videoGuidesEnabled,
      helpCenterEnabled: additionalResourcesData.helpCenterEnabled,
    };

    const hasСhange = !isEqual(settings, dataAdditionalResources);

    if (hasСhange) {
      setHasChange(true);
    } else {
      setHasChange(false);
    }
  }, [
    feedbackAndSupportEnabled,
    videoGuidesEnabled,
    helpCenterEnabled,
    additionalResourcesData,
  ]);

  useEffect(() => {
    if (!(additionalResourcesData && tReady)) return;

    setIsLoadedAdditionalResources(true);
  }, [additionalResourcesData, tReady]);

  const onSave = () => {
    setDisableButtons(true);

    setAdditionalResources(
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
      helpCenterEnabled
    )
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getAdditionalResources().finally(() => setDisableButtons(false));
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  const onRestore = () => {
    setDisableButtons(true);

    restoreAdditionalResources()
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getAdditionalResources().finally(() => setDisableButtons(false));
      })
      .catch((error) => {
        toastr.error(error);
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
            className="checkbox"
            isDisabled={!isSettingPaid}
            label={t("ShowFeedbackAndSupport")}
            isChecked={feedbackAndSupportEnabled}
            onChange={() => setShowFeedback(!feedbackAndSupportEnabled)}
          />

          <Checkbox
            tabIndex={13}
            className="checkbox"
            isDisabled={!isSettingPaid}
            label={t("ShowVideoGuides")}
            isChecked={videoGuidesEnabled}
            onChange={() => setShowVideoGuides(!videoGuidesEnabled)}
          />
          <Checkbox
            tabIndex={14}
            className="checkbox"
            isDisabled={!isSettingPaid}
            label={t("ShowHelpCenter")}
            isChecked={helpCenterEnabled}
            onChange={() => setShowHelpCenter(!helpCenterEnabled)}
          />
        </div>
        {isSettingPaid && (
          <SaveCancelButtons
            tabIndex={15}
            onSaveClick={onSave}
            onCancelClick={onRestore}
            saveButtonLabel={t("Common:SaveButton")}
            cancelButtonLabel={t("Settings:RestoreDefaultButton")}
            displaySettings={true}
            showReminder={(isSettingPaid && hasChange) || disableButtons}
            disableRestoreToDefault={
              additionalResourcesIsDefault || disableButtons
            }
          />
        )}
      </StyledComponent>
    </>
  );
};

export default inject(({ auth, common }) => {
  const { settingsStore } = auth;

  const {
    setIsLoadedAdditionalResources,
    isLoadedAdditionalResources,
  } = common;

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
