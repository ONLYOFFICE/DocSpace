import React, { Component } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/text";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Button from "@appserver/components/button";

import { setDocumentTitle } from "../../../../../helpers/utils";
import { inject } from "mobx-react";

const MainContainer = styled.div`
  width: 100%;

  .save-button {
    margin-top: 32px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
  }
`;

const HeaderContainer = styled.div`
  margin: 0 0 16px 0;
`;

class PureAccessPortal extends Component {
  constructor(props) {
    super(props);

    const { t } = props;
    this.state = { type: "disabled" };

    setDocumentTitle(t("ManagementCategorySecurity"));
  }

  onSelectTfaType = (e) => this.setState({ type: e.target.value });

  saveSettings = () => {
    const { type } = this.state;
    const { setTfaSettings } = this.props;

    console.log("Save settings");
    setTfaSettings(type);
  };

  render() {
    const { t } = this.props;

    console.log("accessPortal render");

    return (
      <MainContainer>
        <HeaderContainer>
          <Text fontSize="18px">{t("TwoFactorAuth")}</Text>
        </HeaderContainer>
        <RadioButtonGroup
          fontSize="13px"
          fontWeight="400"
          name="group"
          orientation="vertical"
          options={[
            {
              label: t("Disabled"),
              value: "none",
            },
            {
              label: t("BySms"),
              value: "sms",
              disabled: true,
            },
            {
              label: t("ByApp"),
              value: "app",
            },
          ]}
          selected="none"
          onClick={this.onSelectTfaType}
        />
        <Button
          label={t("SaveButton")}
          size="medium"
          primary={true}
          className="save-button"
          onClick={this.saveSettings}
        />
      </MainContainer>
    );
  }
}

export default inject(({ auth }) => ({
  organizationName: auth.settingsStore.organizationName,
  setTfaSettings: auth.tfaStore.setTfaSettings,
}))(withTranslation("Settings")(withRouter(PureAccessPortal)));
