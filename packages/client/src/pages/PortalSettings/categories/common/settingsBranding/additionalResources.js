import React from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";
import Checkbox from "@docspace/components/checkbox";

const StyledComponent = styled.div`
  .branding-checkbox {
    padding-bottom: 16px;
  }
`;

const AdditionalResources = (props) => {
  const { t } = props;
  return (
    <StyledComponent>
      <div className="header">Additional resources</div>
      <div className="description">
        Choose whether you want to display links to additional resources in
        Portal modules and sample files in Documents module.
      </div>
      <div className="settings-block">
        <Checkbox
          className="branding-checkbox"
          label="Show Feedback & Support link"
        />
        <Checkbox className="branding-checkbox" label="Show sample documents" />
        <Checkbox
          className="branding-checkbox"
          label="Show link to Video Guides"
        />
        <Checkbox
          className="branding-checkbox"
          label="Show link to Help Center"
        />
      </div>
      <SaveCancelButtons
        id="buttonsCompanyInfoSettings"
        className="save-cancel-buttons"
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
