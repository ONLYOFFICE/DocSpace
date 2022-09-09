import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";
import Checkbox from "@docspace/components/checkbox";
import toastr from "@docspace/components/toast/toastr";
import ModalDialog from "@docspace/components/modal-dialog";
import Link from "@docspace/components/link";

const StyledComponent = styled.div`
  margin-top: 40px;

  .branding-checkbox {
    display: flex;
    flex-direction: column;
    gap: 16px;
    margin-bottom: 24px;
  }
`;

const StyledModalDialog = styled(ModalDialog)`
  #modal-dialog {
    width: auto;
    max-height: none;
  }

  .modal-body {
    padding: 0;
    height: 515px;
  }
  .modal-footer {
    display: none;
  }
`;

const AdditionalResources = (props) => {
  const {
    t,
    isPortalPaid,
    getAdditionalResources,
    setAdditionalResources,
    restoreAdditionalResources,
    additionalResourcesData,
  } = props;
  const [showFeedback, setShowFeedback] = useState(
    additionalResourcesData.feedbackAndSupportEnabled
  );
  const [showVideoGuides, setShowVideoGuides] = useState(
    additionalResourcesData.videoGuidesEnabled
  );
  const [showHelpCenter, setShowHelpCenter] = useState(
    additionalResourcesData.helpCenterEnabled
  );

  const [hasChange, setHasChange] = useState(false);
  const [isFirstAdditionalResources, setIsFirstAdditionalResources] = useState(
    localStorage.getItem("isFirstAdditionalResources")
  );

  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    if (
      showFeedback !== additionalResourcesData.feedbackAndSupportEnabled ||
      showVideoGuides !== additionalResourcesData.videoGuidesEnabled ||
      showHelpCenter !== additionalResourcesData.helpCenterEnabled
    ) {
      setHasChange(true);
    } else {
      setHasChange(false);
    }
  }, [showFeedback, showVideoGuides, showHelpCenter, additionalResourcesData]);

  const onSave = () => {
    //TODO: Add data
    setAdditionalResources()
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getAdditionalResources();

        if (!localStorage.getItem("isFirstAdditionalResources")) {
          localStorage.setItem("isFirstAdditionalResources", true);
          setIsFirstCompanyInfoSettings("true");
        }
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

  const onShowExample = () => {
    if (!isPortalPaid) return;
    setShowModal(true);
  };

  const onCloseModal = () => {
    setShowModal(false);
  };

  return (
    <>
      {showModal && (
        <StyledModalDialog visible={showModal} onClose={onCloseModal}>
          <ModalDialog.Body>
            <img className="img" src="/static/images/docspace.menu.svg" />
          </ModalDialog.Body>
          <ModalDialog.Footer className="modal-footer" />
        </StyledModalDialog>
      )}

      <StyledComponent>
        <div className="header">Additional resources</div>
        <div className="description">
          Choose whether you want to display links to additional resources in
          <Link className="link" onClick={onShowExample} noHover={true}>
            &nbsp;DocSpace menu.&nbsp;
          </Link>
        </div>
        <div className="branding-checkbox">
          <Checkbox
            isDisabled={!isPortalPaid}
            label={t("ShowFeedbackAndSupport")}
            isChecked={showFeedback}
            onChange={() => setShowFeedback(!showFeedback)}
          />

          <Checkbox
            isDisabled={!isPortalPaid}
            label={t("ShowVideoGuides")}
            isChecked={showVideoGuides}
            onChange={() => setShowVideoGuides(!showVideoGuides)}
          />
          <Checkbox
            isDisabled={!isPortalPaid}
            label={t("ShowHelpCenter")}
            isChecked={showHelpCenter}
            onChange={() => setShowHelpCenter(!showHelpCenter)}
          />
        </div>
        <SaveCancelButtons
          onSaveClick={onSave}
          onCancelClick={onRestore}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          displaySettings={true}
          showReminder={isPortalPaid && hasChange}
          // isFirstRestoreToDefault={isFirstCompanyInfoSettings}
        />
      </StyledComponent>
    </>
  );
};

export default inject(({ auth, setup, common }) => {
  const { settingsStore } = auth;
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
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(AdditionalResources))
  )
);
