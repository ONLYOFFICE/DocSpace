import React, { useState } from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";
import Checkbox from "@docspace/components/checkbox";

const StyledComponent = styled.div`
  margin-top: 40px;

  .branding-checkboxs {
    display: flex;
    flex-direction: column;
    gap: 16px;
    margin-bottom: 24px;
  }
`;

const AdditionalResources = (props) => {
  const { t, isPortalPaid } = props;
  const [showFeedback, setShowFeedback] = useState(false);
  const [showSample, setShowSample] = useState(false);
  const [showVideoGuides, setShowVideoGuides] = useState(false);
  const [showHelpCenter, setShowHelpCenter] = useState(false);

  return (
    <StyledComponent>
      <div className="header">Additional resources</div>
      <div className="description">
        Choose whether you want to display links to additional resources in
        Portal modules and sample files in Documents module.
      </div>
      <div className="branding-checkboxs">
        <Checkbox
          isDisabled={!isPortalPaid}
          label={t("ShowFeedbackAndSupport")}
          isChecked={showFeedback}
          onChange={() => setShowFeedback(!showFeedback)}
        />
        <Checkbox
          isDisabled={!isPortalPaid}
          label={t("ShowSampleDocuments")}
          isChecked={showSample}
          onChange={() => setShowSample(!showSample)}
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
        onSaveClick={() => console.log("click")}
        onCancelClick={() => console.log("click")}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Settings:RestoreDefaultButton")}
        displaySettings={true}
      />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(AdditionalResources))
  )
);
