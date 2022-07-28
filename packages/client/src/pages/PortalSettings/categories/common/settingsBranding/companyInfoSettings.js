import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";

import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";

const StyledComponent = styled.div``;

const CompanyInfoSettings = (props) => {
  const { t } = props;
  return (
    <StyledComponent>
      <div className="header">Company info settings</div>
      <div className="description">
        This information will be displayed in the About this program window.
      </div>
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerCompanyName"
          className="field-container-width"
          labelText="Company name:"
          isVertical={true}
        >
          <TextInput id="textInputContainerCompanyName" scale={true} />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerEmail"
          className="field-container-width"
          labelText="Email:"
          isVertical={true}
        >
          <TextInput id="textInputContainerEmail" scale={true} />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerPhone"
          className="field-container-width"
          labelText="Phone:"
          isVertical={true}
        >
          <TextInput id="textInputContainerPhone" scale={true} />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerWebsite"
          className="field-container-width"
          labelText="Website:"
          isVertical={true}
        >
          <TextInput id="textInputContainerWebsite" scale={true} />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerAddress"
          className="field-container-width"
          labelText="Address:"
          isVertical={true}
        >
          <TextInput id="textInputContainerAddress" scale={true} />
        </FieldContainer>
      </div>
      <SaveCancelButtons
        id="buttonsCompanyInfoSettings"
        className="save-cancel-buttons"
        // onSaveClick={onSavePortalRename}
        // onCancelClick={onCancelPortalName}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Settings:RestoreDefaultButton")}
        displaySettings={true}
      />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(CompanyInfoSettings))
  )
);
