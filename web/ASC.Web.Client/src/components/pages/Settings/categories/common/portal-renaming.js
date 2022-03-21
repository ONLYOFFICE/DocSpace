import React, { useState } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Loader from "@appserver/components/loader";
import HelpButton from "@appserver/components/help-button";
import FieldContainer from "@appserver/components/field-container";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";

const StyledComponent = styled.div`
  .settings-block {
    margin-bottom: 70px;
  }

  .settings-block {
    max-width: 350px;
  }

  .combo-button-label {
    max-width: 100%;
  }
`;

const PortalRenaming = ({ t, theme, sectionWidth }) => {
  // todo: Изменить на false
  const [isLoadedData, setIsLoadedData] = useState(true);

  const onSavePortalRename = () => {
    // setPortalRename("waw");
  };

  return !isLoadedData ? (
    <Loader className="pageLoader" type="rombs" size="40px" />
  ) : (
    <>
      <StyledComponent>
        <div className="category-item-heading">
          <div className="category-item-title">{t("Portal Renaming")}</div>
          <HelpButton
            iconName="static/images/combined.shape.svg"
            size={12}
            // tooltipContent={tooltipCustomTitlesTooltip}
          />
        </div>
        <div className="settings-block">
          <FieldContainer
            id="fieldContainerWelcomePage"
            className="field-container-width"
            labelText={`${t("New portal name")}:`}
            isVertical={true}
          >
            <TextInput
              scale={true}
              // value={greetingTitle}
              //onChange={this.onChangeGreetingTitle}
              // isDisabled={isLoadingGreetingSave || isLoadingGreetingRestore}
              placeholder={`${t("room")}`}
            />
          </FieldContainer>
        </div>
        <SaveCancelButtons
          onSaveClick={onSavePortalRename}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Common:CancelButton")}
          displaySettings={true}
          sectionWidth={sectionWidth}
        />
      </StyledComponent>
    </>
  );
};

export default inject(({ auth, setup }) => {
  const { theme } = auth.settingsStore;
  const { setPortalRename } = setup;

  return {
    theme,
    setPortalRename,
  };
})(withTranslation(["Settings", "Common"])(observer(PortalRenaming)));
