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
    padding-bottom: 1px;
  }

  .description {
    color: ${(props) => !props.isPortalPaid && "#A3A9AE"};
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
    isPortalPaid,
    getAdditionalResources,
    setAdditionalResources,
    restoreAdditionalResources,
    additionalResourcesData,
    setIsLoadedAdditionalResources,
    isLoadedAdditionalResources,
  } = props;

  const [feedbackAndSupportEnabled, setShowFeedback] = useState(
    additionalResourcesData.feedbackAndSupportEnabled
  );

  const [videoGuidesEnabled, setShowVideoGuides] = useState(
    additionalResourcesData.videoGuidesEnabled
  );

  const [helpCenterEnabled, setShowHelpCenter] = useState(
    additionalResourcesData.helpCenterEnabled
  );

  const [hasChange, setHasChange] = useState(false);

  const [hasChangesDefaultSettings, setHasChangesDefaultSettings] = useState(
    false
  );

  const defaultAdditionalResources = JSON.parse(
    localStorage.getItem("defaultAdditionalResources")
  );

  useEffect(() => {
    setShowFeedback(additionalResourcesData.feedbackAndSupportEnabled);
    setShowVideoGuides(additionalResourcesData.videoGuidesEnabled);
    setShowHelpCenter(additionalResourcesData.helpCenterEnabled);
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

    const hasСhangeDefault = !isEqual(settings, defaultAdditionalResources);

    if (hasСhange) {
      setHasChange(true);
    } else {
      setHasChange(false);
    }

    if (hasСhangeDefault) {
      setHasChangesDefaultSettings(true);
    } else {
      setHasChangesDefaultSettings(false);
    }
  }, [
    feedbackAndSupportEnabled,
    videoGuidesEnabled,
    helpCenterEnabled,
    additionalResourcesData,
    defaultAdditionalResources,
  ]);

  useEffect(() => {
    if (!(additionalResourcesData && tReady)) return;

    setIsLoadedAdditionalResources(true);
  }, [additionalResourcesData, tReady]);

  const onSave = () => {
    setAdditionalResources(
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
      helpCenterEnabled
    )
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getAdditionalResources();
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  const onRestore = () => {
    restoreAdditionalResources()
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getAdditionalResources();
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  if (!isLoadedAdditionalResources) return <LoaderAdditionalResources />;

  return (
    <>
      <StyledComponent isPortalPaid={isPortalPaid}>
        <div className="header">
          <div className="additional-header">
            {t("Settings:AdditionalResources")}
          </div>
        </div>
        <div className="description additional-description">
          <div className="additional-description">
            {t("Settings:AdditionalResourcesDescription")}
          </div>
        </div>
        <div className="branding-checkbox">
          <Checkbox
            className="checkbox"
            isDisabled={!isPortalPaid}
            label={t("ShowFeedbackAndSupport")}
            isChecked={feedbackAndSupportEnabled}
            onChange={() => setShowFeedback(!feedbackAndSupportEnabled)}
          />

          <Checkbox
            className="checkbox"
            isDisabled={!isPortalPaid}
            label={t("ShowVideoGuides")}
            isChecked={videoGuidesEnabled}
            onChange={() => setShowVideoGuides(!videoGuidesEnabled)}
          />
          <Checkbox
            className="checkbox"
            isDisabled={!isPortalPaid}
            label={t("ShowHelpCenter")}
            isChecked={helpCenterEnabled}
            onChange={() => setShowHelpCenter(!helpCenterEnabled)}
          />
        </div>
        <SaveCancelButtons
          onSaveClick={onSave}
          onCancelClick={onRestore}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          displaySettings={true}
          showReminder={isPortalPaid && hasChange}
          disableRestoreToDefault={!hasChangesDefaultSettings}
        />
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
  } = settingsStore;

  return {
    getAdditionalResources,
    setAdditionalResources,
    restoreAdditionalResources,
    additionalResourcesData,
    setIsLoadedAdditionalResources,
    isLoadedAdditionalResources,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(AdditionalResources))
  )
);
