import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";

import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";

const StyledComponent = styled.div`
  .save-cancel-buttons {
    margin-top: 8px;
  }
`;

const CompanyInfoSettings = (props) => {
  const { t, isSettingPaid } = props;
  const [companyName, setCompanyName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [website, setWebsite] = useState("");
  const [address, setAddress] = useState("");

  return (
    <StyledComponent>
      <div className="header settings_unavailable">Company info settings</div>
      <div className="description settings_unavailable">
        This information will be displayed in the About this program window.
      </div>
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerCompanyName"
          className="field-container-width settings_unavailable"
          labelText={t("Common:CompanyName")}
          isVertical={true}
        >
          <TextInput
            id="textInputContainerCompanyName"
            isDisabled={!isSettingPaid}
            scale={true}
            value={companyName}
            onChange={(e) => setCompanyName(e.target.value)}
          />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerEmail"
          isDisabled={!isSettingPaid}
          className="field-container-width settings_unavailable"
          labelText={t("Common:Email")}
          isVertical={true}
        >
          <TextInput
            id="textInputContainerEmail"
            isDisabled={!isSettingPaid}
            scale={true}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerPhone"
          className="field-container-width settings_unavailable"
          labelText={t("Common:Phone")}
          isVertical={true}
        >
          <TextInput
            id="textInputContainerPhone"
            isDisabled={!isSettingPaid}
            scale={true}
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerWebsite"
          className="field-container-width settings_unavailable"
          labelText={t("Common:Website")}
          isVertical={true}
        >
          <TextInput
            id="textInputContainerWebsite"
            isDisabled={!isSettingPaid}
            scale={true}
            value={website}
            onChange={(e) => setWebsite(e.target.value)}
          />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerAddress"
          className="field-container-width settings_unavailable"
          labelText={t("Common:Address")}
          isVertical={true}
        >
          <TextInput
            id="textInputContainerAddress"
            isDisabled={!isSettingPaid}
            scale={true}
            value={address}
            onChange={(e) => setAddress(e.target.value)}
          />
        </FieldContainer>
      </div>
      {isSettingPaid && (
        <SaveCancelButtons
          className="save-cancel-buttons"
          onSaveClick={() => console.log("click")}
          onCancelClick={() => console.log("click")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          displaySettings={true}
        />
      )}
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(CompanyInfoSettings))
  )
);
