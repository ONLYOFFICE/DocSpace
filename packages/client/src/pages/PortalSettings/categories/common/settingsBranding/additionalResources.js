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
  const { t } = props;
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
          label={t("ShowFeedbackAndSupport")}
          isChecked={showFeedback}
          onChange={() => setShowFeedback(!showFeedback)}
        />
        <Checkbox
          label={t("ShowSampleDocuments")}
          isChecked={showSample}
          onChange={() => setShowSample(!showSample)}
        />
        <Checkbox
          label={t("ShowVideoGuides")}
          isChecked={showVideoGuides}
          onChange={() => setShowVideoGuides(!showVideoGuides)}
        />
        <Checkbox
          label={t("ShowHelpCenter")}
          isChecked={showHelpCenter}
          onChange={() => setShowHelpCenter(!showHelpCenter)}
        />
      </div>
      <SaveCancelButtons
        // onSaveClick={onSavePortalRename}
        // onCancelClick={onCancelPortalName}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Settings:RestoreDefaultButton")}
        displaySettings={true}
        // hasScroll={hasScroll}
      />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(AdditionalResources))
  )
);
